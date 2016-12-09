using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataLayer.DataObject
{
    [DataContract (Name = "Post", Namespace = "Tralala")]
    public class Post
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public Int32 Time { get; set; }
        
        [DataMember]        
        public string Body { get; set; }

        public Post(string UserID,string userName, string Body)
        {
            //this.ID = ID;
            this.UserName = userName;
            this.UserID = UserID;
            Time = Tools.Tools.TimeNowInUnix();
            this.Body = Body;
        }
    }
}
