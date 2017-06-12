using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHelper.RestAPI
{
    public class WebRequestHelper
    {
        public string GetSession(string userName, string password, string url)
        {
            var sessionstr = "";
            //Log.Info("Gettin session for user: " + userName);
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                var str = "{Password:\"" + password + "\",UserName : \"" + userName + "\"}";
                webRequest.Credentials = new NetworkCredential(userName, password);

                webRequest.Method = "POST";
                var buffer = System.Text.Encoding.UTF8.GetBytes(str);
                webRequest.ContentType = "application/json; charset=UTF-8";
                webRequest.Accept = "application/json";
                webRequest.ContentLength = buffer.Length;
                webRequest.KeepAlive = true;
                webRequest.Timeout = System.Threading.Timeout.Infinite;
                webRequest.ProtocolVersion = HttpVersion.Version10;
                webRequest.Headers.Add("Body", str);

                var stream = webRequest.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();

                var response = webRequest.GetResponse();
                var responceStr = GetHtmlFromResponce(response);
                Console.WriteLine(responceStr);

                sessionstr = responceStr.Split('\"')[3].Trim('\\');
                //Log.Info("Session is: " + sessionstr);
            }
            return sessionstr;
        }

        public string GetHtmlFromResponce(WebResponse response)
        {
            var builder = new StringBuilder();
            using (var receiveStream = response.GetResponseStream())
            {
                var encode = System.Text.Encoding.GetEncoding("utf-8");
                var readStream = new StreamReader(receiveStream, encode);
                var read = new Char[256];
                var count = readStream.Read(read, 0, 256);

                while (count > 0)
                {
                    var str = new String(read, 0, count);
                    builder.Append(str);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                response.Close();
            }

            return builder.ToString();
        }

    }
}
