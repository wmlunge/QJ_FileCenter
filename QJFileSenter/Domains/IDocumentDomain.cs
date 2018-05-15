using System;
using System.Collections.Generic;
using System.IO;
using QJ_FileCenter.Models;

namespace QJ_FileCenter.Domains
{
    public interface IDocumentDomain
    {
        string Push(string name, string extension, string contentType, Stream value, string qyCode, string fileinfo);

        dynamic Fetch(string qyCode, string md5);

        IEnumerable<dynamic> Fetch(string qyCode, string[] md5s);

        System.Dynamic.ExpandoObject Exist(string qyCode, string md5);
        string Compress(IEnumerable<dynamic> documents);
        string CompressNestedFolder(IEnumerable<dynamic> documents, IEnumerable<Tuple<string, string>> zipEntryItems);
        string GetZipFile(string md5);
        string PushChunk(string wholeMd5, int chunk, string name, string extension, string contentType, Stream stream);
        System.Dynamic.ExpandoObject FileMerge(string fileMd5, string name, string contentType, string ext, string qyCode, string upinfo);

        SubFolderModel UnCompress(dynamic document, string qyCode);
        string GetUnZipFile(string md5, string name);

    }
}
