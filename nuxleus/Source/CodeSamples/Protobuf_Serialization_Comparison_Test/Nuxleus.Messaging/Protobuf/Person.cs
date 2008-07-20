using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using ProtoBuf;
using System.Collections.Generic;

namespace Nuxleus.Messaging.Protobuf {

    [Serializable, DataContract]
    public class Person {
        [DataMember(Name = "Name", Order = 1, IsRequired = true)]
        public string Name { get; set; }
        [DataMember(Name = "ID", Order = 2, IsRequired = true)]
        [ProtoMember(2, Name = "ID", IsRequired = true, DataFormat = DataFormat.TwosComplement)]
        public int ID { get; set; }
        [DataMember(Name = "Email", Order = 3, IsRequired = true)]
        public string Email { get; set; }
        // No support at the moment for arrays/collections in the protobuf-net project,
        // though I assume that will change soon.
        //[DataMember(Name = "Phone", Order = 4, IsRequired = false)]
        public List<PhoneNumber> Phone { get; set; } 
    }

    [Serializable, DataContract]
    public struct PhoneNumber {
        [DataMember(Name = "Number", Order = 1, IsRequired = true)]
        public string Number { get; set; }
        [DataMember(Name = "Type", Order = 2, IsRequired = true)]
        [DefaultValue(PhoneType.HOME)]
        public PhoneType Type { get; set; }
    }

    [Serializable, DataContract]
    public enum PhoneType { MOBILE, HOME, WORK }
}
