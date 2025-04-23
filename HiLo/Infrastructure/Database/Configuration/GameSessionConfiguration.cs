using HiLo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiLo.Infrastructure.Database.Configuration;

public class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.HasKey(gs => gs.SessionId);
        
        builder
            .HasMany(gs => gs.Players)
            .WithMany();
    }
}