using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using RedisDataLayer.DataManipulate;
using RedisDataLayer.DataObject;
namespace Tralala.Controllers
{
    public class FollowerController : ApiController
    {
        // GET: Follower
        public IEnumerable<User> GetFollower(string userId)//get all followers
        {
            UserManipulate um = new UserManipulate();
            return um.GetAllFollowerUsers(userId);
        }
    }
}