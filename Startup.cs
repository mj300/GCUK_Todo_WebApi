using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using todo_webApi.Database;
using todo_webApi.Helpers;
using todo_webApi.Utils;

namespace todo_webApi
{
   public class Startup
   {
      public IConfiguration Configuration { get; }

      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public void ConfigureServices(IServiceCollection services)
      {
         services.AddCors();





         services.AddDbContext<AppDbContext>(options => options
        .UseSqlServer(AppConst.Settings.DbConnectionString)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

         services.AddControllers().AddNewtonsoftJson();

         services.AddSwaggerDocument(document =>
         {
            document.DocumentName = $"todo.web.api";
            document.Title = $"Todo API";
         });
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi3();

         }
         else
         {
            app.UseHsts();
         }

         app.UseRouting();

         app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());

         app.UseMiddleware<JwtMiddleware>();

         app.UseEndpoints(x => x.MapControllers());
      }
   }
}
