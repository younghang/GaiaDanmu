/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/24
 * Time: 15:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of InitailServer.
	/// </summary>
	///
	public delegate  void   ShowInWnd(string str) ;
	public class InitailServer
	{
		public InitailServer(RoomDetail room)
		{
			if (room != null) {
				this.room = room;
			}
			th2.Elapsed += TimerToConnect;
			th2.Interval = 1000 * 20;
			th2.Start();
		}
		public   bool MESSAGE = false;
		public event ShowInWnd ShowMessage;
		RoomDetail room;
		Socket serverSocket;
		public   int myPort = 15010;
		//端口
		public void ConnectServer(byte[] datas, bool beat = false)
		{
//			if (!beat)
//				ShowMessage("与服务器通信。。。");
			

			IPAddress ip = IPAddress.Parse(room.ServerIP);
			
			if (serverSocket == null) {
				serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				try {
					serverSocket.Connect(new IPEndPoint(ip, myPort)); //配置服务器IP与端口
					ShowMessage("连接服务器成功");

				} catch (Exception e) {
					ShowMessage("error::"+e.Message + "连接服务器失败，请退出！");
					return;
					
				}
			}
			
			//通过 serverSocket 发送数据
			
			try {
				
				string sendMessage = "" + DateTime.Now;
				serverSocket.Send(datas);
				if (!beat)
					ShowMessage("向服务器发送消息成功：" + sendMessage);
			} catch (Exception e) {
				ShowMessage("error::向服务器发送消息Failed!   请关闭软件后，尝试重新连接");
//				serverSocket.Shutdown(SocketShutdown.Both);
//				serverSocket.Close();
				
			}
			//等待300m秒钟
			Thread.Sleep(300);
			//通过clientSocket接收数据
			
			
		}
		public void GetResponse()
		{
			
			if (serverSocket == null) {
				ShowMessage("Socket 没有了");
				return;
			}
			
			byte[] buffer = new byte[1024 * 2];
			try {
				if (serverSocket.Available == 0) {
					
					ShowMessage("没有收到消息");
					return;
				}
			} catch {
				ShowMessage("error::连接异常，可能登陆出现问题,请关闭后重试");
				return;
			}
			
			if (serverSocket == null) {
				ShowMessage("socket没了");
				return;
			}
			int receiveLength = serverSocket.Receive(buffer);
			if (receiveLength == 0) {
				ShowMessage("没有收到消息");
				return;
			}

			try {
				
				byte[] bs = new byte[4];
				for (int i = 0; i < 4; i++) {
					bs[i] = buffer[6 + i];
				}
				
				int size = System.BitConverter.ToInt32(bs, 0);
//				ShowMessage("DataSize:" + size + "字节");
				byte[] data = new byte[size];
				for (int i = 0; i < data.Length; i++) {
					data[i] = buffer[i + 12];
				}
				String strJson = System.Text.Encoding.UTF8.GetString(data);

				JObject jsonObject = JObject.Parse(strJson);
				
				ShowMessage(jsonObject.ToString());
				

			} catch (Exception e) {
				// TODO Auto-generated catch block
				ShowMessage("error::"+e.Message + e.StackTrace);
			}
			
			
		}
		public void ConnectServerStep1()
		{

			ShowMessage("向服务器发送消息1");
			JObject jsonObject = new JObject();
			jsonObject.Add("cmdid", "svrisokreq");
			SendJsonToServer(jsonObject, (byte)0xe8, (byte)0x03);
			GetResponse();
		}

		public void ConnectServerStep2()
		{
			ShowMessage("向服务器发送消息2");
			JObject jsonObject = new JObject();
			jsonObject.Add("cmdid", "getdefaultuc");
			jsonObject.Add("roomid", room.roomid);
			jsonObject.Add("vod", 0);
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			
			GetResponse();
		}
		

		public void ConnectServerStep3(string loginstring)
		{
			Random ran = new Random();
			int randon = ran.Next(1001, 9909);
			
			ShowMessage("向服务器发送消息3:login");
//			String loginstring = "{\"uid\":" + room.data.uid + ",\"os\":\"4.4.2\","
//			                     + "\"sid\":\" " + room.data.sid + "\",\"model\":\"NX403A\","
//			                     + "\"nickname\":\"LoveGaia\",\"imei\":\"zhanqi863"+randon+"685453806\",\"cmdid\":\"loginreq\",\"ver\":\"2\","
//
//			                     + "\"timestamp\":" + room.data.timestamp + "," +
//				"\"roomid\":" + room.roomid + ",\"fhost\":\"android\",\"t\":0,\"r\":0,\"device\":1,"
//			                     + "\"gid\":" + room.data.gid + ","
//				+ "\"ssid\":\""+room.GetSSID()+"\",\"roomdata\":{\"slevel\":{"
//			                     + "\"uid\":\"" + room.data.uid + "\",\"opp\":0,\"curexp\":\"2958\",\"levelexp\":\"0\",\"type\":\"0\",\"consume\":\"2958\",\"pos\":\"0\",\"level\":\"0\","
//			                     + "\"etime\":\"1448896181\",\"name\":\"\",\"nextexp\":\"15000\",\"leftexp\":\"12042\",\"nextname\":\"青铜5\",\"keep\":\"\"},\"vdesc\":\"\",\"vlevel\":0}}";

			//android 2.6.2 匿名
			if (string.IsNullOrEmpty(loginstring))
				loginstring = "{\"uid\":" + room.data.uid + ",\"os\":\"4.4.2\"," +
					"\"sid\":\"" + room.data.sid + "\",\"model\":\"NX403A\",\"nickname\":\"\"," +
					"\"imei\":\"zhanqi8633" + randon + "2153806\",\"cmdid\":\"loginreq\",\"ver\":\"2\"," +
					"\"chatroomid\":" + room.chatRoomID + "," +
					"\"timestamp\":" + room.data.timestamp + "," +
					"\"roomid\":" + room.roomid + ",\"fhost\":\"android\",\"t\":0,\"r\":0,\"device\":1," +
					"\"gid\":" + room.data.gid + "," +
					"\"ssid\":\"" + room.GetSSID() + "\"," +
					"\"roomdata\":{\"slevel\":[],\"vdesc\":\"\",\"vlevel\":0}}";
			
//			string loginstring =	"{\"nickname\":\"\"," +
//				"\"fhost\":\"android\","+
//			                     "\"gid\":" + room.data.gid + ",\"cmdid\":\"loginreq\"," +
//			                     "\"roomid\":" + room.roomid + "," +
//			                     "\"timestamp\":" + room.data.timestamp + "," +
//
//				"\"ssid\":\"" + room.GetSSID() + "\",\"hideslv\":0,\"fhost\":\"\",\"roomdata\":{\"vlevel\":0,\"vdesc\":\"\",\"slevel\":[]},\"tagid\":0,\"imei\":\"3554238337\"," +
//			                     "\"sid\":\"" + room.data.sid + "\"," +
//				"\"ver\":\"1\",\"t\":0,\"thirdaccount\":\"\",\"fx\":0," +
//			                     "\"uid\":" + room.data.uid + "}";
			
//			loginstring = "{\"nickname\":\"\",\"gid\":1709251379,\"cmdid\":\"loginreq\",\"roomid\":49577,\"timestamp\":1464164530,\"ssid\":\"YTExMTgyMzM2MTQ2M2E0ZDNjN2QzMmQzZTdmNDdkNDE=\",\"hideslv\":0,\"fhost\":\"\",\"roomdata\":{\"vlevel\":0,\"vdesc\":\"\",\"slevel\":[]},\"tagid\":0,\"imei\":\"3554238337\",\"sid\":\"MGU3YjVkYTZiNzAyNDRjMmE2MDQ4YjRiNThmMWNkZDE=\",\"ver\":12,\"t\":0,\"thirdaccount\":\"\",\"fx\":0,\"uid\":0}";
			
			JObject jsonObject = (JObject)JObject.Parse(loginstring);
//			ShowMessage(jsonObject.ToString());
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			
			GetResponse();
			
			
		}
		public void ConnectServerStep4()
		{
			ShowMessage("向服务器发送消息4");
			JObject jsonObject = new JObject();
			jsonObject.Add("cmdid", "cdn");
			jsonObject.Add("swrate", 0);
			jsonObject.Add("swline", 4);
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			
			
		}
		
		public void ConnectServerStep5(String Message)
		{
			
			SendJsonToServer(null, (byte)0x59, (byte)0x27);
			ShowMessage("向服务器发送消息:聊天");
			JObject jsonObject = new JObject();
			jsonObject.Add("stype", 0);
			jsonObject.Add("content", Message);
			jsonObject.Add("useemot", "#13");
//		JSONArray localJSONArray = new JSONArray();
//		JObject localJSONObject2 = new JObject();
//		localJSONObject2.Add("color", "red");
//		localJSONArray.add(localJSONArray);
			jsonObject.Add("style", null);
			jsonObject.Add("vlevel", 1);
			jsonObject.Add("permission", 0);
			jsonObject.Add("usexuanzi", 4);
			jsonObject.Add("showmedal", 1);
			jsonObject.Add("vdesc", "");
			jsonObject.Add("time", 0);
			jsonObject.Add("speakinroom", 0);
			jsonObject.Add("cmdid", "chatmessage");
			jsonObject.Add("level", 11);
			jsonObject.Add("toid", 0);
			jsonObject.Add("richertitle", 2);
			jsonObject.Add("slevel", 1);
			jsonObject.Add("fromid", room.data.uid);
//			RUN_CONNECTION=false;
//			if (th!=null) {
//				th.Stop();
//			}
			
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			Thread.Sleep(300);
			GetResponse();
//			RUN_CONNECTION=true;
//			if (th!=null&&!th.Enabled) {
//				th.Start();
//			}
		}
		//		System.Timers.Timer th = null;
		System.Timers.Timer th2 = new System.Timers.Timer();
		Thread messageThread = null;
		public void RunConnection()
		{
//			if (th == null || !th.Enabled) {
//				th = new System.Timers.Timer();
//				th.Elapsed += GetMessage;
//				th.Interval = 300;
//				th.Start();
//			}
			if (messageThread == null || !messageThread.IsAlive) {
				messageThread = new Thread(new ThreadStart(GetMessage));
				messageThread.Start();
			}
			
			
		}
		public   bool RUN_CONNECTION = true;
		void TimerToConnect(object sender, ElapsedEventArgs e)
		{
			if (!RUN_CONNECTION) {
				th2.Stop();
			}
			SendJsonToServer(null, (byte)0x59, (byte)0x27, true);
		}
		void GetMessage()
		{
			try {
				
				while (true) {
					
					if (!RUN_CONNECTION) {
//				th.Stop();
						break;
						
					}
					
					byte[] buffer = new byte[1024 * 2 ];
					int receiveLength = serverSocket.Receive(buffer, 0, 1024 * 2, SocketFlags.None);
					if (receiveLength == 0) {
						ShowMessage("没有收到消息");
						return;
					}
					Thread messageDealThread=new Thread(new ParameterizedThreadStart(GetChatResponse));
					messageThread.Start(buffer);
//					GetChatResponse(buffer);
				}
				
				
			} catch (Exception) {
//				ShowMessage("聊天连接异常，请退出");
//				Disposed();
				return;
			}
			
			
			
		}
		void GetChatResponse(object messagebyte )
		{
			byte[]buffer=messagebyte as byte[];

			try {
				
				
				byte[] bs = new byte[4];
				for (int i = 0; i < 4; i++) {
					bs[i] = buffer[6 + i];
				}
				
				int size = System.BitConverter.ToInt32(bs, 0);
//				ShowMessage("DataSize:" + size + "字节");
				byte[] data = new byte[size];
				for (int i = 0; i < data.Length; i++) {
					data[i] = buffer[i + 12];
				}
				String strJson = System.Text.Encoding.UTF8.GetString(data);

				JObject jsonObject = JObject.Parse(strJson);
				
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
						ShowInfo = slevels +fangguan + strlevel + " :" +  name + "::" + content + "    " + ip;
//							ShowInfo = jsonObject.ToString();
						break;
					case "Gift.Use":
						string nickname = (string)jsonObject["data"]["nickname"];
						int count = (int)jsonObject["data"]["count"];
						int roomId = (int)jsonObject["data"]["roomid"];
						if (roomId != room.roomid) {
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
				
				
				

			} catch (OutOfMemoryException) {
				ShowMessage("error::消息格式出错");
			} catch (Exception e) {
				// TODO Auto-generated catch block
//				ShowMessage(e.Message );
				ShowMessage("error::接收消息出错");
			}
			
			
		}
		void SendJsonToServer(JObject localJSONObject1, byte byte10, byte byte11, bool beat = false)
		{
			int len = 0;
			byte[] arrayOfByte1 = null;
			if (localJSONObject1 != null) {
				String jsonstring = localJSONObject1.ToString();
				string[] fstr = jsonstring.Split('\n', '\r', ' ');
				string sf = "";
				for (int ff = 0; ff < fstr.Length; ff++) {
					sf += fstr[ff];
				}
				jsonstring = sf;
				arrayOfByte1 = Encoding.UTF8.GetBytes(jsonstring);
				len = arrayOfByte1.Length;
			} else
				len = 0;
			

			
			// arrayOfByte2=arrayOfByte1;
			byte[] arrayOfByte2 = new byte[12 + len];

			arrayOfByte2[1] = 0xcc;
			arrayOfByte2[0] = 0xbb;
			fromIntToByteArray(arrayOfByte2, 2, 4, 0);
			fromIntToByteArray(arrayOfByte2, 6, 4, len);
			arrayOfByte2[10] = byte10;
			arrayOfByte2[11] = byte11;
			// fromIntToByteArray(arrayOfByte2, 10, 2, 10000);
			// InsertBytsToByteArray(arrayOfByte2, 10, ShortToBytes(10000));
			// arrayOfByte2[]
			int i = 0;
			while (i < len) {
				if (len == 0) {
					break;
				}
				arrayOfByte2[(i + 12)] = arrayOfByte1[i];
				i += 1;
			}
			ConnectServer(arrayOfByte2, beat);

		}
		static public void fromIntToByteArray(byte[] paramArrayOfByte, int paramInt1, int paramInt2, int paramInt3)
		{

			int j = 0;
			int i = paramInt1;
			for (;;) {
				if (i >= paramInt1 + paramInt2) {
					return;
				}
				paramArrayOfByte[i] = ((byte)(paramInt3 >> j & 0xFF));
				j += 8;
				i += 1;
			}
		}
		public  void Disposed()
		{
			try {
//				th.Stop();
				th2.Stop();
				RUN_CONNECTION = false;
				messageThread.Interrupt();
				serverSocket.Close();
			} catch (Exception e) {
				ShowMessage("error::"+e.Message);
			}
			
		}

		
	}
}

// bool isWatch = true;
//
//        #region 1.被线程调用 监听连接端口
//        /// <summary>
//        /// 被线程调用 监听连接端口
//        /// </summary>
//        void StartWatch()
//        {
//            while (isWatch)
//            {
//                //threadWatch.SetApartmentState(ApartmentState.STA);
//                //监听 客户端 连接请求，但是，Accept会阻断当前线程
//                Socket sokMsg = sokWatch.Accept();//监听到请求，立即创建负责与该客户端套接字通信的套接字
//                ConnectionClient connection = new ConnectionClient(sokMsg, ShowMsg, RemoveClientConnection);
//                //将负责与当前连接请求客户端 通信的套接字所在的连接通信类 对象 装入集合
//                dictConn.Add(sokMsg.RemoteEndPoint.ToString(), connection);
//                //将 通信套接字 加入 集合，并以通信套接字的远程IpPort作为键
//                //dictSocket.Add(sokMsg.RemoteEndPoint.ToString(), sokMsg);
//                //将 通信套接字的 客户端IP端口保存在下拉框里
//                cboClient.Items.Add(sokMsg.RemoteEndPoint.ToString());
//                ShowMsg("接收连接成功......");
//                //启动一个新线程，负责监听该客户端发来的数据
//                //Thread threadConnection = new Thread(ReciveMsg);
//                //threadConnection.IsBackground = true;
//                //threadConnection.Start(sokMsg);
//            }
//        }
//        #endregion
