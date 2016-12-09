using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using RedisDataLayer.DataObject;
using RedisDataLayer.DataManipulate;
namespace Tralala.Controllers
{
    public class PostsController : ApiController
    {
        // GET: Posts
        public IEnumerable<Post> GetPosts()//return timeLine posts
        {
            UserManipulate um = new UserManipulate();
            return um.GetTimeLinePosts(0, -1);
        }
        // POST: 
        public IEnumerable<Post> GetPosts(string userId) // return user Posts
        {

            UserManipulate um = new UserManipulate();
            List<Post> posts = new List<Post>();
            posts = um.GetAllUserPosts(userId, 0, -1);

            if (posts.Count == 0)
            {
                //posts.Add(new Post("", "", "Enter your firist post!"));
                return null;
            }
            return posts; // za sad vracam sve postove ali ovde moze da se doda da vrati npr prvix 20 posta 
                          //return null;
            
        }

        public Post Post([FromBody]Post post) //add new post (by user with userId)
        {
            if (post == null)
                return null;
            if (post.Body == null)
                return null;
            UserManipulate um = new UserManipulate();
            return um.AddPost(post);
        }
    }
}