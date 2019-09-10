# [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) DataSet [converters](https://www.newtonsoft.com/json/help/html/SerializationSettings.htm#Converters)

[![Build Status](https://anarchic.visualstudio.com/Json%20.NET%20DataSet%20Converters/_apis/build/status/AlesDo.DataSetConverters?branchName=master)](https://anarchic.visualstudio.com/Json%20.NET%20DataSet%20Converters/_build/latest?definitionId=1&branchName=master)

DataSet [converters](https://www.newtonsoft.com/json/help/html/SerializationSettings.htm#Converters) for [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) that provide full serialization for [DataSet](https://docs.microsoft.com/en-us/dotnet/api/system.data.dataset?view=netstandard-2.0)s and [DataTable](https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netstandard-2.0)s. 
They preserve all the state like the serialization built in the standard .NET serializers. 
This enables the use of JSON serialization for all scenarios where DataSets or DataTables are used.
Since all the state is preserved it is more data heavy and should be used only when this is required.

## Why this is usefull

The idea for this came of a need to modernize old WCF and ASMX web services to modern ASP.NET Core WebAPI services without the need to completely rewrite everything on the server and client side when DataSets have been used to send the data to clients. This way a slow transition from older technologies to using the latest is possible. It will be especially useful with .NET Core 3 that brings also WinForms and WPF client applications so you can completely move the whole stack into .NET Core with small changes. 

## How to use

The simplest way to use the converters is to add them to the json serializer default settings. This way they will be always used.
```csharp
JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
{
    Converters = new List<JsonConverter>() { new Json.Net.DataSetConverters.DataSetConverter(), new Json.Net.DataSetConverters.DataTableConverter() }
};
```

If you are using them only in specific cases you can specify the converter to use directly when you serialize or deserialize an object.

```csharp
string serializedDataSet = JsonConvert.SerializeObject(dataSet, new Json.Net.DataSetConverters.DataSetConverter())
DataSet deserialiedDataSet = JsonConvert.DeserializeObject<DataSet>(serializedDataSet, new Json.Net.DataSetConverters.DataSetConverter())
```

### In ASP.NET Core

To use the converters in an ASP.NET Core applications you need to set the converters in Startup file of the application when you ad MVC services.

```csharp
services.AddMvc().AddJsonOptions((jsonOptions) => 
{
   jsonOptions.SerializerSettings.Converters.Add(new Json.Net.DataSetConverters.DataTableConverter());
   jsonOptions.SerializerSettings.Converters.Add(new Json.Net.DataSetConverters.DataSetConverter());
});
```

## Performance

The first measurements show that the serialization speed is better than the old BinaryFormater and DataContractSerializer serialization.
Here are the results for performing serialization.

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.720 (1809/October2018Update/Redstone5)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.401
  [Host] : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), 64bit RyuJIT DEBUG

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                      Method | DataSetSize | WithChanges |        Mean |      Error |     StdDev |
|---------------------------- |------------ |------------ |------------:|-----------:|-----------:|
| **JsonDotNetDataSetConverters** |           **0** |       **False** |    **183.5 us** |   **3.631 us** |   **4.036 us** |
|             BinaryFormatter |           0 |       False |    336.0 us |   6.232 us |   5.204 us |
|      DataContractSerializer |           0 |       False |    304.8 us |   6.055 us |  11.809 us |
| **JsonDotNetDataSetConverters** |           **0** |        **True** |    **192.0 us** |   **2.948 us** |   **2.757 us** |
|             BinaryFormatter |           0 |        True |    346.7 us |   6.739 us |   8.763 us |
|      DataContractSerializer |           0 |        True |    321.7 us |   6.415 us |   9.403 us |
| **JsonDotNetDataSetConverters** |          **10** |       **False** |    **316.5 us** |   **5.824 us** |   **5.162 us** |
|             BinaryFormatter |          10 |       False |    615.0 us |  11.002 us |   9.753 us |
|      DataContractSerializer |          10 |       False |    537.9 us |  10.545 us |  17.326 us |
| **JsonDotNetDataSetConverters** |          **10** |        **True** |    **382.8 us** |   **4.058 us** |   **3.597 us** |
|             BinaryFormatter |          10 |        True |    754.2 us |  12.864 us |  11.403 us |
|      DataContractSerializer |          10 |        True |    635.0 us |  12.418 us |  18.587 us |
| **JsonDotNetDataSetConverters** |          **20** |       **False** |    **661.0 us** |  **13.175 us** |  **16.181 us** |
|             BinaryFormatter |          20 |       False |  1,236.3 us |  14.533 us |  12.883 us |
|      DataContractSerializer |          20 |       False |  1,023.7 us |  24.250 us |  22.684 us |
| **JsonDotNetDataSetConverters** |          **20** |        **True** |    **885.6 us** |  **17.272 us** |  **16.156 us** |
|             BinaryFormatter |          20 |        True |  1,701.0 us |  18.558 us |  17.359 us |
|      DataContractSerializer |          20 |        True |  1,431.9 us |  18.737 us |  16.610 us |
| **JsonDotNetDataSetConverters** |          **50** |       **False** |  **2,792.5 us** |  **55.437 us** |  **65.994 us** |
|             BinaryFormatter |          50 |       False |  5,205.5 us | 102.065 us | 104.813 us |
|      DataContractSerializer |          50 |       False |  4,473.5 us |  51.808 us |  48.461 us |
| **JsonDotNetDataSetConverters** |          **50** |        **True** |  **4,014.2 us** |  **80.267 us** |  **92.436 us** |
|             BinaryFormatter |          50 |        True |  7,025.5 us |  76.229 us |  71.304 us |
|      DataContractSerializer |          50 |        True |  6,046.8 us |  69.038 us |  64.578 us |
| **JsonDotNetDataSetConverters** |         **100** |       **False** | **10,271.5 us** | **141.593 us** | **132.446 us** |
|             BinaryFormatter |         100 |       False | 19,365.5 us | 204.383 us | 170.669 us |
|      DataContractSerializer |         100 |       False | 17,356.7 us | 361.559 us | 338.203 us |
| **JsonDotNetDataSetConverters** |         **100** |        **True** | **16,002.6 us** | **316.740 us** | **632.564 us** |
|             BinaryFormatter |         100 |        True | 27,181.9 us | 509.407 us | 500.305 us |
|      DataContractSerializer |         100 |        True | 23,696.7 us | 443.403 us | 455.342 us |

Deserialization results. Here the performance drops for large DataSets this needs more research in the future.

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.720 (1809/October2018Update/Redstone5)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.401
  [Host] : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), 64bit RyuJIT DEBUG

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                      Method | DataSetSize | WithChanges |        Mean |       Error |       StdDev |
|---------------------------- |------------ |------------ |------------:|------------:|-------------:|
| **JsonDotNetDataSetConverters** |           **0** |       **False** |    **659.9 us** |    **10.46 us** |     **9.788 us** |
|             BinaryFormatter |           0 |       False |  1,104.9 us |    14.85 us |    13.161 us |
|      DataContractSerializer |           0 |       False |  1,195.9 us |    10.70 us |    10.007 us |
| **JsonDotNetDataSetConverters** |           **0** |        **True** |    **706.6 us** |    **14.49 us** |    **19.839 us** |
|             BinaryFormatter |           0 |        True |  1,105.4 us |    16.73 us |    14.827 us |
|      DataContractSerializer |           0 |        True |  1,173.9 us |    15.67 us |    13.894 us |
| **JsonDotNetDataSetConverters** |          **10** |       **False** |  **1,247.0 us** |    **24.02 us** |    **22.471 us** |
|             BinaryFormatter |          10 |       False |  1,590.1 us |    19.65 us |    18.378 us |
|      DataContractSerializer |          10 |       False |  1,868.3 us |    14.70 us |    13.032 us |
| **JsonDotNetDataSetConverters** |          **10** |        **True** |  **1,736.0 us** |    **26.80 us** |    **23.759 us** |
|             BinaryFormatter |          10 |        True |  1,819.5 us |    36.09 us |    33.762 us |
|      DataContractSerializer |          10 |        True |  2,210.4 us |    34.44 us |    28.758 us |
| **JsonDotNetDataSetConverters** |          **20** |       **False** |  **2,466.1 us** |    **21.32 us** |    **19.939 us** |
|             BinaryFormatter |          20 |       False |  2,667.8 us |    48.24 us |    42.766 us |
|      DataContractSerializer |          20 |       False |  3,528.1 us |    71.48 us |    66.861 us |
| **JsonDotNetDataSetConverters** |          **20** |        **True** |  **4,133.2 us** |    **77.51 us** |    **72.499 us** |
|             BinaryFormatter |          20 |        True |  3,441.2 us |    43.25 us |    40.459 us |
|      DataContractSerializer |          20 |        True |  4,400.7 us |    79.30 us |    74.180 us |
| **JsonDotNetDataSetConverters** |          **50** |       **False** | **11,015.8 us** |   **169.63 us** |   **158.668 us** |
|             BinaryFormatter |          50 |       False |  9,749.1 us |   188.32 us |   209.321 us |
|      DataContractSerializer |          50 |       False | 14,695.4 us |   284.03 us |   265.686 us |
| **JsonDotNetDataSetConverters** |          **50** |        **True** | **21,371.1 us** |   **271.51 us** |   **240.691 us** |
|             BinaryFormatter |          50 |        True | 13,439.4 us |   151.07 us |   133.920 us |
|      DataContractSerializer |          50 |        True | 19,200.2 us |   224.70 us |   199.193 us |
| **JsonDotNetDataSetConverters** |         **100** |       **False** | **41,566.6 us** |   **799.06 us** |   **820.572 us** |
|             BinaryFormatter |         100 |       False | 35,905.6 us |   715.86 us |   905.336 us |
|      DataContractSerializer |         100 |       False | 54,278.2 us |   603.46 us |   503.914 us |
| **JsonDotNetDataSetConverters** |         **100** |        **True** | **85,454.5 us** | **1,665.73 us** | **2,045.661 us** |
|             BinaryFormatter |         100 |        True | 50,420.4 us |   994.80 us | 1,768.249 us |
|      DataContractSerializer |         100 |        True | 73,338.1 us |   383.41 us |   320.163 us |


## Why F#

To experiment with it and see how it interoperates with C# code. 