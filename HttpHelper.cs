/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/2/7 星期日
 * Time: 9:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ZQDanmuTest
{
	/// <summary>
	/// Description of HttpHelper.
	/// </summary>
	public class HttpHelper
	{
		static CookieContainer cookieContain = new CookieContainer();
		/// <summary>
		/// 创建GET方式的HTTP请求
		/// </summary>
		public static HttpWebResponse CreateGetHttpResponse(string url, int timeout, string userAgent, CookieCollection cookies)
		{
			HttpWebRequest request = null;
			if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
				//对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
				request = WebRequest.Create(url) as HttpWebRequest;
				request.ProtocolVersion = HttpVersion.Version11;    //http版本，默认是1.1,这里设置为1.0
			} else {
				request = WebRequest.Create(url) as HttpWebRequest;
			}
			request.Method = "GET";
			
			
			if (userAgent!=null) 
			//设置代理UserAgent和超时
			request.UserAgent = userAgent;
			//request.Timeout = timeout;
			WebHeaderCollection header=new WebHeaderCollection();
			
			request.CookieContainer = HttpHelper.cookieContain;
			//                if (cookies != null)
			//                {
//
			//                    request.CookieContainer.Add(cookies);
			//                }
			HttpWebResponse response= request.GetResponse() as HttpWebResponse;
			
			return response;
			
		}
		
		static string imei ="";

		public static string Imei {
			get {
				return imei;
			}
			set{
				imei=value;
			}
		}

		static string version = "2.6.1";

		public static string Version {
			get {
				return version;
			}
			set{
				version=value;
			}
		}
	 
		/// <summary>
		/// 创建POST方式的HTTP请求
		/// </summary>
		public static HttpWebResponse CreatePostHttpResponseInt(string url, IDictionary<string, int> parameters, int timeout, string userAgent, CookieCollection cookies,bool saveCookie=true,bool needOtherHeader=false,WebHeaderCollection otherheader=null)
		{
			HttpWebRequest request = null;
			//如果是发送HTTPS请求
			if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
				//ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
				request = WebRequest.Create(url) as HttpWebRequest;
				//request.ProtocolVersion = HttpVersion.Version10;
			} else {
				request = WebRequest.Create(url) as HttpWebRequest;
			}
			
			request.CookieContainer = HttpHelper.cookieContain;
			if (!needOtherHeader) {
				WebHeaderCollection header = new WebHeaderCollection();
			header.Add("appVersion", Version);
			if (Imei!="") {
				    header.Add("imei",Imei);
			}
			            
			request.Headers = header;
			}
			else
				request.Headers=otherheader;
			
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			//设置代理UserAgent和超时
			request.UserAgent = userAgent;
			//request.Timeout = timeout;

			
			//                if (cookies != null)
			//                {
			//                    request.CookieContainer = new CookieContainer();
			//                    request.CookieContainer.Add(cookies);
			//                }
			//发送POST数据
			if (!(parameters == null || parameters.Count == 0)) {
				StringBuilder buffer = new StringBuilder();
				int i = 0;
				foreach (string key in parameters.Keys) {
					if (i > 0) {
						buffer.AppendFormat("&{0}={1}", key, parameters[key]);
					} else {
						buffer.AppendFormat("{0}={1}", key, parameters[key]);
						i++;
					}
				}
				byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
				using (Stream stream = request.GetRequestStream()) {
					stream.Write(data, 0, data.Length);
				}
				 
			}
			string[] values = request.Headers.GetValues("Content-Type");
			HttpWebResponse response= request.GetResponse() as HttpWebResponse;
			
			
			if (saveCookie) {
				
				SaveCookie(request);
			}
			return  response;
		}
		
		/// <summary>
		/// 创建POST方式的HTTP请求
		/// </summary>
		public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies,bool saveCookie=true,bool needOtherHeader=false,WebHeaderCollection otherheader=null)
		{
			HttpWebRequest request = null;
			//如果是发送HTTPS请求
			if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
				//ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
				request = WebRequest.Create(url) as HttpWebRequest;
				//request.ProtocolVersion = HttpVersion.Version10;
			} else {
				request = WebRequest.Create(url) as HttpWebRequest;
			}
			
			request.CookieContainer = HttpHelper.cookieContain;
			if (!needOtherHeader) {
				WebHeaderCollection header = new WebHeaderCollection();
			header.Add("appVersion", Version);
			if (Imei!="") {
				    header.Add("imei",Imei);
			}
			            
			request.Headers = header;
			}
			else
				request.Headers=otherheader;
			
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			//设置代理UserAgent和超时
			request.UserAgent = userAgent;
			//request.Timeout = timeout;

			
			//                if (cookies != null)
			//                {
			//                    request.CookieContainer = new CookieContainer();
			//                    request.CookieContainer.Add(cookies);
			//                }
			//发送POST数据
			if (!(parameters == null || parameters.Count == 0)) {
				StringBuilder buffer = new StringBuilder();
				int i = 0;
				foreach (string key in parameters.Keys) {
					if (i > 0) {
						buffer.AppendFormat("&{0}={1}", key, parameters[key]);
					} else {
						buffer.AppendFormat("{0}={1}", key, parameters[key]);
						i++;
					}
				}
				byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
				using (Stream stream = request.GetRequestStream()) {
					stream.Write(data, 0, data.Length);
				}
				 
			}
			string[] values = request.Headers.GetValues("Content-Type");
			HttpWebResponse response= request.GetResponse() as HttpWebResponse;
			
			
			if (saveCookie) {
				
				SaveCookie(request);
			}
			return  response;
		}
		public static void SaveCookie(HttpWebRequest request)
		{
			 CookieCollection cookiecollection =request.CookieContainer.GetCookies(request.RequestUri);
				  StringBuilder sb=new StringBuilder();
				  for (int ii = 0; ii < cookiecollection.Count; ii++) {
				  	sb.Append(cookiecollection[ii].ToString());
				  	sb.Append(";\n");
				  }
				  string cookiestring=sb.ToString();
				  			FileStream fs = File.Create("./cookie.txt");
			fs.Close();
			File.WriteAllText("./cookie.txt", cookiestring, System.Text.Encoding.Default);
			
		}
		public static bool LoadCookie()
		{
			CookieContainer container=new CookieContainer();
			string[] cookies;
			try{
			 cookies = File.ReadAllText("./cookie.txt", System.Text.Encoding.Default).Split(";\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			}
			catch{
				return false;
			}
			 
		
			for (int i = 0; i < cookies.Length; i++) {
				string[] ct=cookies[i].Split('=');
				Cookie ck=new Cookie();
			 
				ck.Domain = "www.zhanqi.tv";
				ck.Expired =true;
				ck.HttpOnly = true;
				ck.Name = ct[0];
			 
				ck.Secure =false;
				ck.Value = ct[1];
				
				container.Add(new Cookie(ct[0],ct[1],"","www.zhanqi.tv"));
			}
			HttpHelper.cookieContain=container;
			return true;
		}
		

		/// <summary>
		/// 获取请求的数据
		/// </summary>
		public static string GetResponseString(HttpWebResponse webresponse)
		{
			using (Stream s = webresponse.GetResponseStream()) {
				StreamReader reader = new StreamReader(s, Encoding.UTF8);
				return reader.ReadToEnd();

			}
		}
		
		public static CookieContainer GetCookie()
		{
			return cookieContain;
		}
		public static List<Cookie> GetAllCookies(CookieContainer cc)
		{
			List<Cookie> lstCookies = new List<Cookie>();
			Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
			                                                       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
			                                                       System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
			foreach (object pathList in table.Values) {
				SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
				                                                                      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
				                                                                      | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
				foreach (CookieCollection colCookies in lstCookieCol.Values)
					foreach (Cookie c in colCookies)
						lstCookies.Add(c);
			}
			return lstCookies;
		}
		public static void SaveCookie0()
		{
			StringBuilder sbc = new StringBuilder();
			
			List<Cookie> cooklist = GetAllCookies(HttpHelper.cookieContain);
			foreach (Cookie cookie in cooklist)
			{
				sbc.AppendFormat("{0};{1};{2};{3};{4};{5}\r\n",
				                 cookie.Domain,cookie.Name, cookie.Path, cookie.Port,
				                 cookie.Secure.ToString(), cookie.Value);
			}
			FileStream fs = File.Create("./cookie.txt");
			fs.Close();
			File.WriteAllText("./cookie.txt", sbc.ToString(), System.Text.Encoding.Default);
			//读出所有Cookie
			
		}
		public static bool ReadCookie(){
			bool hasCookie=true;
			string[] cookies;
			try{
					 cookies = File.ReadAllText("./cookie.txt", System.Text.Encoding.Default).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			}
			catch{
				return false;
			}
			if (cookies.Length==0) {
				hasCookie=false;
			}
//			 cookieContain=new CookieContainer();
			foreach (string c in cookies)
			{ 
				string[] cc = c.Split(";".ToCharArray());
				 
				
				Cookie ck = new Cookie(); ;
				ck.Discard =false;
				ck.Domain = cc[0];
				ck.Expired =true;
				ck.HttpOnly = true;
				ck.Name = cc[1];
				ck.Path = cc[2];
				ck.Port = cc[3];
				ck.Secure = bool.Parse(cc[4]);
				ck.Value = cc[5];
				HttpHelper.cookieContain.Add(ck);
			}
			CookieContainer cfc= HttpHelper.cookieContain;
			return hasCookie;
		}
		
		
		/// <summary>
		/// 验证证书
		/// </summary>
		private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			if (errors == SslPolicyErrors.None)
				return true;
			return false;
		}
		
	}
}
