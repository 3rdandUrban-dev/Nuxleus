#region using

using System;
using System.Collections;

#endregion

namespace Mvp.Xml.XPointer 
{        
    /// <summary>
    /// XPointer scheme.
    /// </summary>
    internal class XPointerSchema 
    {
        
        #region public members

        public enum SchemaType 
        {
            Element,
            Xmlns,
            XPath1,
            XPointer,
            Unknown
        }
        public static Hashtable Schemas = CreateSchemasTable();
                	    
        public static Hashtable CreateSchemasTable() 
        {
            Hashtable table = new Hashtable(4);
            //<namespace uri>:<ncname>
            table.Add(":element", SchemaType.Element);
            table.Add(":xmlns", SchemaType.Xmlns);
            table.Add(":xpath1", SchemaType.XPath1);
            table.Add(":xpointer", SchemaType.XPointer);
            return table;
        }

        #endregion 
    }
}
