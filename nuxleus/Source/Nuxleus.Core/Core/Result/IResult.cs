using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Nuxleus.Core
{
    public interface IResult
    {
        XElement ToXElement();
        string ToXmlString();
    }
}
