namespace DirFlatten;

public static class DirectoryFlattener
{
    public static void FlattenSingleFileDirectories(DirectoryInfo? directoryInfo)
    {
        var root = directoryInfo?.FullName ?? Directory.GetCurrentDirectory();

        if (!Directory.Exists(root))
        {
            Console.WriteLine($"Directory does not exist: {root}");
            return;
        }
        
        Flatten(root);
    }

    private static void Flatten(string root)
    {
        // Recurse first so we process deepest dirs before parents
        foreach (var dir in Directory.GetDirectories(root))
        {
            Flatten(dir);

            var files = Directory.GetFiles(dir);
            var subdirs = Directory.GetDirectories(dir);

            // Only one file and no subdirectories
            if (files.Length == 1 && subdirs.Length == 0)
            {
                var file = files[0];
                var destPath = Path.Combine(root, Path.GetFileName(file));

                // Handle name conflict – here we just skip
                if (File.Exists(destPath))
                {
                    Console.WriteLine($"Skipping move, file already exists in parent: {destPath}");
                    continue;
                }

                Console.WriteLine($"Moving file:\n  {file}\n-> {destPath}");
                File.Move(file, destPath);

                Console.WriteLine($"Deleting now-empty directory: {dir}");
                Directory.Delete(dir, recursive: false);
            }
        }
    }
}