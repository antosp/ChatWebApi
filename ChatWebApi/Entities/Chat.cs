using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebApi.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        [NotMapped]
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; }
    }
}
