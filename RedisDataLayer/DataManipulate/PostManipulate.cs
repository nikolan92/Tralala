using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDataLayer.DataObject;

namespace RedisDataLayer.DataManipulate
{
    public class PostManipulate
    {
        private string globalCounterPostId = "next_post_id";

        readonly RedisClient redis = new RedisClient(Config.SingleHost);

        public PostManipulate()
        {
            if (!CheckGlobalCounterPostIdExist())
            {
                var redisCounterSetup = redis.As<string>();
                redisCounterSetup.SetEntry(globalCounterPostId, "0");
            }
        }

        public bool CheckGlobalCounterPostIdExist()
        {
            var test = redis.Get<object>(globalCounterPostId);

            return test != null ? true : false;
        }

        public string GetNextPostId()
        {
            long nextCounterKey = redis.Incr(globalCounterPostId);
            return nextCounterKey.ToString("x");//mozda ce da mi ovde treba D umesto X
        }

        public string SavePost(Post post)
        {

            string postId = GetNextPostId();

            redis.Set("post:" + postId, post);

            return postId;
        }
        public Post LoadPost(string postId)
        {
            Post post = redis.Get<Post>("post:" + postId);
            post.ID = postId;
            return post;
        }

    }
}
