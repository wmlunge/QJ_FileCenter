using QJ_FileCenter.Models;
using QJ_FileCenter.Utils;
using QJFile.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace QJ_FileCenter.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public DocumentRepository()
        {

        }

        public bool Exist(string qycode, string md5)
        {
            var month = DateTime.Now.ToString("yyyyMM");
            Document value = new DocumentB().GetEntities(D => D.Qycode == qycode && D.Md5 == md5).FirstOrDefault();
            return value != null;
        }




        public Document SaveDocument(string md5, string qycode, string name, string extension, string contentType, string filePath, DateTime realDate, string strfileinfo)
        {

            Document Model = new Document();
            Model.Md5 = md5;
            Model.Qycode = qycode;
            Model.FileName = name;
            Model.Extension = extension;
            Model.ContentType = contentType;
            Model.Directory = Path.GetDirectoryName(filePath);
            Model.Month = realDate.ToString("yyyyMM");
            Model.FullPath = filePath;
            Model.RDate = realDate;
            Model.LDate = realDate;
            Model.ID = Guid.NewGuid().ToString("N");
            Model.fileinfo = strfileinfo;
            Model.isyl = "0";
            Model.ylinfo = "0";
            FileInfo fileInfo = new FileInfo(filePath);
            Model.filesize = fileInfo.Length.ToString();
            new DocumentB().Insert(Model);


            OfficeConverter Converter = new OfficeConverter();
            Converter.ConverFile(Model);
          


            return Model;

        }



    }
}
