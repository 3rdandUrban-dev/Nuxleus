using System;
namespace Nuxleus.Agent {
    public interface IRequest {
        //IAsyncResult AsyncResult { get; set;}
        IResponse GetResponse { get; }
        Guid ID { get; set; }
        bool IsCompleted { get; set;}
        string Message { get; set; }
        DateTime Timestamp { get; set; }
    }
}
