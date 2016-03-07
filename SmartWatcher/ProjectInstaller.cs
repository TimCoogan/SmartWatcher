using System.ComponentModel;
using System.ServiceProcess;


namespace SmartWatcher.Services
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            var process = new ServiceProcessInstaller {Account = ServiceAccount.LocalService};
            var serviceAdmin = new ServiceInstaller
                                   {
                                       StartType = ServiceStartMode.Manual,
                                       ServiceName = "SmartWatcher",
                                       DisplayName = "Smart Watcher"
                                   };
            Installers.Add(process);
            Installers.Add(serviceAdmin);
        }
    }
}
