using BuildingBlocks.Data.Configs;
using BuildingBlocks.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Comment> Comments { set; get; }
    public DbSet<Message> Messages { set; get; }
    public DbSet<Room> Rooms { set; get; }
    public DbSet<News> News { set; get; }
    public DbSet<Post> Posts { set; get; }
    public DbSet<User> Users { set; get; }
    public DbSet<UserSkill> UserSkills { set; get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommentConfig());
        modelBuilder.ApplyConfiguration(new MessageConfig());
        modelBuilder.ApplyConfiguration(new PostConfig());
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new UserSkillConfig());
        modelBuilder.ApplyConfiguration(new RoomConfig());
        modelBuilder.ApplyConfiguration(new NewsConfig());
        modelBuilder.Seed();
    }
}