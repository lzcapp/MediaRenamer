using System.Text;
using static MediaRenamer.FileHandle;

namespace MediaRenamer {
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

                var fileList = new List<FileSystemInfo>();

                if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) {
                    DirectoryInfo diPath = new DirectoryInfo(path);
                    foreach (FileSystemInfo file in GetFiles(diPath, fileList)) {
                        FileProcess(file);
                    }
                } else {
                    fileList.Add(new FileInfo(path));
                }
            } catch (DirectoryNotFoundException) {
                Console.WriteLine("[-Error-] The folder does not exist.");
            } catch (Exception) {
                Console.WriteLine("[-Error-] Main.");
            }

            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadKey();
        }

        private static List<FileSystemInfo> GetFiles(DirectoryInfo dirInfo, List<FileSystemInfo> fileList) {
            var fsInfos = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo fsInfo in fsInfos) {
                if (fsInfo is DirectoryInfo) {
                    fileList.AddRange(GetFiles(new DirectoryInfo(fsInfo.FullName), fileList));
                } else {
                    fileList.Add(fsInfo);
                }
            }
            return fileList;
        }
    }
}