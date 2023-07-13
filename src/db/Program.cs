// See https://aka.ms/new-console-template for more information
using System.Reflection;
using CommandLine;
using DbUp;
using Microsoft.Extensions.Configuration;

var config = GetConfiguration();
var connectionString = config["DatabaseConnectionString"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Connection string not found.");
}

EnsureDatabase.For.SqlDatabase(connectionString);
UpdateDb(connectionString);

IConfiguration GetConfiguration(){
    var builder = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "DatabaseConnectionString", "Server=.;Initial Catalog=Quotes;Integrated Security=true;" }
        })
        .AddInMemoryCollection(GetParametersConfiguration())
        .AddEnvironmentVariables();

    return builder.Build();
}

IEnumerable<KeyValuePair<string, string?>> GetParametersConfiguration() {
    var options = Parser.Default.ParseArguments<Options>(args).Value;
    var result = new Dictionary<string, string?>();
    if(!string.IsNullOrWhiteSpace(options.ConnectionString)){
        yield return new KeyValuePair<string, string?>("DatabaseConnectionString", options.ConnectionString);
    }
}

void UpdateDb(string connectionString){
    var migrator = DeployChanges.To
        .SqlDatabase(connectionString)
        .WithTransaction()
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .WithExecutionTimeout(TimeSpan.FromSeconds(3600))
        .LogToConsole()
        .Build();

    var result = migrator.PerformUpgrade();

    if (!result.Successful)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(result.Error);
        Console.ResetColor();

        if (Environment.UserInteractive)
        {
            Console.ReadLine();
        }

        return;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Success!");
    Console.ResetColor();
}