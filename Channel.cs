/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/21
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Newtonsoft.Json.Linq;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of Channel.
	/// </summary>
	public class Channel
	{
		public Channel()
		{
		}
		public string roomID{get;set;}
		public string chatRoomID{get;set;}
		public string gID{get;set;}
		public string sID{get;set;}
		public string timestamp{get;set;}
		public string uID{get;set;}
		public JObject roomData{get;set;}
		
	}
}
