var qjfiledata = [];
function QJUpload(t) {
    if (!(t.usercode && t.uploadButtton && t.secret))
        throw new TypeError("缺少参数！");
    this.options = {
        usercode: t.usercode,
        secret: t.secret,
        upinfo: t.upinfo || "",
        webupconfig: JSON.stringify(t.webupconfig || {}),
        filecomplete: t.filecomplete || function () { },
        closeupwin: t.closeupwin || function () { },
        url: location.href,
        width: t.width || "50%",
        height: t.height || "60%",
        top: t.top || "15%",
        left: t.left || "25%",
        urlPrefix: "",
        source: "qj-upload"
    },
    this.uploadButton = document.getElementById(t.uploadButtton),
    this.upid = t.uploadButtton,
    this.url = t.fileapiurl + "/" + t.usercode + "/document/fileupload",
    this._init()
}
QJUpload.prototype = {
    constructor: QJUpload,
    _addHander: function (t, e, o) {
        t.addEventListener ? t.addEventListener(e, o, !1) : t.attachEvent ? t.attachEvent("on" + e, o) : t["on" + e] = o
    },
    _checkH5Support: function () {
        var t = document.createElement("input")
          , e = !(!window.File || !window.FileList)
          , o = new XMLHttpRequest
          , i = !!window.FormData;
        return "multiple" in t && e && "onprogress" in o && "upload" in o && i
    },
    _init: function () {
        var t = this;
        // this._checkH5Support() || (this.url = this.options.urlPrefix + "/upload-flash/index.html");
        var e = this._createIframe()
          , o = function () {
              this.readyState && "complete" !== this.readyState || t.update()
          }
        ;
        this.frameMsg = e.contentWindow,
        e.attachEvent ? e.attachEvent("onload", o) : e.onload = o,
        this._addHander(this.uploadButton, "click", function (event) {
            t.openWrap()
        }),
        this._handleMsgReceive()
    },
    _createIframe: function () {
        var op = this;

        var t = document.createElement("div")
          , e = document.createElement("div")
          , o = document.createElement("div")
          , f = document.createElement("div")
          , i = document.createElement("span")
          , n = document.createElement("iframe");
        return t.setAttribute("id", "qj-wrapAll" + op.upid),
        t.style.display = "none",
        f.style.cssText = "width: 100%;height: 50px;background-color:#fff;margin-top: -6px;text-align: right;",
        e.style.cssText = "display: block;position: fixed;left: 0;top: 0;width: 100%;height: 100%;z-index: 2001;background-color: #000;-moz-opacity: 0.5;opacity: .50;filter: alpha(opacity=50);",
        o.style.cssText = "display: block;position: fixed;left: " + op.options.left + ";top: " + op.options.top + ";width: " + op.options.width + ";height: " + op.options.height + ";BACKGROUND-COLOR: #00b7ee; z-index: 2002;box-shadow: 0 0 25px rgba(0,0,0,0.7);border-radius: 5px;padding-top: 45px;",
        i.innerHTML = "&times;",
        f.innerHTML = " <button id='qjclose" + op.upid + "' style='margin-top: 5px;cursor:pointer;height: 40px;margin-right: 10px;width: 120px;font-size: 14px;background-color: rgb(0, 183, 238); border: 0; color: #fff;'>确定</button>",
        i.style.cssText = "width: 40px;height: 40px;position: absolute;top: 0px;right: 0px;cursor: pointer;text-align: center;line-height: 40px;color: #FFF;font-size: 30px;font-family: microsoft yahei;border-radius: 0 5px 0 0;",
        i.onclick = function () {
            t.style.display = "none"
        },
      //n.setAttribute("src", this.url),
        n.setAttribute("id", "qj-iframe" + op.upid),
        n.setAttribute("name", "qj-iframe" + op.upid),
        n.setAttribute("width", "1000px"),
        n.setAttribute("height", "600px"),
        n.style.cssText = "width: 100%;height: 100%;z-index: 2002;border:none;background-color: #fff;",
        o.appendChild(n),
        o.appendChild(i),
        o.appendChild(f),
        t.appendChild(e),
        t.appendChild(o),
        document.getElementsByTagName("body")[0].appendChild(t),
        document.getElementById("qjclose" + op.upid).onclick = function () {
            document.getElementById("qj-wrapAll" + op.upid).style.display = "none";
            "function" == typeof op.options.closeupwin && op.options.closeupwin(qjfiledata, op.uploadButton);
        },
        n
    },
    _handleMsgReceive: function () {
        var t = this;
        this._addHander(window, "message", function (e) {
            var o = JSON.parse(e.data);
            switch (o.type) {

                case "filecomplete":
                    qjfiledata = o.data;
                    "function" == typeof t.options.filecomplete && t.options.filecomplete(o.data);
                    break;
                case "closewin":
                    closeWrap();
            }
        })
    },
    update: function () {
        if ("object" == typeof arguments[0]) {
            for (var t in arguments[0])
                arguments[0].hasOwnProperty(t) && (this.options[t] = arguments[0][t]);
            arguments[0].ts && (this.options.ptime = arguments[0].ts)
        }
        this.frameMsg.postMessage(JSON.stringify(this.options), this.url)
    },
    openWrap: function () {
        var op = this;
        //this.frameMsg.postMessage(JSON.stringify({
        //    openWrap: !0
        //}), this.url),
        //this.options.openWrap && this.options.openWrap(),
        document.getElementById("qj-iframe" + op.upid).src = this.url;
        document.getElementById("qj-wrapAll" + op.upid).style.display = "block"
    },
    closeWrap: function () {
        var op = this;
        document.getElementById("qj-wrapAll" + op.upid).style.display = "none"
    }
};
