﻿var model = avalon.define({
    $id: "IndexV5",
    userName: ComFunJS.getnowuser(),
    CommonData: [],//消息中心
    yytype: "WORK",
    UserInfo: {},//用户缓存数据
    isshowload: true,
    isiframe: "N",
    XXCount: 0,//消息数量
    QYData: { sysname: "", qyname: "", qyico: "" },
    YYData: [],
    LMData: [],

    QYHDData: [], //企业活动
    wigetdata: [],//工作台组件数据
    initobj: "",//初始化要传给组件的数据
    ishasLeft: false,//是否要隐藏左侧菜单
    FunData: [],//选中模块
    isnull: false,//是否有数据
    MenuResult: [],
    setyy: function () {
        model.yytype = (model.yytype == "WORK" ? "XT" : "WORK");
        $(".nav-list ul li:visible").eq(0).click();

    },//切换工作台
    SelModelMenu: function (item) {
        var nowTime = new Date().getTime();
        var clickTime = $("body").data("ctime");
        if (clickTime != 'undefined' && (nowTime - clickTime < 1000) && item) {
            console.debug('操作过于频繁，稍后再试');
            return false;
        }
        else {
            $("body").data("ctime", nowTime);
            model.FunData.clear();
            if (item) {
                model.SelModel = item;
                model.FunData.pushArray(item.FunData.$model);
                model.ishasLeft = false;
            } else {
                model.SelModel = null;
                if (localStorage.getItem("WIGETDATAV5")) {
                    model.FunData.pushArray(JSON.parse(localStorage.getItem("WIGETDATAV5")));
                }
                else {
                    // model.FunData = [{ code: "RWGL", name: "任务管理", wigetpath: "RWGL/RWGLLIST", issel: true, isshow: true, order: 0 }, { code: "LCSP", name: "流程管理", wigetpath: "RWGL/RWGLLIST", issel: false, isshow: true, order: 2 }, { code: "NOTE", name: "记事本", wigetpath: "RWGL/RWGLLIST", issel: false, isshow: true, order: 3 }, { code: "KJFS", name: "快捷网址", wigetpath: "RWGL/RWGLLIST", issel: false, isshow: true, order: 4 }];
                    model.FunData.pushArray([
                        { PageCode: "/Web/Html/Page/bzview.html", ExtData: "", code: "KJFS", PageName: "帮助中心", issel: true, isshow: true, order: 0 }
                    ]);
                    localStorage.setItem("WIGETDATAV5", JSON.stringify(model.FunData.$model));

                }
            }
            model.selmenulev2(model.FunData[0]);
            $('body,html').animate({ scrollTop: 0 }, '500');
        }

    },//选中最左侧事件
    selmenulev2: function (item, dom) {
        model.isiframe = item.isiframe;
        if (model.isiframe == 'Y') {
            var pagecode = item.PageCode.indexOf("html") > -1 ? item.PageCode : item.PageCode + ".html";
            $("#main").attr("src", pagecode).css("min-height", (window.innerHeight - 150) + 'px').parent().css("height", $("#main").height());
        } else {
            var nowTime = new Date().getTime();
            var clickTime = $("body").data("me2time");
            if (clickTime != 'undefined' && (nowTime - clickTime < 1000) && dom) {
                console.debug('操作过于频繁，稍后再试');
                return false;
            } else {
                $("body").data("me2time", nowTime);
                model.isnull = false;
                model.initobj = null;//先清空数据
                model.PageCode = "/Web/Html/loading.html";
                gomenu = function () {
                    model.PageCode = item.PageCode;
                    if (localStorage.getItem(model.PageCode + "pagecount")) {
                        model.page.pagecount = localStorage.getItem(model.PageCode + "pagecount");
                    } else {
                        model.page.pagecount = 10;
                    }
                    model.initobj = item.ExtData;
                    model.page.pageindex = 1;
                    model.page.total = 0;
                    model.ShowColumns = [];
                    model.TypeData = [];
                    model.ListData = [];
                    model.search.seartype = '1';
                    model.search.searchcontent = '';

                    //清除日历样式
                    $(".datetimepicker").remove()
                }
                setTimeout("gomenu()", 500)
            }
        }

    },
    //选中二级菜单事件
    ChangePage: function (pagedata) {
        model.selmenulev2(pagedata);
    },
    refpage: function (pagecode) {
        model.rdm = ComFunJS.getnowdate('yyyy-mm-dd hh:mm:ss');
        if (model.isiframe == 'Y') {
            $('#main').attr('src', $('#main').attr('src'));
        } else {
            if (pagecode) {

                for (var i = 0; i < model.FunData.length; i++) {
                    if (model.FunData[i].PageCode.indexOf(pagecode) > -1) {
                        model.selmenulev2(model.FunData[i]);
                        return;
                    }
                }
            } else {
                model.refpage(model.PageCode)
            }
        }


    },//刷新页面
    initwork: function () {
        localStorage.removeItem("WIGETDATAV5");
        location.reload();
    },
    setwork: function () {
        if (model.SelModel) {
            var temp = JSON.parse(localStorage.getItem("WIGETDATAV5"));
            temp.forEach(function (el) {
                if (el.PageCode == model.PageCode) {
                    return;
                }
            })
            model.FunData.forEach(function (item) {
                if (model.PageCode == item.PageCode) {
                    var fun = { PageCode: item.PageCode, ExtData: item.ExtData, code: model.SelModel.ModelCode, PageName: item.PageName, issel: true, isshow: true, order: temp.length * 1 + 1 }
                    temp.push(fun)
                }
            })
            localStorage.setItem("WIGETDATAV5", JSON.stringify(temp));
            ComFunJS.winsuccess("操作成功");
        }
    },//设置当前模块到控制台
    PageCode: "/Web/Html/loading.html",//需要加载的模板
    rdm: ComFunJS.getnowdate('yyyy-mm-dd hh:mm'),//随机数
    Temprender: function () {
        if (typeof (tempindex) != "undefined" && model.PageCode != "/Web/Html/loading.html") {
            tempindex.InitWigetData(model.initobj);
        }
    },//组件加载完成事件
    exit: function () {
        ComFunJS.winconfirm("确认要退出吗？", function () {
            ComFunJS.delCookie('szhlcode');

            location.href = "/Web/Login.html"
        })
    },//退出事件
    refiframe: function () {
        location.reload();

    }, //刷新当前页面
    selyyType: function (item, dom) {
        $(".yytype").removeClass("active")
        $(dom).children("a").addClass("active")
        model.yytype = item.TYPE;

        $(".nav-list ul li:visible").eq(0).click();
    },//应用类别
    GetUserData: function (callbact) {
        $.getJSON('/adminapi/ExeAction/GETUSERINFO', {}, function (resultData) {
            if (resultData.ErrorMsg == "") {
                model.UserInfo = resultData.Result;
                model.QYData.sysname = resultData.Result1;
                model.QYData.qyname = resultData.Result2;
                model.QYData.qyico = resultData.Result3;
                ComFunJS.setCookie('qycode', resultData.Result4.Code + "," + resultData.Result4.secret);
                ComFunJS.setCookie("user", resultData.Result.User.username);
                $(document).attr("title", resultData.Result1 + "");//修改title值
                // model.GetCompanyAuth();
                ComFunJS.setCookie('fileapi', "");
                model.GetYYList();
                if (callbact) {
                    return callbact.call(this);
                }

            }
        })
    }, //获取用户信息
    AuthList: [],//用户有权限的菜单
    KJFSData: [],//可以设置快捷方式的功能
    GetCompanyAuth: function () {
        var authmenu = [{ "ID": 27, "ModelName": "企业管理", "ModelType": "工作台", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "XTGL", "ComId": 0, "IsKJFS": 0, "PModelCode": "WORK", "QYModelId": 5657, "FunData": [] }];
        if (authmenu) {
            authmenu.forEach(function (val) {
                val.isshow = true;
            })
        }
        model.AuthList = authmenu;
    }, //用户权限信息

    AddView: function (code, Name, ID, pcode, event) {
        if (event) {
            event.stopPropagation();
        }
        if (code == "QYTX" || code == "DXGL") {
            ComFunJS.winviewform("/View/Base/APP_ADD_WF.html?FormCode=" + code, Name, "1000");
        }
        else {
            if (!ID) {
                ID = "";
            }
            if (pcode == "CRM") {
                code = pcode + "_" + code;
            }
            ComFunJS.winviewform("/ViewV5/AppPage/APP_ADD_WF.html?FormCode=" + code + "&ID=" + ID, Name, "1000");

        }
    },//添加表格
    AddViewNOWF: function (code, name) {
        ComFunJS.winviewform("/ViewV5/AppPage/APP_ADD.html?PathCode=" + code, name, "1000");
    },
    ViewForm: function (code, ID, PIID, event) {
        event = event ? event : window.event
        var obj = event.srcElement ? event.srcElement : event.target;
        if ($(obj).hasClass("icon-check") || $(obj).attr("type") == "checkbox") {
            return;
        } else {
            ComFunJS.winviewform("/ViewV5/AppPage/APPVIEW.html?FormCode=" + code + "&ID=" + ID + "&PIID=" + PIID + "&r=" + Math.random(), "查看");

        }
    },//查看表格方法
    ViewFormNew: function (code, ID, PIID, event) {
        event = event ? event : window.event
        var obj = event.srcElement ? event.srcElement : event.target;
        if ($(obj).hasClass("lk")) {
            ComFunJS.winviewform("/ViewV5/AppPage/APPVIEW.html?FormCode=" + code + "&ID=" + ID + "&PIID=" + PIID + "&r=" + Math.random(), "查看");

        }
    },//查看表格方法
    EditForm: function (code, ID, PIID, event) {
        if (event) {
            event.stopPropagation();
        }
        ComFunJS.winviewform("/ViewV5/AppPage/APP_ADD_WF.html?FormCode=" + code + "&ID=" + ID + "&PIID=" + PIID + "&r=" + Math.random(), "修改", "1000");
    },
    UseYYList: [],
    UseSysYYList: [],
    GetYYList: function () {

        var MenuResult = model.MenuResult;
        MenuResult.forEach(function (val, i) {
            val.issel = val.issel == "True";
        })
        if (MenuResult) {
            MenuResult.forEach(function (val) {
                val.FunData.forEach(function (c) {
                    c.isshow = true;
                })
            })
        }
        // YYData
        var temp = [];
        for (var i = 0; i < MenuResult.length; i++) {
            if ($.inArray(MenuResult[i].ModelType, temp) == -1) {
                temp.push(MenuResult[i].ModelType)
                model.LMData.push({ "TYPE": MenuResult[i].PModelCode, "NAME": MenuResult[i].ModelType, "ISSEL": "N" });
            }

        }
        model.UseYYList = MenuResult;
        $(".nav-list ul li:visible").eq(0).click();

        //var isShow = "N";
        //if (isShow == "Y") {
        //    $(".nav-list ul li:visible").eq(1).click();
        //} else {
        //    model.SelModelMenu('');
        //}

    },
    ModifyPwd: function (dom) {
        var pwd = $("#newPwd").val();
        var pwd2 = $("#newPwd2").val();
        var retmsg = "";
        if ($("#UpdatePDModal .szhl_require")) {
            $("#UpdatePDModal .szhl_require").each(function () {
                if ($(this).val() == "") {
                    retmsg = $(this).parent().find("label").text() + "不能为空";
                }
            })
        }
        if (retmsg !== "") {
            layer.tips(retmsg, dom);
            return;
        }
        if (pwd != pwd2) {
            retmsg = "确认密码不一致";
            layer.tips(retmsg, dom);
            return;
        }
        $.getJSON('/adminapi/ExeActionAuth/UPMM', { P1: pwd, P2: pwd2 }, function (resultData) {
            if (resultData.ErrorMsg == "") {
                ComFunJS.winsuccess("操作成功");
                $('#UpdatePDModal').modal('hide');
            }
        })
        $("#newPwd").val("");
        $("#newPwd2").val("");
    },  //修改密码  待完善
    SaveWigetdata: function () {
        localStorage.setItem("WIGETDATAV5", JSON.stringify(model.FunData.$model));
        model.SelModelMenu('');
    },
    ChangeModelIsShow: function (item, dom, event) {
        if (event) {
            event.stopPropagation();
        }
        item.isshow = !item.isshow;
        model.SaveWigetdata();
    },

    //***通用列表页需要的数据***//
    TypeData: [], //类型数据
    GetTypeData: function (P1, callback) {//P1:字典类别，callback:回调函数,p2:字典类别ID
        $.getJSON('/API/VIEWAPI.ashx?Action=XTGL_GETZIDIANSLIST', { P1: P1 }, function (resultData) {
            if (resultData.ErrorMsg == "") {
                if (callback) {
                    return callback.call(this, resultData.Result);
                }
                else {
                    model.TypeData = resultData.Result;
                }
            }
        })
    },
    UserCustomData: [],

    ListData: [], //列表页数据
    page: { pageindex: 1, pagecount: 10, total: 0 }, //分页参数
    pageNum: [{ "num": 10 }, { "num": 20 }, { "num": 30 }, { "num": 50 }, { "num": 100 }],

    ReSetShow: function (el) { //控制扩展字段展示
        if (el && model.SelModel) {
            if (model.SelModel.ModelCode != 'LCSP' && el.type != "link") {
                el.IsSel = !el.IsSel;
                localStorage.setItem(model.SelModel.ModelCode + "ShowColumns", JSON.stringify(model.ShowColumns));
            }
            else {
                if (typeof (tempindex) != "undefined") {
                    tempindex.ReSetShow(el);
                }
            }
        }
    },
    search: { seartype: "1", searchcontent: "" },
    FnFormat: function (str, fmt) { //格式化
        str = str + "";
        if ((str || fmt.format == "gzstatus") && fmt.format) {

            switch (fmt.format) {
                case "shstate": //审核状态转换成文字
                    {
                        if (str == "0") {
                            str = "未审核";
                        } else if (str == "-1") {
                            str = "审核不通过";
                        } else if (str == "1") {
                            str = "审核通过";
                        }
                    }
                    break;

                case "statename": //审核流程，-1时不需要流程
                    {
                        if (str == "-1") {
                            str = "";
                        }
                        else if (str == "已退回") {
                            str = "<span style='color:red;font-weight:bold'>" + str + "</span>";
                        }
                    }
                    break;
                case "rwstate": //任务状态
                    {
                        if (str == "0") str = "待办任务";
                        else if (str == "1") str = "已办任务";
                        else if (str == "2") str = "过期任务";
                    }
                    break;
                case "dateformat": //日期格式，默认yyyy-mm-dd
                    {
                        str = ComFunJS.getnowdate("yyyy-mm-dd", str);
                    }
                    break;
                case "timeformat": //日期格式，默认yyyy-mm-dd
                    {
                        str = ComFunJS.getnowdate("yyyy-mm-dd hh:mm", str);
                    }
                    break;
                case "username": //用户id转成为用户名
                    {
                        str = ComFunJS.convertuser(str);
                    }
                    break;
                case "qrcode": //二维码图片展示
                    {
                        str = "<img src='" + str + "' style='width:60px;height:60px;' />"
                    }
                    break;
                case "bqh"://表情转换
                    {
                        return ComFunJS.bqhContent(str);
                    }
                    break;
                case "text"://截取字符串
                    {
                        str = ComFunJS.convstr(str);
                    }
                    break;
                case "txfs"://提醒方式
                    {
                        switch (str) {
                            case "0": str = '短信和微信'; break;
                            case "1": str = '短信'; break;
                            case "2": str = '微信'; break;
                        }

                    }
                    break;
                case "xxshzt":     //信息发布审核状态
                    {
                        switch (str) {
                            case "0": str = "未审核"; break;
                            case "1": str = "已审核"; break;
                            case "2": str = "正常"; break;
                            case "-1": str = "<label class='text-danger'>退回</label>"; break;
                        }
                    }
                    break;
                default: {

                }
            }


        }
        if (fmt.len) {
            str = str.length > fmt.len ? str.substring(0, fmt.len) + '...' : str;
        }
        return str;

    },

    //***通用列表页需要的数据***//
    mouseover: function () {
        $(this).find(".tool").css("display", "block");
    },
    mouseout: function () {
        $(this).find(".tool").css("display", "none");
    },//鼠标移动控制显示和隐藏
})
avalon.ready(function () {
    var MenuResult = [
        { "ID": 26, "ModelName": "系统首页", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "XMGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 1, "ModelID": 27, "PageName": "系统首页", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/shouye.html", "isiframe": "" }] },
        { "ID": 35, "ModelName": "系统配置", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "XMGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 2, "ModelID": 27, "PageName": "系统配置", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/appset.html", "isiframe": "" }] },
        { "ID": 27, "ModelName": "空间管理", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "KJGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 3, "ModelID": 27, "PageName": "空间管理", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/qygl.html", "isiframe": "" }] },
        { "ID": 32, "ModelName": "用户管理", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "YHGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 4, "ModelID": 27, "PageName": "用户管理", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/yhgl.html", "isiframe": "" }] },
        { "ID": 28, "ModelName": "文件管理", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "WJGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 5, "ModelID": 27, "PageName": "文件管理", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/wjgl.html", "isiframe": "" }] },
        { "ID": 29, "ModelName": "日志管理", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "RZGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 6, "ModelID": 27, "PageName": "接口调用日志", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/rzgl.html", "isiframe": "" }] },
        { "ID": 31, "ModelName": "企业空间管理", "ModelType": "系统设置", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "QYKJGL", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "XT", "Token": "", "FunData": [{ "ID": 7, "ModelID": 27, "PageName": "企业空间管理", "ExtData": "1", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/qywdgl.html", "isiframe": "" }] },
        { "ID": 30, "ModelName": "个人空间", "ModelType": "工作台", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "GRKJ", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "WORK", "Token": "", "FunData": [{ "ID": 8, "ModelID": 27, "PageName": "个人空间", "ExtData": "2", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/qywdgl.html", "isiframe": "" }] },
        { "ID": 33, "ModelName": "企业空间", "ModelType": "工作台", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "QYKJ", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "WORK", "Token": "", "FunData": [{ "ID": 9, "ModelID": 27, "PageName": "企业空间", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/qywd.html", "isiframe": "" }] },
        { "ID": 34, "ModelName": "用户共享", "ModelType": "工作台", "ModelUrl": "/VIEW/AppPage/APP.html", "ModelCode": "YHGX", "ComId": 0, "ORDERID": 1, "IsSys": 1, "WXUrl": "", "IsKJFS": 0, "PModelCode": "WORK", "Token": "", "FunData": [{ "ID": 10, "ModelID": 27, "PageName": "用户共享", "ExtData": "", "PageUrl": "", "FunOrder": "", "PageCode": "/Web/Html/Page/qywdgx.html", "isiframe": "" }] }
    ];
    model.MenuResult = MenuResult;
    var filecode = ComFunJS.getQueryString("filecode");
    if (filecode) {
        ComFunJS.setCookie("filecode", filecode);
    }
    model.GetUserData(function () {
        var pagemode = ComFunJS.getQueryString("code");
        if (pagemode) {
            for (var i = 0; i < MenuResult.length; i++) {
                if (MenuResult[i].FunData[0].ID == pagemode) {
                    model.selmenulev2(MenuResult[i].FunData[0], null);
                }

            }
        }
       
    });
  
   
})

model.page.$watch("pagecount", function () {
    localStorage.setItem(model.PageCode + "pagecount", model.page.pagecount);
})

