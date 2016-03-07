using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SmartWatcher.Services;

namespace SmartWatcher
{
    class GlobalExceptionsHandler
    {
        public static void AddHandler()
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ConsoleThreadException);
            Application.ThreadException += new ThreadExceptionEventHandler(UIThreadException);
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                string errorMSG = "An application error occured. Please contact the administrator " +
                  "with the following information:\n\n";
                errorMSG += "Error Message: " + e.Exception.Message + "\n\nStack Trace:\n" + e.Exception.StackTrace;
                //result = MadaaMessageBox.ShowError(e.Exception.Message, errorMSG, "Windows Forms Error", MessageBoxButtons.AbortRetryIgnore);
                ServiceLogger.LogError("UIThreadException(), " + errorMSG);
            }
            catch
            {
                try
                {
                    //XtraMessageBox.Show("Fata windows forms error", "Fatal Windows Forms Error",
                    //                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                   // MadaaMessageBox.ShowError("Fata windows forms error", "Fata windows forms error");
                    ServiceLogger.LogError("UIThreadException(), SmartWatcher application encountered a Fatal Error."); 

                }
                finally
                {
                    Application.Exit();
                }
            }

            //Exit the program when the user slicks Abort.
            //if (result == DialogResult.Abort)
            //{
            //    Application.Exit();
            //}

        }

        private static void ConsoleThreadException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {

                Exception ex = e.ExceptionObject as Exception;
                string errorMSG = "An application error occured. Please contact the administrator " +
                  "with the following information:\n\n";
                errorMSG += "Error Message: " + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
                //result = XtraMessageBox.Show(errorMSG, "Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                //result = MadaaMessageBox.ShowError(ex.Message, errorMSG, "Windows Forms Error", MessageBoxButtons.AbortRetryIgnore);
                ServiceLogger.LogError("ConsoleThreadException, " + errorMSG);
            }
            catch
            {
                try
                {
                    //MadaaMessageBox.ShowError("Fata windows forms error", "Fata windows forms error");
                    ServiceLogger.LogError("UIThreadException(), SmartWatcher application encountered a Fatal Error."); 

                }
                finally
                {
                    Environment.Exit(-1);
                }
            }

            //Exit the program when the user slicks Abort.
            // if (result == DialogResult.Abort)
            {
                Environment.Exit(-1);
            }
        }
    }
}
