/*
 * Created by SharpDevelop.
 * User: young
 * Date: 02/08/2016
 * Time: 21:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of GaiaRoom.
	/// </summary>
	 public class Permission  
    {  
        public bool fans { get; set; }  
        public bool guess { get; set; }  
        public bool replay { get; set; }  
        public bool multi { get; set; }  
        public bool shift { get; set; }  
        public bool video { get; set; }  
        public bool firework { get; set; }  
    }  
  
    public class Flashvars  
    {  
        public string Servers { get; set; }  
        public IList<object> ServerIp { get; set; }  
        public IList<object> ServerPort { get; set; }  
        public IList<object> ChatRoomId { get; set; }  
        public string VideoLevels { get; set; }  
        public string cdns { get; set; }  
        public int Status { get; set; }  
        public int RoomId { get; set; }  
        public bool ComLayer { get; set; }  
        public string VideoTitle { get; set; }  
        public string WebHost { get; set; }  
        public string VideoType { get; set; }  
        public int GameId { get; set; }  
        public int Online { get; set; }  
        public string pv { get; set; }  
        public int TuristRate { get; set; }  
        public int UseStIp { get; set; }  
        public int DlLogo { get; set; }  
    }  
  
    public class Hots  
    {  
        public string name { get; set; }  
        public string level { get; set; }  
        public string fight { get; set; }  
        public string nowLevelStart { get; set; }  
        public string nextLevelFight { get; set; }  
    }  
  
    public class AnchorAttr  
    {  
        public Hots hots { get; set; }  
    }  
  
    public class IsStar  
    {  
        public int isWeek { get; set; }  
        public int isMonth { get; set; }  
    }  
  
    public class DData  
    {  
        public int id { get; set; }  
        public string uid { get; set; }  
        public string nickname { get; set; }  
        public string gender { get; set; }  
        public string avatar { get; set; }  
        public string code { get; set; }  
        public string domain { get; set; }  
        public string url { get; set; }  
        public string title { get; set; }  
        public string gameId { get; set; }  
        public string spic { get; set; }  
        public string bpic { get; set; }  
        public string template { get; set; }  
        public string online { get; set; }  
        public string weight { get; set; }  
        public string status { get; set; }  
        public string level { get; set; }  
        public string hotsLevel { get; set; }  
        public string type { get; set; }  
        public string liveTime { get; set; }  
        public string userGroup { get; set; }  
        public string allowRecord { get; set; }  
        public string allowVideo { get; set; }  
        public string publishUrl { get; set; }  
        public string videoId { get; set; }  
        public string chatStatus { get; set; }  
        public string roomNotice { get; set; }  
        public string anchorNotice { get; set; }  
        public object roomCover { get; set; }  
        public object roomCoverType { get; set; }  
        public string editTime { get; set; }  
        public string addTime { get; set; }  
        public string gameName { get; set; }  
        public string gameUrl { get; set; }  
        public string gameIcon { get; set; }  
        public string gameBpic { get; set; }  
        public string videoIdKey { get; set; }  
        public Permission permission { get; set; }  
        public string fansTitle { get; set; }  
        public Flashvars flashvars { get; set; }  
        public AnchorAttr anchorAttr { get; set; }  
        public int follows { get; set; }  
        public int fans { get; set; }  
        public IsStar isStar { get; set; }  
        public bool bonus { get; set; }  
    }  
  
    public class GaiaRoom  
    {  
        public int code { get; set; }  
        public string message { get; set; }  
        public DData data { get; set; }  
    }  
	
}
