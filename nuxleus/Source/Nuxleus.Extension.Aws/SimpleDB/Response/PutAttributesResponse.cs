using System;
using System.Collections.Generic;

namespace Nuxleus.Extension.Aws.SimpleDb {

    public struct PutAttributesResponse : IResponse {
        public KeyValuePair<string, string>[] Headers { get; set; }
        public string Response { get; set; }
    }
}
