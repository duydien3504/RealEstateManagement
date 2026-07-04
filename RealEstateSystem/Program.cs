using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.API.Extensions;
using RealEstateSystem.API.Middlewares;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Application.Services;
using RealEstateSystem.Application.Validators;
using RealEstateSystem.Infrastructure.Repositories;
using RealEstateSystem.Infrastructure.Security;

namespace RealEstateSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddSqlServer(builder.Configuration);
            builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddValidatorsFromAssembly(typeof(RegisterValidation).Assembly);
            builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilterAttribute>();
            });

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
