using System;
using System.Collections.Generic;

namespace QJFile.Data
{
    public partial class FT_File_Vesion
    {
        public int? ComId { get; set; }
        public int ID { get; set; }
        public int? RFileID { get; set; }
        public string VesionSM { get; set; }
        public string FileMD5 { get; set; }
        public string Remark { get; set; }
        public string FileSize { get; set; }
        public string CRUser { get; set; }
        public DateTime? CRDate { get; set; }
    }
}
