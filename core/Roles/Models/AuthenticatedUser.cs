using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using core.Logging;

namespace core.Roles.Models
{
    public interface IValidatableUser
    {
        string GetGoogleId();
    }

    public class AuthenticatedUser : IValidatableUser
    {
        public string Token { get; set; }

        /// <summary>
        /// Returns the Google ID of the given user for the given token.
        /// </summary>
        /// <returns>The Google ID of the user</returns>
        public string GetGoogleId()
        {
            if (string.IsNullOrEmpty(Token))
            {
                return null;
            }
            try
            {
                var request = WebRequest.Create("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + Token);
                request.Method = "GET";
                var userid = "";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(reader.ReadToEnd());
                        userid = obj["user_id"];
                    }
                }

                return string.IsNullOrEmpty(userid) ? null : userid;
            }
            catch (Exception ex)
            {
                LogUtility.Log("AuthenticatedUser", "GetGoogleId", ex.ToString());
                return null;
            }
        }
    }
}