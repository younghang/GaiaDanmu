﻿/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/20
 * Time: 22:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using BilibiliStyleDanmu;
using Newtonsoft.Json.Linq;
using OtherDanmuTest;
using GaiaDanmu;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public static bool RUNNING = true;
		int timeStart = 0;
		ManualResetEvent roomFinish = new ManualResetEvent(false);
		ManualResetEvent gaiaroomFinish = new ManualResetEvent(false);
		DanmakuCurtain dmkCurt = null;
		MainOverlay biliDanmu=null;
		//		DanmuProvider danmuProvider;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
//				MessageBox.Show("有新版本，请去贴吧下载，扬co发的女王那个贴子或者这里http://yun.baidu.com/share/link?shareid=3067013852&uk=3514645625");
			timeStart = int.Parse(GetTimeStamp());
			StartInitial();
			
//			Data data=new Data();
//		data.uid=0;
//			data.gid=1861100490;
//			 data.timestamp=1464180075;
//			 RoomDetail rom=new RoomDetail();
//			 rom.data=data;
			
//			richTextBox1.AppendText(GetTimeStamp()+"\n");
			
			
		}
		InitialRoomDetial initialroom;
		InitialGaiaRoom initialGaiaroom;
		Thread initailSidThread;
		Thread runningMSGThread;
		Thread checkUpdateThread;
		int MessgeCount = 0;
		bool StopMessge;
		static int DanmuVersion = 12;
		List<string> listMsg = new List<string>();
		void StartRunning()
		{
			ShowMessage("开始连接弹幕服务器。。。");
			if (runningMSGThread == null || !runningMSGThread.IsAlive) {
				runningMSGThread = new Thread(new  ThreadStart(TestConnectionServer));
				runningMSGThread.Start();
			}
		}
		void StartInitial()
		{
			if (checkUpdateThread == null || !checkUpdateThread.IsAlive) {
				checkUpdateThread = new Thread(new ThreadStart(() => {
				                                               	try {
				                                               		int NewVersion = int.Parse(new WorkLogin().GetUpdate());
				                                               		if (NewVersion > DanmuVersion) {
				                                               			MessageBox.Show("有新版本，请去贴吧下载，扬co发的女王那个贴子或者这里http://yun.baidu.com/share/link?shareid=3067013852&uk=3514645625");
				                                               			ShowMessage("有新版本，请去贴吧下载，扬co发的女王那个贴子");
				                                               		} else
				                                               			ShowMessage("没有更新");
				                                               	} catch {
				                                               		ShowMessage("检查更新失败");
				                                               	}
				                                               }));
			}
			checkUpdateThread.Start();
			//主线程在这里启动了三个线程，一个等待检测其他两个完成，两个用于初始化相关数据
			
			ShowMessage("初始化用户认证的数据。。。");
			initialroom = new InitialRoomDetial(ref  roomFinish);
			initialGaiaroom = new InitialGaiaRoom(ref gaiaroomFinish);
			if (initailSidThread == null || !initailSidThread.IsAlive)
				initailSidThread = new Thread(new  ThreadStart(InitialRoom));
			initailSidThread.Start();
		}
		void InitialRoom()
		{
//			while (!initialroom.FINISH_INITIAL_ROOM || !initialGaiaroom.FINISH_INITIAL_GAIA_ROOM) {
//
//			}
			ShowMessage("等待中");
			roomFinish.WaitOne();
			gaiaroomFinish.WaitOne();
			initialGaiaroom.FINISH_INITIAL_GAIA_ROOM = false;
			initialroom.FINISH_INITIAL_ROOM = false;
			ShowMessage("初始化相关信息完成");
			//显示一下
			RoomDetail room = initialroom.GetRoomDetail();
			string showinfo = "gid:" + room.data.gid + "\nuid:" + room.data.uid + "\ntimestamp:" + room.data.timestamp + "\nsid:" + room.data.sid;
			ShowMessage(showinfo);
			string showGaiaroomInfo = initialGaiaroom.GetJsonGaiaRoom().ToString();
//			ShowMessage(showGaiaroomInfo);
			ShowMessage("ip:" + initialGaiaroom.chatip);
			ShowMessage("port:" + initialGaiaroom.chatport);
			
			//启动
			StartRunning();
		}
		
		
		InitailServer initialServer = null;
		static string strMessage = "";
		string GetLoginString()
		{
			string login = "";
			try {
				login = File.ReadAllText("loginstr.txt");
				
			} catch {
				login = "";
			}
			
			return login;
		}
		void TestConnectionServer()
		{
			RoomDetail room = initialroom.GetRoomDetail();
			
			room.ServerIP = initialGaiaroom.chatip;
			room.roomid = initialGaiaroom.GetGaiaRoomDetail().data.id;
			room.chatRoomID = initialGaiaroom.chatRoomid;
			
			initialServer = new InitailServer(room);
			initialServer.ShowMessage += ShowMessage;
			initialServer.ConnectServerStep1();
			initialServer.ConnectServerStep2();
			string login = GetLoginString();
			initialServer.ConnectServerStep3(login);
			initialServer.ConnectServerStep4();
			if (login != "") {
				ShowMessage("登陆文件存在，尝试用户登陆");
				initialServer.SendDanmuToServer("0.0");
			}
			
//			initialServer.ConnectServerStep4("1+2");
			initialServer.RunConnection();
			
			
			//为了发送弹幕,其实没必要，这样会让CPU保持20%的运行，线程自己活着在
//			while(RUNNING)
//			{
//
//				if (InitailServer.MESSAGE) {
//					ShowMessage("Message");
//					if (strMessage!="") {
//						initialServer.ConnectServerStep5(strMessage);
//					}
//					InitailServer.MESSAGE=false;
//				}
//
//			}
		}
		
		//		int isInShow = 0;
		//		Random ran = new Random();
		FireWorksList fl = null;
		List<string> StringMsglist = new List<string>();
		void AppendNewMessageToList(string x)
		{
			int pos = x.IndexOf(':');
			int pos2 = x.IndexOf("::");
			int len = richTextBox1.Text.Length;
//			if (!LeftBiliDanmuCheck.Checked && x.Contains(".")) {
//				x = x.Substring(0, x.Length - 15);
//			}
			if (x.Contains("www.zhanqi.tv")) {
				string rooid = x.Split('$')[1];
				x = x.Split('$')[0];
				
				if (fireCheck.Checked) {
					if (fl == null) {
						fl = new FireWorksList();
						fl.Show();
					}
					fl.AddRoomid(rooid);
				}
			}
//			isInShow++;
//			if (isInShow > 1) {
//
//				int timeLong = ran.Next(20, 200);
//				Thread.Sleep(20 + timeLong);
//
//			}
//			if (dmkCurt != null||biliDanmu!=null) {
//				if (pos==-1) {
//					pos=0;
//				}else
//					pos++;
//				string strDanmu="";
//				if(pos2!=-1)
//				 strDanmu=x.Substring(pos,x.Length-pos);
//				else
//					strDanmu=x;
//				if(dmkCurt!=null)
//				ShootDanmaku(strDanmu);
//				if (LeftBiliDanmuCheck.Checked&&biliDanmu!=null) {
//					int posiont=strDanmu.IndexOf("::")+3;
//					if (posiont==-1) {
//						posiont=strDanmu.IndexOf(":")+1;
//					}
//					if (posiont==-1) {
//						posiont=0;
//					}
//					string name=strDanmu.Substring(0,posiont);
//					string text=strDanmu.Substring(posiont,strDanmu.Length-posiont);
//					if (name.Contains("error")) {
//						biliDanmu.AddDMText(name,text,true);
//					}else
//						biliDanmu.AddDMText(name,text);
//				}
//				
//			}
			
			this.richTextBox1.AppendText(x + "\n");
			string str = GetTimeStamp();
			StringMsglist.Add(x + "$" + str);
			string strName="";
			string MessageText="";
			try{
			if (x.Contains("::")) {
				richTextBox1.Select(len, pos);
				richTextBox1.SelectionColor = Color.LimeGreen;
				richTextBox1.Select(len + pos, pos2 - pos);
				if(pos==pos2)
					pos=-1;
				strName=x.Substring(pos+1,pos2-pos-1);
				richTextBox1.SelectionColor = Color.Blue;
				richTextBox1.Select(len + pos2, x.Length - pos2);
				MessageText=x.Substring(pos2+2,x.Length-pos2-2);
				richTextBox1.SelectionColor = Color.Red;
			} else if (x.Contains("error::")) {
				richTextBox1.Select(len, x.Length);			 
				richTextBox1.SelectionColor = Color.Red;
			} else if (x.Contains("###")) {
				richTextBox1.Select(len, x.Length);
				richTextBox1.SelectionColor = Color.Fuchsia;
			} else if (x.Contains("进入直播间")) {
				richTextBox1.Select(len, x.Length);
				richTextBox1.SelectionColor = Color.DarkOrange;
			}
			if (strName=="") {
				strName=x;
			}
			}
			catch{
				ShowMessage("error::处理消息出错");
			}
			
			richTextBox1.SelectionColor = Color.Black;
			
			richTextBox1.SelectionBackColor = Color.White;
			richTextBox1.HideSelection = false;
//			isInShow--;
			if (dmkCurt!=null) {
				ShootDanmaku(strName+"："+MessageText);
			}
			if (biliDanmu!=null&&LeftBiliDanmuCheck.Checked) {
				biliDanmu.AddDMText(strName,MessageText);
			}
			
		}
//		private void ShootDanmaku(string text)
//		{
//			Thread NetServer = new Thread(new ThreadStart(()=>{dmkCurt.Shoot(text);}));
		//      NetServer .SetApartmentState(ApartmentState.STA);
		//      NetServer .IsBackground = true;
//
		//      NetServer.Start();
//		}
		void ShowMessage(string str)
		{
			if (String.IsNullOrEmpty(str)) {
				return;
			}
			if (StopMessge) {
				MessgeCount++;
				listMsg.Add(str);
				try {
					if (lbl_NewMsg.InvokeRequired) {
						// 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
						Action<string> actionDelegate = (x) => {
							lbl_NewMsg.Text = "新消息" + MessgeCount;
						};
						// 或者
						// Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
						this.lbl_NewMsg.Invoke(actionDelegate, str);
						
					} else {
						this.lbl_NewMsg.Text = str;
					}
				} catch {
					ShowMessage("error::显示新信息条数出错");
				}
			} else {
				try {
					if (richTextBox1.InvokeRequired) {
						// 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
						Action<string> actionDelegate = AppendNewMessageToList;
						// 或者
						// Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
						this.richTextBox1.Invoke(actionDelegate, str);
						
					} else {
						this.richTextBox1.AppendText(str + "\n");
					}
				} catch {
//					ShowMessage("error::显示信息出错");
				}
			}

			
		}
		void Btn_SendClick(object sender, EventArgs e)
		{
//			richTextBox1.Text+= GetTimeStamp()+"\n";
			initialServer.RUN_CONNECTION = !initialServer.RUN_CONNECTION;
			
			string info = "";
			if (initialServer.RUN_CONNECTION) {
				info = "启动接收";
				btn_Stop.Text = "停止";
				StartInitial();
			} else {
				info = "停止接收";
				initialServer.Disposed();
				Thread.Sleep(1000);
				btn_Stop.Text = "启动";
			}
			
			ShowMessage(info);
			RUNNING = !RUNNING;
		}
		void Btn_StopClick(object sender, EventArgs e)
		{
//		string info = "";
//			if (initialServer.RUN_CONNECTION) {
//				info = "启动接收";
//				btn_Stop.Text = "停止";
//				StartInitial();
//			} else {
//				info = "停止接收";
//				danmuProvider.StopDanmu();
//				Thread.Sleep(1000);
//				btn_Stop.Text = "启动";
//			}
//
//			ShowMessage(info);
//			RUNNING = !RUNNING;
			initialServer.RUN_CONNECTION = !initialServer.RUN_CONNECTION;
			
			string info = "";
			if (initialServer.RUN_CONNECTION) {
				info = "启动接收";
				btn_Stop.Text = "停止";
				StartInitial();
			} else {
				info = "停止接收";
				initialServer.Disposed();
				Thread.Sleep(1000);
				btn_Stop.Text = "启动";
					roomFinish.Reset();
			gaiaroomFinish.Reset();
			}
			
			ShowMessage(info);
			RUNNING = !RUNNING;
		}


		void Btn_SendMessageClick(object sender, EventArgs e)
		{
//			richTextBox1.AppendText(GetTimeStamp()+"\n");
			if (initailSidThread != null && initailSidThread.IsAlive) {
				ShowMessage("线程还活着");
			}
			
			if (initialServer == null) {
				ShowMessage("InitialServer 对象没有了");
				return;
			}
			initialServer.MESSAGE = true;
			strMessage = textBox1.Text;
			initialServer.SendDanmuToServer(strMessage);
			initialServer.MESSAGE = false;
		}
		void RichTextBox1TextChanged(object sender, EventArgs e)
		{
//		    if (!Roll) {
//		    	 richTextBox1.HideSelection=!Roll;
//		    }
			richTextBox1.HideSelection = false;
//
//			 richTextBox1.Select();
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
//
//			richTextBox1.ScrollToCaret();
		}
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			try {
				initialroom.FINISH_INITIAL_ROOM = true;
				initialGaiaroom.FINISH_INITIAL_GAIA_ROOM = true;
				if (initialServer != null) {
					initialServer.Disposed();
				}
				if (runningMSGThread != null) {
					runningMSGThread.Abort();
				}
				if (dmkCurt!=null) {
					dmkCurt.Close();
				}
				if (biliDanmu!=null) {
					biliDanmu.Close();
				}
				
				Application.Exit();
				
			} catch {
				
			}
			
//			2.danmuProvider.StopDanmu();
		}


		void RichTextBox1MouseDown(object sender, MouseEventArgs e)
		{
			StopMessge = true;
			lbl_NewMsg.Visible = true;
			
		}
		void RichTextBox1MouseUp(object sender, MouseEventArgs e)
		{
			StopMessge = false;
			foreach (var element in listMsg) {
				AppendNewMessageToList(element);
			}
			lbl_NewMsg.Visible = false;
			MessgeCount = 0;
			listMsg.Clear();
			lbl_NewMsg.Text = "咩有新消息";
		}



		/// 获取当前时间戳
		/// </summary>
		/// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
		/// <returns></returns>
		public static string GetTimeStamp(bool bflag = true)
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			string ret = string.Empty;
			if (bflag)
				ret = Convert.ToInt64(ts.TotalSeconds).ToString();
			else
				ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();
			
			return ret;
		}
		void btn_SaveDanmu(object sender, EventArgs e)
		{
			DateTime dt = DateTime.Now;
			string FilePath = dt.Year + "-" + dt.Month + "-" + dt.Day + "-" + dt.Hour.ToString() + "@" + dt.Minute.ToString() + "@" + dt.Second;
			
			FilePath = "./" + FilePath + "danmu.xml";
			ShowMessage("开始生成文件,可能会非常耗时，请耐心等待");
			GenerateXMLFile(FilePath);
			ShowMessage("生成文件完成");
			
		}
		public   void GenerateXMLFile(string fileName)
		{
			try {
				//初始化一个xml实例
				XmlDocument myXmlDoc = new XmlDocument();
				
				myXmlDoc.LoadXml("<?xml version=\"1.0\"?><i xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></i>");
				
//				 //找到Data节点，并在该节点下创建ProjectInfo节点
				XmlNode rootElement = myXmlDoc.SelectSingleNode("/i");    //查找单个节点
				//            XmlNode projInfoNode = myXmlDoc.CreateElement("ProjectInfo");
				//            rootNode.AppendChild(projInfoNode);
//
				

				//初始化第一层的第一个子节点
				XmlElement firstLevelElement1 = myXmlDoc.CreateElement("chatserver");
				firstLevelElement1.InnerText = "chat.bilibili.com";
				//填充第一层的第一个子节点的属性值（SetAttribute）
//				firstLevelElement1.SetAttribute("ID", "11111111");
//				firstLevelElement1.SetAttribute("Description", "Made in China");
				//将第一层的第一个子节点加入到根节点下
				rootElement.AppendChild(firstLevelElement1);
				
//				<chatid>10000</chatid>
				//  <mission>0</mission>
				//  <maxlimit>8000</maxlimit>
				//  <source>e-r</source>
				//  <ds>931869000</ds>
				//  <de>937654881</de>
				//  <max_count>8000</max_count>
				XmlElement secondLevelElement12 = myXmlDoc.CreateElement("chatid");
				secondLevelElement12.InnerText = "10000";
				rootElement.AppendChild(secondLevelElement12);
				XmlElement secondLevelElement13 = myXmlDoc.CreateElement("mission");
				secondLevelElement13.InnerText = "0";
				rootElement.AppendChild(secondLevelElement13);
				XmlElement secondLevelElement14 = myXmlDoc.CreateElement("maxlimit");
				secondLevelElement14.InnerText = "8000";
				rootElement.AppendChild(secondLevelElement14);
				XmlElement secondLevelElement15 = myXmlDoc.CreateElement("source");
				secondLevelElement15.InnerText = "e-r";
				rootElement.AppendChild(secondLevelElement15);
				XmlElement secondLevelElement16 = myXmlDoc.CreateElement("max_count");
				secondLevelElement16.InnerText = "8000";
				rootElement.AppendChild(secondLevelElement16);
				foreach (var element in StringMsglist) {
					string msg = element.Split('$')[0];
					string time = element.Split('$')[1];
					int Timespan = int.Parse(time) - timeStart;
					XmlElement message = myXmlDoc.CreateElement("d");
					message.SetAttribute("p", Timespan + ",1,25,16777215," + time + ",0,0,0");
					message.InnerText = msg;
					rootElement.AppendChild(message);
					
				}
				

				//将xml文件保存到指定的路径下
				myXmlDoc.Save(fileName);
			} catch (Exception ex) {
				ShowMessage("生成文件出错  :" + ex.Message);
				Console.WriteLine(ex.ToString()+ex.StackTrace);
			}
		}
		private static void AddXmlInformation(string xmlFilePath)
		{
			try {
				XmlDocument myXmlDoc = new XmlDocument();
				myXmlDoc.Load(xmlFilePath);
				//添加一个带有属性的节点信息
				foreach (XmlNode node in myXmlDoc.FirstChild.ChildNodes) {
					XmlElement newElement = myXmlDoc.CreateElement("color");
					newElement.InnerText = "black";
					newElement.SetAttribute("IsMixed", "Yes");
					node.AppendChild(newElement);
				}
				//保存更改
				myXmlDoc.Save(xmlFilePath);
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString()+ex.StackTrace);
			}
		}
		void MainFormLoad(object sender, EventArgs e)
		{
			
			
			
//		danmuProvider=new DanmuProvider();
//			danmuProvider.StartDanmu();
//			danmuProvider.ShowMessage+=ShowMessage;
		}
		bool ShowDanmu = false;
		void CheckDeskDanmuCheckedChanged(object sender, EventArgs e)
		{
			
			
		}
		
		void btn_DeskDanmuClick(object sender, EventArgs e)
		{
			
//			deskWindow=new Window1();
//			deskWindow.context= System.Windows.Application.Current;
//			deskWindow.Show();
//			ShowDanmu=true;
			ShowDanmu=!ShowDanmu;
			if (ShowDanmu) {
				button1.Text="关闭弹幕";
			}
			else
			{
				button1.Text="桌面弹幕";
			}
			if (dmkCurt==null&&ShowDanmu) {
				dmkCurt=new DanmakuCurtain(false);
				dmkCurt.Show();
			}
			else
			{
				dmkCurt.Close();
				dmkCurt=null;
			}
			
		}
		public  void ShootDanmaku(string text) {
			//         Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
			//                dmkCurt.Shoot(text);
//
			//            }));
			dmkCurt.Shoot(text);
			//Thread NetServer = new Thread(new ThreadStart(()=>dmkCurt.Shoot(text)));
			//      NetServer .SetApartmentState(ApartmentState.STA);
			//      NetServer .IsBackground = true;
//
			//      NetServer.Start();
		}
//		Thread showLeftDanmu=null;
		void LeftBiliDanmuCheckCheckedChanged(object sender, EventArgs e)
		{
			if (LeftBiliDanmuCheck.Checked&&biliDanmu==null) {
//				showLeftDanmu=new Thread(
//					new ThreadStart(()=>
//					                {
				biliDanmu=new MainOverlay();
				biliDanmu.OpenOverlay();
				biliDanmu.Show();
//					                }
//					               ));
//				showLeftDanmu.Start();
				
			}
			else{
				biliDanmu.Close();
				biliDanmu=null;
//				showLeftDanmu.Abort();
//				showLeftDanmu=null;
			}
			
		}
		
		
		

	}
}
