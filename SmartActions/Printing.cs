using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActions
{
    class Printing
    {

        public static bool PrintFile(string fileName, bool throwException = false)
        {
            bool result = false;

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Verb = "Print";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = true;
                process.Start();
                result = true;
            }
            catch (Exception)
            {
                if (throwException) throw;
            }

            return result;
        }

    }
}
