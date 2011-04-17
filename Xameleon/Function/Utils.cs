using System;

namespace Xameleon.Function {
	  public class Utils {
	    public static string GetGuid() {
	      return Guid.NewGuid().ToString();
	    }
	  }
}