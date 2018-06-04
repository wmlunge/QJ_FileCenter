using Nancy;
using System;

namespace QJ_FileCenter
{
    public class MainController : Nancy.NancyModule
    {

        public MainController()
        {
            Get["/"] = p =>
            {
                return View["login"];
            };
            Get["/login"] = p =>
            {
                return View["login"];
            };

        }
    }
}
