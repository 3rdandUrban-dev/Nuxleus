using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nuxleus.Extension.Aws.SimpleDb {
    public interface IRequest {
        KeyValuePair<string, string>[] Headers { get; }
        RequestType RequestType { get; }
        String RequestMessage { get; set; }
    }
}
