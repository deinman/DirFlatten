namespace DirFlatten.Tests;

public class DirectoryFlattenerTests : IDisposable
{
    private readonly DirectoryInfo _root = CreateTempDir();

    public void Dispose()
    {
        if (Directory.Exists(_root.FullName)) Directory.Delete(_root.FullName, true);
    }

    [Fact]
    public void SingleFileDirectory_IsFlattened()
    {
        var movieDir = CreateDirectory("Wicked (2024)");
        CreateFile(movieDir, "Wicked (2024).mkv");

        DirectoryFlattener.FlattenSingleFileDirectories(_root);

        var movedFile = Path.Combine(_root.FullName, "Wicked (2024).mkv");
        Assert.True(File.Exists(movedFile));
        Assert.False(Directory.Exists(movieDir));
    }

    [Fact]
    public void MultiFileDirectory_IsNotFlattened()
    {
        var movieDir = CreateDirectory("What we do in the Shadows (2014)");
        CreateFile(movieDir, "a.mp4");
        CreateFile(movieDir, "a.srt");

        DirectoryFlattener.FlattenSingleFileDirectories(_root);

        Assert.True(Directory.Exists(movieDir));
        Assert.Equal(2, Directory.GetFiles(movieDir).Length);
    }

    [Fact]
    public void NestedSingleFileDirectories_AreFlattenedWithoutDepthLimit()
    {
        var level2 = CreateDirectory("Level1", "Level2");
        var filePath = CreateFile(level2, "movie.mkv");

        DirectoryFlattener.FlattenSingleFileDirectories(_root);

        var finalPath = Path.Combine(_root.FullName, "movie.mkv");
        Assert.True(File.Exists(finalPath));
        Assert.False(Directory.Exists(Path.Combine(_root.FullName, "Level1")));
        Assert.False(Directory.Exists(level2));
    }

    [Fact]
    public void DepthZero_DoesNotRecurseIntoSubdirectories()
    {
        var level2 = CreateDirectory("Level1", "Level2");
        var filePath = CreateFile(level2, "movie.mkv");

        DirectoryFlattener.FlattenSingleFileDirectories(_root, 0);

        var level1 = Path.Combine(_root.FullName, "Level1");

        Assert.True(Directory.Exists(level1));
        Assert.True(Directory.Exists(level2));
        Assert.True(File.Exists(filePath));

        Assert.False(File.Exists(Path.Combine(level1, "movie.mkv")));
        Assert.False(File.Exists(Path.Combine(_root.FullName, "movie.mkv")));
    }

    [Fact]
    public void DepthLimit_PreventsFlatteningDeeperDirectories()
    {
        var level3 = CreateDirectory("Level1", "Level2", "Level3");
        var filePath = CreateFile(level3, "movie.mkv");

        DirectoryFlattener.FlattenSingleFileDirectories(_root, 1);

        var level1 = Path.Combine(_root.FullName, "Level1");
        var level2 = Path.Combine(level1, "Level2");

        Assert.True(Directory.Exists(level1));
        Assert.True(Directory.Exists(level2));
        Assert.True(Directory.Exists(level3));
        Assert.True(File.Exists(filePath));

        Assert.False(File.Exists(Path.Combine(level2, "movie.mkv")));
        Assert.False(File.Exists(Path.Combine(level1, "movie.mkv")));
        Assert.False(File.Exists(Path.Combine(_root.FullName, "movie.mkv")));
    }

    private static DirectoryInfo CreateTempDir()
    {
        var path = Path.Combine(Path.GetTempPath(), "FlattenTest_" + Guid.NewGuid());
        return Directory.CreateDirectory(path);
    }

    private string CreateDirectory(params string[] segments)
    {
        var path = _root.FullName;
        foreach (var segment in segments) path = Path.Combine(path, segment);

        Directory.CreateDirectory(path);
        return path;
    }

    private static string CreateFile(string directory, string fileName, string contents = "dummy")
    {
        var fullPath = Path.Combine(directory, fileName);
        File.WriteAllText(fullPath, contents);
        return fullPath;
    }
}