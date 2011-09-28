using System;

namespace Xameleon.Llup {
  public class Category {
    private string term = String.Empty;
    private string scheme = String.Empty;
    private string label = String.Empty;
    private string lang = String.Empty;
    
    public Category() {}

    public Category(string term, string scheme, string label, string lang) {
      this.term = term;
      this.scheme = scheme;
      this.label = label;
      this.lang = lang;
    }

    public string Term {
      get {
	return this.term;
      }
      set {
	this.term = value;
      }
    }

    public string Scheme {
      get {
	return this.scheme;
      }
      set {
	this.scheme = value;
      }
    }

    public string Label {
      get {
	return this.label;
      }
      set {
	this.label = value;
      }
    }

    public string Lang {
      get {
	return this.lang;
      }
      set {
	this.lang = value;
      }
    }
  }
} 