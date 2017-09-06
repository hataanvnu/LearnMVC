using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LearnMVC.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace LearnMVC
{
    public class Startup
    {
        //IConfiguration configuration;

        //public Startup(IConfiguration configuration)
        //{
        //    //var builder = new ConfigurationBuilder();
        //    //if (env.IsDevelopment())
        //    //    builder.AddUserSecrets<Startup>();
        //    //else
        //      //  builder.AddEnvironmentVariables();

        //    //this.configuration = builder.Build();

        //    this.configuration = configuration;
        //}

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // todo - lägg connection string i secrets

            var connString = @"Data Source=parskyserver.database.windows.net;Initial Catalog=Parsky;Integrated Security=False;User ID=Parsky;Password=Johan1234;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            services.AddDbContext<QuizDbContext>(o => o.UseSqlServer(connString));
            services.AddDbContext<IdentityDbContext>(o => o.UseSqlServer(connString));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();



            services.AddMvc();
            services.ConfigureApplicationCookie(o => o.LoginPath = "/Members/LogIn");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
