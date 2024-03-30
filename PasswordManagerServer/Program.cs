
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PasswordManagerServer.Data;
using PasswordManagerServer.Helpers;
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
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            _ = builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1", new OpenApiInfo
                        {
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
                    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                }
            );
            _ = builder.Services.AddDbContext<DataContext>(
                options => options.UseSqlite(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );
            _ = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            _ = builder.Services.AddHttpContextAccessor();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseStaticFiles();
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI(
                    options =>
                    {
                        options.SwaggerEndpoint(
                            "/swagger/v1/swagger.json",
                            "PasswordManager Vault API"
                        );
                        if (builder.Configuration.GetValue<bool>("Swagger:DarkMode"))
                        {
                            // Source: https://github.com/Amoenus/SwaggerDark/
                            options.InjectStylesheet("/SwaggerUi/SwaggerDark.css");
                        }
                    }
                );
            }

            Database.Create(app);

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}