﻿var pmodel = avalon.define({
    $id: "APP_ADD",
    nowuser: ComFunJS.getnowuser(),//当前用户
    PathCode: "loading.html",
    FormCode: ComFunJS.getQueryString("FormCode"),
    DataID: ComFunJS.getQueryString("ID", ""),//数据ID

    ExtData: [],//扩展数据
    isPC: true,
    isDraft: false,
    pmtitle: "表单",//手机端标题
    rdm: Math.random(),
    render: function () {
        if (!pmodel.isPC) {
            $("table").hide();
        }
        if (typeof (tempmodel) != "undefined" && tempmodel) {
            if (pmodel.DataID) {
                tempmodel.inittemp(pmodel.DataID);
            } else {
                tempmodel.inittemp();
            }
            if (pmodel.isPC && parent.layer) {//调整标题
                var index = parent.layer.getFrameIndex(window.name)
                parent.layer.title(tempmodel.name, index)
            } else {
                pmodel.pmtitle = tempmodel.name;
                document.title = tempmodel.name;
                $("table").show();
            }
            avalon.templateCache = null;
            setTimeout("ComFunJS.initForm()", 500)

        }


    },
    SaveData: function (dom, isjp, type) {
        if (!pmodel.isPC) {
            $("table").hide();
        }
        var errmsg = "";
        errmsg = pmodel.CheckData();//验证错误
        if (errmsg) {
            ComFunJS.winwarning(errmsg);
            if (!pmodel.isPC) {
                $("table").show();
            }
            return;
        }
        else {

            if (pmodel.isPC) {
                $(dom).attr("disabled", true).find(".fa").show();//加上转圈样式
            }
            tempmodel.SaveData(function (result1) {
                if ($.trim(result1.ErrorMsg) == "") {

                    ComFunJS.winsuccess("操作成功");
                    if (tempmodel && $.isFunction(tempmodel.Complate)) {
                        setTimeout("tempmodel.Complate();", 1000);
                    } else {
                        pmodel.refiframe();
                    }

                 
                }
                else {
                    if (pmodel.isPC) {
                        $(dom).attr("disabled", false).find(".fa").hide();//加上转圈样式
                    }
                    if (!pmodel.isPC) {
                        $("table").show();
                    }
                }
            }, dom);
        }
    },
    CheckData: function () { //验证代码块
        var retmsg = "";
        if (pmodel.isPC) {
            if ($(".szhl_require")) {

                $(".szhl_require:visible, .szhl_Int:visible, .szhl_Phone:visible").each(function () {
                    var title = $(this).attr("title") ? $(this).attr("title") : "";
                    if ($(this).hasClass("szhl_UEEDIT") && $(this).hasClass("szhl_require") && ($(this).prop("tagName") == "DIV" && ($(this).text() == "" && $(this).find("img").length == 0))) {
                        retmsg = title + $(this).parent().parent().parent().parent().find("label").text() + "不能为空";
                    }
                    else if (!$(this).val() && $(this).hasClass("szhl_require") && !$(this).hasClass("szhl_UEEDIT")) {
                        retmsg = title + $(this).parent().prev("label").text().replace('*', '') + "不能为空";
                    } else if ($(this).hasClass("szhl_Int")) {
                        if ($(this).val() == "") {
                            retmsg = title + $(this).parent().prev("label").text().replace('*', '') + "不能为空";
                        }
                        if (!(/^[0-9]*$/.test($(this).val()))) {
                            retmsg = title + $(this).parent().prev("label").text().replace('*', '') + "必须是正整数";
                        }
                    }
                    else if ($(this).hasClass("szhl_Phone")) {
                        if ($(this).val() == "") {
                            retmsg = title + $(this).parent().prev("label").text().replace('*', '') + "不能为空";
                        }
                        if (!(/^0?1[3|4|7|5|8][0-9]\d{8}$/.test($(this).val()))) {
                            retmsg = title + $(this).parent().prev("label").text().replace('*', '') + "填写不正确";
                        }
                    }
                    if (retmsg != "") {
                        return false;
                    }

                })
            }
        }
        else {
            $(".szhl").each(function () {
                var title = $(this).attr("title") ? $(this).attr("title") : "";
                if ($(this).hasClass("szhl_require") && $(this).val() == "") {
                    var str = "请输入";
                    if ($(this).find("select").length > 0) {
                        str = "请选择";
                    }
                    retmsg = str + title + $(this).parent().parent().find(".label").text();
                }
                else if ($(this).hasClass("szhl_Int") && !(/^\+?[1-9][0-9]*$/.test($(this).val()))) {
                    retmsg = title + $(this).parent().parent().find(".label").text() + "必须是正整数";
                }
                else if ($(this).hasClass("szhl_Time") && ComFunJS.compareTime($(this).val(), "")) {
                    retmsg = title + $(this).parent().parent().find(".label").text() + "必须大于当前时间";
                }
                else if ($(this).hasClass("szhl_Phone")) {
                    if ($(this).val()) {
                        if (!(/^0?1[3|4|7|5|8][0-9]\d{8}$/.test($(this).val()))) {
                            retmsg = title + $(this).parent().parent().find(".label").text() + "填写不正确";
                        }
                    }
                }
                else if ($(this).hasClass("szhl_Float")) {
                    if ($(this).val()) {
                        if (!(/^(?!0+(?:\.0+)?$)(?:[1-9]\d*|0)(?:\.\d{1,2})?$/.test($(this).val()))) {
                            retmsg = title + $(this).parent().parent().find(".label").text() + "填写不正确";
                        }
                    }
                }
                if (retmsg != "") {
                    return false;
                }

            })

        }
        return retmsg;
    },
    refiframe: function () {//刷新父框架
        if (pmodel.isPC) {
            if (typeof (tempindex.GetLIST) == 'function') {
                setTimeout("tempindex.GetLIST()", 1000)
            } else {
                setTimeout("model.refpage()", 1000)
            }
            setTimeout("parent.layer.closeAll()", 1000)

        } else {

            if (ComFunJS.getQueryString("mpid")) {
                setTimeout("window.history.back();", 1000)
            }
            else {
                setTimeout("window.location.replace(location.href);", 1000)
            }
        }

    },
    qx: function () {
        parent.layer.closeAll();
    },
    jptj: function (event, dom) {
        if (event.ctrlKey && (event.keyCode == 13 || event.keyCode == 10)) {
            pmodel.SaveData(dom.find(".btnSucc")[0], true);
        }
    },
    init: function () {
        pmodel.PathCode = ComFunJS.getQueryString("PathCode");
    }
})

avalon.ready(function () {
    setTimeout("pmodel.init()", 500)

})

