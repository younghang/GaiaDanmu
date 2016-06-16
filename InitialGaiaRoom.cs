/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/25
 * Time: 11:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of InitialGaiaRoom.
	/// </summary>
	public class InitialGaiaRoom
	{
		public InitialGaiaRoom(ref ManualResetEvent gaiaFinish,bool otherRoom=false)
		{
			other_Room=otherRoom;
			gaiafinish=gaiaFinish;
			thInitial=new Thread(new ThreadStart(Initial));
			worklog=new WorkLogin();
			thInitial.Start();
		}
		ManualResetEvent gaiafinish=null;
		bool other_Room=false;
		WorkLogin worklog;
		public string otherRoomID="";
		public  bool FINISH_INITIAL_GAIA_ROOM=false;
		void Initial()
		{
			if (other_Room) {
				Thread.Sleep(1000);
				jsongaiaRoom=worklog.GetOtherRoom(otherRoomID);
			}else
			  jsongaiaRoom=worklog.GetGaiaRoom();
			if (jsongaiaRoom!=null) {
				
				JsonConvert.PopulateObject(jsongaiaRoom.ToString(),gaiaroom);
				string serverBase64=gaiaroom.data.flashvars.Servers;
				byte[] listipbytes=Convert.FromBase64String(serverBase64);
				string listip= System.Text.Encoding.Default.GetString( listipbytes );
				chatiplist=new ChatServerIP();
				JsonConvert.PopulateObject(listip,chatiplist);
				chatip=chatiplist.list[0].ip;
				chatport=chatiplist.list[0].port;
				chatRoomid=chatiplist.list[0].chatroom_id;
				FINISH_INITIAL_GAIA_ROOM=true;
				if (gaiafinish!=null) {
					gaiafinish.Set();
				}
			}
			
		}
		Thread thInitial;
		public GaiaRoom GetGaiaRoomDetail()
		{
			
			return gaiaroom;
		}
		public JObject GetJsonGaiaRoom()
		{
			return jsongaiaRoom;
		}
		
		GaiaRoom gaiaroom=new GaiaRoom();
		JObject jsongaiaRoom=null;
		ChatServerIP chatiplist;
		public string chatip;
		public int chatRoomid;
		public int chatport;
	}
}
