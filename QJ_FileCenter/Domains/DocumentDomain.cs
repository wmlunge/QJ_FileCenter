using glTech.Log4netWrapper;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using QJ_FileCenter.Models;
using QJ_FileCenter.Repositories;
using QJ_FileCenter.Utils;
using QJFile.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QJ_FileCenter.Domains
{
    public class DocumentDomain : IDocumentDomain
    {
        private IDocumentRepository _documentRepository;
        private Repositories.AppRepository _appRepository;
        private string rootPath;
        private string rootPath2;//2017-07-01新加

        public DocumentDomain(IDocumentRepository documentRepository, AppRepository appRepository)
        {
            _documentRepository = documentRepository;
            _appRepository = appRepository;
            rootPath = "";//2017-07-01改为空了,原来的(rootPath = _appRepository.AppConfigModel.RootPath;)
            rootPath2 = _appRepository.AppConfigModel.RootPath;//2017-07-01新加
        }


        public string Push(string name, string extension, string contentType, Stream stream, string qyCode, string strfileinfo)
        {
            Document FileResult = new Document();
            int start = 0;
            int end = 10 * 1024 * 1024;
            if (end > stream.Length)
            {
                end = (int)stream.Length;
            }

            var buffer = new byte[end];

            int c = stream.Read(buffer, 0, buffer.Length);
            var md5 = MD5Util.GetMd5(buffer);
            var realDate = DateTime.Now;

            string filePath;
            if (string.IsNullOrEmpty(qyCode))
            {
                filePath = string.Format("{0}\\{1:yyyyMM}\\{2}{3}", rootPath, realDate, md5, extension);
            }
            else
            {
                filePath = string.Format("{0}\\{4}\\{1:yyyyMM}\\{2}{3}", rootPath, realDate, md5, extension, qyCode);
            }

            if (!_documentRepository.Exist(qyCode, md5) && SaveDisk(filePath, stream))
            {
                FileResult = _documentRepository.SaveDocument(md5, qyCode, name, extension, contentType, rootPath2 + filePath, realDate, strfileinfo);
            }
            return (md5 + "," + FileResult.ID ?? "" + "," + FileResult.filesize ?? "").TrimEnd(',');
        }

        public string PushChunk(string wholeMd5, int chunk, string name, string extension, string contentType, Stream stream)
        {
            var buffer = new byte[stream.Length];

            int c = stream.Read(buffer, 0, buffer.Length);
            var md5 = MD5Util.GetMd5(buffer);
            var realDate = DateTime.Now;
            var filePath = string.Format("{0}\\{1}\\{2:D3}.{3}", rootPath, wholeMd5, chunk, md5);
            SaveDisk(filePath, stream);
            return md5;
        }

        public System.Dynamic.ExpandoObject FileMerge(string fileMd5, string name, string contentType, string ext, string qyCode, string upinfo)
        {
            dynamic re = new System.Dynamic.ExpandoObject();
            try
            {
                Document FileResult = new Document();
                var chunkPath = string.Format("{0}\\{1}", rootPath2, fileMd5);
                var outputFile = string.Format("{0}\\{1}.{2}", rootPath2, fileMd5, ext);
                if (Directory.Exists(chunkPath))
                {
                    var chunkFiles = Directory.GetFiles(chunkPath);

                    var realDate = DateTime.Now;
                    string filePath;
                    if (string.IsNullOrEmpty(qyCode))
                    {
                        filePath = string.Format("{0}\\{1:yyyyMM}\\{2}{3}", rootPath, realDate, fileMd5, ext);
                    }
                    else
                    {
                        filePath = string.Format("{0}\\{4}\\{1:yyyyMM}\\{2}{3}", rootPath, realDate, fileMd5, ext, qyCode);
                    }


                    if (!_documentRepository.Exist(qyCode, fileMd5) && SaveDisk(filePath, chunkFiles))
                    {
                        FileResult = _documentRepository.SaveDocument(fileMd5, qyCode, name, ext, contentType, rootPath2 + filePath, realDate, upinfo);
                    }

                    Directory.Delete(chunkPath, true);
                }
                re.zyid = FileResult.ID ?? "";
                re.result = true;
            }
            catch (Exception)
            {

                throw;
            }
            return re;
        }






        private bool SaveDisk(string filePath, Stream stream)
        {
            filePath = rootPath2 + filePath;   //2017-07-01上传文件
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                fileStream.Close();
            }

            return true;
        }

        private bool SaveDisk(string filePath, string[] chunkFiles)
        {
            filePath = rootPath2 + filePath;   //2017-07-01上传文件
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                foreach (string chunkFile in chunkFiles)
                {
                    using (Stream input = File.OpenRead(chunkFile))
                    {
                        input.CopyTo(stream);
                    }
                }
            }

            return true;
        }

        private byte[] GetFile(string fullPath)
        {
            return File.ReadAllBytes(fullPath);
        }


        public dynamic Fetch(string qyCode, string md5)
        {
            var doc = new DocumentB().GetEntities(d => d.Qycode == qyCode &&( d.ID == md5 || d.Md5 == md5)).FirstOrDefault();
            if (doc == null) return null;
            return new
            {
                md5 = doc.Md5,
                contenttype = doc.ContentType,
                name = Path.GetFileNameWithoutExtension(doc.FileName),
                extension = doc.Extension.TrimStart('.'),
                rdate = doc.RDate,
                file = doc.FullPath,
            };
        }
        public Document Fetch(string zyid)
        {
            Document doc = new DocumentB().GetEntities(d => d.ID == zyid || d.Md5 == zyid).FirstOrDefault();
            if (doc == null) return null;
            return doc;
            //    new
            //{
            //    md5 = doc.Md5,
            //    contenttype = doc.ContentType,
            //    name = Path.GetFileNameWithoutExtension(doc.FileName),
            //    extension = doc.Extension.TrimStart('.'),
            //    rdate = doc.RDate,
            //    file = doc.FullPath,
            //};
        }
        public IEnumerable<dynamic> Fetch(string qyCode, string[] md5s)
        {
            var docs = new DocumentB().GetEntities(d => d.Qycode == qyCode && (md5s.Contains(d.ID)|| md5s.Contains(d.Md5)));
            return docs.Select(o => new
            {
                md5 = o.Md5,
                contenttype = o.ContentType,
                name = Path.GetFileNameWithoutExtension(o.FileName),
                extension = o.Extension.TrimStart('.'),
                rdate = o.RDate,
                file = o.FullPath,
            });
        }

        public System.Dynamic.ExpandoObject Exist(string qyCode, string md5)
        {
            dynamic re = new System.Dynamic.ExpandoObject();

            try
            {
                var result = _documentRepository.Exist(qyCode, md5);
                if (result)
                {
                    var doc = new DocumentB().GetEntities(d => d.Qycode == qyCode && d.Md5 == md5).FirstOrDefault();

                    doc.RDate = DateTime.Now;
                    doc.LDate = DateTime.Now;
                    doc.ID = Guid.NewGuid().ToString("N");
                    doc.fileinfo = "";
                    new DocumentB().Insert(doc);
                    re.zyid = doc.ID;
                }
                re.result = result;

                if (!result)
                {
                    //不存在，寻找分片内容。
                    string chunkPath = Path.Combine(_appRepository.AppConfigModel.RootPath, md5);

                    if (Directory.Exists(chunkPath))
                    {
                        re.chunkMd5s = Directory.GetFiles(chunkPath).Select(o =>
                        {
                            return Path.GetFileName((string)o).Split('.')[1];
                        }).ToList();
                    }
                }

            }
            catch (Exception ex)
            {

                Logger.LogError(ex.Message.ToString());
            }
            return re;

        }

        public SubFolderModel UnCompress(dynamic document, string qyCode)
        {
            var root = new SubFolderModel();
            var file = document.file;
            var md5 = document.md5;
            root.Name = md5;
            var temp = Path.Combine(_appRepository.AppConfigModel.RootPath, qyCode + "//UnZipFolder", md5);
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            using (var zipInputStream = new ZipInputStream(File.OpenRead(file)))
            {
                ZipEntry zipEntry = null;
                while ((zipEntry = zipInputStream.GetNextEntry()) != null)
                {
                    var zipEntryName = zipEntry.Name.Replace('/', '\\');
                    string directoryName = Path.GetDirectoryName(zipEntryName);
                    string fileName = Path.GetFileName(zipEntryName);

                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(Path.Combine(temp, directoryName));
                    }

                    if (string.IsNullOrEmpty(fileName)) continue;

                    if (zipEntry.CompressedSize == 0)
                        break;

                    var fileUnzipName = Path.Combine(temp, zipEntryName);
                    if (zipEntry.IsDirectory)
                    {
                        directoryName = Path.GetDirectoryName(Path.Combine(temp, zipEntryName));
                        Directory.CreateDirectory(directoryName);

                    }
                    else
                    {
                        root.SetZipEntryItem(zipEntryName);
                    }

                    using (FileStream stream = File.Create(fileUnzipName))
                    {
                        var abyBuffer = new byte[4096];
                        StreamUtils.Copy(zipInputStream, stream, abyBuffer);
                    }
                }
            }
            return root;
        }

        public string GetUnZipFile(string md5, string name)
        {
            var unzipname = Path.Combine(_appRepository.AppConfigModel.RootPath, "ZipTemp", md5, name);

            if (File.Exists(unzipname))
                return unzipname;
            return "";
        }

        public string Compress(IEnumerable<dynamic> documents)
        {
            var temp = Path.Combine(_appRepository.AppConfigModel.RootPath, "ZipTemp");
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            var md5s = string.Join(",", documents.Select(o => o.md5));

            var md5 = MD5Util.GetMd5(md5s);

            var zipName = string.Format("{0}\\{1}{2}", temp, md5, ".zip");

            if (File.Exists(zipName))
                return zipName;

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipName)))
            {
                zipOutputStream.SetLevel(9);
                var abyBuffer = new byte[4096];

                foreach (var document in documents)
                {
                    string filename = document.file;
                    string name = document.name;
                    string extension = document.extension;
                    using (FileStream filestream = File.OpenRead(filename))
                    {
                        var zipEntry = new ZipEntry(name + "." + extension);
                        zipEntry.DateTime = DateTime.Now;
                        zipEntry.Size = filestream.Length;

                        zipOutputStream.PutNextEntry(zipEntry);
                        StreamUtils.Copy(filestream, zipOutputStream, abyBuffer);
                    }
                }
            }

            return zipName;
        }

        public string CompressNestedFolder(IEnumerable<dynamic> documents, IEnumerable<Tuple<string, string>> zipEntryItems)
        {
            var temp = Path.Combine(_appRepository.AppConfigModel.RootPath, "ZipTemp");
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            var md5s = string.Join(",", zipEntryItems.Select(o => o.Item1));

            var md5 = MD5Util.GetMd5(md5s);

            var zipName = string.Format("{0}\\{1}{2}", temp, md5, ".zip");

            if (File.Exists(zipName))
                return md5;

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipName)))
            {
                zipOutputStream.SetLevel(9);
                var abyBuffer = new byte[4096];

                foreach (var zipEntryItem in zipEntryItems)
                {
                    var document = documents.FirstOrDefault(o => o.md5 == zipEntryItem.Item1);
                    if (document == null) continue;
                    string filename = document.file;
                    string name = document.name;
                    string extension = document.extension;
                    using (FileStream filestream = File.OpenRead(filename))
                    {
                        var zipEntry = new ZipEntry(zipEntryItem.Item2);
                        zipEntry.DateTime = DateTime.Now;
                        zipEntry.Size = filestream.Length;

                        zipOutputStream.PutNextEntry(zipEntry);
                        StreamUtils.Copy(filestream, zipOutputStream, abyBuffer);
                    }
                }
            }

            return md5;
        }

        public string GetZipFile(string md5)
        {
            var temp = Path.Combine(_appRepository.AppConfigModel.RootPath, "ZipTemp");
            var zipName = string.Format("{0}\\{1}{2}", temp, md5, ".zip");

            if (File.Exists(zipName))
                return zipName;
            return "";
        }



    }
}
