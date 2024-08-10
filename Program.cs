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
            char[] trimChars = {
                '"', '\'', ' '
            };

            path = path.TrimStart(trimChars).TrimEnd(trimChars).Trim();

            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) {
                DirectoryInfo diPath = new(path);
                var fileList = GetFiles(diPath);
                Console.WriteLine("Totally " + fileList.Count + " Files.");
                foreach (var filePath in fileList) {
                    FileProcess(filePath);
                }
            }
        } catch (DirectoryNotFoundException) {
            Console.WriteLine("[-Error-] The folder does not exist.");
        } catch (Exception ex) {
            Console.WriteLine("[-Error-] Main(): " + ex.Message);
        }

        Console.WriteLine("Press Any Key To Exit...");
        Console.ReadKey();
    }

    private static List<string> GetFiles(DirectoryInfo dirInfo) {
        if (dirInfo is not { Exists: true }) {
            throw new DirectoryNotFoundException($"The directory '{dirInfo.FullName}' does not exist.");
        }

        var fileList = new List<string>();

        var fsInfos = dirInfo.GetFileSystemInfos();
        foreach (FileSystemInfo fsInfo in fsInfos) {
            if (fsInfo is DirectoryInfo subDirInfo) {
                fileList.AddRange(GetFiles(subDirInfo));
            } else {
                fileList.Add(fsInfo.FullName);
            }
        }

        return fileList;
    }
}