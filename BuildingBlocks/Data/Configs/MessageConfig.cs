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
        builder.Property(x => x.SenderId).IsRequired();
        builder.Property(x => x.ReceiverId).IsRequired();

        builder.HasOne(message => message.Receiver)
            .WithMany(user => user.MessageReceivers)
            .HasForeignKey(message => message.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(message => message.Sender)
            .WithMany(user => user.MessageSends)
            .HasForeignKey(message => message.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}