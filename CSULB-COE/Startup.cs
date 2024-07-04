using CSULB_COE.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtFocus.Common.Utilities.Implementation;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.DocumentManager;
using ThoughtFocus.Repository.Implementation;
using ThoughtFocus.Repository.Interfaces;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.HttpOverrides;

namespace CSULB_COE
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
            services.AddHttpClient();
            //services.AddCors(); default CORS 
            // Default Policy added recently for lunerability tests
            services.AddCors(options => 
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8081", "http://180.151.61.78:7852", "http://20.25.58.133", "https://ced-tf01d.campus.ad.csulb.edu", "https://myced.ced.csulb.edu") // prod also needs to be added here 
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
            services.AddControllers().AddNewtonsoftJson();


            // adding db context 
            services.AddDbContext<CSULB_DBContext>(options =>
            {
                options.UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetConnectionString("AppDBConnection"),
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure();

                            });
            });

            // Adding JWT Token

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });

            // user login service 
            services.AddScoped<IUserRepository, UserRepositoryImpl>();
            services.AddScoped<ThoughtFocus.Repository.Interfaces.User.IUserDetailsRepository, ThoughtFocus.Repository.Implementation.User.UserDetailsImpl>();
            services.AddScoped<ThoughtFocus.Repository.Interfaces.User.IUserActivityRepository, ThoughtFocus.Repository.Implementation.User.UserActivityImpl>();
            services.AddTransient<IUserLoginService, UserLoginServiceImpl>();

            // Application service 
            services.AddScoped<IApplicationService, ApplicationServiceImpl>();

            // program service 
            services.AddScoped<IProgramsService, ProgramServiceImpl>();

            // forms service 
            services.AddScoped<IFormsService, FormsServiceImpl>();

            //user service 
            services.AddScoped<IUserService, UserServiceImpl>();

            //Field Work service 
            services.AddScoped<IFieldWorkService, FieldWorkServiceImpl>();

            // DBUtility 
            services.AddScoped<ISqlDBUtility, SqlDBUtility>();

            //Document Service 
            services.AddScoped<IDocumentService, DocumentServiceImpl>();

            //document converter
            services.AddScoped<IFileConverter, FIleConverter>();

            // notifications 
            services.AddScoped<ISendMail, SendMail>();

            // common utilities
            services.AddScoped<ICommonUtils, CommonUtils>();

            // Graduate Programs 
            services.AddScoped<IGraduateProgramService, GraduateProgramServiceImpl>();

            // Initial Credential Programs
            services.AddScoped<IInitialCredentialProgramService, InitialCredentialProgramService>();

            // Search Application
            services.AddScoped<ISearchApplication, SearchApplicationImpl>();

            // Admin
            services.AddScoped<IAdminService, AdminServiceImpl>();

            // Milestones
            services.AddScoped<IMilestonesService, MilestonesServiceImpl>();

            // Travel
            services.AddScoped<ITravelService, TravelServiceImpl>();

            //Student Profile
            services.AddScoped<IStudentProfile, StudentProfileImpl>();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ThoughtFocus CSULB-CED",
                    Version = "v1",
                    Description = "ThoughtFocus CSULB-CED"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // for http request headers
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

            // validate the appsettings for turning On/Off the http requests tracking
            string check=this.Configuration["TrackIncomingRequests"];
            // using the middleware for http-request handling
            if(check=="True")
                app.UseMiddleware<RequestHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            #region CORS Old codes allowing all origins 
            // global cors policy allowing all the origins 
            //app.UseCors(x => x
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .SetIsOriginAllowed(origin => true) // allow any origin
            //    .AllowCredentials()); // allow credentials
            #endregion

            // added the CORS policy above in configure services section
            app.UseCors();

            // Content Security Policy implemented for security vulnerability
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';"); // provide "default-src 'self' cdn.jsdelivr.net;" if you want to load the resources from the CDN 
            //    await next();
            //});

            // Anti click-jacking headers X-Frame-Headers vulnerability 
            // ------------------uncomment after testing --------------
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';"); // provide "default-src 'self' cdn.jsdelivr.net;" if you want to load the resources from the CDN 
            //    context.Response.Headers.Add("X-Frame-Options", "DENY"); // if framed then "SAMEORIGIN" if not framed then "DENY"
            //    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            //    await next();
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThoughtFocus CSULB-CED");
                c.SwaggerEndpoint("./v1/swagger.json", "ThoughtFocus CSULB-CED");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
