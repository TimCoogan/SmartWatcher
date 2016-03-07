using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActions
{
    public class IO
    {
        #region Files

        public static bool DeleteFile(string fileName, bool throwException = false)
        {
            bool result = false;
            try
            {
                System.IO.File.Delete(fileName);
                result = true;
            }
            catch (Exception)
            {
                if (throwException) throw;
                
            }
            return result;
        }

        #endregion

    }
}
