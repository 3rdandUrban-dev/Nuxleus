using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nuxleus.Asynchronous;

namespace Nuxleus.Extension.Aws.SimpleDb {
    public static class ExtensionMethods {
        public static IEnumerable<IAsync> AsIAsync(this ITask operation) {
            Dictionary<IRequest, XElement> responseList = new Dictionary<IRequest, XElement>();
            return operation.InvokeAsync();
        }
    }
}
