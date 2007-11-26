using System;
using System.Collections;

namespace Nuxleus.Messaging
{
    public struct Response
    {
        Guid _id;
        string _message;
        DateTime _timeStamp;

        public Guid ID { get { return _id; } }
        public string Message { get { return _message; } set { _message = value; } }
        public DateTime Timestamp { get { return _timeStamp; } set { _timeStamp = value; } }
    }
}