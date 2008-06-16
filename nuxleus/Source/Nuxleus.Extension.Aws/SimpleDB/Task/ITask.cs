using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EeekSoft.Asynchronous;

namespace Nuxleus.Extension.AWS.SimpleDB {
    interface ITask {
        //Guid TaskID { get; set; }
        //IRequest Request { get; set; }
        //IResponse<T> Response { get; set; }
        RequestType RequestType { get; }
        XElement[] GetXMLBody { get; }
        IEnumerable<IAsync> Invoke<T>(Dictionary<XElement, T> responseList);
    }
}
