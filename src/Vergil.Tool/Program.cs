using System.CommandLine;
using Vergil.Tool.Git;

var repository = new Repository(@"C:\Projects\runtime");
var head = repository.Head;
var sha = head.Tip;

var command = new RootCommand("Calculate semantic version from git");

return await command.Parse(args).InvokeAsync();
