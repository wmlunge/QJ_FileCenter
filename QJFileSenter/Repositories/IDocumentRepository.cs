using QJ_FileCenter.Models;
using QJFile.Data;
using System;
using System.Collections.Generic;

namespace QJ_FileCenter.Repositories
{
    public interface IDocumentRepository
    {

        bool Exist(string qycode, string md5);

        Document SaveDocument(string md5, string qycode, string name, string extension, string contentType, string filePath, DateTime realDate, string strfileinfo);




    }
}
