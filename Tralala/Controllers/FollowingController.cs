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
    public class FollowingController : ApiController
    {

        //Get: Follow
        public IEnumerable<User> GetFollowing(string userId)//get all following users
        {
            UserManipulate um = new UserManipulate();
            return um.GetAllFollowingUsers(userId);
        }

        // Post: Follow
        [System.Web.Http.HttpPost]
        public void Post([FromBody]Follow follow) //add new following user
        {
            if (follow == null)
                return;
            UserManipulate um = new UserManipulate();
            um.AddFollowing(follow);
            
        }


    }

}