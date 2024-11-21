using Core.Helpers;
using Core.Model.Schemas;
using Microsoft.AspNetCore.Identity;

namespace AbnbMock;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        builder.WebHost.ConfigureKestrel(o =>
        {
            o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
        });
        
        services.AddAuthentications(configuration);
        services.AddDbContext<ReservoirDbContext>();
        services.AddIdentity<User, Roles>(o =>
        {
            o.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<ReservoirDbContext>();
        
        services.AddServices<User>();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}