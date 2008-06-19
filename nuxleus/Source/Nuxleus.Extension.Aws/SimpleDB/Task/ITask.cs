using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nuxleus.Asynchronous;

namespace Nuxleus.Extension.AWS.SimpleDB {
    public interface ITask {
        Guid TaskID { get; }
        IRequest Request { get; }
        IResponse Response { get; }
        IEnumerable<IAsync> Invoke<T>(Dictionary<IRequest, T> responseList);
    }
}
