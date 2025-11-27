using System.CommandLine;
using DirFlatten;

var rootCommand = new RootCommand("DirFlatten");

// dir argument: optional, defaults to current directory if omitted
var dirArgument = new Argument<DirectoryInfo?>("dir")
{
    Description = "Root directory to flatten (defaults to current directory)",
    Arity = ArgumentArity.ZeroOrOne
};

var flattenCommand = new Command("flatten", "Flatten the directory");
flattenCommand.Arguments.Add(dirArgument);

flattenCommand.SetAction(parseResult =>
{
    var dir = parseResult.GetValue(dirArgument);
    DirectoryFlattener.FlattenSingleFileDirectories(dir);
});

rootCommand.Subcommands.Add(flattenCommand);

// Parse & invoke
return rootCommand.Parse(args).Invoke();