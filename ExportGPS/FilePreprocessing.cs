using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
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

        private static Dictionary<string, string> ConvertGps(IReadOnlyDictionary<string, string> dictGps) {
            var dictCovt = new Dictionary<string, string>();
            const string apiUrl = "http://api.map.baidu.com/geoconv/v1/?";
            var coords = "coords=" + dictGps["longitude"] + "," + dictGps["latitude"];
            var key = ConfigurationManager.AppSettings["apiKey"];
            var ak = "ak=" + key;
            var requestUrl = apiUrl + coords + "&from=1&to=5&" + ak;
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            string result;
            try {
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8)) {
                    result = reader.ReadToEnd();
                }
            } catch (Exception) {
                return null;
            }
            var o = JObject.Parse(result);
            var status = (string)o.SelectToken("status");
            if (status != "0") {
                return null;
            }
            var resultToken = o.SelectToken("result")[0];
            var longitude = (string)resultToken.SelectToken("x");
            var latitude = (string)resultToken.SelectToken("y");
            dictCovt.Add("longitude", longitude);
            dictCovt.Add("latitude", latitude);
            dictCovt.Add("timestamp", dictGps["timestamp"]);
            return dictCovt;
        }
    }
}
