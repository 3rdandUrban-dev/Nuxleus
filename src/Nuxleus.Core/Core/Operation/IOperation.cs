using System;

namespace Nuxleus.Core
{
    public interface IOperation
    {
        Session Session { get; }
    }

    public class Session
    {
        public Guid ID { get; set; }
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }
    }
}
