/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/24
 * Time: 23:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of InitialRoomID.
	/// </summary>
	public class InitialRoomDetial
	{
		public InitialRoomDetial(ref ManualResetEvent roomFinish)
		{
			thInitial = new Thread(new ThreadStart(Initial));
			roomfinish = roomFinish;
			worklog = new WorkLogin();
			thInitial.Start();
		}
		ManualResetEvent roomfinish;
		WorkLogin worklog;
		public   bool FINISH_INITIAL_ROOM = false;
		void Initial()
		{
		 
				
			
			if (ReadCookie()) {
				roomdetail = worklog.GetRoomDetail(Uid, log);
			} else
				roomdetail = worklog.GetRoomDetail();
			FINISH_INITIAL_ROOM = true;
			if (roomfinish != null) {
				roomfinish.Set();
			}
			  
			
		}
		Thread thInitial;
		public RoomDetail GetRoomDetail()
		{			
			return roomdetail;
		}
		int Uid = 0;
		string log = "";
		public bool ReadCookie()
		{
			string str = "";
			string uid;
			try {
				str = File.ReadAllText("cookie.txt");
			
			} catch {
				str = "";
			}
			if (str != "") {
 
				string uidReg = "tj_uid=[0-9]+";
				uid = Regex.Match(str, uidReg).Groups[0].Value;
				uid = uid.Split('=')[1];
				string log1Reg = "PHPSESSID=.+;";
				string log1 = Regex.Match(str, log1Reg).Groups[0].Value;
				log1 = log1.Substring(10, log1.Length - 11);
				string log2Reg = "ZQ_GUID=.+;";
				string log2 = Regex.Match(str, log2Reg).Groups[0].Value;
				log2 = log2.Substring(8, log2.Length - 9);
			 
				log = log1 + "." + log2;
				Uid = int.Parse(uid);
				
			}
			if (str == "") {
				return false;
			}
			return true;
		}
		
		RoomDetail roomdetail = null;
	}
}
