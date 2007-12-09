using System;
using Nuxleus.Agent;
using System.Collections;

namespace Nuxleus.Geo
{
    public class Agent : IAgent
    {
        #region IAgent Members

        public Hashtable Result
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void AuthenticateRequest ()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ValidateRequest ()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Response MakeRequest (Request request)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetResponse (Guid id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
