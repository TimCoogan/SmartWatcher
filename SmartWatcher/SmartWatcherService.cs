#define DEBUG

using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using SmartWatcher.PlugIn;


namespace SmartWatcher.Services
{

    public partial class SmartWatcherService : ServiceBase, PlugIn.IPluginHost
    {

        #region Variables

        public static string SettingsFailureArchivePath = Properties.Settings.Default.FailureArchivePath;
        public static string SettingsSuccessArchivePath = Properties.Settings.Default.SuccessArchivePath;
        public static int SettingsReadTries = Properties.Settings.Default.ReadTries;
        public static string SettingsZipFileExtention = "zip";
        private readonly string _PluginFolder = AppDomain.CurrentDomain.BaseDirectory + @"Plugin\";

        public static List<IPlugin> lstPlugins = new List<IPlugin>();

        private List<FileSystemWatcher> _lstWatchers = new List<FileSystemWatcher>();

        static FileProcessor processor;
        private SmartTimer _smartTimer;
        private bool _enableTimer = Properties.Settings.Default.EnableTimer;
        private int _interval = Properties.Settings.Default.Interval;
        

        #endregion

        #region Methods

        private void LoadPlugIns()
        {
            ServiceLogger.LogInfo("LoadPlugIns() - Start");
            if(!Directory.Exists(_PluginFolder))
            {
                ServiceLogger.LogInfo("Directory not exist, creating directory, directory name: " + _PluginFolder);
                Directory.CreateDirectory(_PluginFolder);
            }


            string[] pluginFiles = Directory.GetFiles(_PluginFolder, "*.plug");
            foreach (string pluginFile in pluginFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(pluginFile);
                    
                    System.Type[] assemblyTypes = assembly.GetTypes();
                    foreach (System.Type type in assemblyTypes)
                    {
                        if (type.GetInterface("IPlugin") != null)
                        {
                            if (type.GetCustomAttributes(typeof(PlugDisplayNameAttribute),
                    false).Length != 1)
                                throw new PlugNotValidException(type,
                                  "PlugDisplayNameAttribute is not supported");
                            if (type.GetCustomAttributes(typeof(PlugDescriptionAttribute),
                    false).Length != 1)
                                throw new PlugNotValidException(type,
                                  "PlugDescriptionAttribute is not supported");
                            IPlugin plugin = (IPlugin) Activator.CreateInstance(type, new object[] {});
                            plugin.Host = this;
                            ServiceLogger.LogInfo("Add plugin, plugin name:" + plugin.Name);
                            lstPlugins.Add(plugin);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ServiceLogger.LogError("LoadPlugIns() - Error: " + ex.Message);
                }
            }
            ServiceLogger.LogInfo("LoadPlugIns() - Finish");
        }

        private void RegisterWatchers()
        {
            ServiceLogger.LogInfo("RegisterWatchers() - Start");
            foreach (IPlugin plugin in lstPlugins)
            {
                // Create Watching Directory if not exist
                if (!Directory.Exists(plugin.WatchPath))
                {
                    ServiceLogger.LogInfo("Directory not exist, creating watching directory, directory name: " + plugin.WatchPath);
                    Directory.CreateDirectory(plugin.WatchPath);
                }

                try
                {
                    FileSystemWatcher watcher = new FileSystemWatcher();
                    watcher.Path = plugin.WatchPath;
                    watcher.Filter = plugin.Filter;
                    watcher.NotifyFilter = plugin.NotifyFilter;
                    watcher.Changed += fswWatcher_Changed;
                    watcher.Created += fswWatcher_Created;
                    watcher.Deleted += fswWatcher_Deleted;
                    watcher.Renamed += fswWatcher_Renamed;
                    watcher.EnableRaisingEvents = true;
                    _lstWatchers.Add(watcher);
                    ServiceLogger.LogInfo("Watcher added, plugin name: " + plugin.Name);

                }
                catch (Exception ex)
                {
                    ServiceLogger.LogInfo("RegisterWatchers() - Error: " + ex.Message);
                }

                ServiceLogger.LogInfo("RegisterWatchers() - Finish");
            }
        }

        public bool Register(IPlugin plugin)
        {
            ServiceLogger.LogInfo("Register(IPlugin plugin) - Start");
            ServiceLogger.LogInfo("plugin name:" + plugin.Name);


            ServiceLogger.LogInfo("Register(IPlugin plugin) - Finish");
            return true;
        }

        

        #endregion

        #region Constructors

        public SmartWatcherService()
        {

            InitializeComponent();
            

            #if DEBUG
                //Launches and attaches a debugger to the process. Remove it for production
                System.Diagnostics.Debugger.Launch();
                ServiceLogger.LogDebug("Start Debugging.");
            #endif

            
                // intilaize files processor to maintains files queue 
                processor = new FileProcessor();

                LoadPlugIns();
                RegisterWatchers();


               _smartTimer = new SmartTimer(_interval);
               _smartTimer.EventSmartTimerTick += smartTimer_EventSmartTimerTick;
               if (_enableTimer)
               {
                   ServiceLogger.LogInfo("Start: SmartTimer");
                   _smartTimer.StartTimer();
               }


        }



        #endregion

        #region Events

         #region Service Events

        protected override void OnStart(string[] args)
        {
            ServiceLogger.LogInfo("SmartWatcher service starting.");
            try
            {
                // Create Failure Archiving Directory
                if (!Directory.Exists(SettingsFailureArchivePath))
                {
                    Directory.CreateDirectory(SettingsFailureArchivePath);
                }
                // Create Success Archiving Directory
                if (!Directory.Exists(SettingsSuccessArchivePath))
                {
                    Directory.CreateDirectory(SettingsSuccessArchivePath);
                }
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError(ex.Message + "/n" + ex.StackTrace);
                throw;
            }

        }

        protected override void OnStop()
        {
            ServiceLogger.LogInfo("SmartWatcher service stopping.");
            if (_smartTimer != null)
            {
                _smartTimer.StopTimer();
                _smartTimer = null;
            }
        }

        #endregion

         #region FileWatcher Events

        /* DEFINE WATCHER EVENTS... */
        /// <summary>
        /// Event occurs when the contents of a File or Directory are changed
        /// </summary>
        private void fswWatcher_Changed(object sender,
                        System.IO.FileSystemEventArgs e)
        {
            ServiceLogger.LogInfo("Event: File Changed, File Name: " + e.FullPath + " , Change Type: " + e.ChangeType);
            processor.QueueInput(e.FullPath, e.ChangeType);
        }

        /// <summary>
        /// Event occurs when the a File or Directory is created
        /// </summary>
        private void fswWatcher_Created(object sender,
                        System.IO.FileSystemEventArgs e)
        {
            ServiceLogger.LogInfo("Event: File Created, File Name: " + e.FullPath + " , Change Type: " + e.ChangeType);
            processor.QueueInput(e.FullPath, e.ChangeType);
        }
        /// <summary>
        /// Event occurs when the a File or Directory is deleted
        /// </summary>
        private void fswWatcher_Deleted(object sender,
                        System.IO.FileSystemEventArgs e)
        {
            ServiceLogger.LogInfo("Event: File Deleted, File Name: " + e.FullPath + " , Change Type: " + e.ChangeType);
            processor.QueueInput(e.FullPath, e.ChangeType);
        }
        /// <summary>
        /// Event occurs when the a File or Directory is renamed
        /// </summary>
        private void fswWatcher_Renamed(object sender,
                        System.IO.RenamedEventArgs e)
        {
            ServiceLogger.LogInfo("Event: File Renamed, File Name: " + e.FullPath + " , Change Type: " + e.ChangeType);
            processor.QueueInput(e.FullPath, e.ChangeType);
        }

        #endregion

        #region SmartTimer Events

        void smartTimer_EventSmartTimerTick(object sender, SmartTimer.SmartTimerEventArgs e)
        {
            foreach (IPlugin plugin in SmartWatcherService.lstPlugins)
            {
                if (plugin.NotifyTick)
                {
                    plugin.Tick();
                }   
            }
        }

        #endregion SmartTimer Events

        #endregion
    }
}
