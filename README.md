# Localization.AspNetCore.EntityFramework 
![development](https://github.com/semack/Localization.AspNetCore.EntityFramework/workflows/development/badge.svg?branch=development) ![Push to NUGET](https://github.com/semack/Localization.AspNetCore.EntityFramework/workflows/Push%20to%20NUGET/badge.svg?branch=master)

Database independent ASP.NET Core localization library using EntityFramework.
.
## Installation
Before using of the library Nuget Package must be installed.

`Install-Package Localization.AspNetCore.EntityFramework`

## Examples of usage
The examples are included to the current repository.

## Configuration

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(
                        Configuration.GetConnectionString("DefaultConnection"))
                     // Adding necessary localization model to exiting context
                    .UseLocalizationEntities(); 
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Registering Localizer and defing its configuration
            services.AddLocalization<ApplicationDbContext>(options =>
            {
                {
                    options.ReturnOnlyKeyIfNotFound = true;
                    options.CreateNewRecordWhenLocalisedStringDoesNotExist = true;
                    options.NamingConvention = NamingConventionEnum.Name;
                }
            });

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("de-CH"),
                        new CultureInfo("fr-CH"),
                        new CultureInfo("it-CH")
                    };

                    options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            services.AddControllersWithViews()
                .AddViewLocalization();
        }
        
                public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions?.Value);

            app.UseDefaultFiles();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Adding Localizer Middleware
            app.UseLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
```

## License
Please see [LICENSE.md](LICENSE.md).

## Contribute
Contributions are welcome. Just open an Issue or submit a PR. 

## Contact
You can reach me via my [email](mailto://semack@gmail.com).
