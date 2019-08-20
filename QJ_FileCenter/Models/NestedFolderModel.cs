using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QJ_FileCenter.Models
{
    //public class NestedFolderModel
    //{
    //    public string Action { get; set; }
    //    public string ErrorMsg { get; set; }
    //    public string DataLength { get; set; }
    //    public string ResultType { get; set; }
    //    public SubFolderModel Result { get; set; }
    //    public List <IdTypeModel> Result1 { get; set; }
    //    public string Result2 { get; set; }
    //}

    public class SubFolderModel
    {
        public SubFolderModel()
        {
            SubFolder = new List<SubFolderModel>();
            SubFileS = new List<SubFileModel>();
        }

        public string FolderID { get; set; }
        public string Name { get; set; }
        public List<SubFolderModel> SubFolder { get; set; }
        public List<SubFileModel> SubFileS { get; set; }

        //public IEnumerable<string> GetMd5s()
        //{
        //    return SubFolder.SelectMany(o => o.GetMd5s()).Union(SubFileS.Select(o => o.FileMD5));
        //}

        public IEnumerable<Tuple<string, string>> GetZipEntryItems(string upperName = "")
        {
            if (upperName == "")
            {
                upperName = Name;
            }
            else
            {
                upperName = upperName + "/" + Name;
            }

            return SubFolder.SelectMany(o => o.GetZipEntryItems(upperName))
                .Union(SubFileS.Select(o => new Tuple<string, string>(o.FileMD5, upperName + "/" + o.Name + "." + o.FileExtendName)));
        }

        public void SetZipEntryItem(string zipName)
        {
            var sections = zipName.Split('\\');

            var subFiles = this.SubFileS;
            var subFolder = this.SubFolder;
            for (int i = 0; i < sections.Length; i++)
            {
                var section = sections[i];
                if (i == sections.Length - 1)
                {
                    subFiles.Add(new SubFileModel() { Name = section, FolderID = zipName });
                }
                else
                {
                    var sub = subFolder.Find(o => o.Name == section);
                    if (sub == null)
                    {
                        sub = new SubFolderModel() { Name = section };
                        subFolder.Add(sub);
                    }

                    subFolder = sub.SubFolder;
                    subFiles = sub.SubFileS;
                }
            }
        }
    }

    public class SubFileModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string FolderID { get; set; }
        public string FileExtendName { get; set; }
        public string FileSize { get; set; }
        public string FileDownloadCishu { get; set; }
        public string FileMD5 { get; set; }
        public string FileVersin { get; set; }
        public string IsRecycle { get; set; }
        public string FileUrl { get; set; }
        public string ViewAuthUsers { get; set; }
        public string DownloadAuthUsers { get; set; }
        public string CollectUser { get; set; }
        public string ShareCode { get; set; }
        public string SharePasd { get; set; }
        public string ShareType { get; set; }
        public string ShareDueDate { get; set; }
        public string Remark { get; set; }
        public string CRUser { get; set; }
        public string CRDate { get; set; }
    }

    public class IdTypeModel
    {
        public string ID { get; set; }
        public string Type { get; set; }
    }




}
