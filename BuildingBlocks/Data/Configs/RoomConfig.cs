using BuildingBlocks.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Data.Configs;

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Room");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(x => x.CreatorId).IsRequired();
        builder.Property(x => x.MemberId).IsRequired();

        builder.HasOne(room => room.Creator)
            .WithMany(user => user.RoomCreated)
            .HasForeignKey(room => room.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(room => room.Member)
            .WithMany(room => room.RooJoined)
            .HasForeignKey(room => room.MemberId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}