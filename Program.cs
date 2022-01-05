using System.Text;
using static MediaRenamer.FileHandle;

namespace MediaRenamer {
    internal static class Program {
        private static void Main() {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Copyright \u00a9 2019 RainySummer, All Rights Reserved.");
            Console.WriteLine(">> A tool for renaming multi-media files.\n");

            Console.WriteLine("Please Input the Folder Path:");
            Console.Write(">> ");
            string dirInput;
            do {
                dirInput = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(dirInput));

            Console.WriteLine();

            try {
                var diPath = new DirectoryInfo(dirInput);
                var fileList = new List<FileSystemInfo>();
                fileList = GetFiles(diPath, fileList);
                foreach (var file in fileList) {
                    FileProcess(file);
                }
            } catch (DirectoryNotFoundException) {
                Console.WriteLine("[-Error-] The folder does not exist. [Program Exiting...]");
            } catch (Exception ex) {
                Console.WriteLine("[-Error-] " + ex.Message + " [Program Exiting...]");
            }

            Console.Write("\nPress Any Key To Exit...");
            Console.ReadKey();
        }

        private static List<FileSystemInfo> GetFiles(DirectoryInfo dirInfo, List<FileSystemInfo> fileList) {
            var fsInfos = dirInfo.GetFileSystemInfos();
            foreach (var fsInfo in fsInfos) {
                if (fsInfo is DirectoryInfo) {
                    GetFiles(new DirectoryInfo(fsInfo.FullName), fileList);
                } else {
                    fileList.Add(fsInfo);
                }
            }
            return fileList;
        }
    }
}