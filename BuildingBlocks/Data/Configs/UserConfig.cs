using BuildingBlocks.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Data.Configs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(x => x.Username).IsRequired();
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Avatar).IsRequired(false);
        builder.Property(x => x.Email).IsRequired(false);
        builder.Property(x => x.Phone).IsRequired(false);
        builder.Property(x => x.AboutMe).IsRequired(false);
        builder.Property(x => x.Address).IsRequired(false);
        builder.Property(x => x.Role).HasDefaultValue(UserRole.Member).IsRequired();
        builder.Property(x => x.Status).HasDefaultValue(UserStatus.Enabled).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}