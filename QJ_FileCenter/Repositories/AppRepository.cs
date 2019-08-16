using QJ_FileCenter;
using System.IO;
using System.Xml.Linq;

namespace QJ_FileCenter.Repositories
{
    public class AppRepository
    {
        private string _configPath;
        public AppRepository()
        {
            AppConfigModel = new Models.AppConfigModel();

            _configPath = PathUtil.GetAppConfigPath();
            ReadFile();
        }

        public Models.AppConfigModel AppConfigModel { get; set; }

        public void WriteFile()
        {
            XElement xElement = new XElement("RESTFinder",
                new XElement("RootPath", AppConfigModel.RootPath),
                new XElement("NancyPort", AppConfigModel.NancyPort),
                new XElement("Https", AppConfigModel.Https)
                );

            xElement.Save(_configPath);
        }

        public void ReadFile()
        {
            if (!File.Exists(_configPath))
            {
                WriteFile();
            }

            var xElement = XElement.Load(_configPath);
            var element = xElement.Element("RootPath");
            if (element != null)
            {
                AppConfigModel.RootPath = element.Value;
            }

            element = xElement.Element("NancyPort");
            if (element != null)
            {
                AppConfigModel.NancyPort = int.Parse(string.IsNullOrEmpty(element.Value) ? "9100" : element.Value);
            }

            element = xElement.Element("Https");
            if (element != null)
            {
                AppConfigModel.Https = element.Value.ToUpper() == "TRUE";
            }

            element = xElement.Element("IP");
            if (element != null)
            {
                AppConfigModel.IP = string.IsNullOrEmpty(element.Value) ? "localhost" : element.Value;
            }
        }
    }
}
