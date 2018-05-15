$(function () {

    var id = getQueryString("ID");
    var size = getQueryString("size");
    var title = getQueryString("title");

    $(".lnk-file-title").attr('TITLE', title);
    $(".lnk-file-title").text(title);
    for (var i = 1; i < size * 1 + 1; i++) {
        var fileurl = "/document/YL/" + id + "/" + i;
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

    //$.getJSON("/API/VIEWAPI.ashx?Action=Commanage_GETWDZY&r=" + Math.random(), { "P1": Code }, function (r) {

    //    if (r.ErrorMsg == "") {
    //        if (r.Result && r.Result1 && r.Result2) {
    //            $(".lnk-file-title").attr('TITLE', r.Result2);
    //            $(".lnk-file-title").text(r.Result2);
    //            for (var i = 0; i < r.Result1 * 1; i++) {
    //                var html = '<DIV class="container-fluid container-fluid-content">'
    //                + '<DIV class="row-fluid">'
    //                + '<DIV class="span12">'
    //                + '<DIV class="word-page" STYLE="max-width:793px">'
    //                + '<DIV class="word-content">'
    //                + '<embed src="' + r.Result + '/' + (i + 1) + '.svg" width="100%" height="100%" type="image/svg+xml"></embed>'
    //                + '</DIV></DIV></DIV></DIV></DIV>';
    //                $("body").append(html);
    //            }

    //            $("embed").lazyload({
    //                placeholder: "/images/loading.gif",
    //                effect: "fadeIn"
    //            });

    //        }
    //    }
    //    else {
    //        $("body").empty().append(r.ErrorMsg);
    //    }
    //})
})
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(decodeURI(r[2])); return null;
}
