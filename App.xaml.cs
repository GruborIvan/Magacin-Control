using Autofac;
using Autofac.Extensions.DependencyInjection;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Migrations.DbInitialize;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.Modules;
using CSS_MagacinControl_App.Parsers;
using CSS_MagacinControl_App.Repository;
using CSS_MagacinControl_App.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;

namespace CSS_MagacinControl_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

                    var connectionString = configuration.GetConnectionString("MagacinDbConnectionStringLocal");

                    services.AddDbContext<AppDbContext>(
                        options => options.UseSqlServer(connectionString),
                        ServiceLifetime.Transient
                    );

                    services.AddSingleton<AuthenticationWindow>();
                    services.AddSingleton<MainWindow>();

                    services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
                    services.AddTransient<IRobaService, RobaService>();
                    services.AddTransient<IRobaRepository, RobaRepository>();

                    // Csv parser.
                    services.AddTransient<IFileParserRepository, FileParserRepository>();
                    services.AddTransient<IFileParser, FileParser>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutoMapperModule());
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            // Make sure that the database is up to date with migrations.
            // Seed initial admin user to the system.
            var dbCtx = new AppDbContext();
            dbCtx.Database.Migrate();
            DbInitializer.Initialize(dbCtx);

            var startupForm = AppHost.Services.GetRequiredService<AuthenticationWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}