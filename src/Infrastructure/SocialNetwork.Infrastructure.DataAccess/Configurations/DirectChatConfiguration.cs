using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Configurations;

public class DirectChatConfiguration : IEntityTypeConfiguration<DirectChatEntity>
{
    public void Configure(EntityTypeBuilder<DirectChatEntity> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.HasIndex(c => new { c.User1Id, c.User2Id })
            .IsUnique();
        
        builder
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}