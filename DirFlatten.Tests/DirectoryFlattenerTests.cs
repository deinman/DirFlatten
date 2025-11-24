namespace DirFlatten.Tests;

using System;
using System.IO;
using Xunit;

public class DirectoryFlattenerTests
{
    [Fact]
    public void SingleFileDirectory_IsFlattened()
    {
        var root = CreateTempDir();
        try
        {
            var movieDir = Path.Combine(root, "Wicked (2024)");
            Directory.CreateDirectory(movieDir);

            var file = Path.Combine(movieDir, "Wicked (2024).mkv");
            File.WriteAllText(file, "dummy");
            
            DirectoryFlattener.FlattenSingleFileDirectories(root);

            var movedFile = Path.Combine(root, "Wicked (2024).mkv");
            Assert.True(File.Exists(movedFile));
            Assert.False(Directory.Exists(movieDir));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void MultiFileDirectory_IsNotFlattened()
    {
        var root = CreateTempDir();
        try
        {
            var movieDir = Path.Combine(root, "What we do in the Shadows (2014)");
            Directory.CreateDirectory(movieDir);

            File.WriteAllText(Path.Combine(movieDir, "a.mp4"), "dummy");
            File.WriteAllText(Path.Combine(movieDir, "a.srt"), "dummy");

            DirectoryFlattener.FlattenSingleFileDirectories(root);

            Assert.True(Directory.Exists(movieDir));
            Assert.Equal(2, Directory.GetFiles(movieDir).Length);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateTempDir()
    {
        var path = Path.Combine(Path.GetTempPath(), "FlattenTest_" + Guid.NewGuid());
        Directory.CreateDirectory(path);
        return path;
    }
}