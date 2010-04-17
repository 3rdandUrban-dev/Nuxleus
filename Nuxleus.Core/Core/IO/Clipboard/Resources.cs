using System;
using System.Resources;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;

namespace Nuxleus.IO
{
    public partial class GlobalClip
    {
        ResourceManager rm = new ResourceManager("items", Assembly.GetExecutingAssembly());
    }
}

