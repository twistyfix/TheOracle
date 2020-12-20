using Microsoft.EntityFrameworkCore;
using System.IO;
using TheOracle.BotCore;
using TheOracle.GameCore.Assets;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.UserContent;

public class ThirdPartyContentContext : DbContext
{
    public ThirdPartyContentContext()
    {

    }
    public ThirdPartyContentContext(DbContextOptions<ThirdPartyContentContext> dbContextOptions) : base(dbContextOptions)
    {
        DbContextOptions = dbContextOptions;
        Database.EnsureCreated();
    }
    public DbSet<UserSubscriptions> UserSubscriptions { get; set; }
    public DbSet<Asset> UserAssets { get; set; }
    public DbSet<AssetField> AssetField { get; set; }
    public DbSet<StandardOracle> UserOracles { get; set; }

    public DbContextOptions<ThirdPartyContentContext> DbContextOptions { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=ThirdPartyContent.sqlite;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<StandardOracle>().Property(p => p.Id).ValueGeneratedOnAdd();
    }
}

public class DiscordChannelContext : DbContext
{
    public DiscordChannelContext()
    {

    }
    public DiscordChannelContext(DbContextOptions<DiscordChannelContext> dbContextOptions) : base(dbContextOptions)
    {
        DbContextOptions = dbContextOptions;
        Database.EnsureCreated();
    }

    public DbSet<ChannelSettings> ChannelSettings { get; set; }
    public DbContextOptions<DiscordChannelContext> DbContextOptions { get; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"Data Source=ChannelSettings.sqlite;");
    }
}