using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatWebApi.Models
{
    public class CreateChatRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public List<int> UserIds { get; set; }
    }
}
