using Microsoft.EntityFrameworkCore;

namespace ChatWebApi.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
               .HasOne(m => m.Chat)
               .WithMany(c => c.Messages)
               .HasForeignKey(m => m.ChatId);
            modelBuilder.Entity<Message>()
              .HasOne(m => m.User)
              .WithMany(c => c.Messages)
              .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<ChatUser>()
               .HasKey(x => new { x.ChatId, x.UserId });
        }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}
