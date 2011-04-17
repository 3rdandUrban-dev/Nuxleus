using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using Nuxleus.Performance;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProtoBuf;
using Nuxleus.MetaData;
using Nuxleus.Messaging.Protobuf;

namespace Nuxleus.Messaging {

    public interface ISerializerTestAgent {
        object Serializer { get; }
        Stream Serialize<T>(Stream stream, T obj) where T : class, new();
        T Deserialize<T>(Stream stream) where T : class, new();
    }

    // Create the necessary formatter/serializer objects to serialize and deserialize the Person 
    // object into and out of the various binary and text representations represented by each.
    // Each serializer is wrapped in an ISerializerTestObject which provides a common interface
    // for invoking the serialize and deserialize operations of the underlying serializer.
    public struct TestBinarySerializer : ISerializerTestAgent {
        static BinaryFormatter m_binaryFormatter = new BinaryFormatter();
        public object Serializer {
            get {
                return m_binaryFormatter;
            }
        }
        public Stream Serialize<T>(Stream stream, T obj) where T : class, new() {
            m_binaryFormatter.Serialize(stream, obj);
            return stream;
        }
        public T Deserialize<T>(Stream stream) where T : class, new() {
            return (T)m_binaryFormatter.Deserialize(stream);
        }
    }
    public struct TestXmlSerializer : ISerializerTestAgent {
        static XmlSerializer m_xmlSerializer = new XmlSerializer(typeof(Person));
        public object Serializer {
            get {
                return m_xmlSerializer;
            }
        }
        public Stream Serialize<T>(Stream stream, T obj) where T : class, new() {
            m_xmlSerializer.Serialize(stream, obj);
            return stream;
        }
        public T Deserialize<T>(Stream stream) where T : class, new() {
            return (T)m_xmlSerializer.Deserialize(stream);
        }
    }
    public struct TestJsonSerializer : ISerializerTestAgent {
        static JsonSerializer m_jsonSerializer = new JsonSerializer();
        public object Serializer {
            get {
                return m_jsonSerializer;
            }
        }
        public Stream Serialize<T>(Stream stream, T obj) where T : class, new() {
            JsonWriter jsonWriter = new JsonTextWriter(new StreamWriter(stream));
            m_jsonSerializer.Serialize(jsonWriter, obj);
            jsonWriter.Flush();
            return stream;
        }
        public T Deserialize<T>(Stream stream) where T : class, new() {
            return (T)m_jsonSerializer.Deserialize(new StringReader(new StreamReader(stream).ReadToEnd()), typeof(T));
        }
    }
    public struct TestSOAPSerializer : ISerializerTestAgent {
        static SoapFormatter m_soapFormatter = new SoapFormatter();
        public object Serializer {
            get {
                return m_soapFormatter;
            }
        }
        public Stream Serialize<T>(Stream stream, T obj) where T : class, new() {
            m_soapFormatter.Serialize(stream, obj);
            return stream;
        }
        public T Deserialize<T>(Stream stream) where T : class, new() {
            return (T)m_soapFormatter.Deserialize(stream);
        }
    }
#if NET_3_0
    public struct TestDataContractSerializer : ISerializerTestAgent {
        static DataContractSerializer m_dataContractSerializer = new DataContractSerializer(typeof(Person));
        public object Serializer {
            get {
                return m_dataContractSerializer;
            }
        }
        public Stream Serialize<T>(Stream stream, T obj) where T : class, new() {
            m_dataContractSerializer.WriteObject(stream, obj);
            return stream;
        }
        public T Deserialize<T>(Stream stream) where T : class, new() {
            return (T)m_dataContractSerializer.ReadObject(stream);
        }
    }
#endif
    public struct TestProtoBufSerializer : ISerializerTestAgent {
        public object Serializer {
            get {
                return null;
            }
        }
        public Stream Serialize<T>(Stream stream, T obj) where T : class, new() {
            ProtoBuf.Serializer.Serialize<T>(stream, obj);
            return stream;
        }
        public T Deserialize<T>(Stream stream) where T : class, new() {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }
}
