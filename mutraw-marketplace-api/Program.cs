using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mutraw_marketplace_api.Data;
using mutraw_marketplace_api.Models;
using mutraw_marketplace_api.Repositories;
using mutraw_marketplace_api.Utilities;

namespace mutraw_marketplace_api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", b =>
            {
                b.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        // Setting up Dependency Injection
        builder.Services.AddIdentity<Employee, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<AppDbContext>();

        // Cookie settings - But we are not using this
        // builder.Services.ConfigureApplicationCookie(options =>
        // {
        //     options.LoginPath = "/auth/login";
        //     options.LogoutPath = "/auth/logout";
        //     options.AccessDeniedPath = "/auth/access-denied";
        //     options.SlidingExpiration = true;
        //     options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        //     options.Cookie.Name = "mutraw-marketplace-cookie";
        //     options.Cookie.HttpOnly = true;
        //     options.Cookie.SameSite = SameSiteMode.Strict;
        //     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // });
        
        builder.Services.AddScoped<ICredentialRepo, CredentialRepo>();
        builder.Services.AddScoped<IRepository<Product>, ProductRepo>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var secret = builder.Configuration.GetValue<string>("SecretKey") ?? "";
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin", "true"));
            options.AddPolicy("OwnerOrAdminPolicy", policy => policy.Requirements.Add(new OwnerOrAdminRequirement()));
        });
        builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();
        builder.Services.AddHttpContextAccessor();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mutraw Marketplace API v1");
                c.RoutePrefix = string.Empty;
            });
        }
        
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseCors("AllowAngular");
        app.MapControllers();

        app.Run();
    }
}