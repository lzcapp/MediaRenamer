using MetadataExtractor;
using System;

namespace ExifViewer {
    class Program {
        static void Main() {
            string path1 = @"F:\2053428.jpg";

            var directories = ImageMetadataReader.ReadMetadata(path1);

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
