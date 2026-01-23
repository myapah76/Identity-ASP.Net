using DotNetEnv;
using Identity_JWT.Middlewares;
using IdentityService.Application;
using IdentityService.Application.Abstrations;
using IdentityService.Application.Helpers;
using IdentityService.Application.Mappers;
using IdentityService.Application.Persistences.Caches;
using IdentityService.Application.Persistences.Repositories;
using IdentityService.Infrastructure.ApplicationDbContext;
using IdentityService.Infrastructure.Interceptor;
using IdentityService.Infrastructure.Persistences.Cache;
using IdentityService.Infrastructure.Persistences.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using StackExchange.Redis;
using System.Text;

namespace IdentityService.API 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load("../.env");

            var builder = WebApplication.CreateBuilder(args);

            // Controllers (REST)
            builder.Services.AddControllers();

            builder.Services.AddSingleton<GlobalErrorHandlerMiddleware>();
            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Identity API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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

            // ----- JWT Configuration -----
            var jwtSecretKey = builder.Configuration["JWT_SECRET_KEY"]
                ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured");

            var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "CFSM-be";
            var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "CFSM-be";

            var jwtAccessTokenExpirationMinutes =
                int.Parse(builder.Configuration["JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"] ?? "15");

            var jwtRefreshTokenExpirationDays =
                int.Parse(builder.Configuration["JWT_REFRESH_TOKEN_EXPIRATION_DAYS"] ?? "7");

            builder.Services.AddSingleton<JwtHelper>(_ =>
                new JwtHelper(jwtSecretKey, jwtIssuer, jwtAudience,
                    jwtAccessTokenExpirationMinutes, jwtRefreshTokenExpirationDays));

            builder.Services.AddScoped<ITokenBlacklist, TokenBlacklist>();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        var authHeader = context.Request.Headers["Authorization"].ToString();

                        if (string.IsNullOrEmpty(authHeader))
                        {
                            logger.LogWarning("No Authorization header found in request");
                            return Task.CompletedTask;
                        }

                        var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                            ? authHeader.Substring(7).Trim()
                            : authHeader.Trim();

                        // Set the token
                        context.Token = token;

                        logger.LogInformation("Token extracted: {TokenPreview}",
                            token.Length > 50 ? token.Substring(0, 50) + "..." : token);

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(context.Exception, "JWT Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("JWT Challenge: {Error}, {ErrorDescription}",
                            context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                        // Check blacklist trước khi cho phép request
                        var tokenBlacklist = context.HttpContext.RequestServices.GetRequiredService<Application.Persistences.Caches.ITokenBlacklist>();

                        // Lấy token từ request headers (vì TokenValidatedContext không có Token property)
                        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
                        var token = string.Empty;
                        if (!string.IsNullOrEmpty(authHeader))
                        {
                            token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                                ? authHeader.Substring(7).Trim()
                                : authHeader.Trim();
                        }

                        if (!string.IsNullOrEmpty(token))
                        {
                            var isBlacklisted = await tokenBlacklist.IsTokenBlacklistedAsync(token);
                            if (isBlacklisted)
                            {
                                logger.LogWarning("Token is blacklisted for UserId: {UserId}", userId);
                                context.Fail("Token has been revoked");
                                return;
                            }
                        }

                        logger.LogInformation("JWT Token validated successfully for UserId: {UserId}", userId);
                    }
                };
            });

            builder.Services.AddAuthorization();

            // PostgreSQL
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Redis
            var redisConn = builder.Configuration["REDIS_CONFIGURATION"] ?? "localhost:6379";
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
                options.InstanceName = "identity_redis";
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConn));

            // Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddAutoMapper(typeof(UserProfile));
            builder.Services.AddScoped<UpdateTimestampInterceptor>();

            var app = builder.Build();

            // Swagger middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
