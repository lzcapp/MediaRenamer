﻿using System.Security.Cryptography;
using static MediaRenamer.MetadataQuery;

namespace MediaRenamer {
    public static class FileHandle {
        internal static bool FileProcess(FileSystemInfo file) {
            try {
                var dictResult = MetaQuery(file);

                if (dictResult.ContainsKey("error")) {
                    Console.WriteLine("[-Error-] MetaQuery Error: " + file.FullName + ".");
                    return false;
                }

                string strDt = dictResult["datetime"];
                Rename(file, strDt);
            } catch (Exception ex) {
                Console.WriteLine("[-Error-] FileProcess Failed: " + file.FullName + " | " + ex.Message + ".");
                return false;
            }

            return true;
        }

        private static void Rename(FileSystemInfo file, string strDt) {
            var strPath = file.FullName.Replace(file.Name, "");
            var strFullName = Path.Combine(strPath, strDt);
            var strExt = file.Extension.ToLower();
            var strMD5 = CalculateHash(file);

            var strOutName = strFullName + "_" + strMD5 + strExt;

            var fileInfo = new FileInfo(file.FullName);
            if (File.Exists(strOutName)) {
                return;
            }

            try {
                fileInfo.MoveTo(strOutName);
                Console.WriteLine("[-Moved-] " + strOutName);
            } catch (Exception ex) {
                Console.WriteLine("[-Error-] Rename Error: " + ex.Message);
            }
        }

        private static string CalculateHash(FileSystemInfo file) {
            string strMD5;
            using (var md5Instance = MD5.Create()) {
                using (var stream = File.OpenRead(file.FullName)) {
                    var fileHash = md5Instance.ComputeHash(stream);
                    strMD5 = BitConverter.ToString(fileHash).Replace("-", "").ToUpperInvariant();
                }
            }
            return strMD5[..3] + strMD5[^3..^0];
        }
    }
}