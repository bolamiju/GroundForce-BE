using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Groundforce.Services.Data;
using Groundforce.Services.Data.Services;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Groundforce.Services.Core;

namespace Groundforce.Services.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            // db connection string
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DbConnection")));
            

            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IMissionRepository, MissionRepository>();
            services.AddScoped<IAgentRepository, AgentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddTransient<IMailService, MailService>();
            services.AddScoped(typeof(IAllRepo<>), typeof(AllRepo<>));
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<IPasswordVerificationRepository, PasswordVerificationRepository>();
            services.AddScoped<IBankDetailsService, BankDetailsService>();
            //services.AddScoped<IAdminRepository, AdminRepository>();

            // Identity service
            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequiredLength = 4;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
            }
            ).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddCors();

            //register cloudinary
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            //register sendGrid
            services.Configure<SendGridSettings>(Configuration.GetSection("SendGridSettings"));

            //register bankdetails
            services.Configure<BankDetailsSettings>(Configuration.GetSection("BankDetailsSettings"));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Groundforce API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}
                    }
                });
            });

            // JWT token service
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = Configuration["Jwt:Site"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext ctx,
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var err = context.Features.Get<IExceptionHandlerFeature>();

                        if (err != null)
                        {
                            await context.Response.WriteAsync($"{err.Error.Source} {err.Error.StackTrace} {err.Error.Message}");
                        }
                    });
                });
            }

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Groundforce Api V1");
            });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            PreSeeder.Seeder(ctx, roleManager, userManager).Wait();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
