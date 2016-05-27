/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/21
 * Time: 15:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Newtonsoft.Json.Linq;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of Login.
	/// </summary>
	public class Login
	{
		Channel mChannel;
 
		public  Login(Channel gaia )
		{
			mChannel=gaia;
		}
		public JObject  InitialLogin()
		{
		JObject	 localJSONObject = new JObject();
      localJSONObject.Add("cmdid", "loginreq");
      localJSONObject.Add("roomid", int.Parse(this.mChannel.roomID));
      localJSONObject.Add("chatroomid", int.Parse(this.mChannel.chatRoomID));
   
        localJSONObject.Add("gid", int.Parse(this.mChannel.gID));
        localJSONObject.Add("sid", this.mChannel.sID);
        localJSONObject.Add("t", 0);
        localJSONObject.Add("r", 0);
     
        localJSONObject.Add("imei", "zhanqi" +863396021455862 );
       
        localJSONObject.Add("timestamp", this.mChannel.timestamp);
        localJSONObject.Add("device", 1);
       
        localJSONObject.Add("fhost", "androidPad");
        localJSONObject.Add("ver", "2");
        String str1 = this.mChannel.uID + this.mChannel.gID + ">y,V4{{][$@s]qS3" + this.mChannel.timestamp;
//        String str2 = new StringEncoder().StringToLowercaseMD5(str1);
//        localJSONObject.Add("ssid", new String(new Base64().encode(str2.getBytes())));
        if (this.mChannel.roomData != null) {
          localJSONObject.Add("roomdata", this.mChannel.roomData);
        }
        localJSONObject.Add("model", "NX403A");
        localJSONObject.Add("os",  "4.4.2");
        
        return localJSONObject;
	 
		
		
	}
	}
}
