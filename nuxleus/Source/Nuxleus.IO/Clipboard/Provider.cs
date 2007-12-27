using System;
using System.IO;
using Nuxleus.Utility.S3;

namespace Nuxleus.IO
{
    public partial class GlobalClip
    {

        public AWSAuthConnection Connect
        {
            get
            {
                return this._Connect;
            }
            set
            {

                this._Connect = value;
            }
        }
        private bool _Connected = false;
        private bool Connected
        {
            get
            {
                return this._Connected;
            }
            set
            {

                this._Connected = value;
            }
        }

        public string BaseHost
        {
            get
            {
                return this._BaseHost;
            }
            set
            {
                this._BaseHost = value;
            }
        }



        public string StorageBase
        {
            get
            {
                return this._StorageBase;
            }
            set
            {
                this._StorageBase = value;
            }
        }



        public string FilePrefix
        {
            get
            {
                return this._FilePrefix;
            }
            set
            {
                this._FilePrefix = value;
            }
        }


        public string SessionId
        {
            get
            {
                return this._SessionId;
            }
            set
            {
                this._SessionId = value;
            }
        }

        public string DateHash
        {
            get
            {
                return _DateHash;
            }
            set
            {
                this._DateHash = value;
            }
        }

        public string KeyPrefix
        {
            get
            {
                return this._KeyPrefix;
            }

            set
            {
                this._KeyPrefix = value;
            }
        }


        public AWSAuthConnection Clipboard()
        {
            if (Connected) return Connect;
            else
            {
                this.Init();
                this.Connected = true;
                return Connect;

            }
        }


    }
}
