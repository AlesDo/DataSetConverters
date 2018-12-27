# Json.NET DataSet converters
DataSet converters for [Json.Net](https://github.com/JamesNK/Newtonsoft.Json) that provide full serialization for [DataSet](https://docs.microsoft.com/en-us/dotnet/api/system.data.dataset?view=netstandard-2.0)s and [DataTable](https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netstandard-2.0)s. 
They preserve all the state like the serialization built in the standard .NET serializers. 
This enables the use of JSON serialization for all scenarios where DataSets or DataTables are used.
Since all the state is preserved it is more data heavy and should be used only when this is required.

To use it you can simply specify the converters when serializing and deserializing.
``` C#
string serializedDataSet = JsonConvert.SerializeObject(dataSet, new Json.Net.DataSetConverters.DataSetConverter())
DataSet deserialiedDataSet = JsonConvert.DeserializeObject<DataSet>(serializedDataSet, new Json.Net.DataSetConverters.DataSetConverter())
```

Another option is to set the converters in the default settings globally so they will always be used.
``` C#
JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
{
    Converters = new List<JsonConverter>() { new Json.Net.DataSetConverters.DataSetConverter(), new Json.Net.DataSetConverters.DataTableConverter() }
};
```