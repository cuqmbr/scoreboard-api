using DatabaseModels.InitialObjects;
using Microsoft.EntityFrameworkCore;

namespace Server.Data;

public class ServerDbContext : DbContext
{
    public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ScoreboardRecord> Scoreboard { get; set; } = null!;
}