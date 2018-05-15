using glTech.Log4netWrapper;
using Nancy;
using QJ_FileCenter.Models;
using QJ_FileCenter.Utils;
using System;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using QJ_FileCenter.Domains;

namespace QJ_FileCenter.Handler
{
    public class AdminHandler : RavenModule
    {


        public AdminHandler()
            : base()
        {
            Msg_Result Model = new Msg_Result() { Action = "", ErrorMsg = "" };
            Before += ctx =>
            {
                //new userlogB().Insert(new userlog {    });
                return ctx.Response;
            };

            Get["/admin/index"] = p =>
            {
                return View["index"];
            };
            Get["/admin/loading"] = p =>
            {
                return View["Loading"];
            };
            Get["/admin/temp/xtpz"] = p =>
            {
                return View["Temp/xtpz.html"];
            };
            Get["/admin/temp/shouye"] = p =>
            {
                return View["Temp/shouye.html"];
            };
            Get["/admin/temp/qygl"] = p =>
            {
                return View["Temp/qygl.html"];
            };
            Get["/admin/temp/wjgl"] = p =>
            {
                return View["Temp/wjgl.html"];
            };

            After += ctx =>
            {
                Model.Action = Context.Request.Path;
                //添加日志
            };
        }
    }
}
