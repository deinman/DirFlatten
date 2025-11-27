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

    [Fact]
    public void NestedSingleFileDirectories_AreFlattenedWithoutDepthLimit()
    {
        var dir = CreateTempDir();
        try
        {
            var level1 = Path.Combine(dir.FullName, "Level1");
            var level2 = Path.Combine(level1, "Level2");
            Directory.CreateDirectory(level2);

            var file = Path.Combine(level2, "movie.mkv");
            File.WriteAllText(file, "dummy");

            // No depth limit
            DirectoryFlattener.FlattenSingleFileDirectories(dir);

            var finalPath = Path.Combine(dir.FullName, "movie.mkv");
            Assert.True(File.Exists(finalPath));
            Assert.False(Directory.Exists(level1));
            Assert.False(Directory.Exists(level2));
        }
        finally
        {
            Directory.Delete(dir.FullName, recursive: true);
        }
    }

    [Fact]
    public void DepthZero_DoesNotRecurseIntoSubdirectories()
    {
        var dir = CreateTempDir();
        try
        {
            var level1 = Path.Combine(dir.FullName, "Level1");
            var level2 = Path.Combine(level1, "Level2");
            Directory.CreateDirectory(level2);

            var file = Path.Combine(level2, "movie.mkv");
            File.WriteAllText(file, "dummy");

            // Depth 0: process only immediate children of root
            DirectoryFlattener.FlattenSingleFileDirectories(dir, maxDepth: 0);

            // Nothing should have been flattened because root's direct child (Level1)
            // is not itself a single-file directory.
            Assert.True(Directory.Exists(level1));
            Assert.True(Directory.Exists(level2));
            Assert.True(File.Exists(file));

            Assert.False(File.Exists(Path.Combine(level1, "movie.mkv")));
            Assert.False(File.Exists(Path.Combine(dir.FullName, "movie.mkv")));
        }
        finally
        {
            Directory.Delete(dir.FullName, recursive: true);
        }
    }

    [Fact]
    public void DepthLimit_PreventsFlatteningDeeperDirectories()
    {
        var dir = CreateTempDir();
        try
        {
            var level1 = Path.Combine(dir.FullName, "Level1");
            var level2 = Path.Combine(level1, "Level2");
            var level3 = Path.Combine(level2, "Level3");
            Directory.CreateDirectory(level3);

            var file = Path.Combine(level3, "movie.mkv");
            File.WriteAllText(file, "dummy");

            // Depth 1: recurse into Level1 but not into Level2,
            // so Level3 is never inspected as a single-file directory.
            DirectoryFlattener.FlattenSingleFileDirectories(dir, maxDepth: 1);

            // Structure should remain unchanged
            Assert.True(Directory.Exists(level1));
            Assert.True(Directory.Exists(level2));
            Assert.True(Directory.Exists(level3));
            Assert.True(File.Exists(file));

            Assert.False(File.Exists(Path.Combine(level2, "movie.mkv")));
            Assert.False(File.Exists(Path.Combine(level1, "movie.mkv")));
            Assert.False(File.Exists(Path.Combine(dir.FullName, "movie.mkv")));
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