/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/6/7
 * Time: 19:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using ZQDanmuTest;

namespace GaiaDanmu
{
	/// <summary>
	/// Description of DanmuProvider.
	/// </summary>
	public class DanmuProvider
	{
		ManualResetEvent roomFinish = new ManualResetEvent(false);
		ManualResetEvent gaiaroomFinish = new ManualResetEvent(false);
		InitialRoomDetial initialroom;
		InitialGaiaRoom initialGaiaroom;
		Thread initailSidThread;
		Thread runningMSGThread;
 
		InitailServer initialServer = null;
		public DanmuProvider()
		{
			
		}
		public void StartDanmu()
		{
			StartInitial();
		}
		public void StopDanmu()
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
			// disable once EmptyGeneralCatchClause
			} catch(Exception) {
				
			}
		}
		void StartInitial()
		{
			
			//主线程在这里启动了三个线程，一个等待检测其他两个完成，两个用于初始化相关数据
			
			ShowSysMessage("初始化用户认证的数据。。。");
			initialroom = new InitialRoomDetial(ref  roomFinish);
			initialGaiaroom = new InitialGaiaRoom(ref gaiaroomFinish);
			if (initailSidThread == null || !initailSidThread.IsAlive)
				initailSidThread = new Thread(new  ThreadStart(InitialRoom));
			initailSidThread.Start();
			
		}
		public event ShowInWnd ShowMessage;

		void ShowSysMessage(string str)
		{
			 if (ShowMessage!=null) {
				ShowMessage(str);
			 }
		}
		public virtual void  ShowDanmu(JObject jsonObject)
		{
			 
			try {			
					 
				string ShowInfo = "";
				string cmdid = (string)jsonObject.GetValue("cmdid");
				switch (cmdid) {
					case "chatmessage":
						string name = (string)jsonObject.GetValue("fromname");
						string content = (string)jsonObject.GetValue("content");
						int sLevel = 0;
						if (jsonObject.GetValue("slevel") != null) {
							sLevel = (int)jsonObject.GetValue("slevel");
						}
						

						string strlevel = "";
						string ip = (string)jsonObject.GetValue("ip");
						int level;
						string slevels = "";
						string sPhone = "";
						if (jsonObject.GetValue("extra") != null) {
							sPhone = "▌";
						}
						if (sLevel > 15) {
							slevels = "♢";
						}
						
						if (jsonObject.GetValue("level") != null) {
							level = (int)jsonObject["level"];
							strlevel = "【御狐" + level.ToString() + "级】" + sPhone;
						} else
							strlevel = "";
						int permission = (int)jsonObject.GetValue("permission");
						string fangguan = "";
						if (permission == 10) {
							fangguan = "[房管]";
						}
						ShowInfo = slevels + fangguan + strlevel + " :" + name + "::" + content + "    " + ip;
//							ShowInfo = jsonObject.ToString();
						break;
					case "Gift.Use":
						string nickname = (string)jsonObject["data"]["nickname"];
						int count = (int)jsonObject["data"]["count"];
						int roomId = (int)jsonObject["data"]["roomid"];
						if (roomId != 52320) {
							ShowInfo = "";
							break;
						}
						
						JObject json = (JObject)jsonObject.GetValue("data");
						if (json.GetValue("level") != null) {
							level = (int)jsonObject["data"]["level"];
							strlevel = "御狐" + level.ToString() + "级";
						} else
							strlevel = "";
						
						name = (string)jsonObject["data"]["name"];
						if (name == "大宝剑") {
							ShowInfo = jsonObject.ToString();
							break;
						}
						
						ShowInfo = nickname + " :" + strlevel + " \t#####：:送的" + name + ":X" + count + "               ";
						break;
					case "Level.FwList":
						
						ShowInfo = "";
						break;
					case "Gift.AprilSpecial":
						
						ShowInfo = "";
						break;
					case "timegiftupdate":
						
						ShowInfo = "";
						break;
					case "Level.Fans":
						ShowInfo = "";
						break;
					case "useronline":
						ShowInfo = "";
						break;
					case "timegiftbro":
						string	gname = (string)jsonObject.GetValue("gname");
						name = (string)jsonObject.GetValue("name");
						count = (int)jsonObject.GetValue("cnt");
						string unit = (string)jsonObject.GetValue("unit");
						ShowInfo = name + "送给大哥" + count + unit + gname;
						break;
						
					case "Car.Show":
						ShowInfo = "";
						break;
					case "getuc":
						ShowInfo = "";
						break;
					case "thirdchatmsg":
						ShowInfo = "";
						break;
					case "blockusernotify":
						string blockname = (string)jsonObject.GetValue("blockname");
						ShowInfo = blockname + "被禁言" + "               ";
						break;
					case "notefanslevel":
						string fansname = (string)jsonObject["data"]["fansname"];
						int fanslevel = (int)jsonObject["data"]["fanslevel"];
						ShowInfo = fanslevel + "级:" + fansname + "进入直播间                 ";
						break;
					case "live":
						ShowInfo = "";
						break;
					case "system":
						ShowInfo = "";
						break;
					case "sysmsg":
						content = (string)jsonObject.GetValue("content");
						ShowInfo = content;
						break;
					case "Gift.Show":
						nickname = (string)jsonObject["data"]["nickname"];
						string action = (string)jsonObject["data"]["action"];
						count = (int)jsonObject["data"]["count"];
						string classifier = (string)jsonObject["data"]["classifier"];
						name = (string)jsonObject["data"]["name"];
						ShowInfo = nickname + "给主播" + action + count + name + "               ";
						break;
					case "firebro":
						string code = (string)jsonObject.GetValue("code");
						string slevel = (string)jsonObject.GetValue("slevel");
						
						string roomid = ((int)jsonObject.GetValue("roomid")).ToString();
						string url = "http://www.zhanqi.tv/" + code + "$" + roomid;
						ShowInfo = "这里有烟花" + url + "               ";
						break;
					default:
						ShowInfo = jsonObject.ToString();
						break;
						
				}
				if (ShowInfo != "") {
					ShowMessage(ShowInfo);
				}
				
				return;
		
			}
			catch(Exception){
				ShowSysMessage("error::获取弹幕出错了");
			}
			
			
		}

		void InitialRoom()
		{
 
			ShowSysMessage("等待中");
			roomFinish.WaitOne();
			gaiaroomFinish.WaitOne();
			initialGaiaroom.FINISH_INITIAL_GAIA_ROOM = false;
			initialroom.FINISH_INITIAL_ROOM = false;
			ShowSysMessage("初始化相关信息完成");
			//显示一下
			RoomDetail room = initialroom.GetRoomDetail();
			string showinfo = "gid:" + room.data.gid + "\nuid:" + room.data.uid + "\ntimestamp:" + room.data.timestamp + "\nsid:" + room.data.sid;
			ShowSysMessage(showinfo);
			string showGaiaroomInfo = initialGaiaroom.GetJsonGaiaRoom().ToString();
//			ShowMessage(showGaiaroomInfo);
//			ShowMessage("ip:" + initialGaiaroom.chatip);
			ShowSysMessage("port:" + initialGaiaroom.chatport);
			
			//启动
			StartRunning();
		}
		void StartRunning()
		{
			ShowSysMessage("开始连接弹幕服务器。。。");
			if (runningMSGThread == null || !runningMSGThread.IsAlive) {
				runningMSGThread = new Thread(new  ThreadStart(TestConnectionServer));
				runningMSGThread.Start();
			}
		}
		void TestConnectionServer()
		{
			RoomDetail room = initialroom.GetRoomDetail();
			
			room.ServerIP = initialGaiaroom.chatip;
			room.roomid = initialGaiaroom.GetGaiaRoomDetail().data.id;
			room.chatRoomID = initialGaiaroom.chatRoomid;
			
			initialServer = new InitailServer(room);
			initialServer.ShowMessage += ShowSysMessage;
			initialServer.ShowDanmuMessage+=ShowDanmu;
			initialServer.ConnectServerStep1();
			initialServer.ConnectServerStep2();
			string login = GetLoginString();
			initialServer.ConnectServerStep3(login);
			initialServer.ConnectServerStep4();
			if (login != "") {
				ShowSysMessage("登陆文件存在，尝试用户登陆");
				initialServer.SendDanmuToServer("0.0");
			}		
 
			initialServer.RunConnection();
			
		}
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
		
		
		
		
	}
	
}
