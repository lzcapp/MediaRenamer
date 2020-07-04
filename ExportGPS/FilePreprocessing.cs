using System;
using System.Collections.Generic;
using System.IO;
using static ExportGPS.MetadataQuery;

namespace ExportGPS {
    class FilePreprocessing {
        public static List<string> VidExt = new List<string> { ".mp4", ".mov", ".mts", ".mt2s" };
        public static List<string> PicExt = new List<string> { ".dng", ".jpg", ".jpeg" };

        internal static Dictionary<string, string> FileProcess(FileSystemInfo file) {
            try {
                string strDatetime;
                var fileExt = file.Extension.ToLower();
                if (PicExt.Contains(fileExt)) {
                    var dictResult = MetaQuery(file, true);
                    if (dictResult == null) {
                        return null;
                    }
                    strDatetime = dictResult["datetime"];
                    if (string.IsNullOrEmpty(strDatetime)) {
                        return null;
                    }
                    return dictResult;
                }
                /**
                else if (VidExt.Contains(fileExt)) {
                    var dictResult = MetaQuery(file, false);
                    if (dictResult == null) {
                        return null;
                    }
                    strDatetime = dictResult["datetime"];
                    if (string.IsNullOrEmpty(strDatetime)) {
                        return null;
                    }
                    return ConvertGps(dictResult);
                } else {
                    var dictResult = MetaQuery(file, true);
                    if (dictResult == null) {
                        return null;
                    }
                    strDatetime = dictResult["datetime"];
                    if (!string.IsNullOrEmpty(strDatetime)) {
                        dictResult = MetaQuery(file, false);
                        strDatetime = dictResult["datetime"];
                    }
                    if (string.IsNullOrEmpty(strDatetime)) {
                        return null;
                    }
                    return ConvertGps(dictResult);
                }
                **/
            } catch (Exception) {
                return null;
            }
            return null;
        }
    }
}
