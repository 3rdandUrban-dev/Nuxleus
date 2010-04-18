using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Nuxleus.Messaging;
using System.Runtime.Serialization;

namespace Nuxleus.Performance {

    [Serializable]
    [XmlType(Namespace = "http://nuxleus.com/Nuxleus/Performance")]
    [XmlRootAttribute(Namespace = "http://nuxleus.com/Nuxleus/Performance", IsNullable = false)]
    public class PerformanceLogCollection {

        List<PerformanceLog> m_log = new List<PerformanceLog>();

        public void Add(PerformanceLog log) {
            Log.Add(log);
        }

        [XmlElement(ElementName = "PerformanceLog")]
        public List<PerformanceLog> Log {
            get {
                return m_log;
            }
        }
    }

    [Serializable]
    [XmlTypeAttribute(Namespace = "http://nuxleus.com/Nuxleus/Performance")]
    public struct PerformanceLog {

        public void LogData(string description, double value) {
            Entries.Add(new Entry {
                Description = description,
                Value = value
            });
        }

        public void LogData(string description, bool value) {
            Entries.Add(new Entry {
                Description = description,
                Value = value
            });
        }

        [XmlElement(ElementName = "Entry")]
        public List<Entry> Entries { get; set; }

        [XmlAttribute(AttributeName = "UnitPrecision")]
        public UnitPrecision UnitPrecision { get; set; }

    }

    [Serializable]
    [XmlType(Namespace = "http://nuxleus.com/Nuxleus/Performance")]
    public struct Entry {
        public string Description { get; set; }
        public object Value { get; set; }
    }
}
