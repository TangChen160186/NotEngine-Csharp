using System.IO;

namespace Module.ContentExplorer.Utils;

public class FileUtils
{
    public static bool IsSubfolder(string parentFolderPath, string childFolderPath)
    {
        if (string.IsNullOrEmpty(parentFolderPath) || string.IsNullOrEmpty(childFolderPath))
            return false;

        var parentPath = Path.GetFullPath(parentFolderPath);
        var childPath = Path.GetFullPath(childFolderPath);
        
        if (childPath.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase))
        {
            return childPath.Substring(parentPath.Length).StartsWith(Path.DirectorySeparatorChar);
        }

        return false;
    }
    /// <summary>
    /// 重命名文件或文件夹的方法
    /// </summary>
    /// <param name="oldFullPath">原始路径（文件或文件夹）</param>
    /// <param name="newName">新的名字（不含路径，仅名字）</param>
    /// <returns>返回一个布尔值，表示是否重命名成功</returns>
    public static string Rename(string oldFullPath, string newName)
    {
        // 获取旧路径的父目录
        string directory = Path.GetDirectoryName(oldFullPath);
        string newPath = Path.Combine(directory, newName);
        Directory.Move(oldFullPath, newPath);  // 处理文件夹重命名
        return newPath;
    }
    public static bool IsFileOrFolderNameValid(string fileOrFolderName)
    {
        // 获取系统中非法的文件名字符
        char[] invalidChars = Path.GetInvalidFileNameChars();

        // 检查文件名是否包含非法字符
        return !fileOrFolderName.Any(c => invalidChars.Contains(c));
    }
}