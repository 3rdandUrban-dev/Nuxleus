using System;
using System.Xml.XPath;

namespace Nuxleus.Extension.Xml.XPath
{
    public static class Extensions
    {
        public static string GetAttribute (this XPathNavigator navigator, string localName, string nameSpaceURI, bool ignoreCase)
        {
            var result = navigator.GetAttribute (localName, nameSpaceURI);
            if (!ignoreCase || !string.IsNullOrEmpty (result))
                return result;

            navigator.MoveToFirstAttribute ();
            do {
                if (string.Compare (navigator.LocalName, localName, true) == 0) {
                    return navigator.Value;
                }
            } while (navigator.MoveToNextAttribute());
            return string.Empty;
        }
    }
}

