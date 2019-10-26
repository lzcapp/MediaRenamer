using System;
using System.IO;
using System.Security.Cryptography;
using static MediaRenamer.MetadataQuery;
using static MediaRenamer.Program;

namespace MediaRenamer {
    public static class FileHandle {
        internal static void FileProcess(FileSystemInfo file) {
            try {
                string strDt;
                var fileExt = file.Extension.ToLower();
                if (PicExt.Contains(fileExt)) {
                    var dictResult = MetaQuery(file, true);
                    strDt = dictResult["datetime"];
                    if (string.IsNullOrEmpty(strDt)) {
                        return;
                    }

                    Rename(file, string.IsNullOrEmpty(strDt) ? "ERROR" : strDt);
                } else if (VidExt.Contains(fileExt)) {
                    var dictResult = MetaQuery(file, false);
                    strDt = dictResult["datetime"];
                    if (string.IsNullOrEmpty(strDt)) {
                        return;
                    }

                    Rename(file, string.IsNullOrEmpty(strDt) ? "ERROR" : strDt);
                } else {
                    var dictResult = MetaQuery(file, true);
                    strDt = dictResult["datetime"];
                    if (!string.IsNullOrEmpty(strDt)) {
                        dictResult = MetaQuery(file, false);
                        strDt = dictResult["datetime"];
                    }

                    if (string.IsNullOrEmpty(strDt)) {
                    }
                }
            } catch (Exception) {
                // ignored
            }
        }

        private static void Rename(FileSystemInfo file, string strDt) {
            switch (strDt) {
                case "ERROR":
                    Console.WriteLine("[-Error-] " + file.FullName + " --> NO Tag Found");
                    return;
                case "ERROR Unknown Type":
                    Console.WriteLine("[-Error-] " + file.FullName + " --> UNKOWN File Type");
                    return;
                default:
                    var strOriginName = file.FullName;
                    var strPath = file.FullName.Replace(file.Name, "");
                    var strFullName = Path.Combine(strPath, strDt);
                    var strExt = file.Extension.ToLower();
                    var intDupInx = 0;
                    var strDupName = strFullName + strExt;
                    if (File.Exists(strDupName) && FileCompare(strDupName, strOriginName)) {
                        Console.WriteLine("[Skipped] " + file.FullName);
                        return;
                    }

                    while (File.Exists(strDupName)) {
                        intDupInx++;
                        var strDupInx = "_" + intDupInx;
                        strDupName = strFullName + strDupInx + strExt;
                    }

                    var strNewName = strDupName;
                    try {
                        var fileInfo = new FileInfo(strOriginName);
                        fileInfo.MoveTo(strNewName);
                        Console.WriteLine("[-Moved-] " + " --> " + strNewName);
                    } catch (Exception) {
                        Console.WriteLine("[-Error-] " + " --> Error rename");
                    }

                    return;
            }
        }

        private static bool FileCompare(string filePath1, string filePath2) {
            using (var hash = HashAlgorithm.Create()) {
                using (FileStream file1 = new FileStream(filePath1, FileMode.Open),
                                  file2 = new FileStream(filePath2, FileMode.Open)) {
                    var hashByte1 = hash.ComputeHash(file1);
                    var hashByte2 = hash.ComputeHash(file2);
                    var fileByte1 = BitConverter.ToString(hashByte1);
                    var fileByte2 = BitConverter.ToString(hashByte2);
                    return fileByte1 == fileByte2;
                }
            }
        }
    }
}