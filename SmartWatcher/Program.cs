using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace SmartWatcher.Services
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            GlobalExceptionsHandler.AddHandler();

            bool _IsInstalled = false;
            bool serviceStarting = false; // Thanks to SMESSER's implementation V2.0
            string SERVICE_NAME = "SmartWatcher";

            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName.Equals(SERVICE_NAME))
                {
                    _IsInstalled = true;
                    if (service.Status == ServiceControllerStatus.StartPending)
                    {
                        // If the status is StartPending then the service was started via the SCM             
                        serviceStarting = true;
                    }
                    break;
                }
            }

            if (!serviceStarting)
            {
                if (_IsInstalled == true)
                {
                    // Thanks to PIEBALDconsult's Concern V2.0
                    DialogResult dr = new DialogResult();
                    dr = MessageBox.Show("Do you REALLY like to uninstall the " + SERVICE_NAME + "?", "Danger", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Yes)
                    {
                        SelfInstaller.UninstallMe();
                        MessageBox.Show("Successfully uninstalled the " + SERVICE_NAME, "Status",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    DialogResult dr = new DialogResult();
                    dr = MessageBox.Show("Do you REALLY like to install the " + SERVICE_NAME + "?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Yes)
                    {
                        SelfInstaller.InstallMe();
                        MessageBox.Show("Successfully installed the " + SERVICE_NAME, "Status",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                // Started from the SCM
                var ServicesToRun = new ServiceBase[]
                                                  {
                                                      new SmartWatcherService()
                                                  };
                ServiceBase.Run(ServicesToRun);
            }
        }




    }

    public static class SelfInstaller
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;
        public static bool InstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { _exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool UninstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", _exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
