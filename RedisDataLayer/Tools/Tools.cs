using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataLayer.Tools
{
    public class Tools
    {
        //Vraca random string
        public static string RandomString(int length)
        {
            string path = Path.GetRandomFileName();
            path +=Path.GetRandomFileName();
          
            path = path.Replace(".", ""); // Remove period.

            path.Substring(0, length);
            return path;


            //const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            //var random = new Random();
            //return new string(Enumerable.Repeat(chars, length)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // unix time
        public static Int32 TimeNowInUnix()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

    }
}
