using System;
using System.Collections.Generic;
using Nuxleus.Asynchronous;

namespace Nuxleus.Core {

    public delegate void OnSuccessfulTransaction();
    public delegate void OnFailedTransaction();

    public interface ITransaction {
        IRequest Request { get; set; }
        IResponse Response { get; set; }
        bool Successful { get; set; }
        void Commit();
        event OnSuccessfulTransaction OnSuccessfulTransaction;
        event OnFailedTransaction OnFailedTransaction;
    }
}
