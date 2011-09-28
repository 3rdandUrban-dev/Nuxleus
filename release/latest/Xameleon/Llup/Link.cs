using System;

namespace Xameleon.Llup {
  public class Link {
    private string href = String.Empty;
    private string rel = String.Empty;
    private string mediaType = String.Empty;

    public Link() {}

    public Link(string href, string rel, string mediaType) {
      this.href = href;
      this.rel = rel;
      this.mediaType = mediaType;
    }

    public string Href {
      get {
	return this.href;
      }
      set {
	this.href = value;
      }
    }

    public string Rel {
      get {
	return this.rel;
      }
      set {
	this.rel = value;
      }
    }

    public string MediaType {
      get {
	return this.mediaType;
      }
      set {
	this.mediaType = value;
      }
    }
  }
} 