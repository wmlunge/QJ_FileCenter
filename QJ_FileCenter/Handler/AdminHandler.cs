using glTech.Log4netWrapper;
using Nancy;
using Newtonsoft.Json;
using QJ_FileCenter;
using QJ_FileCenter.Domains;
using QJ_FileCenter.Models;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace QJ_FileCenter.Handler
{
    public class AdminHandler : NancyModule
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
            Get["/"] = p =>
            {
                return View["login"];
            };
            Get["/login"] = p =>
            {
                return View["login"];
            };
            Get["/admin/index"] = p =>
            {
                return View["index"];
            };
            Get["/admin/loading"] = p =>
            {
                return View["Loading"];
            };
           
            Get["/admin/page/{page}"] = p =>
            {
                return View["Page/" + p.page + ".html"];
            };
            After += ctx =>
            {
                Model.Action = Context.Request.Path;
                //添加日志
            };
        }
    }
}
