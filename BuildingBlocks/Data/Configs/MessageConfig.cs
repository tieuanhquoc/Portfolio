using BuildingBlocks.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Data.Configs;

public class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Message");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(x => x.Content).IsRequired(false);
        builder.Property(x => x.Status).HasDefaultValue(MessageStatus.Sent).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CreatorId).IsRequired();
        builder.Property(x => x.RoomId).IsRequired();

        builder.HasOne(message => message.Creator)
            .WithMany(user => user.Messages)
            .HasForeignKey(message => message.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(message => message.Room)
            .WithMany(room => room.Messages)
            .HasForeignKey(message => message.RoomId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}