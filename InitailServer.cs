/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/24
 * Time: 15:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
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
	public delegate  void   DealMessageData(JObject str) ;
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
		public event DealMessageData ShowDanmuMessage;
		RoomDetail room;
		Socket serverSocket;
		public   int myPort = 15010;
		//端口
		public void SendBytesToServer(byte[] datas, bool beat = false)
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
					ShowMessage("error::" + e.Message + "连接服务器失败，请退出！");
					return;
					
				}
			}
			
			//通过 serverSocket 发送数据
			
			try {
				
				string sendMessage = "" + DateTime.Now;
				
//				serverSocket.Send(datas);
				serverSocket.BeginSend(datas, 0, datas.Length, SocketFlags.None, null, null);

				if (!beat)
					ShowMessage("向服务器发送消息成功：" + sendMessage);
			} catch (Exception e) {
				Thread.Sleep(1000 * 5);
				ShowMessage("error::向服务器发送消息Failed!   请关闭软件后，尝试重新连接");
//				serverSocket.Shutdown(SocketShutdown.Both);
//				serverSocket.Close();
				
			}
			//等待300m秒钟
			Thread.Sleep(200);
			//通过clientSocket接收数据
			
		}
		public void GetResponse()
		{
			
			if (serverSocket == null||!serverSocket.Connected) {
				ShowMessage("Socket 没有了");
				return;
			}
			
			if (serverSocket.Blocking) {
				StateObject state = new StateObject();
				state.workSocket = serverSocket;
				
				serverSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
				                          new AsyncCallback(GetCallBack), state);
			}
			
//			int receiveLength = serverSocket.Receive(buffer, 0, 1024 * 2, SocketFlags.None);
//			if (receiveLength == 0) {
//				ShowMessage("没有收到消息");
//				return;
//			}
			
		}
		void  GetCallBack(IAsyncResult ar)
		{
			try{
				
				// 从异步state对象中获取state和socket对象.
				StateObject state = (StateObject)ar.AsyncState;
				Socket handler = state.workSocket;

				// 从客户socket读取数据.
				
				int bytesRead = handler.EndReceive(ar);
				if (bytesRead > 0) {

					// 如果接收到数据，则存起来
					byte[] tmp = new byte[bytesRead];
					System.Buffer.BlockCopy(state.buffer, 0, tmp, 0, bytesRead);
					state.byteSource.AddRange(tmp);
					

				} else {

					// 接收未完成，继续接收.
					ShowMessage("error::接收出错");

				}
				
				//新建线程处理接收到的消息
				byte[] buffer = state.byteSource.ToArray();
				Thread messageDealThread = new Thread(new ParameterizedThreadStart(GetChatResponse));
				messageDealThread.Start(buffer);
				
				//下一次接收
				state.byteSource.Clear();
				serverSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
				                          new AsyncCallback(GetCallBack), state);
			}
			catch(Exception e)
			{
				ShowMessage("error::接收出错");
				return;
			}
			
			
		}
		public void ConnectServerStep1()		{

			ShowMessage("向服务器发送消息1");
			JObject jsonObject = new JObject();
			jsonObject.Add("cmdid", "svrisokreq");
			SendJsonToServer(jsonObject, (byte)0xe8, (byte)0x03);
		}

		public void ConnectServerStep2()
		{
			ShowMessage("向服务器发送消息2");
			JObject jsonObject = new JObject();
			jsonObject.Add("cmdid", "getdefaultuc");
			jsonObject.Add("roomid", room.roomid);
			jsonObject.Add("vod", 0);
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			
		}
		

		public void ConnectServerStep3(string loginstring)
		{
			Random ran = new Random();
			int randon = ran.Next(1001, 9909);
			
			ShowMessage("向服务器发送消息3:login");
			
			//android 2.6.2 匿名 没法用了
			if (string.IsNullOrEmpty(loginstring)) {
				loginstring = "{\"uid\":" + room.data.uid + ",\"os\":\"4.4.2\"," +
					"\"sid\":\"" + room.data.sid + "\",\"model\":\"NX803A\",\"nickname\":\"\"," +
					"\"imei\":\"zhanqi8633" + randon + "2153806\",\"cmdid\":\"loginreq\"," +
					"\"ver\":\"11\"," + //“2” 是2.6.2 ；“11” 是2.6.8
					"\"chatroomid\":" + room.chatRoomID + "," +
					"\"timestamp\":" + room.data.timestamp + "," +
					"\"roomid\":" + room.roomid + ",\"fhost\":\"android\",\"t\":0,\"r\":0,\"device\":1," +
					"\"gid\":" + room.data.gid + "," +
					"\"ssid\":\"" + room.GetSSID() + "\"," +
					"\"roomdata\":{\"slevel\":[],\"vdesc\":\"\",\"vlevel\":0}}";
				//这个是电脑的格式 ，主要是ssid的加密字符串不知道是什么，这个问题不解决就没办法用，除非自己抓包找到ssid
//			loginstring="{\"fx\":0,\"ver\":12," +
//				"\"roomid\":"+room.roomid+",\"nickname\":\"\",\"tagid\":0," +
//				"\"uid\":"+room.data.uid+",\"cmdid\":\"loginreq\",\"thirdaccount\":\"\"," +
//				"\"gid\":"+room.data.gid+",\"hideslv\":0,\"fhost\":\"\"," +
//				"\"sid\":\""+room.data.sid+"\"," +
//				"\"timestamp\":"+room.data.timestamp+",\"t\":0," +
//				"\"ssid\":\""+room.GetSSID()+"\",\"roomdata\":{\"vdesc\":\"\",\"slevel\":{\"curexp\":\"1036\",\"nextexp\":\"15000\",\"opp\":0,\"levelexp\":\"0\",\"pos\":\"0\",\"leftexp\":\"13964\",\"nextname\":\"5\",\"keep\":\"\"," +
//				"\"uid\":\""+room.data.uid+"\",\"level\":\"0\",\"name\":\"\"}," +
//				"\"vlevel\":0},\"imei\":\""+randon+"94"+"\"}";
				
				
			}
			JObject jsonObject = (JObject)JObject.Parse(loginstring);
//			ShowMessage(jsonObject.ToString());
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			
		}
		public void ConnectServerStep4()
		{
			ShowMessage("向服务器发送消息4");
			JObject jsonObject = new JObject();
			jsonObject.Add("cmdid", "cdn");
			jsonObject.Add("swrate", 0);
			jsonObject.Add("swline", 4);
			SendJsonToServer(jsonObject, (byte)0x10, (byte)0x27);
			Thread.Sleep(1000);
//			{"type":"quan","cmdid":"timegiftinfo"}
//			byte[] datas=new byte[]{0xbb,0xcc,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x59,0x27,0xbb,0xcc,0x00,0x00,0x00,0x00,0x25,0x00,0x00,0x00,0x10,0x27,0x7b,0x22,0x73,0x77,0x6c,0x69,0x6e,0x65,0x22,0x3a,0x34,0x2c,0x22,0x73,0x77,0x72,0x61,0x74,0x65,0x22,0x3a,0x30,0x2c,0x22,0x63,0x6d,0x64,0x69,0x64,0x22,0x3a,0x22,0x63,0x64,0x6e,0x22,0x7d,0xbb,0xcc,0x00,0x00,0x00,0x00,0x33,0x00,0x00,0x00,0x12,0x27,0x7b,0x22,0x76,0x6f,0x64,0x22,0x3a,0x30,0x2c,0x22,0x63,0x6d,0x64,0x69,0x64,0x22,0x3a,0x22,0x77,0x61,0x74,0x63,0x68,0x62,0x61,0x63,0x6b,0x68,0x6f,0x74,0x72,0x65,0x71,0x22,0x2c,0x22,0x76,0x69,0x64,0x65,0x6f,0x69,0x64,0x22,0x3a,0x35,0x32,0x33,0x32,0x30,0x7d};
//			ConnectServer(datas);
//			Thread.Sleep(1000);
//			JObject jsonObject2 = new JObject();
//			jsonObject2.Add("cmdid", "timegiftinfo");
//			jsonObject2.Add("type", "quan");
			
//			SendJsonToServer(jsonObject2, (byte)0x10, (byte)0x27);
			
		}
		
		public void SendDanmuToServer(String Message)
		{
			
			SendJsonToServer(null, (byte)0x59, (byte)0x27);
			ShowMessage("向服务器发送消息:聊天");
			JObject jsonObject = new JObject();
			jsonObject.Add("stype", 0);
			jsonObject.Add("content", Message);
			jsonObject.Add("useemot", "#13");
			JArray localJSONArray = new JArray();
			JObject localJSONObject2 = new JObject();
			localJSONObject2.Add("color", "red");
			localJSONArray.Add(localJSONArray);
			jsonObject.Add("style", localJSONArray);
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
				messageThread = new Thread(new ThreadStart(GetResponse));
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

		void GetChatResponse(object messagebyte)
		{
			byte[] buffer = messagebyte as byte[];
			String strJson;
			int size;
			JObject jsonObject;
			try {
				
				
				byte[] bs = new byte[4];
				for (int i = 0; i < 4; i++) {
					bs[i] = buffer[6 + i];
				}
				
				size = (int)System.BitConverter.ToInt16(bs, 0);
//				ShowMessage("DataSize:" + size + "字节");
				byte[] data = new byte[size];
				for (int i = 0; i < data.Length; i++) {
					data[i] = buffer[i + 12];
				}
				strJson = System.Text.Encoding.UTF8.GetString(data);

				jsonObject = JObject.Parse(strJson);
				if (ShowDanmuMessage!=null) {
					ShowDanmuMessage(jsonObject);
				}
				else{
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
								ShowInfo = slevels + fangguan + strlevel + ":" + name + ":: " + content ;
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
								
								ShowInfo =  strlevel+ " :" +nickname  + "\t#####：：送的" + name + ":X" + count;
								break;
							case "Level.FwList":
								
								ShowInfo = "";
								break;
							case "userupdate":
								ShowInfo="";
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
								ShowInfo = blockname + "被禁言" ;
								break;
							case "notefanslevel":
								string fansname = (string)jsonObject["data"]["fansname"];
								int fanslevel = (int)jsonObject["data"]["fanslevel"];
								ShowInfo = fanslevel + "级:" + fansname + "进入直播间 ";
								break;
								
							case "ticket":
								ShowInfo = "";
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
								ShowInfo = nickname + ":给主播" + action + count + name + "               ";
								break;
							case "firebro":
								string code = (string)jsonObject.GetValue("code");
								string slevel = (string)jsonObject.GetValue("slevel");
								
								string roomid = ((int)jsonObject.GetValue("roomid")).ToString();
								string url = "http://www.zhanqi.tv/" + code + "$" + roomid;
								ShowInfo = "这里有烟花:" + url + "               ";
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
						ShowMessage("error::获取弹幕出错了");
					}
					
				}
				
			}catch (OutOfMemoryException) {
//				try {
//					File.WriteAllBytes("./outofmemory.txt", buffer);
//				} catch {
//				}
				ShowMessage("error::这条消息接收失败（粘包）");
			} catch (IndexOutOfRangeException e) {
//				try {
//					File.WriteAllBytes("./shuzuyuejue.txt", buffer);
//				} catch {
//				}
				
				ShowMessage("error::这条消息接收失败（大小越界）");
			} catch (Exception e) {
//				try {
//					File.WriteAllBytes("./othererror.txt", buffer);
//				} catch {
//				}
				// TODO Auto-generated catch block
				
//				ShowMessage(e.Message );
				ShowMessage("error::这条消息接收失败（其他原因）");
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
			SendBytesToServer(arrayOfByte2, beat);

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
				messageThread.Abort();
				serverSocket.Shutdown(SocketShutdown.Both);
				serverSocket.Close();
//				Thread.Sleep(1000);//Application.Current.Shutdown();
			} catch (Exception e) {
				ShowMessage("error::" + e.Message);
			}
			
		}

		
	}
	public class StateObject
	{
		// Client  socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 1024*2;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		public   List<byte> byteSource = new List<byte>();
		// Received data string.
		//		public StringBuilder sb = new StringBuilder();
	}
}
//		void GetMessage()
//		{
//			try {
//
////				while (true) {
//
////					if (!RUN_CONNECTION) {
////				th.Stop();
////						break;
//
////					}
//
//				GetResponse();
//
//
////				}
//
//
//			} catch (Exception) {
//				ShowMessage("error::聊天连接异常，请退出");
////				Disposed();
//				return;
//			}
//
//		}
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
