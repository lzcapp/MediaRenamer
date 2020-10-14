using CsvHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace CoordinateConvert {
    internal class Coordinate {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    class Program {
        static void Main() {
            var newcoors = new List<Coordinate>();
            // Read/Import CSV
            using (var reader = new StreamReader(@"D:\Documents\GIS\coordinate.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
                var coors = csv.GetRecords<Coordinate>();
                foreach (var coor in coors) {
                    Coordinate newcoor = null;
                    while (newcoor == null) {
                        newcoor = ConvertCoordinate(coor);
                    }
                    newcoors.Add(newcoor);
                    Console.WriteLine(coor.Longitude + "," + coor.Latitude + " --> " + newcoor.Longitude + "," + newcoor.Latitude);
                }
            }
            // Write/Export CSV
            using (var writer = new StreamWriter(@"D:\Documents\GIS\coordinate_convert.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
                csv.WriteRecords(newcoors);
            }
            Console.ReadKey();
        }

        private static Coordinate ConvertCoordinate(Coordinate coor) {
            const string apiUrl = "http://api.map.baidu.com/geoconv/v1/?";
            var coords = "coords=" + coor.Longitude + "," + coor.Latitude;
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
            var longitude = resultToken.SelectToken("x");
            var latitude = resultToken.SelectToken("y");
            coor.Longitude = longitude.ToString();
            coor.Latitude = latitude.ToString();
            return coor;
        }
    }
}
