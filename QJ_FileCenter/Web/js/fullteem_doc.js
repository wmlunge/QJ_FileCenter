$(function () {
    var zyid = getQueryString("zyid");
    $.post("/adminapi/ExeActionPub/GETWDZY", { "P1": zyid }, function (r) {
        r = JSON.parse(r)
        if (r.ErrorMsg == "") {
            var title = r.Result;
            var size = r.Result1;
            $(".lnk-file-title").attr('TITLE', title);
            $(".lnk-file-title").text(title);
            for (var i = 1; i < size * 1 + 1; i++) {
                var fileurl = "http://" + location.host + "/document/YL/" + zyid + "/" + i;
                var html = '<DIV class="container-fluid container-fluid-content">'
                    + '<DIV class="row-fluid">'
                    + '<DIV class="span12">'
                    + '<DIV class="word-page" STYLE="max-width:793px">'
                    + '<DIV class="word-content">'
                    + '<img   data-original="' + fileurl + '" width="100%" height="100%" ></img>'
                    + '</DIV></DIV></DIV></DIV></DIV>';
                $("body").append(html);
            }
            $("img").lazyload({
                // placeholder: "/images/loading.gif",
                effect: "fadeIn"
            });
        }
    })
})
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(decodeURI(r[2])); return null;
}
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}
