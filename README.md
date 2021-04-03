
# Unity3D Google Spreadsheets v.4 Support

Unity3D Google Spreadsheet export/import library

**Odin Inspector Asset recommended to usage with this Package (https://odininspector.com)**

## Features

- Export Serializable Project Data into Google Spreadsheet
- Support Custom Import/Export data providers
- Support nested synched spreadsheed fields
- Support export/import JSON to serializable classed
- Export/import into Addressables AssetReferences
- Export/import into Unity Scriptable Objects
- Export/import base Unity Assets types

## Table of Content

- [Data Definitions](#data-definitions)
- [Nested Spreadsheet tables](#nested-spreadsheet-tables)
- [Connect to Google Spreadsheet](#connect-to-google-spreadsheet)
- [Supported typed](#supported-types)

## Data Definitions

### Import and Export Attributes

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
![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/sheet_fields.png)

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

## Nested Spreadsheet tables

Nested fields support

```csharp

[SpreadSheetField("DemoTable",syncAllFields: true)]
public class DemoSO : ScriptableObject{

    public string id; // Field with name Id | _id | ID will be used as Primary key by Default

    [SpreadSheetField("ResourcesTable",syncAllFields: true)]
    private int cost; // syncAllFields value active. Import/Export try to find column with name "Cost" from sheet "ResourcesTable" 

}

```


![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/nested_sheet.png)

Nested Table

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/nested_table_field.png)

## Supported Types

Spreadsheet library support all base value types. 

### JSON support

For more complex scenarios JSON serialization can be used

```csharp

[SpreadSheetField("DemoTable",syncAllFields: true)]
public class DemoSO : ScriptableObject{

    public string id; // Field with name Id | _id | ID will be used as Primary key by Default

    [SpreadSheetField("ItemsTable",syncAllFields: true)]
    private ItemData defnition; // sync item data from json value value

}

[Serializable]
public class ItemData
{
    public int id;
    
    public int position;
}


```


![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/json_support1.png)

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/json_support2.png)

## Connect to Google Spreadsheet

### Editor Window

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/menu.png)


### Create Google Api Credentials


- HOWTO create api credentials https://developers.google.com/sheets/api/quickstart/dotnet

- Setup path to credential .json file and press "Connect Spreadsheet" 

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/editorapikey.png)

- Under target Google Profile allow application access to spreadsheets

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/editorapikey2.png)



### Spreadsheet Id's

- Now you can specify your spreadsheets

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/editor.png)

- Id's of your sheet can be found right from web page url

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/sheetid.png)

- Copy your table id and paste into importer window field

![](https://github.com/UniGameTeam/UniGame.GoogleSpreadsheetsImporter/blob/master/GitAssets/sheetid1.png)


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

### Official Newtonsoft.Json Unity Package

```json
{
  "name": "com.unity.nuget.newtonsoft-json",
  "displayName": "Newtonsoft Json",
  "version": "2.0.0",
  "unity": "2018.4",
  "description": "Newtonsoft Json for use in Unity projects and Unity packages. Currently synced to version 12.0.2.\n\nThis package is used for advanced json serialization and deserialization. Most Unity users will be better suited using the existing json tools built into Unity.\nTo avoid assembly clashes, please use this package if you intend to use Newtonsoft Json.",
  "type": "library",
  "repository": {
    "type": "git",
    "url": "git@github.cds.internal.unity3d.com:unity/com.unity.nuget.newtonsoft-json.git",
    "revision": "74ca86c283a2f63ba5b687451a0842ba924da907"
  }
}
```

add into your manifest dependency

```json
  "dependencies": {
    "com.unity.nuget.newtonsoft-json" : "2.0.0",
    ...
    ...
    ...
  }
```


### Trird party Newtonsoft.Json for Unity 

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

## Co-Processors

![2020-11-02_10-25-22](https://user-images.githubusercontent.com/26055406/97841423-d837cc00-1cf6-11eb-867f-5d53f3493664.png)

Selected co-processor execute after main import processor (after parsing and applying every row).


**Custom co-processor**

```csharp

[Serializable]
public class MyCustomCoProcessor : ICoProcessorHandle
{
    // some properties
    
    public void Apply(SheetValueInfo valueInfo, DataRow row)
    {
        // some code
    }
}

```

**Example**

For example, Nested Table Co-Processor applies nested google-table where filter is parsing pattern:

![2020-11-02_10-25-59](https://user-images.githubusercontent.com/26055406/97841997-f651fc00-1cf7-11eb-8fa8-580fd9504fad.png)

![2020-11-02_10-26-44](https://user-images.githubusercontent.com/26055406/97842017-fe11a080-1cf7-11eb-8b30-320bdb31282a.png)
