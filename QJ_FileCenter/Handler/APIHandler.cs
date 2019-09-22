using glTech.Log4netWrapper;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using QJ_FileCenter;
using QJ_FileCenter.Domains;
using QJ_FileCenter.Models;
using QJFile.Data;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace QJ_FileCenter.Handler
{
    public class APIHandler : NancyModule
    {
        public APIHandler()
            : base()
        {
            Msg_Result msg = new Msg_Result() { Action = "", ErrorMsg = "" };
            JH_Auth_UserB.UserInfo UserInfo = new JH_Auth_UserB.UserInfo();

            Before += ctx =>
            {
                try
                {
                    //Logger.LogInfo("请求地址" + ctx.Request.Path);
                    if (ctx.Request.Path == "/adminapi/login")
                    {
                        return ctx.Response;
                    }
                    else
                    if (ctx.Request.Path == "/")
                    {
                        return Response.AsRedirect("/Web/Login.html");
                    }
                    else
                    if (ctx.Request.Path.StartsWith("/adminapi/dfile"))
                    {
                        return ctx.Response;
                    }
                    else
                    if (ctx.Request.Path.StartsWith("/adminapi/ExeActionPub"))
                    {
                        return ctx.Response;
                    }
                    else
                    {
                        string strUser = ctx.Request.Cookies["user"];
                        string strpasd = ctx.Request.Cookies["pasd"];
                        var users = new JH_Auth_UserB().GetEntities(d => d.username == strUser && d.pasd == CommonHelp.GetMD5(strpasd)).ToList();

                        if (users.Count() != 1)
                        {
                            msg.ErrorMsg = "NOSESSIONCODE";
                            return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
                        }
                        else
                        {
                            UserInfo.User = users[0];
                        }
                        return ctx.Response;
                    }
                }
                catch (Exception ex)
                {

                    msg.ErrorMsg = ex.Message.ToString();
                    return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
                }

            };
            Get["/"] = p =>
            {
                return Response.AsRedirect("/Web/Login.html");
                //return View["login"];
            };
            ///获取文件
            Get["/adminapi/dfile/{fileid}"] = p =>
            {
                int fileId = 0;
                int.TryParse(p.fileid.Value.Split(',')[0], out fileId);
                FT_File file = new FT_FileB().GetEntity(d => d.ID == fileId);
                string strzyid = p.fileid.Value;
                if (file != null)
                {
                    strzyid = file.zyid;
                }

                string width = Context.Request.Query["width"].Value ?? "";
                string height = Context.Request.Query["height"].Value ?? "";
                Qycode qy = new QycodeB().GetALLEntities().FirstOrDefault();
                string filename = Context.Request.Url.SiteBase + "/" + qy.Code + "/document/" + strzyid;
                if (width + height != "")
                {
                    filename = Context.Request.Url.SiteBase + "/" + qy.Code + "/document/image/" + strzyid + (width + height != "" ? ("/" + width + "/" + height) : "");
                }
                return Response.AsRedirect(filename);

            };
            //登录接口
            Post["/adminapi/login"] = p =>
            {
                string strUser = Context.Request.Form["user"];
                string strpasd = Context.Request.Form["pasd"];
                var users = new JH_Auth_UserB().GetEntities(d => d.username == strUser && d.pasd == CommonHelp.GetMD5(strpasd));
                if (users.Count() == 1)
                {
                    msg.Result = "Y";
                    msg.Result1 = new JH_Auth_UserB().GetUserInfo(strUser, strpasd);
                    Qycode qycode = new QycodeB().GetALLEntities().FirstOrDefault();
                    msg.Result4 = qycode;

                }
                else
                {
                    msg.Result = "N";
                }
                //JsonConvert.SerializeObject(Model).Replace("null", "\"\"");
                return Response.AsJson(msg);
            };

            //普通用户接口
            Post["/adminapi/ExeAction/{action}"] = p =>
            {
                JObject JsonData = new JObject();

                foreach (string item in Context.Request.Form.Keys)
                {
                    JsonData.Add(item, Context.Request.Form[item].Value);
                }
                string PostData = "";
                string P1 = JsonData["P1"] == null ? "" : JsonData["P1"].ToString();
                string P2 = JsonData["P2"] == null ? "" : JsonData["P2"].ToString();
                string strAction = p.action;


                Qycode qy = new QycodeB().GetALLEntities().FirstOrDefault();
                JH_Auth_QY QYinfo = new JH_Auth_QY();
                QYinfo.FileServerUrl = Context.Request.Url.SiteBase;
                QYinfo.QYCode = qy.Code;
                UserInfo.QYinfo = QYinfo;
                //Dictionary<string, string> results3 = JsonConvert.DeserializeObject<Dictionary<string, string>>(PostData.ToString());
                var function = Activator.CreateInstance(typeof(QYWDManage)) as QYWDManage;
                var method = function.GetType().GetMethod(strAction.ToUpper());
                method.Invoke(function, new object[] { JsonData, msg, P1, P2, UserInfo });

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                string Result = JsonConvert.SerializeObject(msg, Formatting.Indented, timeConverter).Replace("null", "\"\"");
                return Response.AsText(Result, "text/html; charset=utf-8");
            };

            //管理用户接口
            Post["/adminapi/ExeActionAuth/{action}"] = p =>
            {
                JObject JsonData = new JObject();

                foreach (string item in Context.Request.Form.Keys)
                {
                    JsonData.Add(item, Context.Request.Form[item].Value);
                }
                string PostData = "";
                string P1 = JsonData["P1"] == null ? "" : JsonData["P1"].ToString();
                string P2 = JsonData["P2"] == null ? "" : JsonData["P2"].ToString();
                string strAction = p.action;


                Qycode qy = new QycodeB().GetALLEntities().FirstOrDefault();
                JH_Auth_QY QYinfo = new JH_Auth_QY();
                QYinfo.FileServerUrl = Context.Request.Url.SiteBase;
                QYinfo.QYCode = qy.Code;
                UserInfo.QYinfo = QYinfo;
                //Dictionary<string, string> results3 = JsonConvert.DeserializeObject<Dictionary<string, string>>(PostData.ToString());
                var function = Activator.CreateInstance(typeof(QYWDManage)) as QYWDManage;
                var method = function.GetType().GetMethod(strAction.ToUpper());
                method.Invoke(function, new object[] { JsonData, msg, P1, P2, UserInfo });

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                string Result = JsonConvert.SerializeObject(msg, Formatting.Indented, timeConverter).Replace("null", "\"\"");
                return Response.AsText(Result, "text/html; charset=utf-8");
            };

            //外部接口
            Post["/adminapi/ExeActionPub/{action}"] = p =>
            {
                JObject JsonData = new JObject();

                foreach (string item in Context.Request.Form.Keys)
                {
                    JsonData.Add(item, Context.Request.Form[item].Value);
                }
                string PostData = "";
                string P1 = JsonData["P1"] == null ? "" : JsonData["P1"].ToString();
                string P2 = JsonData["P2"] == null ? "" : JsonData["P2"].ToString();
                string strAction = p.action;


                Qycode qy = new QycodeB().GetALLEntities().FirstOrDefault();
                JH_Auth_QY QYinfo = new JH_Auth_QY();
                QYinfo.FileServerUrl = Context.Request.Url.SiteBase;
                QYinfo.QYCode = qy.Code;
                UserInfo.QYinfo = QYinfo;
                //Dictionary<string, string> results3 = JsonConvert.DeserializeObject<Dictionary<string, string>>(PostData.ToString());
                var function = Activator.CreateInstance(typeof(PubManage)) as PubManage;
                var method = function.GetType().GetMethod(strAction.ToUpper());
                method.Invoke(function, new object[] { JsonData, msg, P1, P2, UserInfo });

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                string Result = JsonConvert.SerializeObject(msg, Formatting.Indented, timeConverter).Replace("null", "\"\"");
                return Response.AsText(Result, "text/html; charset=utf-8");
            };

            After += ctx =>
            {
                msg.Action = Context.Request.Path;
                //添加日志
            };
        }
    }
}
