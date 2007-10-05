using System;
using System.Resources;
using System.Configuration;
using System.Collections.Specialized;
using com.amazon.s3;
using System.Reflection;

namespace X5 {
    public partial class GlobalClip {
        ResourceManager rm = new ResourceManager("items", Assembly.GetExecutingAssembly());
    }
}

