using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RealEstateSystem.Api.Extensions;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Application.Services.AuthenService;
using RealEstateSystem.Application.Services.ProfileService;
using RealEstateSystem.Application.Validators;
using RealEstateSystem.Infrastructure.Messaging;
using RealEstateSystem.Infrastructure.Repository;
using RealEstateSystem.Infrastructure.Security;
using RealEstateSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;

namespace RealEstateSystem.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDatabase(builder.Configuration);

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            var secretKey = builder.Configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Cấu hình Jwt:SecretKey không tồn tại.");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidation>();

            var redisConnectionString = builder.Configuration["Redis:ConnectionString"]
                ?? throw new InvalidOperationException("Cấu hình Redis:ConnectionString không tồn tại.");

            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString));

            var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
            var rabbitPort = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");
            var rabbitUsername = builder.Configuration["RabbitMq:Username"] ?? "guest";
            var rabbitPassword = builder.Configuration["RabbitMq:Password"] ?? "guest";

            builder.Services.AddSingleton<IConnection>(serviceProvider =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = rabbitHost,
                    Port = rabbitPort,
                    UserName = rabbitUsername,
                    Password = rabbitPassword
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            builder.Services.AddScoped<IHasherPassword, PasswordHasher>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
            builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
            builder.Services.AddTransient<IMailService, SmtpMailService>();
            builder.Services.AddScoped<RegisterService>();
            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<VerifyOtpService>();
            builder.Services.AddScoped<ForgetPasswordService>();
            builder.Services.AddScoped<VerifyChangePasswordService>();
            builder.Services.AddScoped<ChangePasswordService>();
            builder.Services.AddScoped<IAuthenService, AuthenService>();
            builder.Services.AddScoped<GetProfileService>();
            builder.Services.AddScoped<UpdateProfileService>();
            builder.Services.AddScoped<DeleteProfileService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IEncryptEmail, HmacEncrypt>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập access token JWT của bạn."
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
