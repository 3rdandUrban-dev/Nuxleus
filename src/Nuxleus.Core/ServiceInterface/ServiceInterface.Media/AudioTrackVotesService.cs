//using System.Runtime.Serialization;
//using ServiceStack.ServiceHost;
//using ServiceStack.ServiceInterface;
//using Nuxleus.Data;
//
//namespace Nuxleus.ServiceInterface
//{
//	[DataContract]
//    [RestService("/entity/{EntityType}/votes/{EntityId}")]
//	public class EntityVotes
//    {
//        [DataMember]
//        public string EntityType { get; set; }
//
//		[DataMember]
//		public long EntityId { get; set; }
//
//		[DataMember]
//		public string Direction { get; set; }
//	}
//
//	[DataContract]
//	public class EntityVotesResponse { }
//
//	public class EntityVotesService
//        : RestServiceBase<EntityVotes>
//	{
//		public IDataRepository Repository { get; set; }
//
//        public override object OnPost(EntityVotes request)
//		{
//			var direction = request.Direction ?? "up";
//			var voteUp = direction.ToLower() != "down";
//
//            //if (request.QuestionId.HasValue)
//            //{
//            //    if (voteUp)
//            //        Repository.VoteQuestionUp(request.UserId, request.QuestionId.Value);
//            //    else
//            //        Repository.VoteQuestionDown(request.UserId, request.QuestionId.Value);
//            //}
//            //else if (request.AnswerId.HasValue)
//            //{
//            //    if (voteUp)
//            //        Repository.VoteAnswerUp(request.UserId, request.AnswerId.Value);
//            //    else
//            //        Repository.VoteAnswerDown(request.UserId, request.AnswerId.Value);
//            //}
//
//            return new EntityVotesResponse();
//		}
//	}
//}