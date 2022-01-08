# MediaRenamer

A Tool to **Rename** Media Files by Their `Taken Date`.

## Third-party Usage

- NuGet packages:
  - **MetadataExtractor**
    - NuGet: <https://www.nuget.org/packages/MetadataExtractor/>
    - GitHub: <https://github.com/drewnoakes/metadata-extractor-dotnet>
  - **MediaInfo.Wrapper.Core**
    - NuGet: <https://www.nuget.org/packages/MediaInfo.Wrapper.Core/>
    - GitHub: <https://github.com/yartat/MP-MediaInfo>

## Framework Targeting

- .Net Core 3.1

## Parameters List

- MediaInfo:
  [MediaInfo_Parameters_List.md](https://github.com/RainySummerLuo/MediaRenamer/blob/master/MediaInfo_Parameters_List.md)

## Metadata Sample

(Only Some Specific Types, You Should Also Check Your Own Files)

- Image:
  [MetadataExtractor_Exif_Output.md](https://github.com/RainySummerLuo/MediaRenamer/blob/master/MetadataExtractor_Exif_Output.md)
- Video:
  [MediaInfo_Ouput.md](https://github.com/RainySummerLuo/MediaRenamer/blob/master/MediaInfo_Ouput.md)

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

  - All Tags:
    `Console.WriteLine(mi.Inform());`
  - A Specific Tag:
    - Encoded Time / Tagged Time:
      `strDt = MI.Get(StreamKind.Video, 0, "Encoded_Date");`
    - Recorded Time (in `General Stream`):
      `strDt = MI.Get(StreamKind.General, 0, "Recorded_Date");`

### Date/Time

#### Formatting Standard

- Refer to:
  **Combined date and time representations** in **[ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)**.

`<date>T<time>`

> A single point in time can be represented by concatenating a complete date expression, the letter T as a delimiter, and a valid time expression. For example, "2007-04-05T14:30".
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

### MD5

```c#
string strMD5;
using (var md5Instance = MD5.Create()) {
    using (var stream = File.OpenRead(file.FullName)) {
        var fileHash = md5Instance.ComputeHash(stream);
        strMD5 = BitConverter.ToString(fileHash).Replace("-", "").ToUpperInvariant();
    }
}
return strMD5[..3] + strMD5[^3..^0];
```
