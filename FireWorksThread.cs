/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/28
 * Time: 14:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Threading;
using ZQDanmuTest;

namespace GaiaDanmu
{
	/// <summary>
	/// Description of FireWorksThread.
	/// </summary>
	public class FireWorksThread
	{
		public FireWorksThread(string roomid)
		{
				RoomID=roomid;
				 
		}
		string RoomID="";
		InitialRoomDetial initialroom;
		InitialGaiaRoom initialGaiaroom;
		Thread initailSidThread;
		Thread runningMSGThread;

		void SendMessage(object sender, System.Timers.ElapsedEventArgs e)
		{
			 if (fireMsgCount>14) {
				timer.Stop();
				fireMsgCount=0;
				
			 }
//			initialServer.ConnectServerStep5("我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥");
			initialServer.ConnectServerStep5("0.0");
		}

		void StartRunning()
		{	
			ShowMessage("开始连接弹幕服务器。。。");			
			if (runningMSGThread == null || !runningMSGThread.IsAlive) {
				runningMSGThread = new Thread(new  ThreadStart(TestConnectionServer));
				runningMSGThread.Start();
			}
			timer=new System.Timers.Timer();
			timer.Interval=1000*25;
			timer.Elapsed+=SendMessage;
			timer.Start();
		}
		int fireMsgCount=0;
		public  void StartInitial()
		{
			//主线程在这里启动了三个线程，一个等待检测其他两个完成，两个用于初始化相关数据
			if (initailSidThread == null || !initailSidThread.IsAlive)
				initailSidThread = new Thread(new  ThreadStart(InitialRoom));
			initailSidThread.Start();
			ShowMessage("初始化用户认证的数据。。。");
			initialroom = new InitialRoomDetial();
			initialGaiaroom = new InitialGaiaRoom(true);
			initialGaiaroom.otherRoomID=RoomID;
		
		}
		void InitialRoom()
		{
			while (!InitialRoomDetial.FINISH_INITIAL_ROOM || !InitialGaiaRoom.FINISH_INITIAL_GAIA_ROOM) {
				
			}
			InitialGaiaRoom.FINISH_INITIAL_GAIA_ROOM = false;
			InitialRoomDetial.FINISH_INITIAL_ROOM = false;
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
		 System.Timers.Timer  timer;
	 
	 
		InitailServer initialServer = null;
	 
		  
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
		 		
			initialServer.ConnectServerStep3("");			 
			initialServer.ConnectServerStep4();		 
			initialServer.RunConnection();
 
		}
		public void ShowMessage(string str)
		{
			Console.WriteLine(str);
		}
	
		public void Close()
		{
			try
			{
				initialServer.Disposed();
				runningMSGThread.Abort();
				initailSidThread.Abort();
			}
			catch
			{
				
			}
		}
		 
		
		
		
	}
	 
}
