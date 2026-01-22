
using Application;
using Application.Abstrations;
using Application.Mappers;
using Application.Persistences.Repositories;
using DotNetEnv;
using Identity_JWT.Middlewares;
using Infrastructure.ApplicationDbContext;
using Infrastructure.Interceptor;
using Infrastructure.Persistences;
using Infrastructure.Persistences.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Text;
namespace Identity_JWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load("../.env");
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Connect PostgreSQL 
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options => options.UseNpgsql(connectionString));

            // Redis Cache
            var redisConn = builder.Configuration["REDIS_CONFIGURATION"] ?? "localhost:6379";

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
                options.InstanceName = "identity_redis";
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConn));

            // ----- Repositories -----
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // ----- Services -----
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            //middleware
            builder.Services.AddSingleton<GlobalErrorHandlerMiddleware>();

            // Add memory cache
            builder.Services.AddMemoryCache();

            // ----- AutoMapper -----
            builder.Services.AddAutoMapper(typeof(UserProfile));

            //Interceptor
            builder.Services.AddScoped<UpdateTimestampInterceptor>();




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
}
