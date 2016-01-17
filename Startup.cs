using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Data.Entity;
using Microsoft.Framework.Logging;
using RankDit.DbContext;
using RankDit.Models;
using Microsoft.Framework.Configuration;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration.Json;
 
public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
        var Path = appEnv.ApplicationBasePath;
       var confgienv=new ConfigurationBuilder(appEnv.ApplicationBasePath).AddJsonFile("config.json")
       .  AddEnvironmentVariables();
       
          Startup. Configuration = confgienv.Build();
        }
      public static Microsoft.Framework.Configuration.IConfiguration Configuration { get; set; }
   // public static User CurrentUser { get; set; }
    
   
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
           
            services.AddEntityFramework()
                       .AddSqlServer() 
                        .AddDbContext<UsersContext>(options =>
                        options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"] ))
                        
                        .AddDbContext< PostContext>(options =>
                        options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
           
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            
            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();
        var db = serviceProvider.GetRequiredService<UsersContext>();
        db.Database .EnsureCreated();

        
        }
     
}
