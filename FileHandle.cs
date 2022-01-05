using System.Security.Cryptography;
using static MediaRenamer.MetadataQuery;

namespace MediaRenamer {
    public static class FileHandle {
        internal static bool FileProcess(FileSystemInfo file) {
            try {
                var dictResult = MetaQuery(file, true);

                if (dictResult == null || dictResult.ContainsKey("error")) {
                    dictResult = null;
                    dictResult = MetaQuery(file, false);
                }

                if (dictResult == null) {
                    Console.WriteLine("[-Error-] MetaQuery returns NULL result: " + file.FullName + ".");
                    return false;
                }

                if (dictResult.ContainsKey("error")) {
                    Console.WriteLine("[-Error-] MetaQuery returns ERROR: " + file.FullName + ".");
                    return false;
                }

                string strDt = dictResult["datetime"];
                Rename(file, strDt);
            } catch (Exception ex) {
                Console.WriteLine("[-Error-] FileProcess error: " + ex.Message + ".");
                return false;
            }

            return true;
        }

        private static void Rename(FileSystemInfo file, string strDt) {
            var strOriginName = file.FullName;
            var strPath = file.FullName.Replace(file.Name, "");
            var strFullName = Path.Combine(strPath, strDt);
            var strExt = file.Extension.ToLower();
            var intDupInx = 0;
            var strDupName = strFullName + strExt;

            var fileInfo = new FileInfo(strOriginName);

            while (File.Exists(strDupName)) {
                if (FileCompare(strDupName, strOriginName)) {
                    fileInfo.Delete();
                    Console.WriteLine("[Skipped] " + file.FullName);
                    return;
                }
                intDupInx++;
                strDupName = strFullName + "_" + intDupInx + strExt;
            }

            var strNewName = strDupName;
            try {
                fileInfo.MoveTo(strNewName);
                Console.WriteLine("[-Moved-] " + strNewName);
            } catch (Exception) {
                Console.WriteLine("[-Error-] Error renaming");
            }
        }

        private static bool FileCompare(string filePath1, string filePath2) {
            if (filePath1 == filePath2) {
                return true;
            }

            using (var hash = HashAlgorithm.Create()) {
                using (FileStream file1 = new(filePath1, FileMode.Open),
                                  file2 = new(filePath2, FileMode.Open)) {
                    var hashByte1 = hash.ComputeHash(file1);
                    var hashByte2 = hash.ComputeHash(file2);
                    return hashByte1 == hashByte2;
                }
            }
        }
    }
}