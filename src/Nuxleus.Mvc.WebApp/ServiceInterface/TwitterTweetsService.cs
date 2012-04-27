using System.Collections.Generic;
using ChaweetApi.ServiceModel;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Nuxleus.Mvc.WebApp.ServiceInterface
{
	public class TwitterTweets
	{
		public string ScreenName { get; set; }

		public int? Take { get; set; }

		public string SinceId { get; set; }

		public string MaxId { get; set; }
	}

	public class TwitterTweetsResponse
	{
		public TwitterTweetsResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		public List<Tweet> Results { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class TwitterTweetsService : AppServiceBase<TwitterTweets>
	{
		protected override object Run(TwitterTweets request)
		{
			var cacheKey = "cache:Tweet:" + request.ScreenName + ":tweets"
				+ (request.Take.HasValue ? ":take:" + request.Take : "")
				+ (!request.SinceId.IsNullOrEmpty() ? ":sinceid:" + request.SinceId : "")
				+ (!request.MaxId.IsNullOrEmpty() ? ":maxid:" + request.MaxId : "");

			//This caches and returns the most optimal result the browser can handle, e.g.
			//If the browser requests json and accepts deflate - it returns a deflated json payload from cache
			return base.RequestContext.ToOptimizedResultUsingCache(Cache, cacheKey, () =>
				new TwitterTweetsResponse {
					Results = AuthTwitterGateway.GetTweets(
						request.ScreenName, request.SinceId, request.MaxId, request.Take)
				});
		}
	}

}