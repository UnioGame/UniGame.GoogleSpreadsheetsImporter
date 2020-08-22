# Unity Google Sheets v.4 Converter

Unity3D Google Spreadsheet export/import library

**Odin Inspector Asset recommended to usage with this Package (https://odininspector.com)**

## Features

- Export Serializable Project Data into Google Spreadsheet
- Support Custom Import/Export data providers
- Support nested synched spreadsheed fields
- Export/import into Addressables AssetReferences
- Export/import into Unity Scriptable Objects

## Import/Export Attributes

=============

**SpreadsheetTargetAttribute**

Allow to mark serializable class as Synched with Google Spreadsheet. 

**SpreadsheetTargetAttribute(string sheetName = "",string keyField = "",bool syncAllFields = true)**

Parameters:
- sheetName. Name of target Table into Google Spreadsheet. If **sheetName** empty, then class Type name will be used as SheetName
- keyField. Primary key field name for sync between spreadsheet and serialized class 
- syncAllFields. If TRUE, then try to sync all class fields data. If FALSE - synchronized only fields marked with attribute **[SpreadSheetFieldAttribute]**

=============

**SpreadSheetFieldAttribute**

All fields marked with this attribute will be synchronized with target Spreadsheet data

**public SpreadSheetFieldAttribute(string sheetField = "", bool isKey = false)**

Parameters:
- sheetField. Name of target Spreadsheet column
- isKey. If TRUE, then target field will be used as Primary Key field.

```csharp

[SpreadSheetField("DemoTable")]
public class DemoSO : ScriptableObject{

    [SpreadSheetField("KeyField",true)]
    public string key;

    [SpreadSheetField("ValueField",true)]
    [SerializableField]
    private string value;

}

```
![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/sheet1.png)

=============

Default Sheet Id

```csharp

[SpreadSheetField("DemoTable",syncAllFields: true)]
public class DemoSO : ScriptableObject{

    public string id; // Field with name Id | _id | ID will be used as Primary key by Default

    private string value; // syncAllFields value active. Import/Export try to find Sheet column with name "Value"

}

```

=============

Nested fields support

```csharp

[SpreadSheetField("DemoTable",syncAllFields: true)]
public class DemoSO : ScriptableObject{

    public string id; // Field with name Id | _id | ID will be used as Primary key by Default

    [SpreadSheetField("FooTable",syncAllFields: true)]
    private Foo2 value; // syncAllFields value active. Import/Export try to find Sheet column with name "Value"

}

```

## Editor Window

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/menu.png)

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/editor.png)


## Google API V4 .NET References

- https://developers.google.com/sheets/api/quickstart/dotnet

- https://googleapis.dev/dotnet/Google.Apis.Sheets.v4/latest/api/Google.Apis.Sheets.v4.html

## NPM Installation

```json
{
  "scopedRegistries": [
    {
      "name": "Unity",
      "url": "https://packages.unity.com",
      "scopes": [
        "com.unity"
      ]
    },
    {
      "name": "UniGame",
      "url": "http://packages.unigame.pro:4873/",
      "scopes": [
        "com.unigame"
      ]
    }
  ],
}
```

### Newtonsoft.Json for Unity 

https://github.com/jilleJr/Newtonsoft.Json-for-Unity

```json
{
  "scopedRegistries": [
    {
      "name": "Packages from jillejr",
      "url": "https://npm.cloudsmith.io/jillejr/newtonsoft-json-for-unity/",
      "scopes": ["jillejr"]
    }
  ],
  "dependencies": {
    "jillejr.newtonsoft.json-for-unity": "12.0.201",
  }
}


```

