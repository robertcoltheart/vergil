using System.CommandLine;
using System.Diagnostics;
using Vergil.Git;

var watch = Stopwatch.StartNew();

var repository = new Repository(@"C:\Projects\runtime");
var head = repository.Head;
var tip = head.Tip;
var visits = 0;

while (tip != null)
{
    visits++;
    tip = tip.Parents.FirstOrDefault();
}

watch.Stop();
Console.WriteLine(watch.ElapsedMilliseconds);

var command = new RootCommand("Calculate semantic version from git");

return await command.Parse(args).InvokeAsync();
