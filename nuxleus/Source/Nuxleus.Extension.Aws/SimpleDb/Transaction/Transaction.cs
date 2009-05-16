using System;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    public class Transaction : ITransaction
    {
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }
        public bool Successful { get; set; }
        public void Commit()
        {
            if (Successful) OnSuccessfulTransaction();
            else OnFailedTransaction();
        }
        public event OnSuccessfulTransaction OnSuccessfulTransaction;
        public event OnFailedTransaction OnFailedTransaction;

    }
}
