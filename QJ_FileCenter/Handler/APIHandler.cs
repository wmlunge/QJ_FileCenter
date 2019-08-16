using glTech.Log4netWrapper;
using Nancy;
using QJ_FileCenter.Models;
using QJ_FileCenter;
using System;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using QJ_FileCenter.Domains;
using QJFile.Data;

namespace QJ_FileCenter.Handler
{
    public class APIHandler : NancyModule
    {
        public APIHandler()
            : base()
        {
            Msg_Result msg = new Msg_Result() { Action = "", ErrorMsg = "" };
            Before += ctx =>
            {
                try
                {
                    if (ctx.Request.Path == "/adminapi/login")
                    {
                        return ctx.Response;
                    }
                    else
                    {
                        string strUser = ctx.Request.Cookies["user"];
                        string strpasd = ctx.Request.Cookies["pasd"];
                        if (!new userB().isAuth(strUser, strpasd))
                        {
                            msg.ErrorMsg = "NOSESSIONCODE";
                            return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
                        }
                        return ctx.Response;
                    }
                }
                catch (Exception)
                {

                    msg.ErrorMsg = "程序错误！";
                    return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
                }
               
            };
            //登录接口
            Post["/adminapi/login"] = p =>
            {
                string strUser = Context.Request.Form["user"];
                string strpasd = Context.Request.Form["pasd"];
                var users = new userB().GetEntities(d => d.username == strUser && d.pasd == strpasd);
                if (users.Count() == 1)
                {
                    msg.Result = "Y";
                    msg.Result1 = users.FirstOrDefault();
                }
                else
                {
                    msg.Result = "N";
                }
                //JsonConvert.SerializeObject(Model).Replace("null", "\"\"");
                return Response.AsJson(msg);
            };
            ///获取首页信息
            Post["/adminapi/getsydata"] = p =>
            {


                msg.Result = new QycodeB().GetALLEntities().Count();
                msg.Result1 = new DocumentB().GetALLEntities().Count();
                msg.Result2 = new DocumentB().GetALLEntities().Sum(d => long.Parse(d.filesize)).ToString();

                //JsonConvert.SerializeObject(Model).Replace("null", "\"\"");
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
            
            //获取空间列表
            Post["/adminapi/getqy"] = p =>
            {
                msg.Result = new QycodeB().GetALLEntities();
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
            Post["/adminapi/upmm"] = p =>
            {
                string P1 = Context.Request.Form["P1"];
                string P2 = Context.Request.Form["P2"];
                user User = new userB().GetALLEntities().FirstOrDefault();
                User.pasd = P2;
                new userB().Update(User);
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
            
            //管理存储空间
            Post["/adminapi/mangeqy"] = p =>
            {
                string P1 = Context.Request.Form["P1"];
                Qycode model = JsonConvert.DeserializeObject<Qycode>(P1);

                if (string.IsNullOrEmpty(model.Name))
                {
                    msg.ErrorMsg = "存储空间名称不能为空";
                }
                else
                {
                    if (model.ID == 0)
                    {
                        Qycode nmodel = new QycodeB().GetEntity(d => d.Name == model.Name);
                        if (nmodel != null)
                        {
                            msg.ErrorMsg = "存储空间已存在";
                        }
                    }
                }
                if (model.ID == 0)
                {
                    model.crdate = DateTime.Now;
                    model.filecount = 0;
                    model.space = "0";
                    model.yyspace = "0";
                    new QycodeB().Insert(model);
                }
                else
                {
                    new QycodeB().Update(model);
                }
                msg.Result = model;
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
            //删除存储空间
            Post["/adminapi/delqy"] = p =>
            {

                string QYCODE = Context.Request.Form["P1"];
                if (!new QycodeB().Delete(D => D.Code == QYCODE))
                {
                    msg.ErrorMsg = "删除失败";
                }
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
            //获取文件列表
            Post["/adminapi/getfilelist"] = p =>
            {
                int page = 0;
                int pagecount = 8;
                int.TryParse(Context.Request.Form["p"] ?? "1", out page);
                int.TryParse(Context.Request.Form["pagecount"] ?? "8", out pagecount);//页数
                page = page == 0 ? 1 : page;

                string filename = Context.Request.Form["P1"];

                if (!string.IsNullOrEmpty(filename))
                {
                    int total = new DocumentB().GetEntities(d=>d.FileName.Contains(filename) || d.Qycode.Contains(filename)).Count();
                    var files = new DocumentB().GetEntities(d => d.FileName.Contains(filename) || d.Qycode.Contains(filename)).OrderByDescending(d => d.RDate).Take(pagecount * page).Skip(pagecount * (page - 1)).ToList();
                    msg.Result = files;
                    msg.Result1 = total;
                }
                else
                {
                    int total = new DocumentB().GetALLEntities().Count();
                    var files = new DocumentB().GetALLEntities().OrderByDescending(d => d.RDate).Take(pagecount * page).Skip(pagecount * (page - 1)).ToList();
                    msg.Result = files;
                    msg.Result1 = total;

                }
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
         
            //删除文件
            Post["/adminapi/delwj/{id}"] = p =>
            {
                string ID = p.id;
                Document Model = new DocumentB().GetEntity(D => D.ID == ID);
                string strFile = Model.FullPath;
                if (!new DocumentB().Delete(Model))
                {
                    msg.ErrorMsg = "删除失败";
                }
                else
                {
                    File.Delete(strFile);
                }
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };

            //获取操作日志
            Post["/adminapi/getloglist"] = p =>
            {
                int page = 0;
                int pagecount = 8;
                int.TryParse(Context.Request.Form["p"] ?? "1", out page);
                int.TryParse(Context.Request.Form["pagecount"] ?? "8", out pagecount);//页数
                page = page == 0 ? 1 : page;

                string logname = Context.Request.Form["P1"];

                if (!string.IsNullOrEmpty(logname))
                {
                    int total = new userlogB().GetEntities(d => d.useraction.Contains(logname)).Count();
                    var files = new userlogB().GetEntities(d => d.useraction.Contains(logname)).OrderByDescending(d => d.ID).Take(pagecount * page).Skip(pagecount * (page - 1)).ToList();
                    msg.Result = files;
                    msg.Result1 = total;
                }
                else
                {
                    int total = new userlogB().GetALLEntities().Count();
                    var files = new userlogB().GetALLEntities().OrderByDescending(d => d.ID).Take(pagecount * page).Skip(pagecount * (page - 1)).ToList();
                    msg.Result = files;
                    msg.Result1 = total;

                }
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };

            //删除日志
            Post["/adminapi/delrz"] = p =>
            {
                new userlogB().Delete(d => d.useraction != "");
                return Response.AsText(JsonConvert.SerializeObject(msg), "text/html; charset=utf-8");
            };
            After += ctx =>
            {
                msg.Action = Context.Request.Path;
                //添加日志
            };
        }
    }
}
