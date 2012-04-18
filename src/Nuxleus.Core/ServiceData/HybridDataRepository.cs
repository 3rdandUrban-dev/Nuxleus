/*
// File: Data.HybridDataRepository.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.Security.Cryptography;
using System.Text;
using ServiceStack.Redis;
//using Nuxleus.ServiceModel.Types.Account;

namespace Nuxleus.Data
{
    public class HybridDataRepository : IDataRepository
    {
        //Definition of all the redis keys that are used for indexes
        static class TagIndex
        {
            public static string Questions(string tag)
            {
                return "urn:tags>q:" + tag.ToLower();
            }

            public static string All { get { return "urn:tags"; } }
        }

        static class QuestionUserIndex
        {
            public static string UpVotes(long questionId)
            {
                return "urn:q>user+:" + questionId;
            }

            public static string DownVotes(long questionId)
            {
                return "urn:q>user-:" + questionId;
            }
        }

        static class UserQuestionIndex
        {
            public static string Questions(long userId)
            {
                return "urn:user>q:" + userId;
            }

            public static string UpVotes(long userId)
            {
                return "urn:user>q+:" + userId;
            }

            public static string DownVotes(long userId)
            {
                return "urn:user>q-:" + userId;
            }
        }

        static class AnswerUserIndex
        {
            public static string UpVotes(long answerId)
            {
                return "urn:a>user+:" + answerId;
            }

            public static string DownVotes(long answerId)
            {
                return "urn:a>user-:" + answerId;
            }
        }

        static class UserAnswerIndex
        {
            public static string Answers(long userId)
            {
                return "urn:user>a:" + userId;
            }

            public static string UpVotes(long userId)
            {
                return "urn:user>a+:" + userId;
            }

            public static string DownVotes(long userId)
            {
                return "urn:user>a-:" + userId;
            }
        }

        static readonly long BaseAccountID = 3369267136000;
        static string AccountHashSalt = "db00fa4ee0655e0e6d728c6b173cb96b"; //md5 of "account@amp.fm";
        static Encoding encoding = UTF8Encoding.UTF8;
        static HMACMD5 hmacProvider = new HMACMD5(encoding.GetBytes(AccountHashSalt));

        IRedisClientsManager RedisManager { get; set; }

        public HybridDataRepository(IRedisClientsManager redisManager)
        {
            RedisManager = redisManager;
        }

        //public AccountInfo GetOrCreateAccount (Account request)
        //{

        //    if (request.ProfileName.IsNullOrEmpty ())
        //        throw new ArgumentNullException ("ProfileName");

        //    string lowerCaseAccountName = request.ProfileName.ToLower ();
        //    request.AccountSettings.ProfileName = lowerCaseAccountName;

        //    var userIdAliasKey = "id:UserAccount:ProfileName:" + lowerCaseAccountName;

        //    using (var redis = RedisManager.GetClient()) {

        //        //Get a typed version of redis client that works with <Account>
        //        var redisAccounts = redis.As<AccountInfo> ();

        //        //Find user by DisplayName if exists
        //        var userKey = redis.GetValue (userIdAliasKey);
        //        if (userKey != null)
        //            return redisAccounts.GetValue (userKey);

        //        //Generate Id for New User

        //        string internalAccountIdString = String.Format ("{0}:{1}", BaseAccountID + redisAccounts.GetNextSequence (), lowerCaseAccountName);
        //        string internalAccountId = Convert.ToBase64String (hmacProvider.ComputeHash (encoding.GetBytes (internalAccountIdString)));

        //        AccountInfo accountInfo = new AccountInfo{ ProfileName = lowerCaseAccountName, InternalAccountId = internalAccountId, AccountSettings = request.AccountSettings };
        //        redisAccounts.Store (accountInfo);

        //        //Save reference to User key using the AccountName alias
        //        redis.SetEntry (userIdAliasKey, accountInfo.CreateUrn ());

        //        return accountInfo;//redisAccounts.GetById(user.InternalAccountId);
        //    }
        //}

        //public EntityStatsInfo GetEntityStats (long entityId)
        //{
        //    using (var redis = RedisManager.GetClient()) {
        //        return new EntityStatsInfo
        //        {
        //            EntityId = entityId,
        //            QuestionsCount = redis.GetSetCount(UserQuestionIndex.Questions(entityId)),
        //            AnswersCount = redis.GetSetCount(UserAnswerIndex.Answers(entityId)),
        //        };
        //    }
        //}

        //public AccountProfileNameInfo GetProfileNameAvailability (AccountProfileNameStatus accountProfileNameStatus)
        //{
        //    if (accountProfileNameStatus.ProfileName.IsNullOrEmpty ())
        //        throw new ArgumentNullException ("ProfileName");

        //    string lowerCaseAccountName = accountProfileNameStatus.ProfileName.ToLower ();
        //    var userIdAliasKey = "id:UserAccount:ProfileName:" + lowerCaseAccountName;

        //    AccountProfileNameInfo accountNameInfo = new AccountProfileNameInfo { ProfileName = lowerCaseAccountName, IsAvailable = false };

        //    using (var redis = RedisManager.GetClient()) {
        //        if (redis.GetValue (userIdAliasKey) == null)
        //            accountNameInfo.IsAvailable = true;

        //        return accountNameInfo;
        //    }
        //}

        #region Old Code
        //public List<Question> GetAllQuestions()
        //{
        //    return RedisManager.ExecAs<Question>(redisQuestions => redisQuestions.GetAll()).ToList();
        //}

        //public List<QuestionResult> GetRecentQuestionResults(int skip, int take)
        //{
        //    using (var redis = RedisManager.GetReadOnlyClient())
        //    {
        //        return ToQuestionResults(redis.As<Question>().GetLatestFromRecentsList(skip, take));
        //    }
        //}

        //public List<QuestionResult> GetQuestionsByUser(long userId)
        //{
        //    using (var redis = RedisManager.GetReadOnlyClient())
        //    {
        //        var questionIds = redis.GetAllItemsFromSet(UserQuestionIndex.Questions(userId));
        //        var questions = redis.As<Question>().GetByIds(questionIds);
        //        return ToQuestionResults(questions);
        //    }
        //}

        //public List<QuestionResult> GetQuestionsTaggedWith(string tagName)
        //{
        //    using (var redis = RedisManager.GetReadOnlyClient())
        //    {
        //        var questionIds = redis.GetAllItemsFromSet(TagIndex.Questions(tagName));
        //        var questions = redis.As<Question>().GetByIds(questionIds);
        //        return ToQuestionResults(questions);
        //    }
        //}

        //private List<QuestionResult> ToQuestionResults(IEnumerable<Question> questions)
        //{
        //    var uniqueUserIds = questions.ConvertAll(x => x.UserId).ToHashSet();
        //    var usersMap = GetUsersByIds(uniqueUserIds).ToDictionary(x => x.Id);

        //    var results = questions.ConvertAll(x => new QuestionResult { Question = x });
        //    var resultsMap = results.ToDictionary(q => q.Question.Id);

        //    results.ForEach(x => x.User = usersMap[x.Question.UserId]);

        //    //Batch multiple operations in a single pipelined transaction (i.e. for a single network request/response)
        //    RedisManager.ExecTrans(trans =>
        //    {
        //        foreach (var question in questions)
        //        {
        //            var q = question;

        //            trans.QueueCommand(r => r.GetSetCount(QuestionUserIndex.UpVotes(q.Id)),
        //                voteUpCount => resultsMap[q.Id].VotesUpCount = voteUpCount);

        //            trans.QueueCommand(r => r.GetSetCount(QuestionUserIndex.DownVotes(q.Id)),
        //                voteDownCount => resultsMap[q.Id].VotesDownCount = voteDownCount);

        //            trans.QueueCommand(r => r.As<Question>().GetRelatedEntitiesCount<Answer>(q.Id),
        //                answersCount => resultsMap[q.Id].AnswersCount = answersCount);
        //        }
        //    });

        //    return results;
        //}

        ///// <summary>
        ///// Delete question by performing compensating actions to StoreQuestion() to keep the datastore in a consistent state
        ///// </summary>
        ///// <param name="questionId"></param>
        //public void DeleteQuestion(long questionId)
        //{
        //    using (var redis = RedisManager.GetClient())
        //    {
        //        var redisQuestions = redis.As<Question>();

        //        var question = redisQuestions.GetById(questionId);
        //        if (question == null) return;

        //        //decrement score in tags list
        //        question.Tags.ForEach(tag => redis.IncrementItemInSortedSet(TagIndex.All, tag, -1));

        //        //remove all related answers
        //        redisQuestions.DeleteRelatedEntities<Answer>(questionId);

        //        //remove this question from user index
        //        redis.RemoveItemFromSet(UserQuestionIndex.Questions(question.UserId), questionId.ToString());

        //        //remove tag => questions index for each tag
        //        question.Tags.ForEach(tag => redis.RemoveItemFromSet(TagIndex.Questions(tag), questionId.ToString()));

        //        redisQuestions.DeleteById(questionId);
        //    }
        //}

        //public void StoreQuestion(Question question)
        //{
        //    using (var redis = RedisManager.GetClient())
        //    {
        //        var redisQuestions = redis.As<Question>();

        //        if (question.Tags == null) question.Tags = new List<string>();
        //        if (question.Id == default(long))
        //        {
        //            question.Id = redisQuestions.GetNextSequence();
        //            question.CreatedDate = DateTime.UtcNow;

        //            //Increment the popularity for each new question tag
        //            question.Tags.ForEach(tag => redis.IncrementItemInSortedSet(TagIndex.All, tag, 1));
        //        }

        //        redisQuestions.Store(question);
        //        redisQuestions.AddToRecentsList(question);
        //        redis.AddItemToSet(UserQuestionIndex.Questions(question.UserId), question.Id.ToString());

        //        //Populate tag => questions index for each tag
        //        question.Tags.ForEach(tag => redis.AddItemToSet(TagIndex.Questions(tag), question.Id.ToString()));
        //    }
        //}

        ///// <summary>
        ///// Delete Answer by performing compensating actions to StoreAnswer() to keep the datastore in a consistent state
        ///// </summary>
        ///// <param name="questionId"></param>
        ///// <param name="answerId"></param>
        //public void DeleteAnswer(long questionId, long answerId)
        //{
        //    using (var redis = RedisManager.GetClient())
        //    {
        //        var answer = redis.As<Question>().GetRelatedEntities<Answer>(questionId).FirstOrDefault(x => x.Id == answerId);
        //        if (answer == null) return;

        //        redis.As<Question>().DeleteRelatedEntity<Answer>(questionId, answerId);

        //        //remove user => answer index
        //        redis.RemoveItemFromSet(UserAnswerIndex.Answers(answer.UserId), answerId.ToString());
        //    }
        //}

        //public void StoreAnswer(Answer answer)
        //{
        //    using (var redis = RedisManager.GetClient())
        //    {
        //        if (answer.Id == default(long))
        //        {
        //            answer.Id = redis.As<Answer>().GetNextSequence();
        //            answer.CreatedDate = DateTime.UtcNow;
        //        }

        //        //Store as a 'Related Answer' to the parent Question
        //        redis.As<Question>().StoreRelatedEntities(answer.QuestionId, answer);
        //        //Populate user => answer index
        //        redis.AddItemToSet(UserAnswerIndex.Answers(answer.UserId), answer.Id.ToString());
        //    }
        //}

        //public List<Answer> GetAnswersForQuestion(long questionId)
        //{
        //    using (var redis = RedisManager.GetClient())
        //    {
        //        return redis.As<Question>().GetRelatedEntities<Answer>(questionId);
        //    }
        //}

        //public void VoteQuestionUp(long userId, long questionId)
        //{
        //    //Populate Question => User and User => Question set indexes in a single transaction
        //    RedisManager.ExecTrans(trans =>
        //    {
        //        //Register upvote against question and remove any downvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(QuestionUserIndex.UpVotes(questionId), userId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(QuestionUserIndex.DownVotes(questionId), userId.ToString()));

        //        //Register upvote against user and remove any downvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(UserQuestionIndex.UpVotes(userId), questionId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(UserQuestionIndex.DownVotes(userId), questionId.ToString()));
        //    });
        //}

        //public void VoteQuestionDown(long userId, long questionId)
        //{
        //    //Populate Question => User and User => Question set indexes in a single transaction
        //    RedisManager.ExecTrans(trans =>
        //    {
        //        //Register downvote against question and remove any upvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(QuestionUserIndex.DownVotes(questionId), userId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(QuestionUserIndex.UpVotes(questionId), userId.ToString()));

        //        //Register downvote against user and remove any upvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(UserQuestionIndex.DownVotes(userId), questionId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(UserQuestionIndex.UpVotes(userId), questionId.ToString()));
        //    });
        //}

        //public void VoteAnswerUp(long userId, long answerId)
        //{
        //    //Populate Question => User and User => Question set indexes in a single transaction
        //    RedisManager.ExecTrans(trans =>
        //    {
        //        //Register upvote against answer and remove any downvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(AnswerUserIndex.UpVotes(answerId), userId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(AnswerUserIndex.DownVotes(answerId), userId.ToString()));

        //        //Register upvote against user and remove any downvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(UserAnswerIndex.UpVotes(userId), answerId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(UserAnswerIndex.DownVotes(userId), answerId.ToString()));
        //    });
        //}

        //public void VoteAnswerDown(long userId, long answerId)
        //{
        //    //Populate Question => User and User => Question set indexes in a single transaction
        //    RedisManager.ExecTrans(trans =>
        //    {
        //        //Register downvote against answer and remove any upvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(AnswerUserIndex.DownVotes(answerId), userId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(AnswerUserIndex.UpVotes(answerId), userId.ToString()));

        //        //Register downvote against user and remove any upvotes if any
        //        trans.QueueCommand(redis => redis.AddItemToSet(UserAnswerIndex.DownVotes(userId), answerId.ToString()));
        //        trans.QueueCommand(redis => redis.RemoveItemFromSet(UserAnswerIndex.UpVotes(userId), answerId.ToString()));
        //    });
        //}

        //public QuestionResult GetQuestion(long questionId)
        //{
        //    var question = RedisManager.ExecAs<Question>(redisQuestions => redisQuestions.GetById(questionId));
        //    if (question == null) return null;

        //    var result = ToQuestionResults(new[] { question })[0];
        //    var answers = GetAnswersForQuestion(questionId);
        //    var uniqueUserIds = answers.ConvertAll(x => x.UserId).ToHashSet();
        //    var usersMap = GetUsersByIds(uniqueUserIds).ToDictionary(x => x.Id);

        //    result.Answers = answers.ConvertAll(answer =>
        //        new AnswerResult { Answer = answer, User = usersMap[answer.UserId] });

        //    return result;
        //}

        //public List<User> GetUsersByIds(IEnumerable<long> userIds)
        //{
        //    return RedisManager.ExecAs<User>(redisUsers => redisUsers.GetByIds(userIds)).ToList();
        //}

        //public QuestionStat GetQuestionStats(long questionId)
        //{
        //    using (var redis = RedisManager.GetReadOnlyClient())
        //    {
        //        var result = new QuestionStat
        //        {
        //            VotesUpCount = redis.GetSetCount(QuestionUserIndex.UpVotes(questionId)),
        //            VotesDownCount = redis.GetSetCount(QuestionUserIndex.DownVotes(questionId))
        //        };
        //        result.VotesTotal = result.VotesUpCount - result.VotesDownCount;
        //        return result;
        //    }
        //}

        //public List<Tag> GetTagsByPopularity(int skip, int take)
        //{
        //    using (var redis = RedisManager.GetReadOnlyClient())
        //    {
        //        var tagEntries = redis.GetRangeWithScoresFromSortedSetDesc(TagIndex.All, skip, take);
        //        var tags = tagEntries.ConvertAll(kvp => new Tag { Name = kvp.Key, Score = (int)kvp.Value });
        //        return tags;
        //    }
        //}

        //public SiteStats GetSiteStats()
        //{
        //    using (var redis = RedisManager.GetClient())
        //    {
        //        return new SiteStats
        //        {
        //            QuestionsCount = redis.As<Question>().TypeIdsSet.Count,
        //            AnswersCount = redis.As<Answer>().TypeIdsSet.Count,
        //            TopTags = GetTagsByPopularity(0, 10)
        //        };
        //    }
        //}
        #endregion

        //public AccountInfo CreateAccount (Account account)
        //{
        //    throw new NotImplementedException ();
        //}

        //public AccountInfo GetAccount (string accountName)
        //{
        //    throw new NotImplementedException ();
        //}

        //public AccountInfo UpdateAccount (Account account)
        //{
        //    throw new NotImplementedException ();
        //}

        //public AccountInfo DeleteAccount (string accountName)
        //{
        //    throw new NotImplementedException ();
        //}

        //public ArtistInfo CreateArtist (Artist artist)
        //{
        //    throw new NotImplementedException ();
        //}

        //public ArtistInfo GetArtist (Artist artist)
        //{
        //    throw new NotImplementedException ();
        //}

        //public ArtistInfo UpdateArtist (Artist artist)
        //{
        //    throw new NotImplementedException ();
        //}

        //public ArtistInfo DeleteArtist (Artist artist)
        //{
        //    throw new NotImplementedException ();
        //}
    }
}