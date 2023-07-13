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
        x.AddTransient<Api.Accessors.IQuoteAccessor, Api.Accessors.QuoteAccessor>();
        x.AddSingleton(config.GetSection("Database").Get<Api.Configuration.DatabaseConfiguration>());
    })  
    .Build();

host.Run();
