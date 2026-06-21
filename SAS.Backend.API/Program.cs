
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SAS.Backend.API.Services;
using SAS.Backend.Application;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Application.Common.Settings;
using SAS.Backend.Infrastructure;
using SAS.Backend.Infrastructure.Persistence;
using System.Text;
using Microsoft.OpenApi.Models;

namespace SAS.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SasWeb", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter()
                    );
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "SAS Backend API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                });
            });

            var app = builder.Build();

            var runMigrations = builder.Configuration.GetValue(
                "RunMigrations",
                true
            );

            var migrateOnly = builder.Configuration.GetValue(
                "MigrateOnly",
                false
            );

            if (runMigrations)
            {
                using var scope = app.Services.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                db.Database.Migrate();
            }

            if (migrateOnly)
            {
                return;
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseCors("SasWeb");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet(
                "/health/live",
                () => Results.Ok(new
                {
                    status = "Healthy"
                })
            ).AllowAnonymous();

            app.MapGet(
                "/health/ready",
                async (
                    ApplicationDbContext db,
                    CancellationToken cancellationToken
                ) =>
                {
                    var canConnect =
                        await db.Database.CanConnectAsync(
                            cancellationToken
                        );

                    return canConnect
                        ? Results.Ok(new
                        {
                            status = "Ready"
                        })
                        : Results.StatusCode(
                            StatusCodes.Status503ServiceUnavailable
                        );
                }
            ).AllowAnonymous();

            app.MapGet(
                "/health/instance",
                () => Results.Ok(new
                {
                    instance = Environment.MachineName,
                    timestampUtc = DateTime.UtcNow
                })
            ).AllowAnonymous();

            app.Run();
        }
    }
}