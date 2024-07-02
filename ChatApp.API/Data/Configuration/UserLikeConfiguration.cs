using ChatApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.API.Data.Configuration;
public class UserLikeConfiguration : IEntityTypeConfiguration<UserLike>
{
    public void Configure(EntityTypeBuilder<UserLike> builder)
    {
        builder
            .HasKey(k => new {k.SourceUserId, k.TargetUserId});

        builder
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(f => f.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(t => t.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(f => f.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}