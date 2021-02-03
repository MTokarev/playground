using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using playground.Data;
using playground.Interfaces;
using playground.OptionsConfig;
using playground.Services;
using System.Text;
using System.Threading.Tasks;

namespace playground.Extensions
{
    public static class CustomServices
    {
        public static IServiceCollection RegisterCustomServices(this IServiceCollection services,
            IConfiguration config)
        {
            // Getting Symmetric key from appsettings
            // This requered to overcome DI which is impossible in static classes (correct me if I wrong)
            var cryptoOptions = Options.Create(config.GetSection("CryptoOptions").Get<CryptoOptions>());

            // Binding CryptoOptions from appsettings
            services.Configure<CryptoOptions>(options => config.GetSection("CryptoOptions").Bind(options));

            // Adding authenticationW
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var tcs = new TaskCompletionSource<object>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            cryptoOptions.Value.Key)),
                        ValidateIssuer = false,
                        ValidateAudience = true,
                        ValidAudience = cryptoOptions.Value.ValidAudience,
                        ValidateLifetime = true
                    };

                    // This section required to include bearer token from cookie for each validation request
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                            {
                                context.Token = context.Request.Cookies["X-Access-Token"];
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            // Adding Token Service
            services.AddScoped<ITokenService, TokenService>();

            // Add http context
            services.AddHttpContextAccessor();

            // Add cookies service
            services.AddScoped<ICookiesService, CookiesService>();

            // Configuring SQLite
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            
            // Adding User Service
            services.AddScoped<IUserService, UserService>();

            // Adding Email Service
            services.AddTransient<IEmailService, SendGridService>();
            services.Configure<SendgridOptions>(options => config.GetSection("SendGrid").Bind(options));

            // Adding Key Service (Reset Password)
            services.AddScoped<IActionKeyService, ActionKeyService>();

            return services;
        }
    }
}
