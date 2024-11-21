using Core.Connections.Database;
using Core.Model.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Core.Helpers;

public class ReservoirDbContext(DbContextOptions<ReservoirDbContext> options, IConfiguration configuration)
    : IdentityDbContext<User, Roles, Guid, IdentityUserClaim<Guid>, IdentityUserRoles, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            Id = userId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            CreatedDate = DateTime.UtcNow
        };

        user.PasswordHash = hasher.HashPassword(user, "admin");

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Roles>().HasData(new Roles
            { Id = roleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR".ToUpper() });

        modelBuilder.Entity<User>().HasData(user);

        modelBuilder.Entity<IdentityUserRoles>().HasData(
            new IdentityUserRoles
            {
                RoleId = roleId,
                UserId = userId
            }
        );

        modelBuilder.Entity<BookingInfos>().HasOne(e => e.RoomType).WithOne(e => e.BookingInfos).HasForeignKey<BookingInfos>(e => e.RoomId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Prices>().HasOne(e => e.RoomType).WithOne(e => e.Prices).HasForeignKey<RoomType>(e => e.PriceId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Reservations>().HasOne(e => e.BookInfos).WithOne(e => e.Reservations).HasForeignKey<Reservations>(e => e.BookingId);
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connector = configuration.GetSection(nameof(Database)).Get<Database>();

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = connector?.Host,
            Database = connector?.DataBase,
            Username = connector?.UserName,
            Password = connector?.Password,
        };

        optionsBuilder.EnableSensitiveDataLogging();

        optionsBuilder.UseLazyLoadingProxies().UseNpgsql(builder.ConnectionString);
    }
    
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<BookingInfos> BookingInfos { get; set; }
    public DbSet<Reservations> Reservations { get; set; }
    public DbSet<Prices> Prices { get; set; }
}