using AutoMapper;
using Core.Connections.Jwt;
using Core.Services.IRepository;
using Core.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Core.Helpers;

public static class ServiceExtensions
{
    #region Authentications
    
    public static void AddAuthentications(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConnector = configuration.GetSection(nameof(JwtConnector)).Get<JwtConnector>();

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer("Bearer", options =>
        {
            options.IncludeErrorDetails = true;
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtConnector?.Issuer,
                ValidAudience = jwtConnector?.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(jwtConnector?.SecretKey ?? string.Empty)
                ),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                ValidateLifetime = false,
            };
        });
    }

    #endregion

    #region AutoMapper

    private static IServiceCollection AddMapper(this IServiceCollection service)
    {
        var mappingConfig = new MapperConfiguration(m => m.AddProfile(new Mapper()));
        var mapper = mappingConfig.CreateMapper();
        return service.AddSingleton(mapper);
    }

    #endregion

    public static void AddSwagGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AirBnB", Version = "v1" });
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter `Bearer`[space] and then your valid token "
                }
            );
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );
        });
    }

    public static void AddServices<T>(this IServiceCollection services) where T : IdentityUser<Guid>
    {
        services.AddMapper().AddScoped<Jwt<T>>().AddScoped<IAbnb, Abnb>();
    }
}