using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Permissions;

namespace Nuxleus.Process
{
    public class SVNProcess : System.Diagnostics.Process
    {
        string _path;
        TextWriter _logWriter;
        SVNProcess _proc;
        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public SVNProcess()
        {
            base.StartInfo.FileName = "svn";
            
        }
        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public SVNProcess(string path, TextWriter logWriter)
        {
            _path = path;
            _logWriter = logWriter;
            _proc = new SVNProcess();
            _proc.StartInfo.FileName = "svn";
            
        }

        public TextWriter LogWriter { get { return _logWriter; } set { _logWriter = value; } }
        public string Folder { get { return _path; } set { _path = value; } }

        public void AddFileToSVN(string fullPath)
        {
            this.StartInfo.Arguments="add " + fullPath;
            this.Start();
            this.WaitForExit();
            CommitFileToSVN(fullPath);
        }
        
        public void CommitFileToSVN(string fullPath)
        {
            this.StartInfo.Arguments="ci " + fullPath + " -m 'addition of '" + fullPath;
            this.Start();
            this.WaitForExit();
        }
        
        public void MoveFileInSVN(string oldPath, string newPath)
        {
            AddFileToSVN(newPath);
            RemoveFileFromSVN(oldPath);
        }
        
        public void RemoveFileFromSVN(string fullPath)
        {
            this.StartInfo.Arguments="rm " + fullPath;
            this.Start();
            this.WaitForExit();
            CommitFileToSVN(fullPath);
        }
    }
}