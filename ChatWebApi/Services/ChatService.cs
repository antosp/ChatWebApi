using System;
using System.Collections.Generic;
using System.Linq;
using ChatWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatWebApi.Services
{
    public interface IChatService
    {
        Chat GetChatbyId(int chatId);
        List<Chat> GetUserChats(int userId);
        Chat Create(string title, string type, List<int> userIds);
        Message CreateChatMessage(int chatId, int userId, string messageText);
    }

    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;

        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        public Chat Create(string title, string type, List<int> userIds)
        {
            var entityEntry = _context.Add(new Chat()
            {
                Type = type,
                Title = title,
            });
            _context.SaveChanges();

            var chatId = entityEntry.Entity.Id;
            _context.ChatUsers.AddRange(new List<ChatUser>(userIds.Select(x => new ChatUser() { ChatId = chatId, UserId = x })));
            _context.SaveChanges();

            return entityEntry.Entity;
        }

        public List<Chat> GetUserChats(int userId)
        {
            return _context.ChatUsers
                .Where(x => x.UserId == userId)
                .Join(_context.Chats, x => x.ChatId, y => y.Id, (x, y) => new Chat()
                {
                    Id = y.Id,
                    Title = y.Title,
                    Type = y.Type,
                    Users = _context.ChatUsers
                                    .Where(c => c.ChatId == y.Id)
                                    .Join(_context.Users, a => a.UserId, b => b.Id, (a, b) => b).ToList()
                })
                .ToList();
        }

        public Chat GetChatbyId(int chatId)
        {
            var result = _context.Chats
                .Where(x => x.Id == chatId)
                .Include("Messages")
                .FirstOrDefault();
            result.Users = _context.ChatUsers
                .Where(x => x.ChatId == chatId)
                .Join(_context.Users, x => x.UserId, y => y.Id, (x, y) => y)
                .ToList();
            return result;
        }

        public Message CreateChatMessage(int chatId, int userId, string messageText)
        {
            var entityEntry = _context.Add(new Message()
            {
                ChatId = chatId,
                UserId = userId,
                Text = messageText,
                Created = DateTime.Now
            });
            _context.SaveChanges();

            return entityEntry.Entity;
        }
    }
}