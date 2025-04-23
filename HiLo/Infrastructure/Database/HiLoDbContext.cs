using HiLo.Domain;
using HiLo.Infrastructure.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HiLo.Infrastructure.Database;

public class HiLoDbContext : DbContext
{
    public virtual DbSet<Player> Players { get; set; }
    public virtual DbSet<GameSession> Sessions { get; set; }

    public HiLoDbContext() { }
    
    public HiLoDbContext(DbContextOptions<HiLoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameSessionConfiguration).Assembly);
    }
}