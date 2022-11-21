using MediaInfo;
using MetadataExtractor.Formats.Exif;
using System.Globalization;
using static MetadataExtractor.ImageMetadataReader;
using static System.TimeZoneInfo;

namespace MediaRenamer {
    public static class MetadataQuery {
        private const string StrDtFormat = "yyyy.MM.dd_HHmmss";
        private static Dictionary<string, string> _dictResult = new();

        public static Dictionary<string, string> MetaQuery(FileSystemInfo file) {
            try {
                _dictResult = new Dictionary<string, string> { { "type", "Pic" } };
                var dictDatetime = PicDtQuery(file);

                if (dictDatetime.ContainsKey("error")) {
                    _dictResult = new Dictionary<string, string> { { "type", "Vid" } };
                    dictDatetime = VidDtQuery(file);
                    if (dictDatetime.ContainsKey("error")) {
                        _dictResult.Add("error", "MetaQuery Failed.");
                        return _dictResult;
                    }
                }
                foreach (var dt in dictDatetime) {
                    _dictResult.Add(dt.Key, dt.Value);
                }
                return _dictResult;
            } catch (Exception ex) {
                _dictResult.Add("error", "MetaQuery Exception Caught." + ex.Message);
                return _dictResult;
            }
        }

        private static Dictionary<string, string> PicDtQuery(FileSystemInfo file) {
            var dictDt = new Dictionary<string, string>();
            const string strFormat = "yyyy:MM:dd HH:mm:ss";
            try {
                var directories = ReadMetadata(file.FullName);
                var subdirDt = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                var strDt = subdirDt?.GetDescription(ExifDirectoryBase.TagDateTime);
                if (string.IsNullOrEmpty(strDt)) {
                    dictDt.Add("error", "Pic: No DateTime.");
                    return dictDt;
                }

                var dtDt = DateTime.ParseExact(strDt, strFormat, CultureInfo.CurrentCulture);

                /*
                var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
                var timestamp = (dtDt.Ticks - startTime.Ticks) / 10000;
                if (timestamp == 0) {
                    return null;
                }
                */

                dictDt.Add("datetime", dtDt.ToString(StrDtFormat));
                return dictDt;
            } catch (Exception ex) {
                dictDt.Add("error", "Exception PicDtQuery " + ex.Message);
                return dictDt;
            }
        }

        private static Dictionary<string, string> VidDtQuery(FileSystemInfo file) {
            var dictDt = new Dictionary<string, string>();
            const string strFormat = "yyyy-MM-dd HH:mm:ss";
            const string strAppleFormat = "yyyy:MM:ddTHH:mm:ss";
            var isApple = false;
            try {
                var mi = new MediaInfo.MediaInfo();
                mi.Open(file.FullName);

                var strDt = mi.Get(StreamKind.Video, 0, "Encoded_Date");
                if (string.IsNullOrEmpty(strDt)) {
                    strDt = mi.Get(StreamKind.Video, 0, "Tagged_Date");
                }
                if (string.IsNullOrEmpty(strDt)) {
                    strDt = mi.Get(StreamKind.General, 0, "Recorded_Date");
                }
                if (string.IsNullOrEmpty(strDt)) {
                    isApple = true;
                    strDt = mi.Get(StreamKind.General, 0, "com.apple.quicktime.creationdate");
                }

                mi.Dispose();
                DateTime dtDt;
                if (isApple) {
                    if (strDt.Length > 19) {
                        strDt = strDt[..19];
                    }
                    dtDt = DateTime.ParseExact(strDt, strAppleFormat, CultureInfo.CurrentCulture);
                } else {
                    if (strDt.Length > 19) {
                        strDt = strDt[4..];
                    }
                    dtDt = DateTime.ParseExact(strDt, strFormat, CultureInfo.CurrentCulture);
                    dtDt = ConvertTimeFromUtc(dtDt, Local);
                }
                dictDt.Add("datetime", dtDt.ToString(StrDtFormat));
                return dictDt;
            } catch (Exception ex) {
                dictDt.Add("error", "Exception VidDtQuery " + ex.Message);
                return dictDt;
            }
        }
    }
}