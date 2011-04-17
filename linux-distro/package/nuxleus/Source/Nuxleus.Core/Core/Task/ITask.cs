using System;
using System.Collections.Generic;
using Nuxleus.Asynchronous;
using System.Net;

namespace Nuxleus.Core
{
    public delegate void HttpWebResponseCallback(HttpWebResponse response);

    public interface ITask
    {
        Guid TaskID { get; }
        ITransaction Transaction { get; }
        HttpStatusCode StatusCode { get; set; }
        IEnumerable<IAsync> InvokeAsync();
        IResponse Invoke();
    }
}
