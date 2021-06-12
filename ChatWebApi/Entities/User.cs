using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ChatWebApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public List<Message> Messages { get; set; }
        [NotMapped]
        public List<Chat> Chats { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }

    public class AuthenticatedUser : User
    {
        public string Token { get; set; }

        public AuthenticatedUser(User user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Token = token;
        }
    }

}
