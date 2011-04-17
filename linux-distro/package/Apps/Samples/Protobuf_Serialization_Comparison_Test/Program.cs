using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Nuxleus.Messaging.Protobuf;
using Nuxleus.Performance;


namespace Nuxleus.Messaging {

    // struct for holding the various objects and related meta-data for each 
    // serializer we will be using in our test.  This provides a generic interface
    // which we can then more easily use programmatically throughout the codebase.
    public struct SerializerPerformanceTestAgent {
        public string FileExtension { get; set; }
        public string TypeLabel { get; set; }
        public PerformanceLogCollection PerformanceLogCollection { get; set; }
        public ISerializerTestAgent ISerializerTestAgent { get; set; }
    }

    public class Program {

        // Create a PerformanceTimer to measure performance
        static PerformanceTimer m_timer = new PerformanceTimer();

        // Create a SerializerPerformanceTestAgent array which will then allow us to iterate through each 
        // SerializerPerformanceTestAgent contained in the array, keeping our test code clean and simple
        // by placing placing the various objects and values associated with each SerializerPerformanceTestAgent
        // within easy reach.
        static SerializerPerformanceTestAgent[] serializerPeformanceTestAgent = new SerializerPerformanceTestAgent[] {
            new SerializerPerformanceTestAgent{ 
                TypeLabel = "Binary", 
                ISerializerTestAgent = new TestBinarySerializer(), 
                PerformanceLogCollection = new PerformanceLogCollection(),
                FileExtension = "dat"
            },
            //#if NET_3_0 || NET_3_5
            //new SerializerPerformanceTestAgent{ 
            //    TypeLabel = "DataContract", 
            //    ISerializerTestAgent = new TestDataContractSerializer(), 
            //    PerformanceLogCollection = new PerformanceLogCollection(),
            //    FileExtension = "contract"
            //},
            //#endif
            new SerializerPerformanceTestAgent{ 
                TypeLabel = "JSON", 
                ISerializerTestAgent = new TestJsonSerializer(), 
                PerformanceLogCollection = new PerformanceLogCollection(),
                FileExtension = "json"
            },
            new SerializerPerformanceTestAgent{ 
                TypeLabel = "ProtoBuffer", 
                ISerializerTestAgent = new TestProtoBufSerializer(), 
                PerformanceLogCollection = new PerformanceLogCollection(),
                FileExtension = "proto"
            },
            new SerializerPerformanceTestAgent{ 
                TypeLabel = "SOAP", 
                ISerializerTestAgent = new TestSOAPSerializer(), 
                PerformanceLogCollection = new PerformanceLogCollection(), 
                FileExtension = "soap"
            },
            new SerializerPerformanceTestAgent{ 
                TypeLabel = "XML", 
                ISerializerTestAgent = new TestXmlSerializer(), 
                PerformanceLogCollection = new PerformanceLogCollection(), 
                FileExtension = "xml"
            },
        };

        static void Main(string[] args) {

            // By adjusting this setting we can change the unit precision returned in the timings.
            PerformanceTimer.UnitPrecision = UnitPrecision.NANOSECONDS;

            int repeatTest = 500;

            if (args.Length > 0) {
                int repeat;
                if (int.TryParse(args[0], out repeat)) {
                    repeatTest = repeat;
                }
            }

            using (m_timer) {
                m_timer.Scope = () => {
                    for (int i = 0; i < repeatTest; i++) {
                        foreach (SerializerPerformanceTestAgent agent in serializerPeformanceTestAgent) {
                            agent.PerformanceLogCollection.Add(RunSerializationTest(i, agent));
                        }
                    }
                };
                Console.WriteLine("Completed Serialization Tests in {0}", m_timer.Elapsed);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PerformanceLogCollection));

            foreach (SerializerPerformanceTestAgent agent in serializerPeformanceTestAgent) {
                string fileName = String.Format("../../Report/{0}Performance.xml", agent.TypeLabel);
                using (FileStream stream = new FileStream(fileName, FileMode.Create)) {
                    Console.WriteLine("Generating {0}", fileName);
                    xmlSerializer.Serialize(stream, agent.PerformanceLogCollection);
                    Console.WriteLine("Generation of {0} complete.", fileName);
                }
            }

            //TODO: Generate an HTML report and open it in the default browser
            //System.Diagnostics.Process.Start("report.html");
        }

        static PerformanceLog RunSerializationTest(int fileSequence, SerializerPerformanceTestAgent agent) {

            // Create a new PerformanceLog for logging the performance numbers
            PerformanceLog perfLog = new PerformanceLog {
                Entries = new List<Entry>(),
                UnitPrecision = PerformanceTimer.UnitPrecision
            };

            Person person = null;

            m_timer.Scope = () => {

                m_timer.LogScope("Create a Person object", perfLog, () => {
                    person = CreatePerson(fileSequence);
                });

                m_timer.LogScope("Write values of Person object to stdout", perfLog, () => {
                    WriteValuesToConsole(person);
                });

                Stream fileStream = null;

                m_timer.LogScope("Serialize the Person object to a FileStream", perfLog, () => {
                    fileStream = SerializeToStream<Person>(person, String.Format("Person_{0}.{1}", fileSequence, agent.FileExtension), agent.ISerializerTestAgent);
                }).LogData("Length (in bytes) of FileStream", fileStream.Length);

                Person newPersonFromFileStream = null;

                using (fileStream) {
                    m_timer.LogScope("Deserialize and parse the Person object from a FileStream", perfLog, () => {
                        newPersonFromFileStream = DeserializeFromStream<Person>(fileStream, agent.ISerializerTestAgent);
                    });
                }

                CompareValuesAndLogResults(person, newPersonFromFileStream, perfLog, typeof(FileStream));

                Stream memoryStream = null;

                m_timer.LogScope("Serialize the Person object to a MemoryStream", perfLog, () => {
                    memoryStream = SerializeToStream<Person>(person, null, agent.ISerializerTestAgent);
                }).LogData("Length (in bytes) of memoryStream", memoryStream.Length);

                Person newPersonFromMemoryStream = null;

                using (memoryStream) {
                    m_timer.LogScope("Deserialize and parse the Person object from a MemoryStream", perfLog, () => {
                        newPersonFromMemoryStream = DeserializeFromStream<Person>(memoryStream, agent.ISerializerTestAgent);
                    });
                }

                CompareValuesAndLogResults(person, newPersonFromMemoryStream, perfLog, typeof(MemoryStream));

                //TODO: Store the uncompressed serialized object on S3

                //TODO: Retrieve the uncompressed serialized object from S3 and deserialize into a MemoryStream

                //TODO: Compress and store the serialized object on S3

                //TODO: Retrieve the compressed serialized object from S3 and deserialize into a MemoryStream

            };
            perfLog.LogData("Duration of test", m_timer.Duration);
            return perfLog;
        }

        static Person CreatePerson(int fileSequence) {

            List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();
            phoneNumbers.Add(new PhoneNumber { Number = "555.555.1234", Type = PhoneType.HOME });
            phoneNumbers.Add(new PhoneNumber { Number = "555.555.5678", Type = PhoneType.MOBILE });
            phoneNumbers.Add(new PhoneNumber { Number = "555.555.9012", Type = PhoneType.WORK });

            return new Person {
                Name = String.Format("John Doe{0}", fileSequence),
                Email = String.Format("jdoe{0}@example.com", fileSequence),
                Phone = phoneNumbers
            };
        }


        static void WriteValuesToConsole(Person person) {
            Console.WriteLine("person.Name: {0}, person.Email: {1}, person.ID", person.Name, person.Email, person.ID);
            foreach (PhoneNumber phone in person.Phone) {
                Console.WriteLine("phone.Number: {0}, phone.Type: {1}", phone.Number, phone.Type);
            }
        }

        static void CompareValuesAndLogResults(Person person, Person newPerson, PerformanceLog perfLog, Type streamType) {
            perfLog.LogData(String.Format("newPersonFrom{0}.Name and person.Name are equal", streamType.Name), String.Equals(newPerson.Name, person.Name));
            perfLog.LogData(String.Format("newPersonFrom{0}.ID and person.ID are equal", streamType.Name), int.Equals(newPerson.ID, person.ID));
            perfLog.LogData(String.Format("newPersonFrom{0}.Email and person.Email are equal", streamType.Name), String.Equals(newPerson.Email, person.Email));

            PhoneNumber[] phone = person.Phone.ToArray();
            PhoneNumber[] newPhone = newPerson.Phone.ToArray();

            for (int i = 0; i < phone.Length; i++) {
                perfLog.LogData(String.Format("PhoneNumber[{0}].Number from newPerson{1}From{2}.Phone is the same as PhoneNumber[{0}].Number from person.Phone", i, newPerson.Name, streamType.Name), phone[i].Number.Equals(newPhone[i].Number));
                perfLog.LogData(String.Format("PhoneNumber[{0}].Type from newPerson{1}From{2}.Phone is the same as PhoneNumber[{0}].Type from person.Phone", i, newPerson.Name, streamType.Name), phone[i].Type.Equals(newPhone[i].Type));
            }
        }

        static bool StoreObjectToS3(string fileName, bool compressFile) {
            return true;
        }

        static bool GetObjectFromS3(string fileUri) {
            return true;
        }

        static Stream SerializeToStream<T>(T obj, String fileName, ISerializerTestAgent serializer) where T : class, new() {

            Stream stream = null;
            if (fileName == null) {
                stream = new MemoryStream();
            } else {
                stream = new FileStream(fileName, FileMode.Create);
            }

            return serializer.Serialize<T>(stream, obj);
        }

        static T DeserializeFromStream<T>(Stream stream, ISerializerTestAgent serializer) where T : class, new() {
            stream.Seek(0, 0);
            return serializer.Deserialize<T>(stream);
        }
    }
}
