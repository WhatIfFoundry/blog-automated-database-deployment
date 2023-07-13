using CommandLine;

public class Options
{
    [Option('c', "connection-string", Required = false, HelpText = "Set the database connection string.")]
    public string? ConnectionString { get; set; }
}