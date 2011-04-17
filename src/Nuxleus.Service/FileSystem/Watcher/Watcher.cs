using System;
using System.Collections.Generic;
using Nuxleus.Process;
using System.Text;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Security.Permissions;

namespace Nuxleus.Service.FileSystem
{
    public class Watcher : FileSystemWatcher
    {
        string _path;
        TextWriter _logWriter;
        NotifyFilters _notifyFilters;
        string _filter;
        DarcsProcess _darcsProc;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public Watcher(string path, string filter, TextWriter logWriter)
        {
            _path = path;
            _logWriter = logWriter;
            _filter = filter;
            _notifyFilters =
                NotifyFilters.LastAccess    |
                NotifyFilters.LastWrite     |
                NotifyFilters.FileName      |
                NotifyFilters.DirectoryName;
                
            _darcsProc = new DarcsProcess(path, logWriter);
            this.Path = _path;
            this.NotifyFilter = _notifyFilters;
            this.Filter = _filter;

        }

        public TextWriter LogWriter { get { return _logWriter; } set { _logWriter = value; } }
        public string Folder { get { return _path; } set { _path = value; } }
        public NotifyFilters NotifyFilters { get { return _notifyFilters; } set { _notifyFilters = value; } }
        public string FileFilter { get { return _filter; } set { _filter = value; } }

        public void Watch(bool watchSubDirectories)
        {
            this.Changed += new FileSystemEventHandler(OnChanged);
            this.Created += new FileSystemEventHandler(OnChanged);
            this.Deleted += new FileSystemEventHandler(OnChanged);
            this.Renamed += new RenamedEventHandler(OnRenamed);
            this.IncludeSubdirectories = watchSubDirectories;
            
            this.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Watcher watcher = (Watcher)source;
            DarcsProcess proc = watcher._darcsProc;
            watcher.LogWriter.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

	    switch(e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                {
                    //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost:8080/service/atom/build-atom-entry/");
        	        //req.Headers.Add("Slug", e.Name);
                     
        	        //HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        	        //resp.Close();
                       
        	        proc.AddFileToDarcs(e.FullPath);
		            watcher.LogWriter.Write("File: {0} {1} at Path: {2} \n", e.Name, e.ChangeType, e.FullPath);
        	        break;
                }
                case WatcherChangeTypes.Changed:
                {
                    proc.CommitFileToDarcs(e.FullPath);
	                watcher.LogWriter.Write("File: {0} {1} at Path: {2} \n", e.Name, e.ChangeType, e.FullPath);
                    break;
                }
                case WatcherChangeTypes.Deleted:
                {
                    proc.RemoveFileFromDarcs(e.FullPath);
	                watcher.LogWriter.Write("File: {0} {1} at Path: {2} \n", e.Name, e.ChangeType, e.FullPath);
                    break;
                }
                default:
                {
                    watcher.LogWriter.Write("No ChangeType was caught. \n");
                    watcher.LogWriter.Write("Event Information: File: {0} {1} at Path: {2} \n", e.Name, e.ChangeType, e.FullPath);
                    break;
                }

            }
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Watcher watcher = (Watcher)source;
            DarcsProcess proc = watcher._darcsProc;
            watcher.LogWriter.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            proc.MoveFileInDarcs(e.OldFullPath, e.FullPath);
        }
    }
}
