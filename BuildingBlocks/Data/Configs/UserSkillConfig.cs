using BuildingBlocks.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Data.Configs;

public class UserSkillConfig : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.ToTable("UserSkill");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(x => x.Information).IsRequired(false);
        builder.Property(x => x.Skill).IsRequired(false);
        builder.Property(x => x.PercentSkill).IsRequired();
        builder.Property(x => x.Project).IsRequired(false);
        builder.Property(x => x.Time).IsRequired(false);
        builder.Property(x => x.TitleProject).IsRequired(false);
        builder.Property(x => x.ShortTitle).IsRequired(false);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(userSkill => userSkill.User)
            .WithMany(user => user.UserSkills)
            .HasForeignKey(userSkill => userSkill.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}