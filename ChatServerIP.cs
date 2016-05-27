/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/25
 * Time: 13:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of ChatServerIP.
	/// </summary>
	 
		public class Log  
    {  
        public string ip { get; set; }  
        public int port { get; set; }  
    }  
  
    public class List  
    {  
        public string ip { get; set; }  
        public int port { get; set; }  
        public int chatroom_id { get; set; }  
        public int id { get; set; }  
    }  
  
    public class ChatServerIP  
    {  
        public Log log { get; set; }  
        public IList<List> list { get; set; }  
    }  
	 
}
