using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVMF.SOA.Common;
using System.Net;
using Nuxleus.Asynchronous;
using System.IO;
using System.Xml.Linq;

namespace Nuxleus.Extension.AWS.SimpleDB {
    public class ExceptionHandlerScope : HandlerBase {

        WebException we;

        protected override void Call() {
            try {
                base.Call();
            } catch (WebException ex) {
                Console.WriteLine("!!! WebException handled. Message: '{0}'.", ex.Message);
            } catch (ArgumentNullException ex) {
                Console.WriteLine("!!! ArgumentNullException handled. Message: '{0}', ParamName: '{1}'.", ex.Message, ex.ParamName);
            } catch (ArgumentException ex) {
                Console.WriteLine("!!! ArgumentException handled. Message: '{0}', ParamName: '{1}'.", ex.Message, ex.ParamName);
            } catch (Exception ex) {
                Console.WriteLine("!!! Exception handled. Message: '{0}'.", ex.Message);
            } finally {
                //Something will occur to me eventually ;-)
            }
        }
    }
}
