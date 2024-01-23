using System.Text;
using static MediaRenamer.FileHandle;

namespace MediaRenamer;

internal static class Program {
    private static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Copyright \u00a9 2022 RainySummer, All Rights Reserved.");
        Console.WriteLine(">> A tool for renaming multi-media files.\n");

        Console.WriteLine();

        string path;

        if (args.Length == 0) {
            do {
                Console.Write(">> ");
                path = Console.ReadLine() ?? string.Empty;
            } while (string.IsNullOrWhiteSpace(path));
        } else {
            path = args[0];
        }

        try {
            char[] trimChars = { '"', '\'', ' ' };

            path = path.TrimStart(trimChars).TrimEnd(trimChars).Trim();

            List<FileSystemInfo> fileList = new List<FileSystemInfo>();

            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) {
                var diPath = new DirectoryInfo(path);
                fileList = GetFiles(diPath, fileList);
            } else {
                fileList.Add(new FileInfo(path));
            }

            foreach (FileSystemInfo file in fileList) {
                FileProcess(file);
            }
        } catch (DirectoryNotFoundException) {
            Console.WriteLine("[-Error-] The folder does not exist.");
        } catch (Exception e) {
            Console.WriteLine("[-Error-] Main: " + e.Message);
        }

        Console.WriteLine("Press Any Key To Exit...");
        Console.ReadKey();
    }

    private static List<FileSystemInfo> GetFiles(DirectoryInfo dirInfo, List<FileSystemInfo> fileList) {
        var fsInfos = dirInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsInfo in fsInfos) {
            if (fsInfo is DirectoryInfo) {
                GetFiles(new DirectoryInfo(fsInfo.FullName), fileList);
            } else {
                fileList.Add(fsInfo);
            }
        }
        return fileList;
    }
}