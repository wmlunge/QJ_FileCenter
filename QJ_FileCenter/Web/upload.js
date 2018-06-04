(function ($) {
    // 当domReady的时候开始初始化
    $(function () {
        var $wrap = $('#uploader'),

			// 文件容器
			$queue = $('<ul class="filelist"></ul>')
				.appendTo($wrap.find('.queueList')),

			// 状态栏，包括进度和控制按钮
			$statusBar = $wrap.find('.statusBar'),

			// 文件总体选择信息。
			$info = $statusBar.find('.info'),

			// 状态信息。
			$tips = $statusBar.find('.tips'),

			// 上传按钮
			$upload = $wrap.find('.uploadBtn'),

			// 没选择文件之前的内容。
			$placeHolder = $wrap.find('.placeholder'),

			$progress = $statusBar.find('.progress').hide(),

			// 添加的文件数量
			fileCount = 0,

			// 添加的文件总大小
			fileSize = 0,

			// 优化retina, 在retina下这个值是2
			ratio = window.devicePixelRatio || 1,

			// 缩略图大小
			thumbnailWidth = 110 * ratio,
			thumbnailHeight = 110 * ratio,

			// 可能有pedding, ready, uploading, confirm, done.
			state = 'pedding',

		    // 所有文件的进度信息，key为file id
	        percentages = {},

			supportTransition = (function () {
			    var s = document.createElement('p').style,
					r = 'transition' in s ||
							'WebkitTransition' in s ||
							'MozTransition' in s ||
							'msTransition' in s ||
							'OTransition' in s;
			    s = null;
			    return r;
			})(),

			// WebUploader实例
			uploader;

        if (!WebUploader.Uploader.support()) {
            alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
            throw new Error('WebUploader does not support the browser you are using.');
        }

        // 实例化
        var defaults = {
            pick: {
                id: '#filePicker',
                label: '点击选择文件'
            },
            dnd: '#uploader .queueList',
            paste: document.body,

            //accept: {
            //	title: 'Images',
            //	extensions: 'gif,jpg,jpeg,bmp,png',
            //	mimeTypes: 'image/*'
            //},

            // swf文件路径
            swf: '../Web/webuploader/Uploader.swf',
            auto: true,
            disableGlobalDnd: true,
            //runtimeOrder :'flash',
            chunked: true,
            chunkSize: 1000 * 1024,
            server: '/document/fileupload',
            fileNumLimit: 300,
            fileSizeLimit: 2048 * 1024 * 1024,    // 2G
            fileSingleSizeLimit: 1024 * 1024 * 1024,    // 1G
            compress: false,
            formData: { guid: generateUUID(), code: "", secret: "", upinfo: "" }
        }
        var option = {};
        var optionwebup = {};

        window.addEventListener("message", function (e) {
            if (e.data != null) {
                if (JSON.parse(e.data).source === "qj-upload") {
                    option = JSON.parse(e.data);
                    if (option.webupconfig) {
                        optionwebup = $.extend(defaults, JSON.parse(option.webupconfig));
                        optionwebup.formData.code = option.usercode;
                        optionwebup.formData.secret = option.secret;
                        optionwebup.formData.upinfo = option.upinfo;


                    }
                    $.post("/document/checkauth", { code: option.usercode, secret: option.secret }, function (result, status) {
                        if (result == "Y") {
                            uploader = WebUploader.create(optionwebup);
                            initupload();
                        } else {
                            alert('secret有误,请检查后再上传')
                        }
                    });

                }

            }
        });



        function generateUUID() {
            var d = new Date().getTime();
            var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
            });
            return uuid;
        };

        function getQueryString(name, defauval) {//获取URL参数,如果获取不到，返回默认值，如果没有默认值，返回空格
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) { return unescape(r[2]); }
            else {
                return defauval || "";
            }
        };//获取参数中数据




        // 当有文件添加进来时执行，负责view的创建
        function addFile(file) {
            var $li = $('<li id="' + file.id + '">' +
					'<p class="title">' + file.name + '</p>' +
					'<p class="imgWrap"></p>' +
					'<p class="progress"><span></span></p>' +
					'</li>'),

				$btns = $('<div class="file-panel">' +
					'<span class="cancel">删除</span>' +
					'<span class="rotateRight">向右旋转</span>' +
					'<span class="rotateLeft">向左旋转</span></div>').appendTo($li),
				$prgress = $li.find('p.progress span'),
				$wrap = $li.find('p.imgWrap'),
				$info = $('<p class="error"></p>'),

				showError = function (code) {
				    switch (code) {
				        case 'exceed_size':
				            text = '文件大小超出';
				            break;

				        case 'interrupt':
				            text = '上传暂停';
				            break;

				        default:
				            text = '上传失败，请重试';
				            break;
				    }

				    $info.text(text).appendTo($li);
				};

            if (file.getStatus() === 'invalid') {
                showError(file.statusText);
            } else {
                // @todo lazyload
                $wrap.text('预览中');
                uploader.makeThumb(file, function (error, src) {


                    if (error) {
                        $wrap.text('不能预览');
                        return;
                    }

                    var img = $('<img src="' + src + '">');
                    $wrap.empty().append(img);
                }, thumbnailWidth, thumbnailHeight);

                percentages[file.id] = [file.size, 0];
                file.rotation = 0;
            }

            file.on('statuschange', function (cur, prev) {
                if (prev === 'progress') {
                    $prgress.hide().width(0);
                } else if (prev === 'queued') {
                    //$li.off('mouseenter mouseleave');
                    //$btns.remove();
                }

                // 成功
                if (cur === 'error' || cur === 'invalid') {
                    console.log(file.statusText);
                    showError(file.statusText);
                    percentages[file.id][1] = 1;
                } else if (cur === 'interrupt') {
                    showError('interrupt');
                } else if (cur === 'queued') {
                    percentages[file.id][1] = 0;
                } else if (cur === 'progress') {
                    $info.remove();
                    $prgress.css('display', 'block');
                } else if (cur === 'complete') {
                    $li.append('<span class="success"></span>');
                }

                $li.removeClass('state-' + prev).addClass('state-' + cur);
            });

            //$li.on('mouseenter', function () {
            //    $btns.stop().animate({ height: 30 });
            //});

            //$li.on('mouseleave', function () {
            //    $btns.stop().animate({ height: 0 });
            //});

            $btns.on('click', 'span', function () {
                var index = $(this).index(),
					deg;

                switch (index) {
                    case 0:
                        uploader.removeFile(file);
                        return;

                    case 1:
                        file.rotation += 90;
                        break;

                    case 2:
                        file.rotation -= 90;
                        break;
                }

                if (supportTransition) {
                    deg = 'rotate(' + file.rotation + 'deg)';
                    $wrap.css({
                        '-webkit-transform': deg,
                        '-mos-transform': deg,
                        '-o-transform': deg,
                        'transform': deg
                    });
                } else {
                    $wrap.css('filter', 'progid:DXImageTransform.Microsoft.BasicImage(rotation=' + (~~((file.rotation / 90) % 4 + 4) % 4) + ')');
                    // use jquery animate to rotation
                    // $({
                    //     rotation: rotation
                    // }).animate({
                    //     rotation: file.rotation
                    // }, {
                    //     easing: 'linear',
                    //     step: function( now ) {
                    //         now = now * Math.PI / 180;

                    //         var cos = Math.cos( now ),
                    //             sin = Math.sin( now );

                    //         $wrap.css( 'filter', "progid:DXImageTransform.Microsoft.Matrix(M11=" + cos + ",M12=" + (-sin) + ",M21=" + sin + ",M22=" + cos + ",SizingMethod='auto expand')");
                    //     }
                    // });
                }


            });

            $li.appendTo($queue);
        }
        function sendmesg(t) {
            window.parent.postMessage(JSON.stringify(t), t.url)
        }
        // 负责view的销毁
        function removeFile(file) {
            var $li = $('#' + file.id);

            delete percentages[file.id];
            updateTotalProgress();
            $li.off().find('.file-panel').off().end().remove();
            var filedata = JSON.stringify(getFileData());
            var msgdata = { type: "filecomplete", data: filedata, url: option.url };
            sendmesg(msgdata);
            window.name = filedata
        }

        function updateTotalProgress() {
            var loaded = 0,
				total = 0,
				spans = $progress.children(),
				percent;

            $.each(percentages, function (k, v) {
                total += v[0];
                loaded += v[0] * v[1];
            });

            percent = total ? loaded / total : 0;


            spans.eq(0).text(Math.round(percent * 100) + '%');
            spans.eq(1).css('width', Math.round(percent * 100) + '%');
            updateStatus();
        }

        function updateStatus() {
            var text = '', stats;

            if (state === 'ready') {
                text = '选中' + fileCount + '个文件，共' +
						WebUploader.formatSize(fileSize) + '。';
            } else if (state === 'confirm') {
                stats = uploader.getStats();
                if (stats.uploadFailNum) {
                    text = '已成功上传' + stats.successNum + '个文件至文件库，' +
						//stats.uploadFailNum + '个文件上传失败，<a class="retry" href="#">重新上传</a>失败文件或<a class="ignore" href="#">忽略</a>'
						stats.uploadFailNum + '个文件上传失败，<a class="retry" href="#">重新上传</a>失败文件'
                }

            } else {
                stats = uploader.getStats();
                text = '共' + fileCount + '个（' +
						WebUploader.formatSize(fileSize) +
						'），已上传' + stats.successNum + '个';

                if (stats.uploadFailNum) {
                    text += '，失败' + stats.uploadFailNum + '个';
                }
            }

            $info.html(text);
        }

        function setState(val) {
            var file, stats;

            if (val === state) {
                return;
            }

            $upload.removeClass('state-' + state);
            $upload.addClass('state-' + val);
            state = val;

            switch (state) {
                case 'pedding':
                    $placeHolder.removeClass('element-invisible');
                    $queue.parent().removeClass('filled');
                    $queue.hide();
                    $statusBar.addClass('element-invisible');
                    uploader.refresh();
                    break;

                case 'ready':
                    $placeHolder.addClass('element-invisible');
                    $('#filePicker2').removeClass('element-invisible');
                    $queue.parent().addClass('filled');
                    $queue.show();
                    $statusBar.removeClass('element-invisible');
                    uploader.refresh();
                    break;

                case 'uploading':
                    $('#filePicker2').addClass('element-invisible');
                    $progress.show();
                    $upload.text('暂停上传');
                    break;

                case 'paused':
                    $progress.show();
                    $upload.text('继续上传');
                    break;

                case 'confirm':
                    $progress.hide();

                    //$upload.text('开始上传');  //暂时取消掉，因为现在自动进行上传操作

                    stats = uploader.getStats();
                    if (stats.successNum && !stats.uploadFailNum) {
                        setState('finish');
                        return;
                    }
                    break;
                case 'finish':

                    $('#filePicker2').removeClass('element-invisible');
                    stats = uploader.getStats();
                    if (stats.successNum) {
                        //var timer = null;
                        //int i =0;
                        //clearInterval(timer);
                        //timer = setInterval(function () {

                        //    var mm = i++ % 2 ? "none" : "block";//还是有收获的，这个写法比if..else想必简单了好多 
                        //    i > 8 && clearInterval(timer);//这个短路用的经典啊 
                        //}, 80);
                        $tips.removeClass('element-invisible')
                        $tips.text('上传成功');
                        setTimeout(function () { $tips.addClass('element-invisible') }, 3000);
                    } else {
                        // 没有成功的文件，重设
                        state = 'done';
                        location.reload();
                    }
                    break;
            }

            updateStatus();
        }
        function getFileData() {
            var filedata = [];
            $(".state-complete").each(function () {
                var md5 = $(this).find(".md5").text();
                var filename = $(this).find(".filename").text();
                var filesize = $(this).find(".filesize").text();
                var zyid = $(this).find(".zyid").text();
                filedata.push({ filename: filename, md5: md5, filesize: filesize, zyid: zyid });
            });
            return filedata;
        }
        function initupload() {
            // 添加“添加文件”的按钮，
            uploader.addButton({
                id: '#filePicker2',
                label: '继续添加'


            });
            uploader.onUploadProgress = function (file, percentage) {
                var $li = $('#' + file.id),
                    $percent = $li.find('.progress span');

                $percent.css('width', percentage * 100 + '%');
                percentages[file.id][1] = percentage;
                updateTotalProgress();
            };
            uploader.onFileQueued = function (file) {
                fileCount++;
                fileSize += file.size;

                if (fileCount === 1) {
                    $placeHolder.addClass('element-invisible');
                    $statusBar.show();
                }

                addFile(file);
                setState('ready');
                updateTotalProgress();
            };
            uploader.onFileDequeued = function (file) {
                fileCount--;
                fileSize -= file.size;

                if (!fileCount) {
                    setState('pedding');
                }

                removeFile(file);
                updateTotalProgress();

            };
            uploader.on('all', function (type) {
                var stats;
                switch (type) {
                    case 'uploadFinished':
                        setState('confirm');
                        break;

                    case 'startUpload':
                        setState('uploading');
                        break;

                    case 'stopUpload':
                        setState('paused');
                        break;

                }
            });
            uploader.onError = function (code) {
                var text = code;
                switch (code) {
                    case "F_DUPLICATE":
                        text = "已重复添加该文件.";
                        break;
                    case "UPLOAD_ERROR":
                        text = "上传出错.";
                        break;
                    case "Q_TYPE_DENIED":
                        text = "类型不匹配或上传文件大小为0.";
                        break;
                    case "Q_EXCEED_NUM_LIMIT":
                        text = "超过上传数量限制.";
                        break;
                    case "Q_EXCEED_SIZE_LIMIT":
                        text = "超过上传大小限制.";
                        break;
                }
                alert('Error: ' + text);
            };
            $upload.on('click', function () {
                if ($(this).hasClass('disabled')) {
                    return false;
                }

                if (state === 'ready') {
                    uploader.upload();
                } else if (state === 'paused') {
                    uploader.upload();
                } else if (state === 'uploading') {
                    uploader.stop(true);
                }
            });

            // 最后显示md5值。
            uploader.on('uploadSuccess', function (file, response) {
                var $li = $('#' + file.id);
                $li.append('<span class = "filename">' + file.name + '</span>');
                $li.append('<span class = "filesize" >' + file.size + '</span>');
                if (response&&response.split(',').length > 1) {
                    $li.append('<span class = "zyid" >' + response.split(',')[1] + '</span>');
                }

                var filedata = JSON.stringify(getFileData());
                var msgdata = { type: "filecomplete", data: filedata, url: option.url };
                sendmesg(msgdata);
                window.name = filedata
            });

            $info.on('click', '.retry', function () {
                uploader.retry();
            });

            //$info.on('click', '.ignore', function () {
            //    uploader.removeFile(file);
            //});

            $upload.addClass('state-' + state);
            updateTotalProgress();
        }





    });

    //分片
    //https://github.com/fex-team/webuploader/issues/142
    //https://github.com/fex-team/webuploader/issues/139
    WebUploader.Uploader.register({
        "before-send-file": "beforeSendFile",
        'before-send': 'beforeSend',
        'after-send-file': 'afterSendFile'
    }, {
        beforeSendFile: function (file) {
            var me = this,
                owner = this.owner,
                server = me.options.server,
                deferred = WebUploader.Deferred();


            owner.md5File(file.source, 0, 10 * 1024 * 1024)

            // 如果读取出错了，则通过reject告诉webuploader文件上传出错。
            .fail(function () {
                deferred.reject();
            })

            // md5值计算完成
            .then(function (md5) {
                //alert(md5);
                $.post(server + "/checkwholefile", { md5: md5, code: me.options.formData.code, secret: me.options.formData.secret }, function (result, status) {
                    var $li = $('#' + file.id);
                    if (result.result == true) {
                        owner.skipFile(file);
                        if (result.zyid) {
                            $li.append('<span  class = "zyid">' + result.zyid + '</span>');

                        }
                        //console.log('文件重复，已跳过');
                    }
                    else {
                        file.wholeMd5 = md5;
                        file.chunkMd5s = result.chunkMd5s;  //如果后台已经有该文件的分片记录
                    }
                    // 介绍此promise, webuploader接着往下走。
                    deferred.resolve();
                    //alert("Data: " + data + "\nStatus: " + status);
                    $li.append('<span  class = "md5">' + md5 + '</span>');
                });
            });
            return deferred.promise();
        },

        beforeSend: function (block) {
            var me = this,
               owner = this.owner,
               server = me.options.server,
               deferred = WebUploader.Deferred();
            var chunkFile = block.blob;
            var file = block.file;
            var chunk = block.chunk;
            var chunks = block.chunks;
            var start = block.start;
            var end = block.end;
            var total = block.total;
            file.chunks = chunks;


            if (chunks > 1) { //文件大于chunksize 分片上传
                owner.md5File(chunkFile)
                .progress(function (percentage) {
                    //分片MD5计算可以不知道计算进度
                })
                .then(function (chunkMd5) {
                    //owner.options.formData.chunkMd5 = chunkMd5;
                    block.chunkMd5 = chunkMd5;
                    var chunkMd5s = file.chunkMd5s;
                    var exists = false;
                    if (typeof (chunkMd5s) == "undefined") {
                        exists = false;
                    } else {

                        for (var i = 0; i < chunkMd5s.length; i++) {
                            if (chunkMd5 == chunkMd5s[i]) {
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (exists) {
                        deferred.reject();
                    } else {
                        deferred.resolve();
                    }
                });

            } else {//未分片文件上传
                block.chunkMd5 = file.wholeMd5;
                owner.options.formData.chunkMd5 = file.wholeMd5;
                deferred.resolve();
            }

            owner.options.formData.fileMd5 = file.wholeMd5;
            return deferred.promise();
        },

        afterSendFile: function (file) {
            var me = this,
                server = me.options.server,
                name = file.name,
                ext = file.ext,
                deferred = $.Deferred(),
                fileMd5 = file.wholeMd5,
                chunks = file.chunks;



            if (chunks > 1) {//TODO 向server发送文件合并请求，根据结果决定文件上传成功与否
                $.ajax({
                    cache: false,
                    async: true,
                    type: "post",
                    dataType: "json",
                    url: server + "/fileMerge",
                    data: {
                        fileMd5: fileMd5,
                        ext: ext, //文件扩展名称
                        name: name,
                        contentType: file.type,
                        code: me.options.formData.code,
                        secret: me.options.formData.secret,
                        upinfo: me.options.formData.upinfo

                    },
                    success: function (result) {
                        if (result.result) {
                            //合并文件成功
                            deferred.resolve();
                            if (result.zyid) {
                                var $li = $('#' + file.id);
                                $li.append('<span  class = "zyid">' + result.zyid + '</span>');

                            }

                        } else {
                            //合并文件失败
                            deferred.reject();
                        }
                    }
                });

            } else {
                deferred.resolve();
            }

            return deferred.promise();
        }
    });

})(jQuery);