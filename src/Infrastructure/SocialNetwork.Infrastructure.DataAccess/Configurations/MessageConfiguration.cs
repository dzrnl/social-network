using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId)
            .IsRequired(false);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(m => m.SentAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}