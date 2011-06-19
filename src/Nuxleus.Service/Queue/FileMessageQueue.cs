using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Service.Drawing;
using System.IO;
using System.Security.Permissions;

namespace Nuxleus.Service.FileQueue {

    public class FileQueueWatcher : FileSystemWatcher {
        string m_path;
        TextWriter m_logWriter;
        NotifyFilters m_notifyFilters;
        string m_filter;
        static NuxleusImageResizeProcessManager m_imageResizeProcessManager;


        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public FileQueueWatcher ( string path, string filter, TextWriter logWriter ) {
            m_path = path;
            m_logWriter = logWriter;
            m_filter = filter;
            m_imageResizeProcessManager = new NuxleusImageResizeProcessManager(path, logWriter);
            m_notifyFilters =
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.FileName;

            this.Path = m_path;
            this.NotifyFilter = m_notifyFilters;
            this.Filter = m_filter;

        }

        public TextWriter LogWriter { get { return m_logWriter; } set { m_logWriter = value; } }
        public string Folder { get { return m_path; } set { m_path = value; } }
        public NotifyFilters NotifyFilters { get { return m_notifyFilters; } set { m_notifyFilters = value; } }
        public string FileFilter { get { return m_filter; } set { m_filter = value; } }

        public void Watch ( bool watchSubDirectories ) {
            this.Created += new FileSystemEventHandler(OnCreated);
            this.Changed += new FileSystemEventHandler(OnCreated);
            this.EnableRaisingEvents = true;
        }

        private static void OnCreated ( object source, FileSystemEventArgs e ) {
            FileQueueWatcher watcher = (FileQueueWatcher)source;
            watcher.LogWriter.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            switch (e.ChangeType) {
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.Changed: {
                        m_imageResizeProcessManager.AddFile(e.FullPath);
                        watcher.LogWriter.Write("File: {0} {1} at Path: {2} \n", e.Name, e.ChangeType, e.FullPath);
                        break;
                    }
                default: {
                        // This should never be reached.
                        watcher.LogWriter.Write("This should never be reached. \n");
                        watcher.LogWriter.Write("Event Information: File: {0} {1} at Path: {2} \n", e.Name, e.ChangeType, e.FullPath);
                        break;
                    }

            }
        }
    }
}
