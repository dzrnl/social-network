using Microsoft.EntityFrameworkCore;
using SocialNetwork.Infrastructure.DataAccess.Configurations;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess;

public class SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

    public DbSet<FriendRequestEntity> FriendRequests { get; set; }

    public DbSet<MessageEntity> Messages { get; set; }

    public DbSet<PostEntity> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new FriendRequestConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new PostConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}