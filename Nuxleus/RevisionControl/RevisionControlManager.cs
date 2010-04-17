using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;

namespace Nuxleus.RevisionControl
{
	public enum OperationType { ADD, UPDATE, DELETE }
	
    public class RevisionControlManager
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
        public RevisionControlManager()
        {
            ///TODO:
        }
        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public RevisionControlManager(string path, TextWriter logWriter)
        {
            _path = path;
            _logWriter = logWriter;
			///TODO:
        }

        public TextWriter LogWriter { get { return _logWriter; } set { _logWriter = value; } }
        public string Folder { get { return _path; } set { _path = value; } }

        public void AddFile(string filePath)
        {    
            if(_addQueue.Count == 0)
			/// If the _addQueue is empty, process the file update.
            {
                addFile(filePath);
            }
			else if (_addQueueLock)
			/// If a file is being added or the _addQueue is currently being processed the _addQueueLock will be set 
			/// to true.  As such, add the file to the queue for processing.
            {
                _addQueue.Enqueue(filePath);
            }
            else 
			/// If it's not locked, add it to the queue and then process that queue.
            {
				_addQueue.Enqueue(filePath);
                addFile(_addQueue);
            }
        }

		public void UpdateFile(string filePath)
        {
            if(_updateQueue.Count == 0)
            /// If the _updateQueue is empty, process the file update.
			{
                updateFile(filePath);
            }
            else if (_updateQueueLock)
            /// If a file is being added or the _updateQueue is currently being processed the _updateQueueLock will be set 
			/// to true.  As such, add the file to the queue for processing.
			{
                _updateQueue.Enqueue(filePath);
            }
            else 
			/// If it's not locked, add it to the queue and then process that queue.
            {
				_updateQueue.Enqueue(filePath);
                updateFile(_addQueue);
            }
        }
		
        public void MoveFile(string oldPath, string newPath)
        {
            DeleteFile(oldPath);
            AddFile(newPath);
        }

		public void DeleteFile(string filePath)
        {
            if(_deleteQueue.Count == 0) 
			/// If the deleteQueue is empty, process the file deletion
            {
                updateFile(filePath);
            }
            else if (_deleteQueueLock) 
			/// If a file is being added or the _deleteQueue is currently being processed the _deleteQueueLock will be set 
			/// to true.  As such, add the file to the queue for processing.
            {
                _deleteQueue.Enqueue(filePath);
            }
            else 
			/// If it's not locked, add it to the queue and then process that queue.
            {
				_deleteQueue.Enqueue(filePath);
                updateFile(_deleteQueue);
            }
        }

        private void addFile(Queue queue)
        {
            _addQueueLock = true;
            ProcessQueue(queue, OperationType.ADD);
            _addQueueLock = false;
        }
        
        private void addFile(string filePath) 
        {
            DateTime start = DateTime.Now;
			
            lock(filePath)
            {
				_addQueueLock = true;
                ///TODO:
				_addQueueLock = false;
            }
            
            DateTime stop = DateTime.Now;
            long diff = stop.Subtract(start).Ticks;
            
            _logWriter.WriteLine("Start time in ticks: {0}", start.Ticks);
            _logWriter.WriteLine("Stop time in ticks: {0}", stop.Ticks);
            _logWriter.WriteLine("Total elapsed ticks: {0}", diff);
            _logWriter.WriteLine("{0} was committed to the repository in {1} ticks", filePath, diff);
        }
		
		private void updateFile(string fullPath)
	    {
			///TODO:
		}
		
		private void updateFile(Queue queue)
	    {
			///TODO:
		}
		
		private void deleteFile(string fullPath)
	    {
			///TODO:
		}
		
		private void deleteFile(Queue queue)
	    {
			///TODO:
		}
		
		private void ProcessQueue(Queue queue, OperationType operation)  
        {
            while(queue.Count >= 0)
            {
				switch(operation)
				{
					case OperationType.ADD:
					{
						addFile((string)queue.Dequeue());
						break;
					}
					
					case OperationType.UPDATE:
					{
						updateFile((string)queue.Dequeue());
						break;
					}
					
					case OperationType.DELETE:
					{
						deleteFile((string)queue.Dequeue());
						break;
					}
					
					default:
					{
						break;
					}
				}
        	}
        }
    }
}
