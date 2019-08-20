using System;
using System.Collections.Generic;

namespace QJFile.Data
{
    public partial class FT_File
    {
        public int? ComId { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int? FolderID { get; set; }
        public string FileExtendName { get; set; }
        public string FileSize { get; set; }
        public int? FileDownloadCishu { get; set; }
        public string FileMD5 { get; set; }
        public int? FileVersin { get; set; }
        public string IsRecycle { get; set; }
        public string FileUrl { get; set; }
        public string ViewAuthUsers { get; set; }
        public string DownloadAuthUsers { get; set; }
        public string CollectUser { get; set; }
        public string ShareCode { get; set; }
        public string SharePasd { get; set; }
        public string ShareType { get; set; }
        public DateTime? ShareDueDate { get; set; }
        public string ISYL { get; set; }
        public string Remark { get; set; }
        public string UPUser { get; set; }
        public DateTime? UPDDate { get; set; }
        public string CRUser { get; set; }
        public DateTime? CRDate { get; set; }
        public string YLUrl { get; set; }
        public string YLCode { get; set; }
        public string YLPath { get; set; }
        public string YLCount { get; set; }
        public string TBStatus { get; set; }
        public int? TotalTime { get; set; }
        public string zyid { get; set; }
    }
}
