using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataLayer.DataObject
{
    [DataContract(Name = "Follow", Namespace = "Tralala")]
    public class Follow
    {
        [DataMember]
        public string FollowingID { get; set; }
        [DataMember]
        public string UserID { get; set; }

        public Follow(string userId, string followerId)
        {
            FollowingID = followerId;
            UserID = userId;
        }
    }
}
