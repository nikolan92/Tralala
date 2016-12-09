using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataLayer.DataObject
{
    [DataContract(Name = "User", Namespace = "Tralala")]

    public class User
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Auth { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        public User(
            string UserName, string Password,
            string FiristName, string LastName)
        {
            this.ID = null;
            this.UserName = UserName;
            this.FirstName = FiristName;
            this.LastName = LastName;
            this.Password = Password;
            Auth = Tools.Tools.RandomString(20);
        }

        public string ToJsonString()
        {
            return JsonSerializer.SerializeToString<User>(this);  
        }
    }
}
