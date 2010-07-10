using System;
using System.Collections.Generic;
using System.Text;

namespace Xameleon.Transform {

  public class BaseXsltContext {
    Uri _BaseXsltUri;
    String _UriHash;
    String _Name;

    public BaseXsltContext(Uri baseXsltUri, String uriHash, String name) {
      _BaseXsltUri = baseXsltUri;
      _UriHash = uriHash;
      _Name = name;
    }

    public Uri BaseXsltUri {
      get { return _BaseXsltUri; }
      set { this._BaseXsltUri = value; }
    }
    public String UriHash {
      get { return _UriHash; }
      set { this._UriHash = value; }
    }
    public String Name {
      get { return _Name; }
      set { this._Name = value; }
    }

  }
}
