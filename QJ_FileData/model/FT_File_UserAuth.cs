using System;
using System.Collections.Generic;

namespace QJFile.Data
{
    public partial class FT_File_UserAuth
    {
        public int? ComId { get; set; }
        public int ID { get; set; }
        public string AuthType { get; set; }
        public string AuthUser { get; set; }
        public int? RefID { get; set; }
        public string RefType { get; set; }
        public string CRUser { get; set; }
        public DateTime? CRDate { get; set; }
    }
}
