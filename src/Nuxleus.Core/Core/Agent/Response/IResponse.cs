using System;
namespace Nuxleus.Agent {
    public interface IResponse {
        object Result { get; set; }
        Guid ID { get; }
        string Message { get; set; }
        DateTime Timestamp { get; set; }
    }
}
