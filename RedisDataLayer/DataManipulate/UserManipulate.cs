using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RedisDataLayer.DataObject;
namespace RedisDataLayer.DataManipulate
{
    public class UserManipulate
    {
        private string globalCounterUserId = "next_user_id";

        readonly RedisClient redis = new RedisClient(Config.SingleHost);

        public UserManipulate()
        {
            if (!CheckNextGlobalCounterUserIdExist())
            {
                var redisCounterSetup = redis.As<string>();
                redisCounterSetup.SetEntry(globalCounterUserId, "0");
            }//provera da li vec postoji brojac za user-a (ako ne postoji postavlja se novi na 0)
        }

        public bool CheckNextGlobalCounterUserIdExist()
        {
            var test = redis.Get<object>(globalCounterUserId);
            return (test != null) ? true : false;
        }
        public string GetNextUserid()
        {
            long nextCounterKey = redis.Incr(globalCounterUserId);
            return nextCounterKey.ToString("x");
        }

        public string SaveUser(User newUser)
        {

            string userID = GetNextUserid();
            newUser.ID = userID;

            //redis.Set("proba", Encoding.UTF8.GetBytes(userID));
            redis.Set("username:" + newUser.UserName, Encoding.UTF8.GetBytes(userID));//sa ovim kasnije po username mogu da nadjem id korisnika
                                                              //morao sam da dodam Encoding.Utf8 zato sto kad mu se prosledi cist string on mu doda apostrofe npr:"\"1\""
            redis.Set("user:" + userID, newUser);
            
            redis.Set("auth:" + newUser.Auth, Encoding.UTF8.GetBytes(userID));//treba zbog proveru stanja da li je user ulogovan
            return userID;

        }
        public User LoadUser(string id)
        {
            return redis.Get<User>("user:" + id);
        }
        public Post AddPost(Post post)
        {

            User user = redis.Get<User>("user:" + post.UserID);
            Post newPost = new Post(post.UserID, user.UserName, post.Body);

            PostManipulate pm = new PostManipulate();

            string postID = pm.SavePost(newPost);

            List<string> allFolowers = redis.GetRangeFromSortedSet("followers:" + post.UserID, 0, -1);
            allFolowers.Add(post.UserID);

            redis.LPush("timeline", Encoding.UTF8.GetBytes(postID));//globalna lista postova korisnici mogu da vide objave svih korisnika bez obzila da li ih prate ili ne
            //meni treba lPush ovo dole je RPush

            //redis.PushItemToList("timeline", postID);

            redis.TrimList("timeline", 0, 500);

            foreach (string followerId in allFolowers)
            {
                redis.LPush("posts:" + followerId, Encoding.UTF8.GetBytes(postID));
            }
            return pm.LoadPost(postID);
        }
        public List<Post> GetTimeLinePosts(int start,int stop)//vraca praznu listu ako nema nijednog posta (lako moze da se ubaci stranicenje)
        {
            List<Post> timeLinePosts = new List<Post>();
            PostManipulate pm = new PostManipulate();
            byte[][] timeline = redis.LRange("timeline", start, stop);//za sad vracam sve postove

            foreach (byte[] post in timeline)
            {
                string postId = Encoding.UTF8.GetString(post);

                timeLinePosts.Add(pm.LoadPost(postId));
            }
            return timeLinePosts;
        }
        public List<Post> GetAllUserPosts(string userId, int start, int stop)
        {
            List<Post> userPosts = new List<Post>();
            PostManipulate pm = new PostManipulate();
            byte[][] timeline = redis.LRange("posts:" + userId, start, stop);

            foreach (byte[] post in timeline)
            {
                string postId = Encoding.UTF8.GetString(post);

                userPosts.Add(pm.LoadPost(postId));
            }
            return userPosts;

        }
        public User LogIn(string username,string password)
        {
            string userId = redis.Get<string>("username:" + username);
            if (userId == null)
                return null;

            User user = redis.Get<User>("user:" + userId);

            if (user.Password != password)
                return null;

            return user;
        }
        public bool IsLogedIn(string auth)
        {

            string userId = redis.Get<string>("auth:" + auth);
            if (userId == null)//ako nemam setovan auth: za dati auth znaci user nije ulogovan sigurno
                return false;

            User user = redis.Get<User>("user:" + userId);

            if (user.Auth != auth)//ako se auth kod usera i cookie ne slazu znaci da se user u medjuvremenu izlogovao ili slucajno postoji ista vrednost auth u tabeli(od nekog dr user-a)
                return false;

            return true;
        }
        public void LogOut(string userId)
        {
            User user = redis.Get<User>("user:" + userId);
            string oldauth = user.Auth;
            user.Auth = Tools.Tools.RandomString(20);

            redis.Set("user:" + userId, user);
            redis.Set("auth:" + user.Auth,userId);

            redis.Del("auth:" + oldauth);

        }
        public User getUserIdFromAuthToken(string auth)
        {
            string userId = redis.Get<string>("auth:" + auth);
            
            return  LoadUser(userId);
        }
        public bool UserNameIsFree(string userName)
        {
           string userId = redis.Get<string>("username:" + userName);
            if (userId == null)
                return true;
            else
                return false;
        }
        //public void AddFollower(Follow f)
        //{
            
        //    redis.AddItemToSortedSet("followers:" + f.UserID, f.FollowerID, Tools.Tools.TimeNowInUnix());

        //    redis.AddItemToSortedSet("following:" + f.FollowerID, f.UserID,Tools.Tools.TimeNowInUnix());
        //}

        public void AddFollowing(Follow f)
        {
            redis.AddItemToSortedSet("following:" + f.UserID, f.FollowingID, Tools.Tools.TimeNowInUnix());

            redis.AddItemToSortedSet("followers:" + f.FollowingID, f.UserID, Tools.Tools.TimeNowInUnix());
        }
        public void RemoveFollowing(Follow f)
        {
            redis.RemoveItemFromSortedSet("following:" + f.UserID, f.FollowingID);
            redis.RemoveItemFromSortedSet("followers:" +  f.FollowingID,f.UserID);
        }

        public List<User> GetAllFollowingUsers(string userId)
        {
            List<string> allFolowing = redis.GetRangeFromSortedSet("following:" + userId, 0, -1);
            List<User> allUser = new List<User>(); 
            foreach (string followingId in allFolowing)
            {
                User u = LoadUser(followingId);
                u.Password = null;
                u.Auth = null;
                allUser.Add(u);
            }

            return allUser;
        }
        public List<User> GetAllFollowerUsers(string userId)
        {
            List<string> allFolowing = redis.GetRangeFromSortedSet("followers:" + userId, 0, -1);
            List<User> allUser = new List<User>();
            foreach (string followingId in allFolowing)
            {
                User u = LoadUser(followingId);
                u.Password = null;
                u.Auth = null;
                allUser.Add(u);
            }

            return allUser;
        }
    }
}








//    redis.HMSet("user:" + userID,

//    new byte[][] {
//    Encoding.ASCII.GetBytes("auth"),
//    Encoding.ASCII.GetBytes("username"),
//    Encoding.ASCII.GetBytes("password"),
//    Encoding.ASCII.GetBytes("firstname"),
//    Encoding.ASCII.GetBytes("lastname")
//    },

//    new byte[][] {
//    Encoding.ASCII.GetBytes(newUser.Auth),
//    Encoding.ASCII.GetBytes(newUser.UserName),
//    Encoding.ASCII.GetBytes(newUser.Password),
//    Encoding.ASCII.GetBytes(newUser.FirstName),
//    Encoding.ASCII.GetBytes(newUser.LastName)
//    });

//newUser.ID = userID;