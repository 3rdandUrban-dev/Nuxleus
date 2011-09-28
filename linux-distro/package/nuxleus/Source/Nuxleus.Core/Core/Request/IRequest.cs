using System;
using System.Collections.Generic;

namespace Nuxleus.Core
{
    public interface IRequest {
        List<KeyValuePair<string, string>> Headers { get; }
        Enum RequestType { get; }
        String RequestMessage { get; set; }
    }
}
