using glTech.Log4netWrapper;
using Nancy;
using QJ_FileCenter.Models;
using QJ_FileCenter;
using System;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft;
using QJ_FileCenter.Domains;
using QJFile.Data;
using QJ_FileCenter.Repositories;
using System.Collections.Generic;
using System.Text;
using Nancy.Helpers;

namespace QJ_FileCenter.Handler
{
    public class FileAPIHandler : NancyModule
    {


        public FileAPIHandler()
            : base()
        {
            DocumentDomain documentDomain = new DocumentDomain(new DocumentRepository(), new AppRepository());
            userlog log = new userlog();
            Before += ctx =>
            {
                // new userlogB().Insert(new userlog {   });
                log.ip = ctx.Request.UserHostAddress;
                log.loginfo = ctx.Request.Path;
                log.remark = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                return ctx.Response;
            };
            Post["/document/checkauth"] = p =>
            {
                // new DocumentRepository().SaveDocument("3b31d546f6f94a8029f966a8d71c7c66", "SWXX", "123123", ".png", "image/png", @"D:\FileCenter\SWXX\201601\3b31d546f6f94a8029f966a8d71c7c66.png", DateTime.Now);
                string code = this.Request.Form.code;
                string secret = this.Request.Form.secret;
                Qycode space = new QycodeB().GetEntities(d => d.Code == code && d.secret == secret).FirstOrDefault();
                return space == null ? "N" : "Y";
            };
            Post["/document/fileupload"] = p =>
            {
                //借助上传组件工具上传
                try
                {
                    string size = this.Request.Form.size;
                    string chunks = this.Request.Form.chunks;
                    string strchunk = this.Request.Form.chunk;
                    string fileMd5 = this.Request.Form.fileMd5;
                    string spacecode = this.Request.Form.code;
                    string upinfo = this.Request.Form.upinfo;
                    if (!string.IsNullOrEmpty(spacecode))
                    {
                        spacecode = spacecode.Trim('\'');
                        int chunkcount = 0, chunk;
                        if (!string.IsNullOrEmpty(chunks) && int.TryParse(chunks, out chunkcount) && chunkcount > 1 && int.TryParse(strchunk, out chunk))
                        {
                            //属于分片文件
                            var file = this.Request.Files.FirstOrDefault();
                            if (file != null)
                            {
                                var md5 = documentDomain.PushChunk(fileMd5, chunk, file.Name, Path.GetExtension(file.Name), file.ContentType, file.Value);
                                return Response.AsJson(md5);
                            }
                        }
                        else
                        {
                            var file = this.Request.Files.FirstOrDefault();
                            if (file != null)
                            {
                                var md5 = documentDomain.Push(file.Name, System.IO.Path.GetExtension(file.Name), file.ContentType, file.Value, spacecode, upinfo);
                                return Response.AsJson(md5);
                            }
                        }
                    }
                    return Response.AsRedirect("/document");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message.ToString());
                    return Response.AsRedirect("/document");
                }


            };
            Post["/document/fileupload/{qycode}/{upinfo}"] = p =>
            {
                //后台上传
                try
                {
                    string qycode = p.qycode;
                    string upinfo = p.upinfo;
                    var file = this.Request.Files.FirstOrDefault();
                    if (file != null)
                    {
                        var md5 = documentDomain.Push(file.Name, System.IO.Path.GetExtension(file.Name), file.ContentType, file.Value, qycode, upinfo);
                        return Response.AsText(md5);
                    }
                    return Response.AsRedirect("/document");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message.ToString());
                    return Response.AsRedirect("/document");
                }


            };
            Post["/document/fileupload/{qycode}"] = p =>
            {
                //后台上传
                try
                {
                    string qycode = p.qycode;
                    var file = this.Request.Files.FirstOrDefault();
                    if (file != null)
                    {
                        var md5 = documentDomain.Push(file.Name, System.IO.Path.GetExtension(file.Name), file.ContentType, file.Value, qycode, "");
                        return Response.AsText(md5);
                    }
                    return Response.AsRedirect("/document");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message.ToString());
                    return Response.AsRedirect("/document");
                }


            };
            Post["/document/fileupload/checkwholefile"] = p =>
            {
                var md5Client = this.Request.Form.md5;
                string spacecode = this.Request.Form.code;
                //MD5值验证
                string md5 = md5Client.Value.ToLower();

                var result = documentDomain.Exist(spacecode, md5);
                return Response.AsJson(result);
            };
            Post["/document/fileupload/fileMerge"] = p =>
            {
                string fileMd5 = this.Request.Form.fileMd5;
                string ext = this.Request.Form.ext;
                string name = this.Request.Form.name;
                string contentType = this.Request.Form.contentType;
                string spacecode = this.Request.Form.code;
                string upinfo = this.Request.Form.upinfo;


                if (!ext.StartsWith(".")) ext = "." + ext;
                string strID = "0";
                var result = documentDomain.FileMerge(fileMd5, name, contentType, ext, spacecode, upinfo);

                return Response.AsJson(result);
            };
            Get["/{qycode}/document/fileupload"] = p =>
            {
                string strCode = p.qycode;
                Qycode space = new QycodeB().GetEntities(d => d.Code == strCode).FirstOrDefault();
                return View["FileUpload"];
            };

            Get["/{qycode}/document/info/{id}"] = p =>
            {
                string strCode = p.qycode;
                string idS = p.id;
                string[] ids = idS.Split(',');
                List<Document> spaces = new DocumentB().GetEntities(d => d.Qycode == "-1").ToList();
                foreach (string id in ids)
                {
                    Document space = new DocumentB().GetEntities(d => d.Qycode == strCode && d.ID == id).FirstOrDefault();
                    if (space != null)
                    {
                        spaces.Add(space);
                    }
                }

                return Response.AsJson(spaces);

            };

            Get["/{qycode}/document/{id}"] = p =>
            {
                log.useraction = "下载文件";

                string id = p.id;
                string qycode = p.qycode;
                if (!string.IsNullOrEmpty(id) && id.Contains(","))
                {
                    var md5s = id.Split(',');

                    var documents = documentDomain.Fetch(qycode, md5s);

                    if (documents != null && documents.Any())
                    {
                        //压缩，并返回
                        var mimeType = "application/zip";
                        var extension = "zip";
                        var name = "打包下载的文件";
                        var file = documentDomain.Compress(documents);
                        return Response.AsFile(file, mimeType, extension, name);
                    }
                    else
                    {
                        return "找不到该文件" + p.id;
                    }
                }
                else
                {
                    var document = documentDomain.Fetch(qycode, p.id);

                    if (document == null)
                    {
                        return "找不到该文件 " + p.id;
                    }
                    string mimeType = document.contenttype;
                    string extension = document.extension;
                    string file = document.file;
                    string name = document.name;

                    return Response.AsFile(file, mimeType, extension, name);
                }
            };

            Get["/document/zipfile/{md5}"] = p =>
            {

                string md5 = p.md5;
                var filename = documentDomain.GetZipFile(md5);
                var mimeType = "application/zip";
                var extension = "zip";
                var name = "打包下载的文件";
                return Response.AsFile(filename, mimeType, extension, name);
            };
            Get["/{qycode}/document/image/{id}"] = p =>
            {
                log.useraction = "预览图片";

                string id = p.id;
                string qycode = p.qycode;

                var document = documentDomain.Fetch(qycode, p.id);

                if (document == null)
                    return "找不到该文件" + p.id;

                string mimeType = document.contenttype;
                string extension = document.extension;
                string file = document.file;
                string name = document.name;

                return Response.AsPreviewFile(file, mimeType, extension, name);
            };


            //修改缩略图接口
            Get["/{qycode}/document/image/{id}/{width?0}/{height?0}"] = p =>
            {
                log.useraction = "预览压缩图片";

                string id = p.id;
                string qycode = p.qycode;

                var document = documentDomain.Fetch(qycode, p.id);

                if (document == null)
                    return "找不到该文件 " + p.id;

                int width = p.width;
                int height = p.height;

                string mimeType = document.contenttype;
                string extension = document.extension;
                string file = document.file;
                string name = document.name;

                var smallImg = ImageCLass.GenSmallImg(file, width, height);

                return Response.AsPreviewFile(smallImg, mimeType, extension, name);
            };

            Get["/{qycode}/document/unzip/{id}"] = p =>
            {

                string qycode = p.qycode;
                var document = documentDomain.Fetch(qycode, p.id);
                if (document == null)
                    return "找不到该文档 " + p.id;

                string extension = document.extension;

                if (extension == "zip")
                {
                    SubFolderModel model = documentDomain.UnCompress(document, p.qycode);
                    return Response.AsJson(model);
                }

                return Response.AsJson("");
            };


            Post["/{qycode}/document/zipfolder"] = p =>
            {

                var data = Request.Form.data;
                string qycode = p.qycode;

                string md5 = "";
                if (data)
                {
                    SubFolderModel subFolderModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SubFolderModel>(data);

                    var zipItems = subFolderModel.GetZipEntryItems().ToArray();
                    var md5s = zipItems.Select(o => o.Item1).ToArray();
                    var documents = documentDomain.Fetch(qycode, md5s);
                    if (documents != null && documents.Any())
                    {
                        md5 = documentDomain.CompressNestedFolder(documents, zipItems);
                    }
                }

                return md5;
            };


            Get["/{qycode}/document/zipfile/{md5}"] = p =>
            {

                string md5 = p.md5;
                var filename = documentDomain.GetZipFile(md5);
                var mimeType = "application/zip";
                var extension = "zip";
                var name = "打包下载的文件";
                return Response.AsFile(filename, mimeType, extension, name);
            };

            Post["/{qycode}/document/nestedfolder"] = p =>
            {

                var data = Request.Form.data;
                string qycode = p.qycode;
                if (data)
                {
                    SubFolderModel subFolderModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SubFolderModel>(data);

                    var zipItems = subFolderModel.GetZipEntryItems().ToArray();
                    var md5s = zipItems.Select(o => o.Item1).ToArray();
                    var documents = documentDomain.Fetch(qycode, md5s);
                    if (documents != null && documents.Any())
                    {
                        //压缩，并返回
                        var mimeType = "application/zip";
                        var extension = "zip";
                        var name = subFolderModel.Name;
                        string file = documentDomain.CompressNestedFolder(documents, zipItems);
                        return Response.AsFile(file, mimeType, extension, name);
                    }
                    else
                    {
                        return "找不到该文档 " + subFolderModel.Name;
                    }
                }
                return "";
            };


            #region 多媒体相关接口
            Get["/document/officecov/{id}"] = p =>
            {
                string strReturn = "0,0";
                string id = p.id;

                Document document = documentDomain.Fetch(p.id);

                if (document == null)
                {
                    return "找不到该文件" + p.id;
                }
                else
                {
                    strReturn = document.isyl + "," + document.ylinfo;
                    if (document.isyl == "0")
                    {
                        string extension = document.Extension.TrimStart('.');
                        string file = document.FullPath;
                        //OfficeConverter Converter = new OfficeConverter();
                        //if (new List<string>() { "pdf" }.Contains(extension.ToLower()))
                        //{
                        //    Converter.PdfToImage(file, file.Substring(0, file.LastIndexOf('.')), 0, 0, 400);
                        //}
                        //if (new List<string>() { "doc", "docx" }.Contains(extension.ToLower()))
                        //{
                        //    Converter.WordToImage(document, file.Substring(0, file.LastIndexOf('.')), 0, 0, null, 400);
                        //}
                        //if (new List<string>() { "ppt", "pptx" }.Contains(extension.ToLower()))
                        //{
                        //    Converter.PPTToImage(file, file.Substring(0, file.LastIndexOf('.')), 0, 0, 400);
                        //}
                    }


                    //if (new List<string>() { "xls", "xlsx" }.Contains(extension.ToLower()))
                    //{
                    //    strReturn = Converter.YLExcel(file);
                    //    return Response.AsRedirect(strReturn);
                    //}
                    return strReturn;
                }

            };

            //返回office预览文件
            Get["/document/YL/{id}"] = p =>
            {
                log.useraction = "预览OFFICE文件";

                Document document = documentDomain.Fetch(p.id);
                if (document == null)
                {
                    return "找不到可预览的文件 " + p.id;
                }
                else if (document.isyl != "2")
                {
                    return "预览文件正在准备中 " + p.id;
                }
                else
                {
                    string strFileName = document.FileName.Substring(0, document.FileName.LastIndexOf('.'));
                    string strReturn = "/Web/Html/doc.html?ID=" + p.id + "&size=" + document.ylinfo + "&title=" + HttpUtility.UrlEncode(strFileName);
                    return Response.AsRedirect(strReturn);
                }
            };

            //获取可用的视频播放路径
            Get["/document/viedo/{id}"] = p =>
            {
                Document document = documentDomain.Fetch(p.id);
                if (document == null)
                {
                    return "找不到文件 " + p.id;
                }
                else
                {
                    FileStream fileStream = new FileStream(document.FullPath, FileMode.Open);
                    return Response.FromStream(fileStream, "octet-stream");
                }


            };

            //获取office转换后的图片
            Get["/document/YL/{id}/{index}"] = p =>
            {
                log.useraction = "预览OFFICE图片";

                string strReturn = "无法预览";
                string id = p.id;
                string index = p.index;
                Document document = documentDomain.Fetch(p.id);
                if (document == null)
                {
                    return "找不到可预览的文件 " + p.id;
                }
                else
                {

                    if (document.isyl == "1")
                    {
                        strReturn = "预览准备中";
                    }
                    if (document.isyl == "2")
                    {
                        string ylfile = document.FullPath;
                        ylfile = ylfile.Substring(0, ylfile.LastIndexOf('.')) + "/" + document.Md5 + "_" + index + ".png";
                        string strName = document.Md5 + "_" + index + ".png";
                        if (new List<string>() { ".ppt", ".pptx", ".doc", ".docx", ".pdf" }.Contains(document.Extension.ToLower()))
                        {
                            if (File.Exists(ylfile))
                            {
                                return Response.AsPreviewFile(ylfile, "image/png", ".png", strName);
                            }
                        }
                    }

                }
                return strReturn;
            };

            After += ctx =>
            {
                //添加日志
                new userlogB().Insert(log);

            };
            #endregion
        }
    }
}
