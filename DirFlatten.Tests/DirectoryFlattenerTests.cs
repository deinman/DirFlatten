namespace DirFlatten.Tests;

using System;
using System.IO;
using Xunit;

public class DirectoryFlattenerTests
{
    [Fact]
    public void SingleFileDirectory_IsFlattened()
    {
        var dir = CreateTempDir();
        try
        {
            var movieDir = Path.Combine(dir.FullName, "Wicked (2024)");
            Directory.CreateDirectory(movieDir);

            var file = Path.Combine(movieDir, "Wicked (2024).mkv");
            File.WriteAllText(file, "dummy");
            
            DirectoryFlattener.FlattenSingleFileDirectories(dir);

            var movedFile = Path.Combine(dir.FullName, "Wicked (2024).mkv");
            Assert.True(File.Exists(movedFile));
            Assert.False(Directory.Exists(movieDir));
        }
        finally
        {
            Directory.Delete(dir.FullName, recursive: true);
        }
    }

    [Fact]
    public void MultiFileDirectory_IsNotFlattened()
    {
        var dir = CreateTempDir();
        try
        {
            var movieDir = Path.Combine(dir.FullName, "What we do in the Shadows (2014)");
            Directory.CreateDirectory(movieDir);

            File.WriteAllText(Path.Combine(movieDir, "a.mp4"), "dummy");
            File.WriteAllText(Path.Combine(movieDir, "a.srt"), "dummy");

            DirectoryFlattener.FlattenSingleFileDirectories(dir);

            Assert.True(Directory.Exists(movieDir));
            Assert.Equal(2, Directory.GetFiles(movieDir).Length);
        }
        finally
        {
            Directory.Delete(dir.FullName, recursive: true);
        }
    }

    private static DirectoryInfo CreateTempDir()
    {
        var path = Path.Combine(Path.GetTempPath(), "FlattenTest_" + Guid.NewGuid());
        var dirInfo = Directory.CreateDirectory(path);
        return dirInfo;
    }
}