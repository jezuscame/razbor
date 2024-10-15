using API.Features.PingPong;
using Responses.AutoMapperProfiles;
using Razbor.DAL;
using Microsoft.EntityFrameworkCore;
using Services.HashingService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Services.TokenService;
using Services.Options.JwtOptions;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly string _corsPolicyName = "AllowAll";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(_corsPolicyName);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();


            // Options Setup
            services.ConfigureOptions<JwtOptionsConfiguration>();
            services.ConfigureOptions<JwtBearerOptionsConfiguration>();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicyName,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IHashingService, HashingService>();
            services.AddSingleton<ITokenService, TokenService>();

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureAutoMapper();
            services.AddHttpContextAccessor();

            services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<GetPingPongHandler>());
        }
    }
}
