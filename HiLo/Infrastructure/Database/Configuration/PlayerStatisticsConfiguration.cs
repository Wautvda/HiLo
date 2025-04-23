using HiLo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiLo.Infrastructure.Database.Configuration;

public class PlayerStatisticsConfiguration : IEntityTypeConfiguration<PlayerStatistics>
{
    public void Configure(EntityTypeBuilder<PlayerStatistics> builder)
    {
        builder.HasKey(ps => new { ps.PlayerName, ps.SessionId });

        builder
            .HasOne(ps => ps.Player)
            .WithMany()
            .HasForeignKey(ps => ps.PlayerName);

        builder
            .HasOne(ps => ps.Session)
            .WithMany(gs => gs.Statistics)
            .HasForeignKey(ps => ps.SessionId);
    }
}