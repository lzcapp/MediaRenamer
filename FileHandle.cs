using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static MediaRenamer.MetadataQuery;

namespace MediaRenamer {
    public static class FileHandle {
        private const string StrDtFormat = "yyyy.MM.dd_HHmmss";

        internal static string FileProcess(string filePath) {
            FileInfo file = new(filePath);
            try {
                string result = MetaQuery(filePath);

                switch (result) {
                    case "NOTMEDIA":
                        return "[SKIPD]";
                    case "": {
                        string fileName = file.Name.Replace(file.Extension, "");
                        if (DateTime.TryParse(fileName, out DateTime dt)) {
                            return Rename(file, dt.ToString(StrDtFormat));
                        }
                        Match match = new Regex(@"\d{8}_\d{6}").Match(fileName);
                        if (match.Success) {
                            DateTime dateTime = DateTime.ParseExact(match.Value, "yyyyMMdd_HHmmss", null);
                            return Rename(file, dateTime.ToString(StrDtFormat));
                        }
                        break;
                    }
                    default:
                        return Rename(file, result);
                }
            } catch (Exception) {
                return "[ERROR]";
            }
            return "[SKIPD]";
        }

        private static string Rename(FileSystemInfo file, string strDt) {
            string strPath = file.FullName.Replace(file.Name, "");
            string strFullName = Path.Combine(strPath, strDt);
            string strExt = file.Extension.ToLower();
            string strMd5 = CalculateHash(file);

            string strOutName = strFullName + "_" + strMd5 + strExt;

            if (File.Exists(strOutName)) {
                return "[EXIST]";
            }

            FileInfo fileInfo = new(file.FullName);

            try {
                fileInfo.MoveTo(strOutName);
                return "[NAMED]";
            } catch (Exception) {
                return "[ERROR]";
            }
        }

        private static string CalculateHash(FileSystemInfo file) {
            string strMd5;
            using (MD5 md5Instance = MD5.Create()) {
                using FileStream stream = File.OpenRead(file.FullName);
                byte[] fileHash = md5Instance.ComputeHash(stream);
                strMd5 = BitConverter.ToString(fileHash).Replace("-", "").ToUpperInvariant();
            }
            return strMd5[..3] + strMd5[^3..];
        }
    }
}