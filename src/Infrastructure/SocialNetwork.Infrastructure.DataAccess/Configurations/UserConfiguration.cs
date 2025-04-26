using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .HasMany(u => u.Friends)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Friendship",
                join => join
                    .HasOne<UserEntity>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join
                    .HasOne<UserEntity>()
                    .WithMany()
                    .HasForeignKey("FriendId")
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}