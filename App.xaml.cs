using Autofac;
using Autofac.Extensions.DependencyInjection;
using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Migrations.DbInitialize;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.Modules;
using CSS_MagacinControl_App.Parsers;
using CSS_MagacinControl_App.Repository;
using CSS_MagacinControl_App.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
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
                .UseSerilog((host, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .WriteTo.File(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Magacin App Deploy\Log.txt");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = DbConnectionModule.GetConnectionString();

                    services.AddDbContext<AppDbContext>(
                        options => options.UseSqlServer(connectionString),
                        ServiceLifetime.Transient
                    );

                    services.AddDbContextFactory<AppDbContext>(
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

            try
            {
                var dbContext = new AppDbContext();
                dbContext.Database.Migrate();
                DbInitializer.Initialize(dbContext);
                DbInitializer.DeleteUnusedBarcodeIdentRelations(dbContext);
                
                var startupForm = AppHost.Services.GetRequiredService<AuthenticationWindow>();
                startupForm.Show();

                base.OnStartup(e);
            }
            catch (SqlException ex)
            {
                Console.Write(ex.ToString());
                new DialogHandler().GetDatabaseNotAccessibleDialog(ex.Message);
                Shutdown(1);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}