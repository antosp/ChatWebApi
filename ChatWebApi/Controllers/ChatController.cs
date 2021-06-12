using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatWebApi.Services;
using ChatWebApi.Models;
using System.Linq;

namespace ChatWebApi.Controllers
{
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private IUserService _userService;
        private IChatService _chatService;

        public ChatController(IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
        }

        [HttpGet]
        [Route("api/chat/search-people")]
        public IActionResult SearchPeople([FromQuery] string q)
        {
            return Ok(_userService.Search(q == null ? string.Empty : q));
        }

        [HttpPost]
        [Route("api/chat")]
        public IActionResult CreateChat([FromBody]CreateChatRequest request)
        {
            var user = Helpers.AuthHelper.GetUser(HttpContext.User);
            return Ok(_chatService.Create(request.Title, request.Type, request.UserIds));
        }

        [HttpGet]
        [Route("api/chat")]
        public IActionResult GetUserChats()
        {
            var user = Helpers.AuthHelper.GetUser(HttpContext.User);
            return Ok(_chatService.GetUserChats(user.Id));
        }

        [HttpGet]
        [Route("api/chat/{chatId}")]
        public IActionResult GetChat(int chatId)
        {
            return Ok(_chatService.GetChatbyId(chatId));
        }

        [HttpPost]
        [Route("api/chat/{chatId}/messages")]
        public IActionResult CreateChatMessage(int chatId, [FromBody]CreateChatMessageRequest request)
        {
            var user = Helpers.AuthHelper.GetUser(HttpContext.User);
            return Ok(_chatService.CreateChatMessage(chatId, user.Id, request.Text));
        }
    }
}
