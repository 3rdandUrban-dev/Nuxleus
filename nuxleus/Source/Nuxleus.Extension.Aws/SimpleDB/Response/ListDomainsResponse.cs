using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuxleus.Extension.Aws.SimpleDb {

    public struct ListDomainsResponse : IResponse {
        public KeyValuePair<string, string>[] Headers { get; set; }
        public string Response { get; set; }
    }
}
