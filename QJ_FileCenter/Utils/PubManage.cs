using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using QJFile.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;


namespace QJ_FileCenter
{
    public class PubManage
    {



        #region 分享查看文档操作
        /// <summary>
        /// 获取文档资源
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="UserInfo"></param>
        public void GETWDZY(JObject context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            Document ff = new DocumentB().GetEntities(p => p.ID == P1).FirstOrDefault();
            if (ff != null)
            {
                msg.Result = ff.FileName;
                msg.Result1 = ff.ylinfo;
            }
            else
            {
                msg.ErrorMsg = "此文件不存在或您没有权限！";
            }
        }
        public void GETSHAREINFO(JObject context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int ID = 0;
            int.TryParse(P1, out ID);
            if (ID > 0)
            {
                FT_File_Share Model = new FT_File_ShareB().GetEntity(d => d.ID == ID);


                if (Model.SharePasd == P2 || Model.ShareType == "0")//公开链接或者输入提取码正确
                {
                    string strSql = string.Format(@"SELECT share.CRUserName,share.RefType,share.ShareDueDate,share.CRDate,CASE WHEN share.RefType='file' then f.Name WHEN share.RefType='wj'  THEN folder.Name END Name 
                                            ,CASE WHEN share.RefType='file' then f.ID WHEN share.RefType='wj'  THEN folder.ID END  ID ,f.FileExtendName,f.FileSize,share.ComId,f.ISYL,f.FileMD5,f.YLUrl
                                            from FT_File_Share share 
                                            LEFT join FT_File f on share.RefID=f.ID and share.ComId=f.ComId and share.RefType='file'
                                            LEFT join  FT_Folder folder on share.RefID=folder.ID and share.RefType='wj' where share.ID={0} and share.IsDel!='Y'", ID);
                    DataTable dt = new FT_File_ShareB().GetDTByCommand(strSql);
                    if (dt.Rows.Count > 0)
                    {
                        DateTime dueDate = DateTime.Now;
                        if (DateTime.TryParse(dt.Rows[0]["ShareDueDate"].ToString(), out dueDate) && dueDate > DateTime.Now)
                        {
                            msg.Result = dt;
                            msg.Result1 = appsetingB.GetValueByKey("qyname");
                            msg.Result2 = appsetingB.GetValueByKey("qyico");
                            msg.Result3 = appsetingB.GetValueByKey("sysname");

                        }
                        else
                        {
                            msg.ErrorMsg = "分享已过期";
                        }
                    }
                    else
                    {
                        msg.ErrorMsg = "分享已取消";
                    }
                }
                else
                {
                    msg.Result = 1;
                    msg.ErrorMsg = "提取码有误，请重新输入";
                }

            }
        }
        //分享页面根据文件夹Id获取文件列表
        public void GETFILELIST(JObject context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int FolderID = int.Parse(P1);//
            msg.Result = new FT_FolderB().GetEntities(d => d.PFolderID == FolderID);
            msg.Result1 = new FT_FileB().GetEntities(d => d.FolderID == FolderID);
            return;
        }


        /// <summary>
        /// 判断是否公开链接
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="UserInfo"></param>
        public void ISPUBLIC(JObject context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            int ID = int.Parse(P1);//
            FT_File_Share Model = new FT_File_ShareB().GetEntity(d => d.ID == ID && d.IsDel != "Y");
            msg.Result = "0";//默认公开分享
            if (Model != null)
            {
                msg.Result = Model.ShareType;
                if (Model.ShareDueDate < DateTime.Now)
                {
                    msg.Result1 = "-1";//过期了
                }
            }
        }
        /// <summary>
        /// 向服务器发送压缩目录命令
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="P1">目录ID</param>
        /// <param name="P2"></param>
        /// <param name="UserInfo"></param>
        public void COMPRESSFOLDER(JObject context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string strCode = P1;
            FT_FolderB.FoldFile Mode = new FT_FolderB.FoldFile();
            Mode.FolderID = -1;
            Mode.Name = "压缩文件";
            Mode.SubFileS = new List<FT_File>();
            Mode.SubFolder = new List<FT_FolderB.FoldFile>();
            foreach (string item in P1.SplitTOList(','))
            {
                int FileID = int.Parse(item.Split('|')[0].ToString());
                string strType = item.Split('|')[1].ToString();
                if (item.Split('|')[1].ToString() == "file")
                {
                    FT_File file = new FT_FileB().GetEntity(d => d.ID == FileID);
                    file.YLUrl = "";
                    Mode.SubFileS.Add(file);
                }
                else
                {
                    List<FT_FolderB.FoldFileItem> ListID = new List<FT_FolderB.FoldFileItem>();
                    FT_FolderB.FoldFile obj = new FT_FolderB().GetWDTREE(FileID, ref ListID);
                    Mode.SubFolder.Add(obj);
                }
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string Result = JsonConvert.SerializeObject(Mode, Formatting.Indented, timeConverter).Replace("null", "\"\"");
            string strData = new FileHelp().CompressZip(Result, UserInfo.QYinfo);
            msg.Result = strData;
        }

        /// <summary>
        /// 获取页面html(excel)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="UserInfo"></param>
        public void GETHTML(JObject context, Msg_Result msg, string P1, string P2, JH_Auth_UserB.UserInfo UserInfo)
        {
            string strWDYM = CommonHelp.GetConfig("WDYM");

            FT_File ff = new FT_FileB().GetEntities(p => p.YLCode == P1).FirstOrDefault();
            if (ff != null)
            {
                //定义局部变量
                HttpWebRequest httpWebRequest = null;
                HttpWebResponse httpWebRespones = null;
                Stream stream = null;
                string htmlString = string.Empty;
                string url = strWDYM + ff.YLPath;

                //请求页面
                try
                {
                    httpWebRequest = WebRequest.Create(url + ".html") as HttpWebRequest;
                }
                //处理异常
                catch
                {
                    msg.ErrorMsg = "建立页面请求时发生错误！";
                }
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; Maxthon 2.0)";
                //获取服务器的返回信息
                try
                {
                    httpWebRespones = (HttpWebResponse)httpWebRequest.GetResponse();
                    stream = httpWebRespones.GetResponseStream();
                }
                //处理异常
                catch
                {
                    msg.ErrorMsg = "接受服务器返回页面时发生错误！";
                }

                StreamReader streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
                //读取返回页面
                try
                {
                    htmlString = streamReader.ReadToEnd();
                }
                //处理异常
                catch
                {
                    msg.ErrorMsg = "读取页面数据时发生错误！";
                }
                //释放资源返回结果
                streamReader.Close();
                stream.Close();

                msg.Result = htmlString;
                msg.Result1 = url;

            }
            else
            {
                msg.ErrorMsg = "此文件不存在或您没有权限！";
            }
        }
        #endregion










    }
}