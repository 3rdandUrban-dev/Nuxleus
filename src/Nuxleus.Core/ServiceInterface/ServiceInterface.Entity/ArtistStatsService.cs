using System.Runtime.Serialization;
using Nuxleus.Data;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Nuxleus.ServiceInterface
{

    [RestService("/artist/{EntityId}/stats")]
    public class ArtistStats
    {
     [DataMember]
        public long EntityId { get; set; }
    }

    public class EntityStatsInfo
    {
        public long EntityId { get; set; }

        public int QuestionsCount { get; set; }

        public int AnswersCount { get; set; }
    }

    public class ArtistStatsResponse
    {
        public EntityStatsInfo Result { get; set; }
    }

    public class ArtistStatsService
        : RestServiceBase<ArtistStats>
    {
        public IDataRepository Repository { get; set; }

        public override object OnGet (ArtistStats request)
        {
            return new ArtistStatsResponse
         {
             Result = Repository.GetEntityStats(request.EntityId)
         };
        }

    }
}