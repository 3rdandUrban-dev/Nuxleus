using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nuxleus.Extension.AWS.SimpleDB {
    interface IResponse<T> {
        KeyValuePair<string,string>[] Headers { get; set;}
        T Response { get; set; }
    }
}
