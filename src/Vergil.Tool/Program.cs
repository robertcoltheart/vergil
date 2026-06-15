using System.CommandLine;

var command = new RootCommand("Calculate semantic version from git");

return await command.Parse(args).InvokeAsync();
