using System.IO;
using SmartWatcher.PlugIn;

// Sample Plugin
namespace SamplePlugin
{
    [SmartWatcher.PlugIn.PlugDisplayName("SamplePlugin")]
    [SmartWatcher.PlugIn.PlugDescription("This plug is a sample")]
    public class SamplePlugin : IPlugin
    {
        public IPluginHost Host
        {
            get { return _Host; }
            set
            {
                _Host = value;
                _Host.Register(this);
            }
        }       
        private string _strName = "SmaplePlugin";
        private string _strWatchPath = Properties.Settings.Default.SmaplePlugin_WatchPath;
        // Only watch xml files.
        private string _strFilter = "*.xml";
        /* Watch for changes in LastAccess and LastWrite times, and
           the renaming of files or directories. */
        private NotifyFilters _notifyFilters = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        private bool _blnCatchAllFiles = false;
        private IPluginHost _Host;
        private string _strZipPassword = "ADM";
        private string _strFilePrefix = "ADM";
        private bool _strArchiveFiles = true;
        private bool _strDeleteFiles = true;
        private bool _blnNotifyTick = false;

        public string Name
        { get { return _strName; } }

        public string WatchPath
        { get { return _strWatchPath; } }

        public string Filter
        { get { return _strFilter; } }

        public NotifyFilters NotifyFilter
        { get { return _notifyFilters; } }

        public bool CatchAllFiles
        { get { return _blnCatchAllFiles; } }

        public string ZipPassword
        { get { return _strZipPassword; } }

        public string FilePrefix
        { get { return _strFilePrefix; } }

        public bool ArchiveFiles
        { get { return _strArchiveFiles; } }

        public bool DeleteFiles
        { get { return _strDeleteFiles; } }

        public bool NotifyTick
        { get { return _blnNotifyTick; } }
        
        public bool FileCreated(string fileName)
        {
            bool result = true;

            // Your Code Here

            return result;
        }

        public bool FileChanged(string fileName)
        {
            bool result = true;

            // Your Code Here

            return result;
        }

        public bool FileDeleted(string fileName)
        {
            bool result = true;

            // Your Code Here

            return result;
        }

        public bool FileRenamed(string fileName)
        {
            bool result = true;

            // Your Code Here

            return result;
        }

        public bool Tick()
        {
            bool result = true;
            
            // Your Code Here

            return result;
        }


    }
}
