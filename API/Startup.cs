using Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Swashbuckle.AspNetCore.Swagger;

namespace API
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://spoty.pie"));
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "SpotyPie API V 1.1",
                        Version = "v1",
                        Description = "For development purpose",
                        TermsOfService = "WTFPL",
                        Contact = new Contact
                        {
                            Email = "noreika.eimantas@gmail.com",
                            Name = "Eimantas Noreika"
                        }
                    }
                );
                options.DescribeAllEnumsAsStrings();
            });


            services.AddScoped<IDb, Services.Service>();
            services.AddScoped<ISongService, Services.SongService>();
            services.AddScoped<IAlbumService, Services.AlbumService>();
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IErrorService, ErrorService>();

            services.AddDbContext<SpotyPieIDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotyPie API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseMvc();
        }
    }
}
