using System.Reflection;
using AutoMapper;
using dao;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using service;
using web.Auth;
using web.Automapper;
using web.Configuration;
using web.Localization;
using web.Middleware;
using web.Swagger;

namespace web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o
                .AddDefaultPolicy(builder => builder
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));

            services.AddAutoMapper(typeof(AutoMapperProfile).GetTypeInfo().Assembly);
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(Configuration.GetConnectionString("Default")));

            services.AddDaoServices();
            services.AddApplicationServices();

            services.AddSwagger();

            services.AddCustomOptions(Configuration);
            services.AddCustomAuthentication(Configuration);
            services.AddCustomAuthorization();

            services.AddControllers()
                .AddMvcOptions(options =>
                {
                    // https://github.com/dotnet/aspnetcore/issues/11584
                    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
                }).AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            app.UseCustomLocalization();

            app.UseCustomSwagger();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseCustomAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
