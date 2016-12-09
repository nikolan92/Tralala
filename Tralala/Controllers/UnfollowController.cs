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
    public class UnfollowController : ApiController
    {
        // Post: Unfollow
        [System.Web.Http.HttpPost]
        public void Post([FromBody]Follow follow) 
        {
            if (follow == null)
                return;
            UserManipulate um = new UserManipulate();
            um.RemoveFollowing(follow);

        }
    }
}