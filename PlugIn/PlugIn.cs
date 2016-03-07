using System.IO;

namespace SmartWatcher.PlugIn
{
    /// <summary>
    /// File watcher plugin interface
    /// </summary>
    public interface IPlugin
    {
        string Name { get;}
        string WatchPath { get;}
        string Filter { get;}
        NotifyFilters NotifyFilter { get;}
        bool CatchAllFiles { get;}
        string ZipPassword { get;}
        string FilePrefix { get;}
        IPluginHost Host { get; set; }
        bool ArchiveFiles { get;}
        bool DeleteFiles { get;}
        bool NotifyTick { get; }

        /// <summary>
        /// Event occurs when the a File or Directory is created
        /// </summary>
        /// <param name="fileName">The file full path name</param>
        /// <returns></returns>
        bool FileCreated(string fileName);
        /// <summary>
        /// Event occurs when the contents of a File or Directory are changed
        /// </summary>
        /// <param name="fileName">The file full path name</param>
        /// <returns></returns>
        bool FileChanged(string fileName);
        /// <summary>
        /// Event occurs when the a File or Directory is deleted
        /// </summary>
        /// <param name="fileName">The file full path name</param>
        /// <returns></returns>
        bool FileDeleted(string fileName);
        /// <summary>
        /// Event occurs when the a File or Directory is renamed
        /// </summary>
        /// <param name="fileName">The file full path name</param>
        /// <returns></returns>
        bool FileRenamed(string fileName);

        /// <summary>
        /// Event occurs when the Smart timer interval has elapsed and the timer is enabled.
        /// </summary>
        /// <param name="fileName">The file full path name</param>
        /// <returns></returns>
        bool Tick();
        
    }


    /// <summary>
    /// An interfance represent the plugin host assembly
    /// </summary>
    public interface IPluginHost
    {
        bool Register(IPlugin ipi);
    }
}