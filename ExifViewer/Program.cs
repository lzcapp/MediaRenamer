using MediaInfoLib;
using MetadataExtractor;
using System;
using System.IO;

namespace ExifViewer {
    class Program {
        static void Main() {
            string path = "";

            var directories = ImageMetadataReader.ReadMetadata(path);

            FileSystemInfo file = new FileInfo(path);
            var mi = new MediaInfo();
            mi.Open(file.FullName);
            Console.WriteLine(mi.Inform());
            mi.Close();
            mi.Dispose();

            foreach (var directory in directories) {
                foreach (var tag in directory.Tags) {
                    Console.WriteLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                }

                if (directory.HasError) {
                    foreach (var error in directory.Errors) {
                        Console.WriteLine($"ERROR: {error}");
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
