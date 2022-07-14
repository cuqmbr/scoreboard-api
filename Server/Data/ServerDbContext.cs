using DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Server.Data;

public class ServerDbContext : DbContext
{
    public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options) { }

    public DbSet<ScoreboardRecord> Scoreboard { get; set; } = null!;
}