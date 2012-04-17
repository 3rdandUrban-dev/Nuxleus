using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Text;
using Nuxleus.Service;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.IO;
using Nuxleus.Service.FileQueue;

namespace Nuxleus.Web.HttpApplication {

    public class FileMessageQueue_Global : System.Web.HttpApplication {

        FileQueueWatcher m_watcher;
        FileStream m_fs;
        StreamWriter m_writer;
        Dictionary<string, string> m_statusQueue;

        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        protected void Application_Start ( object sender, EventArgs e ) {

            using (XmlReader configReader = XmlReader.Create(HttpContext.Current.Server.MapPath("~/App_Data/aws.config"))) {
                while (configReader.Read()) {
                    if (configReader.IsStartElement()) {
                        switch (configReader.Name) {
                            case "s3-access-key": {
                                    Environment.SetEnvironmentVariable("S3_ACCESS_KEY", configReader.ReadString());
                                    break;
                                }
                            case "s3-secret-key": {
                                    Environment.SetEnvironmentVariable("S3_SECRET_KEY", configReader.ReadString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }

            m_statusQueue = new Dictionary<string, string>();
            Application["as_statusQueue"] = m_statusQueue;

            m_fs = new FileStream(HttpContext.Current.Server.MapPath("~/App_Data/Queue.log"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            m_writer = new StreamWriter(m_fs, System.Text.Encoding.Default);

            m_watcher = new FileQueueWatcher(HttpContext.Current.Server.MapPath("~/App_Data/process-queue"), "", m_writer);
            m_watcher.Watch(false);
        }

        protected void Application_BeginRequest ( object sender, EventArgs e ) {
            Application["statusQueue"] = Application["as_statusQueue"];
        }

        protected void Application_EndRequest ( object sender, EventArgs e ) {

        }

        protected void Application_AuthenticateRequest ( object sender, EventArgs e ) {

        }

        protected void Application_Error ( object sender, EventArgs e ) {

        }

        protected void Application_End ( object sender, EventArgs e ) {
            m_writer.Close();
            m_fs.Close();
            m_writer.Dispose();
            m_fs.Dispose();
            m_watcher.Dispose();
        }
    }
}
