/*
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
using Newtonsoft.Json.Linq;
using GaiaDanmu;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public static bool RUNNING = true;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			StartInitial();
			
//			Data data=new Data();
//		data.uid=0;
//			data.gid=1861100490;
//			 data.timestamp=1464180075;
//			 RoomDetail rom=new RoomDetail();
//			 rom.data=data;
//			 richTextBox1.Text= rom.GetSSID();
			
			
			
			
			
			
			
		}
		InitialRoomDetial initialroom;
		InitialGaiaRoom initialGaiaroom;
		Thread initailSidThread;
		Thread runningMSGThread;
		Thread checkUpdateThread;
		int MessgeCount=0;
		bool StopMessge;
		static int DanmuVersion=12;
		List<string> listMsg=new List<string>();
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
			if (checkUpdateThread==null||!checkUpdateThread.IsAlive) {
				checkUpdateThread=new Thread(new ThreadStart(()=>{
				                                             	try{
				                                             		int NewVersion= int.Parse(new WorkLogin().GetUpdate());
				                                             		if (NewVersion>DanmuVersion) {
				                                             			MessageBox.Show("有新版本，请去贴吧下载，扬co发的女王那个贴子");
				                                             			ShowMessage("有新版本，请去贴吧下载，扬co发的女王那个贴子");
				                                             		}
				                                             		else
				                                             			ShowMessage("没有更新");
				                                             	}
				                                             	catch{ShowMessage("检查更新失败");}
				                                             }));
			}
			checkUpdateThread.Start();
			//主线程在这里启动了三个线程，一个等待检测其他两个完成，两个用于初始化相关数据
			
			ShowMessage("初始化用户认证的数据。。。");
			initialroom = new InitialRoomDetial();
			initialGaiaroom = new InitialGaiaRoom();
			if (initailSidThread == null || !initailSidThread.IsAlive)
				initailSidThread = new Thread(new  ThreadStart(InitialRoom));
			initailSidThread.Start();
			
		}
		void InitialRoom()
		{
			while (!initialroom.FINISH_INITIAL_ROOM || !initialGaiaroom.FINISH_INITIAL_GAIA_ROOM) {
				
			}
			initialGaiaroom.FINISH_INITIAL_GAIA_ROOM = false;
			initialroom.FINISH_INITIAL_ROOM = false;
			ShowMessage("初始化相关信息完成");
			//显示一下
			RoomDetail room = initialroom.GetRoomDetail();
			string showinfo = "gid:" + room.data.gid + "\nuid:" + room.data.uid + "\ntimestamp:" + room.data.timestamp + "\nsid:" + room.data.sid;
			ShowMessage(showinfo);
			string showGaiaroomInfo = initialGaiaroom.GetJsonGaiaRoom().ToString();
//			ShowMessage(showGaiaroomInfo);
//			ShowMessage("ip:" + initialGaiaroom.chatip);
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
				initialServer.ConnectServerStep5("0.0");
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
		FireWorksList fl = null;
		void AppendNewMessageToList(string x)
		{
			int pos = x.IndexOf(':');
			int pos2 = x.IndexOf("::");
			int len = richTextBox1.Text.Length;
			if (!IPCheck.Checked && x.Contains(".")) {
				x = x.Substring(0, x.Length - 15);
			}
			if (x.Contains("www.zhanqi.tv")) {
				string rooid=x.Split('$')[1];
				x=x.Split('$')[0];
				
				if(fireCheck.Checked){
					if (fl==null) {
						fl=new FireWorksList();
						fl.Show();
					}
					fl.AddRoomid(rooid);
				}
			}
			
			this.richTextBox1.AppendText(x + "\n");
			if (x.Contains("::")) {
				richTextBox1.Select(len, pos);
				richTextBox1.SelectionColor = Color.LimeGreen;
				richTextBox1.Select(len+pos,  pos2-pos);
				richTextBox1.SelectionColor = Color.Blue;
				richTextBox1.Select(len+pos2,  x.Length-pos2);
				richTextBox1.SelectionColor = Color.Red;
			}else
				if (x.Contains("error::")) {
				richTextBox1.Select(len,  x.Length);
				richTextBox1.SelectionColor = Color.Red;
			}else

				if (x.Contains("###")) {
				richTextBox1.Select(len,  x.Length);
				richTextBox1.SelectionColor = Color.Fuchsia;
			}else
				if (x.Contains("进入直播间")) {
				richTextBox1.Select(len,  x.Length);
				richTextBox1.SelectionColor = Color.DarkOrange;
			}
			richTextBox1.SelectionColor = Color.Black;
			
			richTextBox1.SelectionBackColor = Color.White;
			richTextBox1.HideSelection = false;
			
			
			
		}
		void ShowMessage(string str)
		{
			if (StopMessge) {
				MessgeCount++;
				listMsg.Add(str);
				try{
					if (lbl_NewMsg.InvokeRequired) {
						// 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
						Action<string> actionDelegate =(x)=>{lbl_NewMsg.Text="新消息"+MessgeCount;};
						// 或者
						// Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
						this.lbl_NewMsg.Invoke(actionDelegate, str);
					} else {
						this.lbl_NewMsg.Text= str;
					}
				} catch {
					ShowMessage("error::显示新信息条数出错");
				}
			}
			
			
			else{
				try {
					if (richTextBox1.InvokeRequired) {
						// 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
						Action<string> actionDelegate =AppendNewMessageToList;
						// 或者
						// Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
						this.richTextBox1.Invoke(actionDelegate, str);
					} else {
						this.richTextBox1.AppendText( str + "\n");
					}
				} catch {
					ShowMessage("error::显示信息出错");
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
				btn_Send.Text = "停止";
				StartInitial();
			} else {
				info = "停止接收";
				btn_Send.Text = "启动";
			}
			
			ShowMessage(info);
			RUNNING = !RUNNING;
		}


		void Btn_SendMessageClick(object sender, EventArgs e)
		{
			
			if (initailSidThread != null && initailSidThread.IsAlive) {
				ShowMessage("线程还活着");
			}
			
			if (initialServer == null) {
				ShowMessage("InitialServer 对象没有了");
				return;
			}
			initialServer.MESSAGE = true;
			strMessage = textBox1.Text;
			initialServer.ConnectServerStep5(strMessage);
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
			initialroom.FINISH_INITIAL_ROOM = true;
			initialGaiaroom.FINISH_INITIAL_GAIA_ROOM = true;
			if (initialServer != null) {
				initialServer.Disposed();
			}
			if (runningMSGThread != null) {
				runningMSGThread.Abort();
			}
			
		}


		void RichTextBox1MouseDown(object sender, MouseEventArgs e)
		{
			StopMessge=true;
			lbl_NewMsg.Visible=true;
			
		}
		void RichTextBox1MouseUp(object sender, MouseEventArgs e)
		{
			StopMessge=false;
			foreach (var element in listMsg) {
				AppendNewMessageToList(element);
			}
			lbl_NewMsg.Visible=false;
			MessgeCount=0;
			listMsg.Clear();
			lbl_NewMsg.Text="咩有新消息";
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


		#region MD5测试

		public  string   GetMD5(string   myString)
		{
			MD5 md5 = new   MD5CryptoServiceProvider();
			byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
			byte[] targetData = md5.ComputeHash(fromData);
			string byte2String = null;
			
			for (int i = 0; i < targetData.Length; i++) {
				byte2String += targetData[i].ToString("x");
			}
			
			return   byte2String;
		}
		public string GetMDfive(string s)
		{
			
			MD5 md = new MD5CryptoServiceProvider();
			byte[] ss = md.ComputeHash(Encoding.UTF8.GetBytes(s));
			return byteArrayToHexString(ss);
			
		}
		public  void TestMD5()
		{
			string a; //加密前数据
			a = 107616417 + 1798171239 + ">y,V4{{][$@s]qS3" + 1463742150;
			
			string b; //加密后数据
//			b = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(a, "MD5");
//			byte[] result = Encoding.Default.GetBytes(a.Trim());    //tbPass为输入密码的文本框
//			MD5 md5 = new MD5CryptoServiceProvider();
//			byte[] output = md5.ComputeHash(result);
//			b = BitConverter.ToString(output).Replace("-","");  //tbMd5pass为输出加密文本的文本框
			b = GetMDfive(a);
			b = b.ToLower();
			byte[] bytes = Encoding.Default.GetBytes(b);
			string c = Convert.ToBase64String(bytes);
			richTextBox1.Text = c;
		}

		private static string[] HexCode = {
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"a",
			"b",
			"c",
			"d",
			"e",
			"f"
		};


		public static string byteToHexString(byte b)
		{
			int n = b;
			if (n < 0) {
				n = 256 + n;
			}
			int d1 = n / 16;
			int d2 = n % 16;
			return HexCode[d1] + HexCode[d2];
		}

		public static String byteArrayToHexString(byte[] b)
		{
			String result = "";
			for (int i = 0; i < b.Length; i++) {
				result = result + byteToHexString(b[i]);
			}
			return result;
		}

		#endregion

	}
}
