using ChatWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ChatWebApi.Hubs
{
    [Authorize]
    public class ChatHub: Microsoft.AspNetCore.SignalR.Hub
    {
        public ChatHub(IChatService chatService)
        {
            this._chatService = chatService;
        }

        private IChatService _chatService;
 
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Join(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task Leave(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task SendMessage(int chatId, string messageText)
        {
            var user = Helpers.AuthHelper.GetUser(Context.User);
            var message = _chatService.CreateChatMessage(chatId, user.Id, messageText);
            await Clients.Group(chatId.ToString()).SendAsync("OnMessage", message);
        }
    }
}
