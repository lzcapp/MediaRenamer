using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MediaInfoLib;
using MetadataExtractor.Formats.Exif;
using static System.TimeZoneInfo;
using static MetadataExtractor.ImageMetadataReader;

namespace PhotoToolbox
{
    public static class MetadataQuery
    {
        public static Dictionary<string, string> MetaQuery(FileSystemInfo file, bool filetype)
        {
            try
            {
                Dictionary<string, string> dictResult;
                Dictionary<string, string> dictDt;
                Dictionary<string, string> dictGps;
                switch (filetype)
                {
                    case true:
                        dictResult = new Dictionary<string, string>
                        {
                            { "type", "Pic" }
                        };
                        var directories = ReadMetadata(file.FullName);
                        dictDt = PicDtQuery(directories);
                        foreach (var dt in dictDt)
                        {
                            dictResult.Add(dt.Key, dt.Value);
                        }
                        dictGps = PicGpsQuery(directories);
                        if (dictGps == null) return dictResult;
                        foreach (var gps in dictGps)
                        {
                            dictResult.Add(gps.Key, gps.Value + "");
                        }
                        return dictResult;
                    case false:
                        dictResult = new Dictionary<string, string>
                        {
                            { "type", "Vid" }
                        };
                        dictDt = VidDtQuery(file);
                        foreach (var dt in dictDt)
                        {
                            dictResult.Add(dt.Key, dt.Value);
                        }
                        dictGps = VidGpsQuery(file);
                        if (dictGps == null) return dictResult;
                        foreach (var gps in dictGps)
                        {
                            dictResult.Add(gps.Key, gps.Value + "");
                        }
                        return dictResult;
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static Dictionary<string, string> PicDtQuery(IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            try
            {
                var subdirDt = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                var strDt = subdirDt?.GetDescription(ExifDirectoryBase.TagDateTime);
                const string strDtFormat = "yyyy.MM.dd_HHmmss";
                var dtDt = DateTime.ParseExact(strDt, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture);
                var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
                var timestamp = (dtDt.Ticks - startTime.Ticks) / 10000;
                var dictDt = new Dictionary<string, string>
                {
                    {"datetime", dtDt.ToString(strDtFormat)},
                    {"timestamp", timestamp + ""}
                };
                return dictDt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static Dictionary<string, string> PicGpsQuery(IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            try
            {
                var subdirGps = directories.OfType<GpsDirectory>().FirstOrDefault();
                var strLng = subdirGps?.GetDescription(GpsDirectory.TagLongitude);
                var strLat = subdirGps?.GetDescription(GpsDirectory.TagLatitude);
                var strLngRef = subdirGps?.GetDescription(GpsDirectory.TagLongitudeRef);
                var strLatRef = subdirGps?.GetDescription(GpsDirectory.TagLatitudeRef);
                var strAlt = subdirGps?.GetDescription(GpsDirectory.TagAltitude);
                var strAltRef = subdirGps?.GetDescription(GpsDirectory.TagAltitudeRef);
                int intLngRef = 1, intLatRef = 1, intAltRef = 1;
                switch (strLngRef)
                {
                    case "E":
                        intLngRef = 1;
                        break;
                    case "W":
                        intLngRef = -1;
                        break;
                }
                switch (strLatRef)
                {
                    case "N":
                        intLatRef = 1;
                        break;
                    case "S":
                        intLatRef = -1;
                        break;
                }
                var dblLngHor = double.Parse(strLng?.Split('°', ' ')[0] ?? throw new InvalidOperationException());
                var strRmHor = strLng.Replace(dblLngHor + "° ", "");
                var dblLngMin = double.Parse(strRmHor.Split('\'', ' ')[0]);
                var dblLngSec = double.Parse(strLng.Replace(dblLngHor + "° " + dblLngMin + "\' ", "").Replace("\"", ""));
                var dblLng = intLngRef * Math.Round(dblLngHor + dblLngMin / 60 + dblLngSec / 3600, 8);
                var dblLatHor = double.Parse(strLat.Split('°', ' ')[0]);
                var dblLatMin = double.Parse(strLat.Replace(dblLatHor + "° ", "").Split('\'', ' ')[0]);
                var dblLatSec = double.Parse(strLat.Replace(dblLatHor + "° " + dblLatMin + "\' ", "").Replace("\"", ""));
                var dblLat = intLatRef * Math.Round(dblLatHor + dblLatMin / 60 + dblLatSec / 3600, 8);
                switch (strAltRef)
                {
                    case "Above sea level":
                        intAltRef = 1;
                        break;
                    case "Below sea level":
                        intAltRef = -1;
                        break;
                }
                var dblAlt = intAltRef * double.Parse(strAlt?.Replace(" metres", "") ?? throw new InvalidOperationException());
                var dictGps = new Dictionary<string, string>
                {
                    {"longitude", dblLng + ""},
                    {"latitude", dblLat + ""},
                    {"altitude", dblAlt + ""}
                };
                return dictGps;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Dictionary<string, string> VidDtQuery(FileSystemInfo file)
        {
            string strDt;
            var mi = new MediaInfo();
            mi.Open(file.FullName);
            try
            {
                strDt = mi.Get(StreamKind.Video, 0, "Encoded_Date");
                if (string.IsNullOrEmpty(strDt))
                {
                    strDt = mi.Get(StreamKind.Video, 0, "Tagged_Date");
                }
                if (string.IsNullOrEmpty(strDt))
                {
                    strDt = mi.Get(StreamKind.General, 0, "Recorded_Date");
                }
                mi.Close();
            }
            catch (Exception)
            {
                mi.Close();
                return null;
            }
            mi.Dispose();
            const string strDtFormat = "yyyy.MM.dd_HHmmss";
            var strTz = strDt.Substring(0, 3);
            strDt = strDt.Replace(strTz + " ", "");
            var dtDt = DateTime.ParseExact(strDt, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
            dtDt = TimeZoneInfo.ConvertTimeFromUtc(dtDt, Local);
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var timestamp = (dtDt.Ticks - startTime.Ticks) / 10000;
            var dictDt = new Dictionary<string, string>
            {
                {"datetime", dtDt.ToString(strDtFormat)},
                {"timestamp", timestamp + ""}
            };
            return dictDt;
        }

        private static Dictionary<string, string> VidGpsQuery(FileSystemInfo file)
        {
            try
            {
                var mi = new MediaInfo();
                mi.Open(file.FullName);
                string strGps;
                try
                {
                    strGps = mi.Get(StreamKind.General, 0, "xyz");
                }
                catch (Exception)
                {
                    mi.Close();
                    return null;
                }
                mi.Close();
                var strGpsSplit = strGps.Split('+', '-');
                var strLat = strGpsSplit[1];
                var strLng = strGpsSplit[2];
                var strRef = strGps.Replace(strLat, "").Replace(strLng, "");
                int intLatRef = 1, intLngRef = 1;
                switch (strRef[0])
                {
                    case '+':
                        intLatRef = 1;
                        break;
                    case '-':
                        intLatRef = -1;
                        break;
                }
                switch (strRef[1])
                {
                    case '+':
                        intLngRef = 1;
                        break;
                    case '-':
                        intLngRef = -1;
                        break;
                }
                var dblLat = intLatRef * double.Parse(strLat);
                var dblLng = intLngRef * double.Parse(strLng.Replace("/", ""));
                var dictGps = new Dictionary<string, string>
                {
                    {"longitude", dblLng + ""},
                    {"Latitude", dblLat + ""}
                };
                return dictGps;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
