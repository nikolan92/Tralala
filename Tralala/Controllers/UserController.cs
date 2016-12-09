using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using RedisDataLayer.DataObject;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Net.Http.Headers;
using System.Net.Http;
using RedisDataLayer.DataManipulate;
namespace Tralala.Controllers
{
    public class UserController : ApiController
    {
        // Post: User
        public User GetUser()
        {       
            string userAuth;
            CookieHeaderValue cookie1 = Request.Headers.GetCookies("auth").FirstOrDefault();
            if (cookie1 != null)
            {
                UserManipulate um = new UserManipulate();
                userAuth = cookie1["auth"].Value;

                if (um.IsLogedIn(userAuth))
                {
                    User user = um.getUserIdFromAuthToken(userAuth);
                    user.Password = null;

                    return user;
                }
                else
                    return null;
            }
            return null;

        }
        public void GetUserLogOut(string userId)
        {
            UserManipulate um = new UserManipulate();
            um.LogOut(userId);      
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostLogIn([FromBody] User user)
        {

            var resp = new HttpResponseMessage();
            //treba da se provere polja ..
            if (user == null)
            {
                resp.Content = new StringContent("{ \"success\":\"false\", \"message\":\"Please enter your Username and Password!\"}");
                return resp;
            }
            UserManipulate um = new UserManipulate();
            user = um.LogIn(user.UserName, user.Password);
            if (user != null)
            {
                //create and set cookie in response 
                var cookie = new CookieHeaderValue("auth", user.Auth);
                cookie.Expires = DateTimeOffset.Now.AddDays(1);
                //cookie.Domain = Request.RequestUri.Host; ovo je zebavalo 
                cookie.Path = "/";

                resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });

                resp.Content = new StringContent("{\"success\":\"true\"}");
                Uri baseUrl = Request.RequestUri;
                resp.Headers.Location = new Uri("http://" + baseUrl.Authority + "/Home/UserHome");

                return resp;
            }
            resp.Content = new StringContent("{ \"success\":\"false\",  \"message\":\"Wrong Username or Password!\" }");
            return resp;
        }
    }
}