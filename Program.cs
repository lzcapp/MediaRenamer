using System.Text;
using static MediaRenamer.FileHandle;

namespace MediaRenamer {
    internal static class Program {
        private static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Copyright \u00a9 2022 RainySummer, All Rights Reserved.");
            Console.WriteLine(">> A tool for renaming multi-media files.\n");

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
                    List<string> fileList = GetFiles(diPath);
                    int totalCount = fileList.Count;
                    Console.WriteLine("Totally " + totalCount + " Files.");
                    (int _, int top) = Console.GetCursorPosition();
                    for (int index = 0; index < totalCount; index++) {
                        string filePath = fileList[index];
                        string status = FileProcess(filePath);
                        Console.SetCursorPosition(0, top);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, top);
                        double percentage = index == 0 ? 0 : (double)index / totalCount * 100;
                        Console.Write("[" + percentage.ToString("F2") + "%]" + " " + status + " " + filePath);
                    }
                }
            } catch (Exception) {
                // ignored
            }

            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadKey();
        }

        private static List<string> GetFiles(DirectoryInfo dirInfo) {
            if (dirInfo is not { Exists: true }) {
                throw new DirectoryNotFoundException($"The directory '{dirInfo.FullName}' does not exist.");
            }

            List<string> fileList = new();

            FileSystemInfo[] fsInfos = dirInfo.GetFileSystemInfos();
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
}