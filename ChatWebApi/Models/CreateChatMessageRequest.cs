using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatWebApi.Models
{
    public class CreateChatMessageRequest
    {
        [Required]
        public string Text { get; set; }
    }
}
