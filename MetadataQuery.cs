using System.Globalization;
using MediaInfo;
using MetadataExtractor.Formats.Exif;
using Microsoft.WindowsAPICodePack.Shell;
using static MetadataExtractor.ImageMetadataReader;
using Directory = MetadataExtractor.Directory;

namespace MediaRenamer {
    public static class MetadataQuery {
        private const string StrDtFormat = "yyyy.MM.dd_HHmmss";

        private enum MediaType {
            Image,
            Video,
            None
        }

        public static string MetaQuery(string filePath) {
            try {
                MediaType mediaType = IsMediaFile(filePath);
                DateTime? result;
                switch (mediaType) {
                    case MediaType.None:
                        return "NOTMEDIA";
                    case MediaType.Image:
                        result = MetadataExtractorQuery(filePath);
                        break;
                    case MediaType.Video:
                        result = MediaInfoQuery(filePath);
                        break;
                    default:
                        return string.Empty;
                }
                if (result != null) {
                    return result.Value.ToString(StrDtFormat);
                }
                //result = ShellQuery(filePath);
                return result != null ? result.Value.ToString(StrDtFormat) : string.Empty;
            } catch (Exception) {
                return string.Empty;
            }
        }

        private static MediaType IsMediaFile(string filePath) {
            ShellFile shellFile = ShellFile.FromFilePath(filePath);
            if (shellFile == null) {
                return MediaType.None;
            }
            if (shellFile.Properties.System.Media.Duration.Value != null) {
                return MediaType.Video;
            }
            return shellFile.Properties.System.Image.Dimensions.Value != null ? MediaType.Image : MediaType.None;
        }

        private static DateTime? ShellQuery(string filePath) {
            try {
                ShellObject? shell = ShellObject.FromParsingName(filePath);
                DateTime? dtDateTaken = shell.Properties.System.Photo.DateTaken.Value;
                if (dtDateTaken != null) {
                    return dtDateTaken;
                }
                DateTime? dtDateEncoded = shell.Properties.System.Media.DateEncoded.Value;
                return dtDateEncoded;
            } catch (Exception) {
                return null;
            }
        }

        private static DateTime? MetadataExtractorQuery(string filePath) {
            const string strFormat = "yyyy:MM:dd HH:mm:ss";
            try {
                IReadOnlyList<Directory> directories = ReadMetadata(filePath);
                ExifIfd0Directory? subDt = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                string strDt = subDt?.GetDescription(ExifDirectoryBase.TagDateTime) ?? string.Empty;
                if (!IsValidDateTime(strDt)) {
                    ExifSubIfdDirectory? subDt2 = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    strDt = subDt2?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal) ?? string.Empty;
                    if (!IsValidDateTime(strDt)) {
                        strDt = subDt2?.GetDescription(ExifDirectoryBase.TagDateTimeDigitized) ?? string.Empty;
                        if (!IsValidDateTime(strDt)) {
                            return null;
                        }
                    }
                }
                if (strDt is { Length: > 19 }) {
                    strDt = strDt[..19];
                }
                return DateTime.ParseExact(strDt, strFormat, CultureInfo.InvariantCulture);
            } catch (Exception) {
                return null;
            }
        }

        private static DateTime? MediaInfoQuery(string filePath) {
            const string strFormat = "yyyy-MM-dd HH:mm:ss";
            const string strFormatUtc = "yyyy-MM-dd HH:mm:ss zzz";
            const string strAppleFormat = "yyyy-MM-ddTHH:mm:sszzz";
            bool isApple = false;
            try {
                using MediaInfo.MediaInfo mi = new();
                mi.Option("ParseSpeed", "0");
                mi.Option("ReadByHuman", "0");
                mi.Open(filePath);

                string? strDt = string.Empty;
                if (!IsValidDateTime(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "com.apple.quicktime.creationdate");
                }
                if (!IsValidDateTime(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Encoded_Date");
                } else {
                    isApple = true;
                }
                if (!IsValidDateTime(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Tagged_Date");
                }
                if (!IsValidDateTime(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Recorded_Date");
                }
                if (!IsValidDateTime(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Mastered_Date");
                }

                DateTime dtDt;
                if (isApple) {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(
                        strDt,
                        strAppleFormat,
                        CultureInfo.InvariantCulture);
                    dtDt = dateTimeOffset.ToOffset(TimeSpan.FromHours(8)).DateTime;
                } else if (strDt.Length > 19) {
                    bool isParsed = DateTimeOffset.TryParseExact(
                        strDt,
                        strFormatUtc,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal,
                        out DateTimeOffset dateTimeOffset);
                    if (!isParsed) {
                        strDt = strDt.Replace("UTC", "");
                        isParsed = DateTimeOffset.TryParse(
                            strDt,
                            out dateTimeOffset);
                    }
                    if (isParsed) {
                        dtDt = dateTimeOffset.ToOffset(TimeSpan.FromHours(8)).DateTime;
                    } else {
                        throw new Exception();
                    }
                } else {
                    strDt = strDt[..19];
                    dtDt = DateTime.ParseExact(strDt, strFormat, CultureInfo.InvariantCulture);
                }
                return dtDt;
            } catch (Exception) {
                return null;
            }
        }

        private static bool IsValidDateTime(string? strDt) {
            if (strDt is null) {
                return false;
            }
            bool isNullOrEmpty = string.IsNullOrEmpty(strDt);
            if (isNullOrEmpty) {
                return false;
            }
            bool isUnixDate = strDt.Contains("1970");
            return !isUnixDate;

        }
    }
}