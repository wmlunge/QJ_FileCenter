namespace QJ_FileCenter
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.QJ_FileCenterService = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // QJ_FileCenterService
            // 
            this.QJ_FileCenterService.DelayedAutoStart = true;
            this.QJ_FileCenterService.Description = "QJ_FileCenter后台运行服务，持续提供API函数功能，此服务启动后，关联程序才能有效运行。";
            this.QJ_FileCenterService.DisplayName = "QJ_FileCenter";
            this.QJ_FileCenterService.ServiceName = "QJ_FileCenterService";
            this.QJ_FileCenterService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.QJ_FileCenterService.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.QJ_FileCenterService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller QJ_FileCenterService;
    }
}