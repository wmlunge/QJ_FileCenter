var ComFunJS = new Object({
    yuming: "",
    isIE: function () {
        if (!!window.ActiveXObject || "ActiveXObject" in window)
            return true;
        else
            return false;
    },
    isPC: function () {
        var userAgentInfo = navigator.userAgent;
        var Agents = ["Android", "iPhone",
                    "SymbianOS", "Windows Phone",
                    "iPad", "iPod"];
        var flag = true;
        for (var v = 0; v < Agents.length; v++) {
            if (userAgentInfo.indexOf(Agents[v]) > 0) {
                flag = false;
                break;
            }
        }
        return flag;
    },
    setCookie: function (name, value) {
        var Days = 30;
        var exp = new Date();
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString() + ";path=/";
    },
    getCookie: function (name) {
        var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

        if (arr = document.cookie.match(reg))

            return unescape(arr[2]);
        else
            return null;
    },
    delCookie: function (name) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = this.getCookie(name);
        if (cval != null) {
            document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString() + ";path=/;";
        }
    },
    getfileapi: function () {
        return ComFunJS.getCookie("fileapi");
    },
    getnowuser: function () {
        return ComFunJS.getCookie("username");
    },
    delendchar: function (str) {
        if (str.length > 1) {
            return str.substring(0, str.length - 1);
        } else {
            return str;
        }
    },
    loadJs: function (url, callback) {
        var done = false;
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.language = 'javascript';
        script.src = url;
        script.onload = script.onreadystatechange = function () {
            if (!done && (!script.readyState || script.readyState == 'loaded' || script.readyState == 'complete')) {
                done = true;
                script.onload = script.onreadystatechange = null;
                if (callback) {
                    callback.call(script);
                }
            }
        }
        document.getElementsByTagName("head")[0].appendChild(script);
    },
    loadCss: function (url, callback) {
        var link = document.createElement('link');
        link.rel = 'stylesheet';
        link.type = 'text/css';
        link.media = 'screen';
        link.href = url;
        document.getElementsByTagName('head')[0].appendChild(link);
        if (callback) {
            callback.call(link);
        }
    },
    /**
    *@param {int|double} s:传入的float数字 
    *@param{int} n:希望返回小数点几位
    */
    fmoney: function (s, n) {
        n = n > 0 && n <= 20 ? n : 2;
        s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
        var l = s.split(".")[0].split("").reverse(),
        r = s.split(".")[1];
        t = "";
        for (i = 0; i < l.length; i++) {
            t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
        }
        return t.split("").reverse().join("") + "." + r;
    },
    getTree: function (nodes) {
        /**
             *将普通数组转成树形数组，根据数组内对象的id跟parent属性过滤
                *@nodes {array} nodes
                *@return {array} 树形数组
      */
        //获取树形递归,nodes参数是一个对象数组，根据对象的id跟parent属性过滤
        return function (parentid) {
            var cn = new Array();
            for (var i = 0; i < nodes.length; i++) {
                var n = nodes[i];
                n.id = n.value;
                if (n.parent == parentid) {
                    n.children = arguments.callee(n.id);
                    cn.push(n);
                }
            }
            return cn;
        }(undefined);
    },
    getRootPath: function () {
        //获取当前网址，如： http://localhost:8083/uimcardprj/share/meun.jsp
        var curWwwPath = window.document.location.href;
        //获取主机地址之后的目录，如： uimcardprj/share/meun.jsp
        var pathName = window.document.location.pathname;
        var pos = curWwwPath.indexOf(pathName);
        //获取主机地址，如： http://localhost:8083
        var localhostPaht = curWwwPath.substring(0, pos);
        //获取带"/"的项目名，如：/uimcardprj
        var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
        return (localhostPaht + projectName);
    },//获取路径
    DelItem: function (items, delkey, delval) {
        for (i = items.length - 1; i >= 0; i--) {
            if (items[i][delkey] == delval) {
                items.splice(i, 1)
            }
        }
    },//删除数据项
    isOffice: function (extname) {
        return $.inArray(extname.toLowerCase(), ['doc', 'docx', 'xls', 'xlsx', 'ppt', 'pptx', 'pdf']) != -1
    },
    isPic: function (extname) {
        return $.inArray(extname.toLowerCase(), ['jpg', 'jpeg', 'gif', 'png', 'bmp']) != -1
    },
    getfile: function (fileid) {
        var url = ComFunJS.yuming + "/ToolS/DownFile.aspx?szhlcode=" + ComFunJS.getCookie("szhlcode");
        if (fileid) {
            url = url + "&fileId=" + fileid;
        }       
        return url;
    },
    viewbigimg: function (dom, defacalss) {
        if (!defacalss) {
            defacalss = ".img-rounded";
        }
        var ImagesData = {
            "title": "图片预览", //相册标题
            "id": 123, //相册id
            "start": $(defacalss).index(dom), //初始显示的图片序号，默认0
            "data": []   //相册包含的图片，数组格式
        };
        $(defacalss).each(function () {
            var image = {
                "alt": $(this).attr("filename") ? $(this).attr("filename") : "",
                "pid": "", //图片id
                "src": $(this).attr("imgyt"), //原图地址
                "thumb": $(this).attr("src") //缩略图地址
            }
            ImagesData.data.push(image);
        })
        //使用相册
        top.layer.ready(function () {
            top.layer.photos({
                photos: ImagesData //$(dom).parent().parent()
            });
        });
        return;
    },
    viewfile: function (file) {
        if (ComFunJS.isPic(file.FileExtendName)) {
            ComFunJS.viewbigimg(this)
            return;
        }
        if (ComFunJS.isOffice(file.FileExtendName) && file.ISYL == "Y") {
            window.location = file.YLUrl;
            return;
        }
        window.location = ComFunJS.getfile(file.ID);
    },//文件查看方法

    FindItem: function (arrs, func) {
        var temp = [];
        for (var i = 0; i < arrs.length; i++) {
            if (func(arrs[i])) {
                temp[temp.length] = arrs[i];
            }
        }
        return temp;
    },
    getQueryString: function (name, defauval) {//获取URL参数,如果获取不到，返回默认值，如果没有默认值，返回空格
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) { return unescape(r[2]); }
        else {
            return defauval || "";
        }
    },//获取参数中数据
    convstr: function (str, len) {
        if (str) {
            str = str.replace(/<[^>]+>/g, "");
            if (len) {
                return str.length > len ? str.substr(0, len) + "..." : str;
            }
            else {
                return str;
            }
        }
        return "";
    },//转字符串
    getnowday: function () {
        var d = new Date();
        var year = d.getFullYear();
        var month = d.getMonth() + 1;
        var date = d.getDate();
        var week = d.getDay();
        var hours = d.getHours();
        var minutes = d.getMinutes();
        var seconds = d.getSeconds();
        var ms = d.getMilliseconds();
        var curDateTime = year;
        if (month > 9)
            curDateTime = curDateTime + "年" + month;
        else
            curDateTime = curDateTime + "年0" + month;
        if (date > 9)
            curDateTime = curDateTime + "月" + date + "日";
        else
            curDateTime = curDateTime + "月0" + date + "日";

        var weekday = "";
        if (week == 0)
            weekday = "星期日";
        else if (week == 1)
            weekday = "星期一";
        else if (week == 2)
            weekday = "星期二";
        else if (week == 3)
            weekday = "星期三";
        else if (week == 4)
            weekday = "星期四";
        else if (week == 5)
            weekday = "星期五";
        else if (week == 6)
            weekday = "星期六";
        curDateTime = curDateTime + " " + weekday;
        return curDateTime;
    },
    getnowdate: function (format, date) {
        var now = new Date();
        if (date) {
            now = new Date(Date.parse(date.replace(/-/g, '/')));
        }

        var year = now.getFullYear();       //年
        var month = now.getMonth() + 1;     //月
        var day = now.getDate();            //日
        var hh = now.getHours();
        var mm = now.getMinutes();
        var ss = now.getSeconds();

        var clock = year + "-";

        if (format == "yyyy-mm") {
            if (month < 10)
                clock += "0";
            clock += month + "-";
        }

        if (format == "yyyy-mm-dd") {
            if (month < 10)
                clock += "0";
            clock += month + "-";
            if (day < 10) {
                clock += "0";
            }
            clock += day + "-";
        }
        if (format == "yyyy-mm-dd hh:mm") {
            if (month < 10)
                clock += "0";
            clock += month + "-";
            if (day < 10) {
                clock += "0";
            }
            clock += day + " ";

            if (hh < 10)
                clock += "0";
            clock += hh + ":";
            if (mm < 10)
                clock += "0";
            clock += mm + ":";

        }
        if (format == "yyyy-mm-dd hh:mm:ss") {
            if (month < 10)
                clock += "0";
            clock += month + "-";
            if (day < 10) {
                clock += "0";
            }
            clock += day + " ";

            if (hh < 10)
                clock += "0";
            clock += hh + ":";
            if (mm < 10)
                clock += "0";
            clock += mm + ":";
            if (ss < 10)
                clock += "0";
            clock += ss + ":";

        }
        return (clock.substr(0, clock.length - 1));
    },//获取当前时间
    daysBetween: function (start, end) {
        var OneMonth = start.substring(5, start.lastIndexOf('-'));
        var OneDay = start.substring(start.length, start.lastIndexOf('-') + 1);
        var OneYear = start.substring(0, start.indexOf('-'));
        var TwoMonth = end.substring(5, end.lastIndexOf('-'));
        var TwoDay = end.substring(end.length, end.lastIndexOf('-') + 1);
        var TwoYear = end.substring(0, end.indexOf('-'));
        var cha = ((Date.parse(TwoMonth + '/' + TwoDay + '/' + TwoYear) - Date.parse(OneMonth + '/' + OneDay + '/' + OneYear)) / 86400000);
        return cha;
    },

    Arabia_to_Chinese: function (Num) {
        for (i = Num.length - 1; i >= 0; i--) {
            Num = Num.replace(",", "")//替换tomoney()中的“,”
            Num = Num.replace(" ", "")//替换tomoney()中的空格
        }
        //Num = Num.replace("￥","")//替换掉可能出现的￥字符
        if (isNaN(Num)) { //验证输入的字符是否为数字
            alert("请检查小写金额是否正确");
            return;
        }
        //---字符处理完毕，开始转换，转换采用前后两部分分别转换---//
        part = String(Num).split(".");
        newchar = "";
        //小数点前进行转化
        for (i = part[0].length - 1; i >= 0; i--) {
            if (part[0].length > 10) { alert("位数过大，无法计算"); return ""; }//若数量超过拾亿单位，提示
            tmpnewchar = ""
            perchar = part[0].charAt(i);
            switch (perchar) {
                case "0": tmpnewchar = "零" + tmpnewchar; break;
                case "1": tmpnewchar = "壹" + tmpnewchar; break;
                case "2": tmpnewchar = "贰" + tmpnewchar; break;
                case "3": tmpnewchar = "叁" + tmpnewchar; break;
                case "4": tmpnewchar = "肆" + tmpnewchar; break;
                case "5": tmpnewchar = "伍" + tmpnewchar; break;
                case "6": tmpnewchar = "陆" + tmpnewchar; break;
                case "7": tmpnewchar = "柒" + tmpnewchar; break;
                case "8": tmpnewchar = "捌" + tmpnewchar; break;
                case "9": tmpnewchar = "玖" + tmpnewchar; break;
            }
            switch (part[0].length - i - 1) {
                case 0: tmpnewchar = tmpnewchar + "元"; break;
                case 1: if (perchar != 0) tmpnewchar = tmpnewchar + "拾"; break;
                case 2: if (perchar != 0) tmpnewchar = tmpnewchar + "佰"; break;
                case 3: if (perchar != 0) tmpnewchar = tmpnewchar + "仟"; break;
                case 4: tmpnewchar = tmpnewchar + "万"; break;
                case 5: if (perchar != 0) tmpnewchar = tmpnewchar + "拾"; break;
                case 6: if (perchar != 0) tmpnewchar = tmpnewchar + "佰"; break;
                case 7: if (perchar != 0) tmpnewchar = tmpnewchar + "仟"; break;
                case 8: tmpnewchar = tmpnewchar + "亿"; break;
                case 9: tmpnewchar = tmpnewchar + "拾"; break;
            }
            newchar = tmpnewchar + newchar;
        }
        //小数点之后进行转化
        // if(Num.indexOf(".")!=-1){
        if (part.length > 1) {
            if (part[1].length > 2) {
                alert("小数点之后只能保留两位,系统将自动截段");
                part[1] = part[1].substr(0, 2)
            }
            for (i = 0; i < part[1].length; i++) {
                tmpnewchar = ""
                perchar = part[1].charAt(i)
                switch (perchar) {
                    case "0": tmpnewchar = "零" + tmpnewchar; break;
                    case "1": tmpnewchar = "壹" + tmpnewchar; break;
                    case "2": tmpnewchar = "贰" + tmpnewchar; break;
                    case "3": tmpnewchar = "叁" + tmpnewchar; break;
                    case "4": tmpnewchar = "肆" + tmpnewchar; break;
                    case "5": tmpnewchar = "伍" + tmpnewchar; break;
                    case "6": tmpnewchar = "陆" + tmpnewchar; break;
                    case "7": tmpnewchar = "柒" + tmpnewchar; break;
                    case "8": tmpnewchar = "捌" + tmpnewchar; break;
                    case "9": tmpnewchar = "玖" + tmpnewchar; break;
                }
                if (i == 0) tmpnewchar = tmpnewchar + "角";
                if (i == 1) tmpnewchar = tmpnewchar + "分";
                newchar = newchar + tmpnewchar;
            }
        }
        // }
        //替换所有无用汉字
        while (newchar.search("零零") != -1)
            newchar = newchar.replace("零零", "零");
        newchar = newchar.replace("零亿", "亿");
        newchar = newchar.replace("亿万", "亿");
        newchar = newchar.replace("零万", "万");
        newchar = newchar.replace("零元", "元");
        newchar = newchar.replace("零角", "");
        newchar = newchar.replace("零分", "");

        if (newchar.charAt(newchar.length - 1) == "元" || newchar.charAt(newchar.length - 1) == "角")
            newchar = newchar + "整"
        return newchar;
    },//选择人员

  
    getuser: function (user) {

    },
    converfilesize: function (size) {
        if (size) {
            var size = parseFloat(size);
            var rank = 0;
            var rankchar = 'Bytes';
            while (size > 1024) {
                size = size / 1024;
                rank++;
            }
            if (rank == 1) {
                rankchar = "KB";
            }
            else if (rank == 2) {
                rankchar = "MB";
            }
            else if (rank == 3) {
                rankchar = "GB";
            }
            return size.toFixed(2) + " " + rankchar;
        }

    },

    //弹出帮助函数
    winsuccess: function (content) {
        top.toastr.success(content);

    },//成功窗口
    winsuccessnew: function (content) {
        top.toastr.success(content);

    },//成功窗口New
    winprompt: function (callbact) {
        layer.prompt(function (val) {
            callbact.call(this, val);
        });
    },

    winwarning: function (content) {
        top.toastr.warning(content)
    },//警告窗口
    wintips: function (content, callback) {
        layer.msg(content, {
            offset: 200,
            shift: 6
        });
    },//提示
    wintip: function (tip, dom) {
        layer.tips(tip, dom)
    },//提示
    winload: function () {
        layer.load();
    },//加载
    wintab: function (data) {
        layer.tab({
            area: ['600px', '300px'],
            tab: [{
                title: 'TAB1',
                content: '内容1'
            }, {
                title: 'TAB2',
                content: '内容2'
            }, {
                title: 'TAB3',
                content: '内容3'
            }]
        });
    },
    wincloseload: function () {
        layer.closeAll('loading');
    },//关闭加载
    winconfirm: function (title, yes, no) {
        layer.confirm(title, {
            btn: ['确认', '取消'], //按钮
            shade: false //不显示遮罩
        }, function () {
            layer.closeAll('dialog');
            return yes.call(this);
        }, function () {
            return no && no.call(this);
        });
    },
    winAlert: function (title) {//带一个确认按钮的提示框，点击确认按钮关闭
        layer.confirm(title, {
            btn: ['确认'], //按钮
            shade: false //不显示遮罩
        }, function () {
            layer.closeAll('dialog');
        });
    },//确认框
    winAlert2: function (title, yes) {//带一个确认按钮的提示框，点击确认按钮关闭
        layer.confirm(title, {
            btn: ['确认'], //按钮
            shade: true //显示遮罩
        }, function () {
            layer.closeAll('dialog');
            return yes.call(this);
        });
    },//确认框
    winviewform: function (url, title, width, height, callbact) {
        var width = width || $("body").width() * 2 / 3;
        var height = height || $(window).height() - 140; //$("body").height();
        var optionwin = {
            type: 2,
            fix: true, //不固定
            area: [width + 'px', height + 'px'],
            maxmin: true,
            content: url,
            title: title,
            shadeClose: false, //加上边框
            scrollbar: false,
            shade: 0.4,
            shift: 0,
            success: function (layero, index) {

            },
            end: function () {
                if (callbact) {
                    return callbact.call(this);
                }
            }
        }
        layer.open(optionwin);
    },
    winviewformmax: function (url, title, width, height, callbact) {
        var width = width || $("body").width() - 300;
        var height = height || $("#main").height();
        var optionwin = {
            type: 2,
            fix: true, //不固定
            area: [width + 'px', height + 'px'],
            maxmin: false,
            content: url,
            title: title,
            shadeClose: false, //加上边框
            success: function (layero, index) {
                $(".layui-layer-shade").css("opacity", "0.1");

            },
            end: function () {
                if (callbact) {
                    return callbact.call(this);
                }
            }
        }
        var layerForm = layer.open(optionwin);
        layer.full(layerForm);
    },
    winviewformNoClose: function (url, title, width, height, option) {
        var width = width || $("body").width() - 300;
        var height = height || $("#main").height();
        var optionwin = {
            type: 2,
            fix: true, //不固定
            closeBtn: 0,
            area: [width + 'px', height + 'px'],
            maxmin: false,
            content: url,
            title: title,
            shadeClose: false, //加上边框
            success: function (layero, index) {
            }
        }
        layer.open(optionwin);
    },//普通查看网页窗口
    winviewformright: function (url, title, callbact) {
        if ($(".layui-layer").length == 0) {
            var width = $("body").width() * 2 / 3;
            var height = $(window).height();
            var optionwin = {
                type: 2,
                fix: true, //不固定
                area: [width + 'px', height + 'px'],
                content: url,
                title: title,
                shift: 0,
                shade: 0.4,
                shadeClose: false,
                scrollbar: false,
                success: function (layero, index) {
                    var frameid = $("iframe", $(layero)).attr('id');
                    //$(layero).css("left", $(layero).position().left * 1 + 5 + "px");              
                    //$("body", top.frames[frameid].document).css({ "background": "#F8F8F8" });
                    $("body").one("click", function (event) {
                        layer.close(index)
                    });
                },
                end: function () {
                    if (callbact) {
                        return callbact.call(this);
                    }
                }
            }
            return layer.open(optionwin);
        } else {
            var framename = $(".layui-layer").find("iframe")[0].name;
            window.frames[framename].document.location.href = url;
        }

    },
    winbtnwin: function (url, title, width, height, option, btcallbact) {
        var width = width || $("body").width() - 300;
        var height = height || $(window).height() * 0.8; //var height = height || $("#main").height();
        var optionwin = {
            type: 2,
            fix: true, //不固定
            area: [width + 'px', height + 'px'],
            maxmin: true,
            content: url,
            title: title,
            shade: 0.4,
            shift: 0,
            shadeClose: false,
            scrollbar: false,
            success: function (layero, index) {
                if ($(layero).find(".successfoot").length == 0) {
                    var footdv = $('<div class="successfoot" style="border-bottom-width: 1px; padding: 0 20px 0 10px;margin-top: -3px;height:50px;background: #fff;"></div>');
                    var btnConfirm = $("<a href='javascript:void(0)' class='btn btn-sm btn-success' style='float:right; margin-top: 10px;width: 140px;'><i class='fa fa-spinner fa-spin' style='display:none'></i> 确   认</a>");
                    var btnCancel = $("<a href='javascript:void(0)' class='btn btn-sm btn-danger' style='float:right; margin-top: 10px;margin-right: 10px;width: 80px;'>取  消</a>");
                    var msg = $("<input type='hidden' class='r_data' >");

                    btnConfirm.appendTo(footdv).bind('click', function () {
                        return btcallbact.call(this, layero, index, btnConfirm);
                    })
                    btnCancel.appendTo(footdv).bind('click', function () {
                        layer.close(index)
                    })
                    $(layero).append(footdv).append(msg);

                    try {
                    } catch (e) { }
                }

            }
        }
        layer.open(optionwin);
    },//带确认框的窗口
    winviewhtml: function (content, title, width, height, callbact) {
        var width = width || 600;
        var height = height || 400;
        layer.open({
            type: 1,
            title: title,
            skin: 'layui-layer-rim', //加上边框
            area: [width + 'px', height + 'px'],
            content: content,
            shadeClose: true,
            success: function (layero, index) {
                if (callbact) {
                    return callbact.call(this, layero, index);
                }
            }

        });
    },//加载HTML
    winold: function (url) {
        var iWidth = 610;                          //弹出窗口的宽度;
        var iHeight = 600;                       //弹出窗口的高度;
        //获得窗口的垂直位置
        var iTop = (window.screen.availHeight - 30 - iHeight) / 2;
        //获得窗口的水平位置
        var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;
        var params = 'width=' + iWidth
                   + ',height=' + iHeight
                   + ',top=' + iTop
                   + ',left=' + iLeft
                   + ',channelmode=yes'//是否使用剧院模式显示窗口。默认为 no
                   + ',directories=yes'//是否添加目录按钮。默认为 yes
                   + ',fullscreen=no' //是否使用全屏模式显示浏览器
                   + ',location=no'//是否显示地址字段。默认是 yes
                   + ',menubar=no'//是否显示菜单栏。默认是 yes
                   + ',resizable=no'//窗口是否可调节尺寸。默认是 yes
                   + ',scrollbars=yes'//是否显示滚动条。默认是 yes
                   + ',status=yes'//是否添加状态栏。默认是 yes
                   + ',titlebar=yes'//默认是 yes
                   + ',toolbar=no'//默认是 yes
        ;
        window.open(url, name, params);
    },//老窗口
    initForm: function () {
        //时间
        //日期
        if ($(".szhl_form_date")[0]) {
            $(".szhl_form_date").datetimepicker({
                format: "yyyy-mm-dd"
            });
            $(".szhl_form_date").each(function () {
                if ($(this).val() === "" && !$(this).hasClass("null")) {
                    $(this).val(ComFunJS.getnowdate("yyyy-mm-dd"));
                }
                $(this).trigger('change')
            })
        }
        //年份
        if ($(".szhl_form_date_year")[0]) {
            $(".szhl_form_date_year").datetimepicker({
                minView: 4,
                startView: 4,
                format: "yyyy"
            })
            if ($(".szhl_form_date_year").val() === "") { $(".szhl_form_date_year").val(ComFunJS.getnowdate("yyyy")); }
            $(".szhl_form_date_year").trigger('change')
        }

        //月份
        if ($(".szhl_form_date_mon")[0]) {
            $(".szhl_form_date_mon").datetimepicker({
                minView: 3,
                startView: 3,
                format: "yyyy-mm"
            })
            if ($(".szhl_form_date_mon").val() === "") { $(".szhl_form_date_mon").val(ComFunJS.getnowdate("yyyy-mm")); }
            $(".szhl_form_date_mon").trigger('change')
        }
        //月份
        if ($(".szhl_form_date_hour")[0]) {
            $(".szhl_form_date_hour").datetimepicker({
                minView: 0,
                startView: 1,
                format: "hh:ii"
            })
            if ($(".szhl_form_date_mon").val() === "") { $(".szhl_form_date_mon").val(ComFunJS.getnowdate("yyyy-mm")); }
            $(".szhl_form_date_mon").trigger('change')
        }
        //时间
        if ($(".szhl_form_date_time")[0]) {
            $(".szhl_form_date_time").datetimepicker({
                minView: 0,
                format: "yyyy-mm-dd hh:ii"
            });
            if ($(".szhl_form_date_time").val() === "" && $(".szhl_form_date_time").attr("novalue") == undefined) { $(".szhl_form_date_time").val(ComFunJS.getnowdate("yyyy-mm-dd hh:mm")); }
            $(".szhl_form_date_time").trigger('change')
        }
        if ($(".szhl_UEEDIT").length > 0) {
            $(".szhl_UEEDIT").each(function () {
                var input = $(this);
                var ubid = $(this).attr("id");
                var um = UM.getEditor(ubid);
                if ($(this).attr("umheight")) {
                    um.setHeight($(this).attr("umheight"));
                } else {
                    um.setHeight("100")
                }
                um.ready(function () {
                    if (input.val() != "") {
                        //需要ready后执行，否则可能报错
                        um.setContent(input.val());
                    }
                    if (input.hasClass("focus")) {
                        um.focus()
                    }
                   


                })
                um.addListener('contentChange', function () {
                    input.val(UM.getEditor(ubid).getContent())
                })
                um.addListener('blur', function () {
                    input.val(UM.getEditor(ubid).getContent());
                })
            })
        }
      
    },
  
    facePath: [//表情json
            { faceName: "微笑", facePath: "0_微笑.gif" },
            { faceName: "撇嘴", facePath: "1_撇嘴.gif" },
            { faceName: "色", facePath: "2_色.gif" },
            { faceName: "发呆", facePath: "3_发呆.gif" },
            { faceName: "得意", facePath: "4_得意.gif" },
            { faceName: "流泪", facePath: "5_流泪.gif" },
            { faceName: "害羞", facePath: "6_害羞.gif" },
            { faceName: "闭嘴", facePath: "7_闭嘴.gif" },
            { faceName: "睡", facePath: "8_睡.gif" },
            { faceName: "大哭", facePath: "9_大哭.gif" },
            { faceName: "尴尬", facePath: "10_尴尬.gif" },
            { faceName: "发怒", facePath: "11_发怒.gif" },
            { faceName: "调皮", facePath: "12_调皮.gif" },
            { faceName: "龇牙", facePath: "13_龇牙.gif" },
            { faceName: "惊讶", facePath: "14_惊讶.gif" },
            { faceName: "难过", facePath: "15_难过.gif" },
            { faceName: "酷", facePath: "16_酷.gif" },
            { faceName: "冷汗", facePath: "17_冷汗.gif" },
            { faceName: "抓狂", facePath: "18_抓狂.gif" },
            { faceName: "吐", facePath: "19_吐.gif" },
            { faceName: "偷笑", facePath: "20_偷笑.gif" },
            { faceName: "可爱", facePath: "21_可爱.gif" },
            { faceName: "白眼", facePath: "22_白眼.gif" },
            { faceName: "傲慢", facePath: "23_傲慢.gif" },
            { faceName: "饥饿", facePath: "24_饥饿.gif" },
            { faceName: "困", facePath: "25_困.gif" },
            { faceName: "惊恐", facePath: "26_惊恐.gif" },
            { faceName: "流汗", facePath: "27_流汗.gif" },
            { faceName: "憨笑", facePath: "28_憨笑.gif" },
            { faceName: "大兵", facePath: "29_大兵.gif" },
            { faceName: "奋斗", facePath: "30_奋斗.gif" },
            { faceName: "咒骂", facePath: "31_咒骂.gif" },
            { faceName: "疑问", facePath: "32_疑问.gif" },
            { faceName: "嘘", facePath: "33_嘘.gif" },
            { faceName: "晕", facePath: "34_晕.gif" },
            { faceName: "折磨", facePath: "35_折磨.gif" },
            { faceName: "衰", facePath: "36_衰.gif" },
            { faceName: "骷髅", facePath: "37_骷髅.gif" },
            { faceName: "敲打", facePath: "38_敲打.gif" },
            { faceName: "再见", facePath: "39_再见.gif" },
            { faceName: "擦汗", facePath: "40_擦汗.gif" },

            { faceName: "抠鼻", facePath: "41_抠鼻.gif" },
            { faceName: "鼓掌", facePath: "42_鼓掌.gif" },
            { faceName: "糗大了", facePath: "43_糗大了.gif" },
            { faceName: "坏笑", facePath: "44_坏笑.gif" },
            { faceName: "左哼哼", facePath: "45_左哼哼.gif" },
            { faceName: "右哼哼", facePath: "46_右哼哼.gif" },
            { faceName: "哈欠", facePath: "47_哈欠.gif" },
            { faceName: "鄙视", facePath: "48_鄙视.gif" },
            { faceName: "委屈", facePath: "49_委屈.gif" },
            { faceName: "快哭了", facePath: "50_快哭了.gif" },
            { faceName: "阴险", facePath: "51_阴险.gif" },
            { faceName: "亲亲", facePath: "52_亲亲.gif" },
            { faceName: "吓", facePath: "53_吓.gif" },
            { faceName: "可怜", facePath: "54_可怜.gif" },
            { faceName: "菜刀", facePath: "55_菜刀.gif" },
            { faceName: "西瓜", facePath: "56_西瓜.gif" },
            { faceName: "啤酒", facePath: "57_啤酒.gif" },
            { faceName: "篮球", facePath: "58_篮球.gif" },
            { faceName: "乒乓", facePath: "59_乒乓.gif" },
            { faceName: "拥抱", facePath: "78_拥抱.gif" },
            { faceName: "握手", facePath: "81_握手.gif" }
    ],
    insertAtCursor: function (myField, myValue) {
        if (document.selection) {
            myField.focus();
            sel = document.selection.createRange();
            sel.text = myValue;
            sel.select();
        } else if (myField.selectionStart || myField.selectionStart == "0") {
            var startPos = myField.selectionStart;
            var endPos = myField.selectionEnd;
            var restoreTop = myField.scrollTop;
            myField.value = myField.value.substring(0, startPos) + myValue + myField.value.substring(endPos, myField.value.length);
            if (restoreTop > 0) {
                myField.scrollTop = restoreTop;
            }
            myField.focus();
            myField.selectionStart = startPos + myValue.length;
            myField.selectionEnd = startPos + myValue.length;
        } else {
            myField.value += myValue;
            //myField.focus();
        }
    },
    bqhContent: function (str) {//表情化内容
        if (str) {
            var regx = /(\[[\u4e00-\u9fa5]*\w*\]){1}/g;
            var rs = str.match(regx);
            if (rs) {
                for (i = 0; i < rs.length; i++) {
                    for (n = 0; n < ComFunJS.facePath.length; n++) {
                        if (ComFunJS.facePath[n].faceName == rs[i].substring(1, rs[i].length - 1)) {
                            var t = "<img title=\"" + ComFunJS.facePath[n].faceName + "\" style='display: initial;' src=\"/View_Mobile/Images/face/" + ComFunJS.facePath[n].facePath + "\" />";
                            str = str.replace(rs[i], t);
                            break;
                        }
                    }
                }
            }
        }
        return str;
    }

});

$(function () {
    $.getJSON = function (url, data, success, opt) {
        data.szhlcode ="";
        var fn = {
            success: function (data, textStatus) { }
        }
        if (success) {
            fn.success = success;
        }
        $.ajax({
            url: ComFunJS.yuming + url,
            data: data,
            dataType: "json",
            type: "post",
            success: function (data, textStatus) {
                if (data.ErrorMsg == "NOSESSIONCODE") {
                    top.ComFunJS.winwarning("页面超时!")
                    top.window.location.href = "/Login";
                    return;
                }
                if (data.ErrorMsg) {
                    top.ComFunJS.winwarning(data.ErrorMsg)
                }
                fn.success(data, textStatus);
            },

            beforeSend: function (XHR) {
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            },
            complete: function (XHR, TS) {

            }
        });
    };
    $.post = function (url, data, success, opt) {
        data.szhlcode ="";
        var fn = {
            success: function (data, textStatus) { }
        }
        if (success) {
            fn.success = success;
        }
        $.ajax({
            type: "post",
            url: ComFunJS.yuming + url,
            data: data,
            success: function (data, textStatus) {
                if (data.ErrorMsg == "NOSESSIONCODE") {
                    top.ComFunJS.winwarning("页面超时!")
                    top.window.location.href = "/Login";
                    return;
                }
                if (data.ErrorMsg) {
                   top.ComFunJS.winwarning(data.ErrorMsg)
                }
                fn.success(data, textStatus);
            },
            dataType: "json",
            beforeSend: function (XHR) {
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                  top.ComFunJS.winwarning("请求执行有误,请检查系统运行环境")
            },
            complete: function (XHR, TS) {

            }
        });
    };
    //ComFunJS.initsetajax(true);//初始化数据

})

