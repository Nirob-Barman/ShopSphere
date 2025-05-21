
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Infrastructure.Identity.Entity;

namespace ShopSphere.Infrastructure.Persistence.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Token);
            builder.Property(rt => rt.Token).IsRequired();
            builder.Property(rt => rt.UserId).IsRequired();
            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.IsRevoked).HasDefaultValue(false);
            builder.Property(rt => rt.IsUsed).HasDefaultValue(false);
            builder.Property(rt => rt.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
