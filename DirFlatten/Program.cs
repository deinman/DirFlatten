using DirFlatten;

// Root directory to process: pass as first arg or use current directory
var root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

if (!Directory.Exists(root))
{
    Console.WriteLine($"Directory does not exist: {root}");
    return;
}

DirectoryFlattener.FlattenSingleFileDirectories(root);