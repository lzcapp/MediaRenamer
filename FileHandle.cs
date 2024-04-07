using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static MediaRenamer.MetadataQuery;

namespace MediaRenamer {
    public static class FileHandle {
        private const string StrDtFormat = "yyyy.MM.dd_HHmmss";

        internal static void FileProcess(FileSystemInfo file) {
            try {
                var result = MetaQuery(file);

                if (result == string.Empty) {
                    var fileName = file.Name.Replace(file.Extension, "");
                    if (DateTime.TryParse(fileName, out DateTime dt)) {
                        Rename(file, dt.ToString(StrDtFormat));
                        return;
                    }
                    Match match = new Regex(@"\d{8}_\d{6}").Match(fileName);
                    if (!match.Success) {
                        return;
                    }
                    DateTime dateTime = DateTime.ParseExact(match.Value, "yyyyMMdd_HHmmss", null);
                    Rename(file, dateTime.ToString(StrDtFormat));
                    return;
                    return;
                }
                Rename(file, result);
            } catch (Exception) {
                Console.WriteLine("[-Error-] FileProcess: " + file.Name);
            }
        }

        private static void Rename(FileSystemInfo file, string strDt) {
            var strPath = file.FullName.Replace(file.Name, "");
            var strFullName = Path.Combine(strPath, strDt);
            var strExt = file.Extension.ToLower();
            var strMd5 = CalculateHash(file);

            var strOutName = strFullName + "_" + strMd5 + strExt;

            FileInfo? fileInfo = new FileInfo(file.FullName);
            if (File.Exists(strOutName)) {
                Console.WriteLine("[-Exist-] " + strOutName);
                return;
            }

            try {
                fileInfo.MoveTo(strOutName);
                Console.WriteLine("[-Moved-] " + strOutName);
            } catch (Exception) {
                Console.WriteLine("[-Error-] Rename: " + file.Name);
            }
        }

        private static string CalculateHash(FileSystemInfo file) {
            string strMd5;
            using (MD5? md5Instance = MD5.Create()) {
                using FileStream stream = File.OpenRead(file.FullName);
                var fileHash = md5Instance.ComputeHash(stream);
                strMd5 = BitConverter.ToString(fileHash).Replace("-", "").ToUpperInvariant();
            }
            return strMd5[..3] + strMd5[^3..];
        }
    }
}