using System;
using System.Linq;
using System.Security.Claims;
using ChatWebApi.Entities;
using Newtonsoft.Json;

namespace ChatWebApi.Helpers
{
    public class AuthHelper
    {
        public static User GetUser(ClaimsPrincipal user)
        {
            return JsonConvert.DeserializeObject<User>(user?.Identities.FirstOrDefault(t => t.AuthenticationType == "JwtAuthentication")?.Claims.FirstOrDefault(t => t.Type == "User")?.Value);
        }
    }
}
