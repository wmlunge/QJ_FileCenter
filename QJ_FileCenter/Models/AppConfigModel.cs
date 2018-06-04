namespace QJ_FileCenter.Models
{
    public class AppConfigModel
    {
        public AppConfigModel()
        {
         
        }
        public string RootPath { get; set; }

        public int NancyPort { get; set; }

        public bool Https { get; set; }
        public string IP { get; set; }

    }
}
