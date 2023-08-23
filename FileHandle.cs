using System.Globalization;
using System.Security.Cryptography;
using static MediaRenamer.MetadataQuery;

namespace MediaRenamer {
    public static class FileHandle {
        private const string StrDtFormat = "yyyy.MM.dd_HHmmss";

        internal static bool FileProcess(FileSystemInfo file) {
            try {
                var dictResult = MetaQuery(file);

                if (dictResult.ContainsKey("error")) {
                    var fileName = file.Name;
                    fileName = fileName[..17];
                    var dtDt = DateTime.ParseExact(fileName, StrDtFormat, CultureInfo.InvariantCulture);
                    Rename(file, dtDt.ToString(StrDtFormat));
                    return false;
                }

                var strDt = dictResult["datetime"];
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
            var strMd5 = CalculateHash(file);

            var strOutName = strFullName + "_" + strMd5 + strExt;

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
            string strSha1;
            using (var sha1 = new SHA1Managed()) {
                using (var stream = File.OpenRead(file.FullName)) {
                    var fileHash = sha1.ComputeHash(stream);
                    strSha1 = BitConverter.ToString(fileHash).Replace("-", "").ToUpperInvariant();
                }
            }
            return strSha1[..3] + strSha1[^3..];
        }
    }
}