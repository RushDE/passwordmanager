
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PasswordManagerServer.Data;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;

namespace PasswordManagerServer
{
    /// <summary>
    /// The main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Mainly for configuration, also the entry point.
        /// </summary>
        /// <param args="The command line args."></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1", new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "PasswordManager Vault API",
                            Description = "An ASP.NET Core Web API for managing the users and their passwords from the PaswordManager."
                        }
                    );
                    options.AddSecurityDefinition(
                        "oauth2", new OpenApiSecurityScheme()
                        {
                            Description = "JWT Bearer",
                            In = ParameterLocation.Header,
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey
                        }
                    );
                    options.OperationFilter<SecurityRequirementsOperationFilter>();
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                }

            );
            builder.Services.AddDbContext<DataContext>(
                options => options.UseSqlite(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options => options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                             Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:TokenKey").Value!)
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    }
                );
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}