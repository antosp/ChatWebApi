using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ChatWebApi.Entities;
using ChatWebApi.Helpers;
using ChatWebApi.Models;

namespace ChatWebApi.Services
{
    public interface IUserService
    {
        AuthenticatedUser Authenticate(string userName, string password);
        User GetById(int id);
        User Create(string userName, string password, string firstName, string lastName);
        List<User> Search(string q);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly AppDbContext _context;

        public UserService(IOptions<AppSettings> appSettings, AppDbContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public AuthenticatedUser Authenticate(string userName, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == userName && x.Password == CryptoHelper.Encrypt(password));

            if (user == null) return null;

            var token = generateJwtToken(user);

            return new AuthenticatedUser(user, token);
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public User Create(string userName, string password, string firstName, string lastName)
        {
            var entityEntry = _context.Add(new User()
            {
                Username = userName,
                FirstName = firstName,
                LastName = lastName,
                Password = CryptoHelper.Encrypt(password)
            });
            _context.SaveChanges();

            return entityEntry.Entity;
        }

        public List<User> Search(string q)
        {
            return _context.Users.Where(x => string.Format("{0} {1} {2}", x.FirstName, x.LastName, x.Username).Contains(q, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}