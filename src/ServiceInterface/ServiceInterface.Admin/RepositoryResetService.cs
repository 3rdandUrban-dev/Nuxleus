using System.Runtime.Serialization;
using Nuxleus.Data;
using ServiceStack.Redis;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Nuxleus.ServiceInterface
{
    [DataContract]
    [RestService("/repository/reset", "GET")]
    public class RepositoryReset { }

    [DataContract]
    public class RepositoryResetResponse : IHasResponseStatus
    {
        public RepositoryResetResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        [DataMember]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class RepositoryResetService
        : RestServiceBase<RepositoryReset>
    {
        public IRedisClientsManager RedisManager { get; set; }

        public IDataRepository Repository { get; set; }

        public override object OnGet(RepositoryReset request)
        {
            //Uncomment if you want this feature
            //throw new NotSupportedException("Disabling for Demo site. Based on the XSS attacks I know it will only be a matter of time before someone pulls the trigger.");

            RedisManager.Exec(r => r.FlushAll());

            return new RepositoryResetResponse();
        }

    }
}