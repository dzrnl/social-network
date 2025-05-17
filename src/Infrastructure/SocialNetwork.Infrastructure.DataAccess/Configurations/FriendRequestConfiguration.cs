using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Configurations;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequestEntity>
{
    public void Configure(EntityTypeBuilder<FriendRequestEntity> builder)
    {
        builder.HasKey(fr => fr.Id);

        builder.Property(fr => fr.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(fr => fr.FromUser)
            .WithMany(u => u.SentRequests)
            .HasForeignKey(fr => fr.FromUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fr => fr.ToUser)
            .WithMany(u => u.ReceivedRequests)
            .HasForeignKey(fr => fr.ToUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(fr => new { fr.FromUserId, fr.ToUserId })
            .IsUnique();
    }
}