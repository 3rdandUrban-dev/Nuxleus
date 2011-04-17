using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Transform {

  public struct BaseXsltContext {
    Uri m_BaseXsltUri;
    String m_UriHash;
    String m_name;

    public BaseXsltContext(Uri baseXsltUri, String uriHash, String name) {
      m_BaseXsltUri = baseXsltUri;
      m_UriHash = uriHash;
      m_name = name;
    }

    public Uri BaseXsltUri {
      get { return m_BaseXsltUri; }
      set { this.m_BaseXsltUri = value; }
    }
    public String UriHash {
      get { return m_UriHash; }
      set { this.m_UriHash = value; }
    }
    public String Name {
      get { return m_name; }
      set { this.m_name = value; }
    }
  }
}
