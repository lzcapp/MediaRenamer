# MediaFileRenamer
A Tool to **Rename** Media Files by Their `Taken Date`. <br/>

## Prerequisites
- Unzip the files in **[Debug.zip](https://github.com/RainySummerLuo/PhotoToolbox/releases/download/v1.0.1/Debug.zip)** in **Release v1.0.1** and put them under `.\VideoRenamer\bin\Debug` or `.\VideoRenamer\bin\Release` folder (depends on your configuration). <br/>
- ~~**or** Download **[MediaInfo.Dll](https://mediaarea.net/en/MediaInfo) corresponding to the platform to** `.\VideoRenamer\bin\Debug` or `.\VideoRenamer\bin\Release` folder (depends on your configuration).~~

## Third-party Usage
- Using nuget package: 
  - **MetadataExtractor**&nbsp;&nbsp;&nbsp;&nbsp;version: 2.0.0
    - NuGet:  https://www.nuget.org/packages/MetadataExtractor/
    - GitHub: https://github.com/drewnoakes/metadata-extractor-dotnet
  - **MediaInfoDotNet**&nbsp;&nbsp;&nbsp;&nbsp;version: 0.7.79.40925
    - NuGet:  https://www.nuget.org/packages/MediaInfoDotNet/
    - GitHub: https://github.com/cschlote/MediaInfoDotNet
- DDL:
  - **MediaInfo.Dll**&nbsp;&nbsp;&nbsp;&nbsp;version: 18.08.1.0
    - https://mediaarea.net/en/MediaInfo/Download/Windows

### Alternative
- Using official DDL and C# wrapper: <br/>
  SourceForge: https://sourceforge.net/p/mediainfo/code/5846/tree/MediaInfoLib/trunk/Project/MSCS2010/ <br/>
  - MediaInfoDLL.cs: https://sourceforge.net/p/mediainfo/code/5846/tree/MediaInfoLib/trunk/Project/MSCS2010/Example/MediaInfoDLL.cs
  - MediaInfo.Dll:  <br/>https://mediaarea.net/en/MediaInfo
  > To make ~~this example~~ working, you must put MediaInfo.Dll ~~and Example.ogg~~ in the ~~"./Bin/\_\_ConfigurationName\_\_"~~ `.\VideoRenamer\bin\Debug or .\VideoRenamer\bin\Release` folder and add **MediaInfoDll.cs** to your project <br/>

### DDL Update (Haven't Try Yet)
Download new [MediaInfo.Dll](https://mediaarea.net/en/MediaInfo) corresponding to the platform and replace the file in `.\VideoRenamer\bin\Debug` or `.\VideoRenamer\bin\Release` folder.

## Built With
- Visual Studio Community 2017

## Parameters List
- MediaInfo:
  [MediaInfo_Parameters_List.md](https://github.com/RainySummerLuo/MediaFileRenamer/blob/master/MediaInfo_Parameters_List.md)

## Metadata Sample (Only Some Specific Types, You Should Also Check Your Own Files)
- Image:
  [MetadataExtractor_Exif_Output.md](https://github.com/RainySummerLuo/VideoRenamer/blob/master/MetadataExtractor_Exif_Output.md)
- Video:
  [MediaInfo_Ouput.md](https://github.com/RainySummerLuo/VideoRenamer/blob/master/MediaInfo_Ouput.md)

## Implementation
### Metadata Querying
- Image:
  ```c#
  var directories = ImageMetadataReader.ReadMetadata(file.FullName);
  foreach (var t in directories) {
    Console.WriteLine(t.Name);
    for (var j = 0; j < t.TagCount; j++) {
      Console.WriteLine(t.Tags[j].ToString());
    }
  }
  ```
- Video:
  ```c#
  var mi = new MediaInfo();
  mi.Open(file.FullName);
  ```
    - All Tags: <br/>
      ```Console.WriteLine(mi.Inform());```
    - A Specific Tag: <br/>
      - Encoded Time / Tagged Time: <br/>
        ```strDt = MI.Get(StreamKind.Video, 0, "Encoded_Date");``` 
      - Recorded Time (in `General Stream`): <br/>
        ```strDt = MI.Get(StreamKind.General, 0, "Recorded_Date");``` <br/>

### Date/Time

#### Formating Standard
- Refer to: <br/>
  **Combined date and time representations** in **[ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)**, <br/>
  and **完全表示法** in **GB/T 7408-2005**. <br/>

`<date>T<time>` <br/>
> A single point in time can be represented by concatenating a complete date expression, the letter T as a delimiter, and a valid time expression. For example, "2007-04-05T14:30". <br/>
> If a time zone designator is required, it follows the combined date and time. For example, "2007-04-05T14:30Z" or "2007-04-05T12:30-02:00".

```c#
const string strDtFormat = "yyyy-MM-ddTHH:mm:ssZ";
var dtUtc = DateTime.ParseExact(strDt, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture);
return dtUtc.ToString(strDtFormat);
```

#### Timezone Conversion
```c#
ConvertTimeFromUtc(dtUtc, Local)
```

### Renaming when Facing Duplication
**Format:** xxx<b>_1</b>.jpg / xxx<b>_2</b>.jpg / ...
```c#
if (File.Exists(Path.Combine(dirOutputFull + file.Extension))) {
  var intDupInx = 1;
  var strDupName = dirOutputFull + "_" + intDupInx;
  while (File.Exists(Path.Combine(strDupName + file.Extension))) {
    intDupInx++;
    strDupName = dirOutputFull + "_" + intDupInx;
  }
  dirOutputFull = strDupName;
}
```
  
### Export GPS Data with CSV output
Export Photos' `GPS Data` into **[CSV](https://en.wikipedia.org/wiki/Comma-separated_values)** File.
Currently Only **Image** Files' GPS Data will be Extracted and Exported.

#### String Formating for CSV File
```c#
str = str.Replace("\"", "\"\"");
if (str.Contains(',') || str.Contains('"') || str.Contains('\r') || str.Contains('\n')) {
  str = $"\"{str}\"";
}
```

#### CSV File ~~I/~~ O (Only Output :stuck_out_tongue_winking_eye:)
```c#
var csvPath = Path.Combine(dirInput + "output.csv");
var csvFileStream = new FileStream(csvPath, FileMode.Create, FileAccess.Write);
var csvStreamWriter = new StreamWriter(csvFileStream, Encoding.UTF8);
var data = OutputCsv(file, strDtResult);
csvStreamWriter.WriteLine(data);
csvStreamWriter.Close();
csvFileStream.Close();
```
