using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using SmartWatcher.PlugIn;
using ThreadState = System.Threading.ThreadState;
using System.Text.RegularExpressions;

namespace SmartWatcher.Services
{
    /// <summary>
    /// FileProcessor is the class which maintains a queue of files to be processed and controls a worker thread which processes each file in the queue.
    /// </summary>
    class FileProcessor
    {

        #region Variables

            private Queue<string> workQueue;
            private Thread workerThread;
            private EventWaitHandle waitHandle;

        #endregion

        public FileProcessor() 
        {
            workQueue = new Queue<string>();
            waitHandle = new AutoResetEvent(true);
           
        }

        public void QueueInput(string filepath, WatcherChangeTypes changeType)
        {
            workQueue.Enqueue(changeType.ToString() + "|" + filepath);

            // Initialize and start thread when first file is added
            if (workerThread == null) 
            {
                workerThread = new Thread(new ThreadStart(Work));
                workerThread.Start();
            }

            // If thread is waiting then start it
            else if (workerThread.ThreadState == ThreadState.WaitSleepJoin) 
            {
                waitHandle.Set();
            }
        }

        private void Work()
        {
            while (true)
            {
                string filepath = RetrieveFile();

                if (filepath != null)
                    ProcessFile(filepath);
                else
                    // If no files left to process then wait
                    waitHandle.WaitOne(); 
            }
        }

        private string RetrieveFile()
        {
            if (workQueue.Count > 0)
                return workQueue.Dequeue();
            else
                return null;
        }

        private void ProcessFile(string fileInfo)
        {
            ServiceLogger.LogInfo("ProcessFile() - Start");
            string[] file = fileInfo.Split('|');
            WatcherChangeTypes changeType;
            WatcherChangeTypes.TryParse(file[0], true, out changeType);
            string filepath = file[1];
            

            if (!File.Exists(filepath))
            {
                ServiceLogger.LogInfo("File is not exist, File name: " + filepath);
                return;
            }

            // Check if file is accessble
            if (!WaitReady(filepath, SmartWatcherService.SettingsReadTries))
            {
                ServiceLogger.LogInfo("File can be opened with write permission, File name: " + filepath);
                return;
            }

            foreach (IPlugin plugin in SmartWatcherService.lstPlugins)
            {
                string strFiletr = plugin.Filter.Replace("*", @"\\*");
                Match match = Regex.Match(Path.GetFileName(filepath), strFiletr);
                if (match.Success == true)
                {
                    Func<string, bool> pluginFunc = null;
                    switch (changeType)
                    {
                            case WatcherChangeTypes.Created:
                            {
                                pluginFunc = plugin.FileCreated;
                                break;
                            }
                            case WatcherChangeTypes.Changed:
                            {
                                pluginFunc = plugin.FileChanged;
                                break;
                            }
                            case WatcherChangeTypes.Renamed:
                            {
                                pluginFunc = plugin.FileRenamed;
                                break;
                            }
                            case WatcherChangeTypes.Deleted:
                            {
                                pluginFunc = plugin.FileDeleted;
                                break;
                            }
                    }
                    ServiceLogger.LogInfo("ProcessPluginAction, Plugin Name: " + plugin.Name);
                    ProcessPluginAction(pluginFunc, filepath, plugin.ArchiveFiles, plugin.DeleteFiles, plugin.FilePrefix,
                                        plugin.ZipPassword);
                }
            }
            ServiceLogger.LogInfo("ProcessFile() - Finish");
        }

        /// <summary>
        /// Waits until a file can be opened with write permission
        /// </summary>
        private static bool WaitReady(string fileName, int numberOfTries)
        {
            // file not ready
            bool result = false;
            int intCounter = 0;
            while (intCounter <= numberOfTries)
            {
                try
                {
                    using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            Trace.WriteLine(String.Format("Output file {0} ready.", fileName));
                            // file is ready
                            result = true;
                            break;
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    ServiceLogger.LogError("WaitReady - Error:" + String.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (IOException ex)
                {
                    ServiceLogger.LogError("WaitReady - Error:" + String.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (UnauthorizedAccessException ex)
                {
                    ServiceLogger.LogError("WaitReady - Error:" + String.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                Thread.Sleep(500);
                intCounter += 1;
            }
            return result;
        }

        private void ProcessPluginAction(Func<string, bool> pluginMethod, string fileFullName, bool archiveFiles, bool deleteFiles, string filePrefix, string zipPassword)
        {
            ServiceLogger.LogInfo("ProcessPluginAction() - Start");
            string strArchivePath = SmartWatcherService.SettingsFailureArchivePath;
            try
            {
                pluginMethod(fileFullName);
                strArchivePath = SmartWatcherService.SettingsSuccessArchivePath;

            }
            catch (Exception ex)
            {
                //ToDo: add exception handeling   
                ServiceLogger.LogError("() - Error: " + ex.Message);
            }
            finally
            {
                if (archiveFiles == true)
                {
                    string strArchiveFileName = filePrefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssss") + "_" +
                                                Path.GetFileName(fileFullName) + "." + SmartWatcherService.SettingsZipFileExtention;
                    ZipUtil.ZipFile(fileFullName, strArchivePath + strArchiveFileName, zipPassword);
                }
                if (deleteFiles == true)
                {
                    if (File.Exists(fileFullName))
                    {
                        // Check if file is accessble
                        if (FileProcessor.WaitReady(fileFullName, SmartWatcherService.SettingsReadTries) == true)
                        {
                            try
                            {
                                File.Delete(fileFullName);
                            }
                            catch (Exception ex)
                            {
                                //ToDo: add exception handeling   
                                ServiceLogger.LogError("ProcessPluginAction() - Failed to delete file, file name: " + fileFullName + " Error Message: " + ex.Message);
                            }
                            
                        }
                    }
                }
            }

            ServiceLogger.LogInfo("ProcessPluginAction() - Finish");

        }
    }
}
