using MediaInfo;
using MetadataExtractor.Formats.Exif;
using System.Globalization;
using Microsoft.WindowsAPICodePack.Shell;
using static MetadataExtractor.ImageMetadataReader;

namespace MediaRenamer
{
    public static class MetadataQuery
    {
        private const string StrDtFormat = "yyyy.MM.dd_HHmmss";

        public static string MetaQuery(FileSystemInfo file) {
            try {
                var filePath = file.FullName;
                var result = ShellQuery(filePath);
                if (result != null) {
                    return result.Value.ToString(StrDtFormat);
                }
                result = VidQuery(filePath);
                if (result != null) {
                    return result.Value.ToString(StrDtFormat);
                }
                result = PicQuery(filePath);
                return result != null ? result.Value.ToString(StrDtFormat) : string.Empty;
            } catch (Exception) {
                return string.Empty;
            }
        }
        
        private static DateTime? ShellQuery(string filePath) {
            try {
                var shell = ShellObject.FromParsingName(filePath);
                var dtDateTaken = shell.Properties.System.Photo.DateTaken.Value;
                if (dtDateTaken != null) {
                    return dtDateTaken;
                }
                var dtDateEncoded = shell.Properties.System.Media.DateEncoded.Value;
                return dtDateEncoded;
            } catch (Exception) {
                return null;
            }
        }

        private static DateTime? PicQuery(string filePath) {
            const string strFormat = "yyyy:MM:dd HH:mm:ss";
            try {
                var directories = ReadMetadata(filePath);
                var subDt = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                var strDt = subDt?.GetDescription(ExifDirectoryBase.TagDateTime);
                if (string.IsNullOrEmpty(strDt)) {
                    var subDt2 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    strDt = subDt2?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);
                    if (string.IsNullOrEmpty(strDt)) {
                        strDt = subDt2?.GetDescription(ExifDirectoryBase.TagDateTimeDigitized);
                        if (string.IsNullOrEmpty(strDt)) {
                            return null;
                        }
                    }
                }
                strDt = strDt[..19];
                return DateTime.ParseExact(strDt, strFormat, CultureInfo.InvariantCulture);
            } catch (Exception) {
                return null;
            }
        }

        private static DateTime? VidQuery(string filePath) {
            const string strFormat = "yyyy-MM-dd HH:mm:ss";
            const string strAppleFormat = "yyyy:MM:ddTHH:mm:ss";
            var isApple = false;
            try {
                var mi = new MediaInfo.MediaInfo();
                mi.Open(filePath);

                var strDt = mi.Get(StreamKind.General, 0, "Encoded_Date");
                if (string.IsNullOrEmpty(strDt)) {
                    isApple = true;
                    strDt = mi.Get(StreamKind.General, 0, "com.apple.quicktime.creationdate");
                }
                if (string.IsNullOrEmpty(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Tagged_Date");
                }
                if (string.IsNullOrEmpty(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Recorded_Date");
                }
                if (string.IsNullOrEmpty(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Mastered_Date");
                }

                mi.Dispose();
                DateTime dtDt;
                if (isApple) {
                    strDt = strDt[..19];
                    dtDt = DateTime.ParseExact(strDt, strAppleFormat, CultureInfo.InvariantCulture);
                } else {
                    if (strDt.Contains("UTC")) {
                        strDt = strDt.Replace("UTC ", "");
                        dtDt = DateTime.ParseExact(strDt, strFormat, CultureInfo.InvariantCulture);
                        var dtIsUtc = DateTime.SpecifyKind(dtDt, DateTimeKind.Utc);
                        dtDt = dtIsUtc.ToLocalTime();
                    } else {
                        strDt = strDt[..19];
                        dtDt = DateTime.ParseExact(strDt, strFormat, CultureInfo.InvariantCulture);
                    }
                }
                return dtDt;
            } catch (Exception) {
                return null;
            }
        }
    }
}