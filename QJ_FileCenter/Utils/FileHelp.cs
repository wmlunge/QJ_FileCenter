using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QJ_FileCenter
{
    public class FileHelp
    {
        public static IPAddress[] GetIP()
        {
            IPAddress[] ipAddr = Dns.Resolve(Dns.GetHostName()).AddressList;//获得当前IP地址
            return ipAddr;
        }
        public static string GetWWIP()
        {
            string tempip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据

                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            return tempip;
        }



        public static string GetConfig(string strKey, string strDefault = "")
        {
            return ConfigurationManager.AppSettings[strKey] ?? strDefault;
        }
    }
    public class Msg_Result
    {
        public string Action { get; set; }
        public string ErrorMsg { get; set; }
        public int DataLength { get; set; }
        public string ResultType { get; set; }
        public dynamic Result { get; set; }
        public dynamic Result1 { get; set; }
        public dynamic Result2 { get; set; }



    }
}
