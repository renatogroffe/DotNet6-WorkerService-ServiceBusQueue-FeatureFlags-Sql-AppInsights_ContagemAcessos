using WorkerContagem;
using WorkerContagem.Data;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.FeatureManagement;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ContagemRepository>();
        services.AddHostedService<Worker>();

        services.AddFeatureManagement(
            hostContext.Configuration.GetSection("FeatureFlags"));

        if (Convert.ToBoolean(hostContext.Configuration["FeatureFlags:Monitoring"]))
        {
            services.AddApplicationInsightsTelemetryWorkerService(options =>
                {
                    options.ConnectionString =
                        hostContext.Configuration.GetConnectionString("ApplicationInsights");
                });

            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
                (module, o) =>
                {
                    module.EnableSqlCommandTextInstrumentation = true;
                });
        }
    })
    .Build();

await host.RunAsync();