using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nuxleus.Extension.AWS.SimpleDB {
    interface IRequest {
        KeyValuePair<string, string>[] Headers { get; set; }
        RequestType RequestType { get; set; }
        //T Request { get; set; }
    }
}
