using System;
using System.Collections;
using Saxon.Api;

namespace Nuxleus.ResultDocumentHandler {

    ///<summary>
    ///</summary>
    public class S3ResultDocumentHandler : IResultDocumentHandler {

        private Hashtable results;

        ///<summary>
        ///</summary>
        public S3ResultDocumentHandler() : this(null)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="table"></param>
        public S3ResultDocumentHandler(Hashtable table) {
            if (table != null) results = table;
        }

        public XmlDestination HandleResultDocument(string href, Uri baseUri) {
            DomDestination destination = new DomDestination();
            results[href] = destination;
            return destination;
        }
    }
}
