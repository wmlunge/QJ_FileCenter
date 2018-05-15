using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace QJ_FileCenter
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            using (var sc = new ServiceController(QJ_FileCenterService.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
