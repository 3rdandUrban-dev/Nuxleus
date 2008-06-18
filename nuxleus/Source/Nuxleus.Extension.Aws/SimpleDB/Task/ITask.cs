using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EeekSoft.Asynchronous;

namespace Nuxleus.Extension.AWS.SimpleDB {
    public interface ITask {
        Guid TaskID { get; }
        IRequest Request { get; }
        IResponse Response { get; set; }
        IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList);
    }
}
