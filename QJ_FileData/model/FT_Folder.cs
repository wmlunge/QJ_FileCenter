using System;
using System.Collections.Generic;

namespace QJFile.Data
{
    public partial class FT_Folder
    {
        public int? ComId { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string FolderType { get; set; }
        public int? PFolderID { get; set; }
        public int? FolderLev { get; set; }
        public string Remark { get; set; }
        public string FolderSpace { get; set; }
        public string ViewAuthUsers { get; set; }
        public string DownloadAuthUsers { get; set; }
        public string UploadaAuthUsers { get; set; }
        public string CollectUser { get; set; }
        public string FoldUploadUrl { get; set; }
        public string FoldDowmLoadUrl { get; set; }
        public string ShareCode { get; set; }
        public string SharePasd { get; set; }
        public string ShareType { get; set; }
        public DateTime? ShareDueDate { get; set; }
        public string CRUser { get; set; }
        public DateTime? CRDate { get; set; }
    }
}
