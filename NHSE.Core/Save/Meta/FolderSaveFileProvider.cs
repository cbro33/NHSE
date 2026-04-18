using System;
using System.IO;

namespace NHSE.Core;

/// <summary>
/// Provides file access from a filesystem folder, optionally mirroring writes to sibling folders.
/// </summary>
public sealed class FolderSaveFileProvider : ISaveFileProvider
{
    private readonly string RootPath;
    private readonly string[] MirrorRoots;

    public FolderSaveFileProvider(string rootPath) : this(rootPath, []) { }

    public FolderSaveFileProvider(string rootPath, string[] mirrorRoots)
    {
        RootPath = rootPath;
        MirrorRoots = mirrorRoots;
    }

    public byte[] ReadFile(string relativePath)
    {
        var fullPath = Path.Combine(RootPath, relativePath);
        return File.ReadAllBytes(fullPath);
    }

    public void WriteFile(string relativePath, ReadOnlySpan<byte> data)
    {
        WriteTo(RootPath, relativePath, data);
        foreach (var mirror in MirrorRoots)
            WriteTo(mirror, relativePath, data);
    }

    private static void WriteTo(string root, string relativePath, ReadOnlySpan<byte> data)
    {
        var fullPath = Path.Combine(root, relativePath);
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(fullPath, data);
    }

    public bool FileExists(string relativePath)
    {
        var fullPath = Path.Combine(RootPath, relativePath);
        return File.Exists(fullPath);
    }

    public string[] GetDirectories(string searchPattern)
    {
        var dirs = Directory.GetDirectories(RootPath, searchPattern, SearchOption.TopDirectoryOnly);
        var result = new string[dirs.Length];
        for (int i = 0; i < dirs.Length; i++)
            result[i] = new DirectoryInfo(dirs[i]).Name;
        return result;
    }

    public ISaveFileProvider GetSubdirectoryProvider(string subdirectory)
    {
        var subPath = Path.Combine(RootPath, subdirectory);
        if (MirrorRoots.Length == 0)
            return new FolderSaveFileProvider(subPath);

        var subMirrors = new string[MirrorRoots.Length];
        for (int i = 0; i < MirrorRoots.Length; i++)
            subMirrors[i] = Path.Combine(MirrorRoots[i], subdirectory);
        return new FolderSaveFileProvider(subPath, subMirrors);
    }

    public void Flush()
    {
        // No-op for folder provider; writes are immediate.
    }
}
