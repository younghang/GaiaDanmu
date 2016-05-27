/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/20
 * Time: 22:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of RoomDetail.
	/// </summary>
	public class Prop
	{
		public int speaker { get; set; }
		public int ticket { get; set; }
	}
	
	public class Slevel
	{
		public string name { get; set; }
		public string pos { get; set; }
		public string keep { get; set; }
		public string nextname { get; set; }
		public string level { get; set; }
		public string nextexp { get; set; }
		public string leftexp { get; set; }
		public string levelexp { get; set; }
		public string curexp { get; set; }
		public string uid { get; set; }
		public int consume { get; set; }
		public int opp { get; set; }
		public string type { get; set; }
		public string etime { get; set; }
	}
	
	public class Roomdata
	{
		public int vlevel { get; set; }
		public string vdesc { get; set; }
//		public List<Slevel> slevel { get; set; }//电脑
//		public  Slevel  slevel { get; set; }//手机
	}
	
	public class Rt
	{
		public double init { get; set; }
		public double session { get; set; }
		public string redis { get; set; }
		public double all { get; set; }
		public string elapsed { get; set; }
	}
	
	public class Data
	{
		public int uid { get; set; }
		public int gid { get; set; }
		public string sid { get; set; }
		public int timestamp { get; set; }
		public Prop prop { get; set; }
		public bool isFollow { get; set; }
		public Roomdata roomdata { get; set; }
		public Rt rt { get; set; }
	}
	
	public class RoomDetail
	{
		public int code { get; set; }
		public string message { get; set; }
		public Data data { get; set; }
		public string ServerIP;
		public int chatRoomID;
		 
		public int roomid;
		public string GetSSID()
		{
			 
			 string beforestr=data.uid+""+data.gid+">y,V4{{][$@s]qS3"+data.timestamp;	//2.6.2		 
//			string beforestr=data.uid+data.gid+"www.zhanqi.tv%H``68](U#s91n.k"+data.timestamp;//2.6.8
//			string beforestr=data.uid+""+data.gid+"qwer"+data.timestamp;//2.5.5
			MD5 md = new MD5CryptoServiceProvider();
//
			byte[] ss = md.ComputeHash(Encoding.UTF8.GetBytes(beforestr));
			string b=  bytesToHexString(ss);
//			            b=b.ToLower();
 
			byte[] bytes = Encoding.Default.GetBytes(b);
			string c = Convert.ToBase64String(bytes);
 
			 
			return c;
		}
		public static string bytesToHexString(byte[] paramString1)
		{
			    char[] arrayOfChar = null;
	      
	      int k = 0;
	      int i = 0;
	      int j = 0;
			  k = paramString1.Length;
	      arrayOfChar = new char[k * 2];
	      for (;;)
	      {
	        if (i >= k) {
	          return new String(arrayOfChar);
	        }
	        int m = paramString1[i];
	        int n = j + 1;
	        arrayOfChar[j] = hexDigits_L[(MoveByte(m,4) & 0xF)];
	        j = n + 1;
	        arrayOfChar[n] = hexDigits_L[(m & 0xF)];
	        i += 1;
	      }
		}
		// <summary>
        /// 特殊的右移位操作，补0右移，如果是负数，需要进行特殊的转换处理（右移位）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int MoveByte(int value, int pos)
        {
            if (value < 0)
            {
                string s = Convert.ToString(value, 2);    // 转换为二进制
                for (int i = 0; i < pos; i++)
                {
                    s = "0" + s.Substring(0, 31);
                }
                return Convert.ToInt32(s, 2);            // 将二进制数字转换为数字
            }
            else
            {
                return value >> pos;
            }
        }
		static char[] hexDigits_L = {
			(char)48,
			(char)49,
			(char)50,
			(char)51,
			(char)52,
			(char)53,
			(char)54,
			(char)55,
			(char)56,
			(char)57,
			(char)97,
			(char)		98,
			(char)			99,
			(char)		100,
			(char)		101,
			(char)		102
		};
		
		 
		
	}
}
