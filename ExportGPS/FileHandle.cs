using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using static PhotoToolbox.MetadataQuery;
using static PhotoToolbox.Program;

namespace PhotoToolbox
{
    public static class FileHandle
    {
        internal static void FileProcess(FileSystemInfo file)
        {
            try
            {
                string strDt;
                var fileExt = file.Extension.ToLower();
                if (PicExt.Contains(fileExt))
                {
                    var dictResult = MetaQuery(file, true);
                    strDt = dictResult["datetime"];
                    if (string.IsNullOrEmpty(strDt))
                    {
                        return;
                    }
                    AddGps(ConvertGps(dictResult));
                }
                else if (VidExt.Contains(fileExt))
                {
                    var dictResult = MetaQuery(file, false);
                    strDt = dictResult["datetime"];
                    if (string.IsNullOrEmpty(strDt))
                    {
                        return;
                    }
                    AddGps(ConvertGps(dictResult));
                }
                else
                {
                    var dictResult = MetaQuery(file, true);
                    strDt = dictResult["datetime"];
                    if (!string.IsNullOrEmpty(strDt))
                    {
                        dictResult = MetaQuery(file, false);
                        strDt = dictResult["datetime"];
                    }
                    if (string.IsNullOrEmpty(strDt))
                    {
                        return;
                    }
                    AddGps(ConvertGps(dictResult));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void AddGps(IReadOnlyDictionary<string, string> dictResult)
        {
            if (dictResult == null)
            {
                return;
            }
            var dataRow = Datatable.NewRow();
            //dataRow["timestamp"] = dictResult["timestamp"];
            //dataRow["datetime"] = dictResult["datatime"];
            dataRow["longitude"] = dictResult["longitude"];
            dataRow["latitude"] = dictResult["latitude"];
            //dataRow["altitude"] = dictResult["altitude"];
            Datatable.Rows.Add(dataRow);
        }

        private static Dictionary<string, string> ConvertGps(IReadOnlyDictionary<string, string> dictGps)
        {
            var dictCovt = new Dictionary<string, string>();
            const string apiUrl = "http://api.map.baidu.com/geoconv/v1/?";
            var coords = "coords=" + dictGps["longitude"] + "," + dictGps["latitude"];
            string key = ConfigurationManager.AppSettings["apikey"];
            string ak = "ak=" + key;
            var requestUrl = apiUrl + coords + "&from=1&to=5&" + ak;
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            string result;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return null;
            }
            var o = JObject.Parse(result);
            var status = (string)o.SelectToken("status");
            if (status != "0")
            {
                return null;
            }
            var resultToken = o.SelectToken("result")[0];
            var longitude = (string)resultToken.SelectToken("x");
            var latitude = (string)resultToken.SelectToken("y");
            dictCovt.Add("longitude", longitude);
            dictCovt.Add("latitude", latitude);
            return dictCovt;
        }
    }
}
