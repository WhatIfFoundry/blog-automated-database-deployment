using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(x => {
        x.AddEnvironmentVariables();
        x.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((h,x) =>{
        var config = h.Configuration;
        x.AddSingleton(config.GetSection("Quotes").Get<Quote[]>());
    })
    .Build();

host.Run();
