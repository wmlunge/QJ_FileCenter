
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QJ_FileCenter
{
    public class CommonHelp
    {


        /// <summary>
        /// 从html中提取纯文本
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public string StripHT(string strHtml)  //从html中提取纯文本
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string strOutput = regex.Replace(strHtml, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");
            return strOutput;
        }


        /// <summary>
        /// 移除html标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;

            Regex regex = new Regex("<.+?>");
            var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                html = html.Replace(match.Value, "");
            }
            return html;
        }



        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/json; charset=UTF-8";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        public static HttpWebResponse CreateHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies, string strType = "POST")
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = strType;
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies, string strType = "POST")
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = strType;
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.Default);
                return reader.ReadToEnd();

            }
        }



        public static string GetConfig(string strKey, string strDefault = "")
        {
            return ConfigurationManager.AppSettings[strKey] ?? strDefault;
        }







        public static string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }


        private int rep = 0;

        /// <summary>
        /// 生成随机不重复的字符串（分享码用）
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetMD5(string content)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(content));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }





        /// <summary>
        /// 获取数字验证码
        /// </summary>
        /// <param name="codenum"></param>
        /// <returns></returns>
        public static string numcode(int codenum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string[] stray = new string[codenum];
            Random random = new Random();
            for (int i = 0; i < codenum; i++)
            {
                int iNum = 0;
                while ((iNum = Convert.ToInt32(VcArray.Length * random.NextDouble())) == VcArray.Length)
                {
                    iNum = Convert.ToInt32(VcArray.Length * random.NextDouble());
                }
                stray[i] = VcArray[iNum];
            }

            string identifycode = string.Empty;
            foreach (string s in stray)
            {
                identifycode += s;
            }
            return identifycode;
        }
        /// <summary>
        /// 登录验证码
        /// </summary>
        /// <param name="codenum"></param>
        /// <returns></returns>
        public static string yzmcode(int codenum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
            string[] VcArray = Vchar.Split(',');
            string[] stray = new string[codenum];
            Random random = new Random();
            for (int i = 0; i < codenum; i++)
            {
                int iNum = 0;
                while ((iNum = Convert.ToInt32(VcArray.Length * random.NextDouble())) == VcArray.Length)
                {
                    iNum = Convert.ToInt32(VcArray.Length * random.NextDouble());
                }
                stray[i] = VcArray[iNum];
            }

            string identifycode = string.Empty;
            foreach (string s in stray)
            {
                identifycode += s;
            }
            return identifycode;
        }

        private static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }




        public static void WriteLOG(string err)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(path + "/log/"))
                {
                    Directory.CreateDirectory(path + "/log/");
                }

                string name = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (!File.Exists(path + "/log/" + name))
                {
                    FileInfo myfile = new FileInfo(path + "/log/" + name);
                    FileStream fs = myfile.Create();
                    fs.Close();
                }

                StreamWriter sw = File.AppendText(path + "/log/" + name);
                sw.WriteLine(err + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public static bool ProcessSqlStr(string Str, int type)
        {
            string SqlStr = "";
            if (type == 1)  //Post方法提交  
            {
                SqlStr = "script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|substring(|mid(|master|truncate|char(|declare|replace(|varchar(|cast(";
            }
            else if (type == 2) //Get方法提交  
            {
                SqlStr = "'|script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|*|asc(|chr(|substring(|mid(|master|truncate|char(|declare|replace(|;|varchar(|cast(";
            }
            else if (type == 3) //Cookie提交  
            {
                SqlStr = "script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|asc(|chr(|substring(|mid(|master|truncate|char(|declare";
            }
            else  //默认Post方法提交  
            {
                SqlStr = "script|iframe|xp_loginconfig|xp_fixeddrives|Xp_regremovemultistring|Xp_regread|Xp_regwrite|xp_cmdshell|xp_dirtree|count(|asc(|chr(|substring(|mid(|master|truncate|char(|declare|replace(";
            }

            bool ReturnValue = true;
            try
            {
                if (Str != "")
                {
                    string[] anySqlStr = SqlStr.ToUpper().Split('|'); ;
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.ToUpper().IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }


        public static string Filter(string str)
        {
            string[] pattern = { "insert ", "delete", "count\\(", "drop table", "update", "truncate", "xp_cmdshell", "exec   master", "netlocalgroup administrators", "net use " };
            for (int i = 0; i < pattern.Length; i++)
            {
                str = str.Replace(pattern[i].ToString(), "");
            }
            return str;
        }









        /// <summary>
        /// 生成流水号格式：8位日期加3位顺序号，如20100302001。
        /// </summary>
        public static string GetWFNumber(string serialNumber, string ywcode)
        {
            if (serialNumber != "0")
            {
                string headDate = serialNumber.Substring(ywcode.Length + 1, 8);
                int lastNumber = int.Parse(serialNumber.Substring(ywcode.Length + 1 + 8));
                //如果数据库最大值流水号中日期和生成日期在同一天，则顺序号加1
                if (headDate == DateTime.Now.ToString("yyyyMMdd"))
                {
                    lastNumber++;
                    return ywcode + "-" + headDate + lastNumber.ToString("000");
                }
            }
            return ywcode + "-" + DateTime.Now.ToString("yyyyMMdd") + "001";
        }








    }



  


    public static class MyExtensions
    {

        public static string Request(this JObject JData, string strPro, string strDefault = null)
        {
            return JData[strPro] != null ? JData[strPro].ToString() : strDefault;

        }

        public static int[] SplitTOInt(this string strs, char ch)
        {
            string[] arrstr = strs.Split(ch);
            int[] arrint = new int[arrstr.Length];
            for (int i = 0; i < arrstr.Length; i++)
            {
                arrint[i] = int.Parse(arrstr[i].ToString());
            }
            return arrint;
        }

        public static List<string> SplitTOList(this string strs, char ch)
        {
            List<string> Lstr = new List<string>();
            string[] arrstr = strs.Split(ch);
            int[] arrint = new int[arrstr.Length];
            for (int i = 0; i < arrstr.Length; i++)
            {
                Lstr.Add(arrstr[i].ToString());
            }
            return Lstr;
        }

        public static Dictionary<string, string> SplitTODictionary(this string strs, char ch, string strKey)
        {
            Dictionary<string, string> Lstr = new Dictionary<string, string>();
            string[] arrstr = strs.Split(ch);
            int[] arrint = new int[arrstr.Length];
            for (int i = 0; i < arrstr.Length; i++)
            {
                Lstr.Add(arrstr[i].ToString(), strKey);
            }
            return Lstr;
        }

        /// <summary>
        /// 将List转化为String
        /// </summary>
        /// <param name="Lists"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string ListTOString(this List<string> Lists, char ch)
        {
            string strReturn = "";
            foreach (var item in Lists)
            {
                strReturn = strReturn + item.ToString() + ch;
            }
            return strReturn.TrimEnd(ch);
        }

        /// <summary>
        /// 将List(int)转化为String
        /// </summary>
        /// <param name="Lists"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string ListTOString(this List<int> Lists, char ch)
        {
            string strReturn = "";
            foreach (var item in Lists)
            {
                strReturn = strReturn + item.ToString() + ch;
            }
            return strReturn.TrimEnd(ch);
        }
        /// <summary>
        /// 获取需要IN的格式
        /// </summary>
        /// <param name="strKys"></param>
        /// <returns></returns>
        public static string ToFormatLike(this string strKys)
        {
            StringBuilder sbKeys = new StringBuilder();
            foreach (var item in strKys.Split(','))
            {
                sbKeys.AppendFormat("'" + item.ToString() + "',");
            }
            return sbKeys.Length > 0 ? sbKeys.ToString().TrimEnd(',').Trim('\'') : "";
        }



        /// <summary>
        /// 获取需要IN的格式
        /// </summary>
        /// <param name="strKys"></param>
        /// <returns></returns>
        public static string ToFormatLike(this string strKys, char ch)
        {
            StringBuilder sbKeys = new StringBuilder();
            foreach (var item in strKys.Split(ch))
            {
                sbKeys.AppendFormat("'" + item.ToString() + "',");
            }
            return sbKeys.Length > 0 ? sbKeys.ToString().TrimEnd(',').Trim('\'') : "";
        }




        /// <summary>
        /// 取前N个字符,后面的用省略号代替
        /// </summary>
        /// <param name="strKys"></param>
        /// <returns></returns>
        public static string ToMangneStr(this string strKys, int intLenght)
        {

            return strKys.Length > intLenght ? strKys.Substring(0, intLenght) + "…………" : strKys;
        }



        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public static DataTable OrderBy(this DataTable dt, string orderBy)
        {
            dt.DefaultView.Sort = orderBy;
            return dt.DefaultView.ToTable();
        }
        public static DataTable Where(this DataTable dt, string where)
        {
            DataTable resultDt = dt.Clone();
            DataRow[] resultRows = dt.Select(where);
            foreach (DataRow dr in resultRows) resultDt.Rows.Add(dr.ItemArray);
            return resultDt;
        }


        /// <summary>
        /// Datatable转换为Json
        /// </summary>
        /// <param name="table">Datatable对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(this DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            if (jsonString.Length != 0)
            {
                jsonString.Append("]");
            }
            return jsonString.ToString();
        }



        public static int ToInt32(this Object obj)
        {
            return Convert.ToInt32(obj);
        }


        public static DateTime ToDateTime(this Object obj)
        {
            return Convert.ToDateTime(obj);
        }

        public static T GetProperty<T>(this object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return (T)property.GetValue(obj, null);
            }
            throw new ArgumentNullException(propertyName);
        }



        /// <summary>
        /// DataTable转成Json
        /// </summary>
        /// <param name="jsonName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 格式化字符型、日期型、布尔型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(Int32))
            {
                if (str.Trim() == "")
                {
                    str = "\"" + str + "\"";
                }
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }
            return str;
        }



        /// <summary>
        /// 过滤特殊字符
        /// 如果字符串为空，直接返回。
        /// </summary>
        /// <param name="str">需要过滤的字符串</param>
        /// <returns>过滤好的字符串</returns>
        public static string FilterSpecial(this string str)
        {
            if (str == "")
            {
                return str;
            }
            else
            {
                str = str.Replace("'", "");
                str = str.Replace("<", "");
                str = str.Replace(">", "");
                str = str.Replace("%", "");
                str = str.ToLower().Replace("'delete", "");
                str = str.ToLower().Replace("'truncate", "");
                str = str.Replace("''", "");
                str = str.Replace("\"\"", "");
                str = str.Replace(",", "");
                str = str.Replace(".", "");
                str = str.Replace(">=", "");
                str = str.Replace("=<", "");
                str = str.Replace(";", "");
                str = str.Replace("||", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");
                str = str.Replace("&", "");
                str = str.Replace("#", "");
                str = str.Replace("/", "");
                str = str.Replace("|", "");
                str = str.Replace("?", "");
                str = str.Replace(">?", "");
                str = str.Replace("?<", "");
                return str;
            }
        }

      

        /// <summary>   
        /// 根据条件过滤表   
        /// </summary>   
        /// <param name="dt">未过滤之前的表</param>   
        /// <param name="filter">过滤条件</param>   
        /// <returns>返回过滤后的表</returns>   
        public static DataTable FilterTable(this DataTable dt, string filter, string isSJ = "N")
        {

            DataTable newTable = dt.Clone();
            DataRow[] drs = dt.Select(filter);
            foreach (DataRow dr in drs)
            {
                newTable.Rows.Add(dr.ItemArray);
            }

            return newTable;
        }


        /// <summary>
        /// 随机排序
        /// </summary>
        /// <param name="newTable"></param>
        /// <returns></returns>
        public static DataTable SJTable(this DataTable newTable)
        {

            Random ran = new Random();
            newTable.Columns.Add("sort", typeof(int));
            for (int i = 0; i < newTable.Rows.Count; i++)
            {
                newTable.Rows[i]["sort"] = ran.Next(0, 100);
            }
            DataView dv = newTable.DefaultView;
            dv.Sort = "sort asc";
            newTable = dv.ToTable();
            return newTable;
        }




        /// <summary>
        /// 处理生成的DataTable
        /// </summary>
        /// <param name="newTable"></param>
        /// <param name="addclName">添加的列</param>
        /// <param name="clNames">需要合并的列</param>
        /// <returns></returns>
        public static DataTable AddColum(this DataTable DTTable, string addclName, char strChar, params string[] clNames)
        {

            DTTable.Columns.Add(addclName);
            for (int i = 0; i < DTTable.Rows.Count; i++)
            {
                string strTemp = "";
                for (int m = 0; m < clNames.Length; m++)
                {
                    strTemp = strTemp + DTTable.Rows[i][clNames[m]].ToString() + strChar;
                }
                DTTable.Rows[i][addclName] = strTemp.TrimEnd(strChar);

            }

            return DTTable;
        }

        /// <summary>
        /// dataTable分页
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static DataTable SplitDataTable(this DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
                return dt;
            DataTable newdt = dt.Clone();
            //newdt.Clear();
            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }

            return newdt;
        }
    }

}