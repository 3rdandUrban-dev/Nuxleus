using System;
using System.Collections;

namespace Nuxleus.Messaging
{
    public delegate void AsyncRequestCallback(Response response);

    public struct Request
    {
        Guid _id;
        string _message;
        DateTime _timeStamp;
        bool _isComplete;
        Response _response;

        public Request(string message) 
        {
            _id = Guid.NewGuid();
            _message = message;
            _timeStamp = DateTime.Now;
            _isComplete = false;
            _response = new Response();
        }

        public void BeginRequest()
        {
            ProcessRequest(new AsyncRequestCallback(CompleteRequest));
        }

        private void ProcessRequest(AsyncRequestCallback callback)
        {
            ///TODO: Build out request processing logic
            Response myResponse = new Response();
            myResponse.Message = "<foo>bar</foo>";
            myResponse.Timestamp = DateTime.Now;
            callback(myResponse);
        }

        public void CompleteRequest(Response response)
        {
            _isComplete = true;
            _response = response;
        }

        public Guid ID { get { return _id; } }
        public string Message { get { return _message; } }
        public DateTime Timestamp { get { return _timeStamp; } set { _timeStamp = value; } }
        public Response GetResponse { get { return _response; } }
        public bool IsCompleted { get { return _isComplete; } }
    }
}
