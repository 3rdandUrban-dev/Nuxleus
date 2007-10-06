using System;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Specialized;
using Nuxleus.Configuration;
using Nuxleus.Utility.S3;

namespace Nuxleus.IO
{

    public partial class GlobalClip
    {

        public bool Init()
        {
            /// This is a hack. FIXME FIRST.
            try
            {
                if (InitMode(SetMode(this.AppMode)))
                {

                    try
                    {
                        this.Connect = new AWSAuthConnection(GC_PUBLIC_KEY, GC_PRIVATE_KEY);
                        this.Connected = true;
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }


        public int SetMode(int mode)
        {
            return (this.AppMode = mode);
        }

        private bool InitMode(int mode)
        {
            // 'bout as agile as a turtle...  but it will work for now.
            switch (mode)
            {
                case 0:
                    return AspNetMode();
                case 1:
                    return ConsoleMode();
                case 2:
                    return WinAppMode();
                case 3:
                    return DynamicMode();
                default:
                    // NOTE: Given that we preset the value of _Mode to ASPNET (see: GlobalClip_SessionVariables.cs), 
                    // this should never be reached unless the value of int is set to anything other than 0, 1, 2, or 3
                    // As such, we'll throw an exception alerting the powers that be that they've done something 
                    // very, very bad.
                    throw new Exception(rm.GetString("bad_hacker"));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool AspNetMode()
        {
            // This is an UGLY, crumbly, break at the first sign of anything human hack! 
            // FIXME BEFORE FIRST!!!!!
            try
            {
                NameValueCollection appSet_Private_Public_Keys = WebConfigurationManager.AppSettings;
                this.GC_PUBLIC_KEY = appSet_Private_Public_Keys[1];
                this.GC_PRIVATE_KEY = appSet_Private_Public_Keys[0];
                InitSessionVarDefaults();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ConsoleMode()
        {
            try
            {
                this.GC_PUBLIC_KEY = Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY_ID");
                this.GC_PRIVATE_KEY = Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY_ID");
                InitSessionVarDefaults();
                return true;
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool DynamicMode()
        {
            throw new Exception(rm.GetString("TODO"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool WinAppMode()
        {
            throw new Exception(rm.GetString("TODO"));
        }

        private void InitSessionVarDefaults()
        {
            if (this.FilePrefix == null) this.FilePrefix = "_anonymous_";
            if (this.SessionId == null) this.SessionId = "_default_";
            if (this.DateHash == null) this.DateHash = DateTime.Now.Date.ToShortDateString().GetHashCode().ToString(provider);
            if (this.KeyPrefix == null) this.KeyPrefix = this.FilePrefix + this.SessionId + this.DateHash;
        }

    }
}
