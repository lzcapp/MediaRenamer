using System;
using System.Collections.Generic;
using System.IO;
using static PhotoToolbox.FileHandle;

namespace PhotoToolbox
{
    internal static class Program
    {
        public static readonly List<string> VidExt = new List<string> { ".mp4", ".mov", ".mts", ".mt2s" };
        public static readonly List<string> PicExt = new List<string> { ".dng", ".jpg", ".jpeg" };
        // ReSharper disable once FieldCanBeMadeReadOnly.Global

        private static void Main() {
            Console.WriteLine("Copyright © 2019 RainySummer, All Rights Reserved.");

            Console.WriteLine("Please Input the Folder Path:");
            string dirInput;
            do
            {
                Console.WriteLine(@"[d] for default folder (E:\DCIM\)");
                dirInput = Console.ReadLine();
            } while (string.IsNullOrEmpty(dirInput));

            var isDefaultDir = dirInput.ToLower() == "d";

            try {
                var diDefault = new DirectoryInfo(@"E:\DCIM\");
                var diPath = isDefaultDir ? diDefault : new DirectoryInfo(dirInput);

                var files = new List<FileSystemInfo>();
                files = GetFiles(diPath, files);
                foreach (var file in files) FileProcess(file);
            }
            catch (Exception) {
                // ignored
            }
        }

        private static List<FileSystemInfo> GetFiles(DirectoryInfo dirInfo, List<FileSystemInfo> fileList) {
            var fsInfos = dirInfo.GetFileSystemInfos();
            foreach (var fsInfo in fsInfos)
                if (fsInfo is DirectoryInfo) {
                    GetFiles(new DirectoryInfo(fsInfo.FullName), fileList);
                }
                else {
                    var fileExt = fsInfo.Extension.ToLower();
                    if (PicExt.Contains(fileExt) || VidExt.Contains(fileExt))
                        fileList.Add(fsInfo);
                }

            return fileList;
        }
    }
}