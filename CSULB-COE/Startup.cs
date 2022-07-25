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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Repository.Implementation;
using ThoughtFocus.Repository.Interfaces;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;
using ThoughtFocus.Common.WorkFlowDataAccess;
using ThoughtFocus.Workflow;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Common;

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

            services.AddDbContext<WorkFlowContext>(options =>
            {
                options.UseLazyLoadingProxies()
                   .UseSqlServer(Configuration.GetConnectionString("AppDBConnection"),
                               sqlServerOptionsAction: sqlOptions =>
                               { sqlOptions.EnableRetryOnFailure(); });
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

            // configure strongly typed settings object
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.Configure<SqlConnectionStrings>(Configuration.GetSection("ConnectionStrings"));


            services.AddSingleton<SqlConnectionStrings>(new SqlConnectionStrings()
            {
                AppDBConnection = Configuration.GetConnectionString("AppDBConnection")
            });

            // user login service 
            services.AddScoped<IUserRepository, UserRepositoryImpl>();
            services.AddScoped<ThoughtFocus.Repository.Interfaces.User.IUserDetailsRepository, ThoughtFocus.Repository.Implementation.User.UserDetailsImpl>();
            services.AddScoped<ThoughtFocus.Repository.Interfaces.User.IUserActivityRepository, ThoughtFocus.Repository.Implementation.User.UserActivityImpl>();
            services.AddTransient<IUserLoginService, UserLoginServiceImpl>();


            services.AddScoped<WorkflowInit>();
            services.AddScoped<WorkflowRole>();
            services.AddScoped<WorkflowRule>();
            services.AddScoped<WorkflowActions>();

            // Application service 
            services.AddScoped<IApplicationService, ApplicationServiceImpl>();
            // Workflow service
            var serviceProvider = services.BuildServiceProvider();
            DependencyHelper.WorkflowInit = serviceProvider.GetService<WorkflowInit>();
            DependencyHelper.WorkflowRole = serviceProvider.GetService<WorkflowRole>();
            DependencyHelper.WorkflowRule = serviceProvider.GetService<WorkflowRule>();
            DependencyHelper.SqlConnectionStrings = serviceProvider.GetService<SqlConnectionStrings>();
            DependencyHelper.WorkflowActions = serviceProvider.GetService<WorkflowActions>();
            


            // Enable Swagger   
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ThoughtFocus CSULB-CED",
                    Version = "v1",
                    Description = "ThoughtFocus CSULB-CED"
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThoughtFocus CSULB-CED");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
