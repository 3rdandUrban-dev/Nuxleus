using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;

namespace Nuxleus.Process
{
    public class DarcsProcess : System.Diagnostics.Process
    {
        string _path;
        TextWriter _logWriter;
        bool _addQueueLock = false;
        bool _updateQueueLock = false;
        bool _deleteQueueLock = false;
        Queue _addQueue = new Queue();
        Queue _updateQueue = new Queue();
        Queue _deleteQueue = new Queue();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public DarcsProcess()
        {
            base.StartInfo.FileName = "darcs";
            base.EnableRaisingEvents = false;
        }
        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public DarcsProcess(string path, TextWriter logWriter)
        {
            _path = path;
            _logWriter = logWriter;
            this.StartInfo.FileName = "darcs";
            this.EnableRaisingEvents = false;
            
        }

        public TextWriter LogWriter { get { return _logWriter; } set { _logWriter = value; } }
        public string Folder { get { return _path; } set { _path = value; } }

        public void AddFileToDarcs(string filePath)
        {    
            if(_addQueue.Count == 0)
            {
                AddFile(filePath);
            }
            else if (_addQueueLock)
            {
                Enqueue(filePath);
            }
            else 
            {
                AddFile(_addQueue);
            }
        }
        
        private void AddFile(Queue queue)
        {
            _addQueueLock = true;
            ProcessQueue(queue);
            _addQueueLock = false;
        }
        
        private void AddFile(string filePath)
        {
            _addQueueLock = true;
            addFileTransaction(filePath);
            _addQueueLock = false;
        }
        
        private void Enqueue(string filePath)
        {
            _addQueue.Enqueue(filePath);
        }
        
        private void ProcessQueue(Queue queue)  
        {
            while(queue.Count >= 0)
              {
                 addFileTransaction((string)queue.Dequeue());
              }
        }
        
        private void addFileTransaction(string filePath) 
        {
            DateTime start = DateTime.Now;
            
            
            this.StartInfo.Arguments="add --case-ok " + filePath;
            lock(filePath)
            {
                this.Start();
                CommitFileToDarcs(filePath);
            }
            
            DateTime stop = DateTime.Now;
            long diff = stop.Subtract(start).Ticks;
            
            _logWriter.WriteLine("Start time in ticks: {0}", start.Ticks);
            _logWriter.WriteLine("Stop time in ticks: {0}", stop.Ticks);
            _logWriter.WriteLine("Total elapsed ticks: {0}", diff);
            _logWriter.WriteLine("{0} was committed to the repository in {1} ticks", filePath, diff);
        }
        
        public void CommitFileToDarcs(string fullPath)
        {
            this.StartInfo.Arguments = "record -a --skip-long-comment --patch-name=" + fullPath + ":" + Guid.NewGuid().ToString() + " " + fullPath;
            this.Start();
        }

	public void CommitFileToDarcs()
        {
            this.StartInfo.Arguments = "record -a --skip-long-comment --patch-name=" + Guid.NewGuid().ToString();
            this.Start();
        }

        public void MoveFileInDarcs(string oldPath, string newPath)
        {
            RemoveFileFromDarcs(oldPath);
            AddFileToDarcs(newPath);
        }
        
        public void RemoveFileFromDarcs(string fullPath)
        {
            this.StartInfo.Arguments = "remove " + fullPath;
            this.Start();
            CommitFileToDarcs(fullPath);
        }
        
        public void KillProcess()
        { 
            this.Kill(); 
        }
    }
}
