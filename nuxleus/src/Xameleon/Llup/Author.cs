using System;

namespace Xameleon.Llup {
  public class Author {
    private string name = String.Empty;
    private string uri = String.Empty;
    private string email = String.Empty;
    
    public Author() {}

    public Author(string name, string uri, string email) {
      this.name = name;
      this.uri = uri;
      this.email = email;
    }

    public string Name {
      get {
	return this.name;
      }
      set {
	this.name = value;
      }
    }

    public string Uri {
      get {
	return this.uri;
      }
      set {
	this.uri = value;
      }
    }

    public string Email {
      get {
	return this.email;
      }
      set {
	this.email = value;
      }
    }
  }
} 