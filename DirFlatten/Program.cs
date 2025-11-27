using System.CommandLine;
using DirFlatten;

var rootCommand = new RootCommand("DirFlatten");

// dir argument: optional, defaults to current directory if omitted
var dirArgument = new Argument<DirectoryInfo?>("dir")
{
    Description = "Root directory to flatten (defaults to current directory)",
    Arity = ArgumentArity.ZeroOrOne
};

var depthOption = new Option<int>("--depth")
{
    Description = "Maximum recursive depth",
    Arity = ArgumentArity.ZeroOrOne
};

depthOption.Validators.Add(result =>
{
    if (result.GetValueOrDefault<int>() < 0)
    {
        result.AddError("Depth must be non-negative");
    }
});

var flattenCommand = new Command("flatten", "Flatten the directory");
flattenCommand.Arguments.Add(dirArgument);
flattenCommand.Options.Add(depthOption);

flattenCommand.SetAction(parseResult =>
{
    var dir = parseResult.GetValue(dirArgument);
    DirectoryFlattener.FlattenSingleFileDirectories(dir);
});

rootCommand.Subcommands.Add(flattenCommand);

// Parse & invoke
return rootCommand.Parse(args).Invoke();