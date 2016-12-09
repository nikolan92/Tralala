using RedisDataLayer.DataManipulate;
using RedisDataLayer.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Tralala.Controllers
{
    public class RegistrationController : ApiController
    {


        // GET api/Register/?username=dd //moze kao dodatak provere npr prenego sto koriskik kligne register da se proveri da li je slobodan ovaj user name
        [System.Web.Http.HttpGet]
        public string GetUserNameAvailability(string username)
        {
            UserManipulate um = new UserManipulate();
            if(um.UserNameIsFree(username))
                return "{ \"success\":\"true\" }";
            else
                return "{ \"success\":\"false\" }";
        }
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PostUser([FromBody] User user)
        {

            UserManipulate um = new UserManipulate();
            var resp = new HttpResponseMessage();

            if (um.UserNameIsFree(user.UserName))
            {
                //moze da se doda kasnije provera podataka
                string userId = um.SaveUser(user);


                //create and set cookie in response 
                var cookie = new CookieHeaderValue("auth", user.Auth);
                cookie.Expires = DateTimeOffset.Now.AddDays(1);
                //cookie.Domain = Request.RequestUri.Host; ovo je zebavalo 
                cookie.Path = "/";

                resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });

                resp.Content = new StringContent("{\"success\":\"true\"}");

                Uri baseUrl = Request.RequestUri;
                resp.Headers.Location = new Uri("http://" + baseUrl.Authority + "/Home/UserHome");
                //
                ///resp.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = userId }));
            }
            else
            {
                resp.Content = new StringContent("{\"success\":\"false\", \"message\":\"This UserName is used by another user!\"}");
            }
            return resp;
        }

    }
}