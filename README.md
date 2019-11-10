# [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) DataSet [converters](https://www.newtonsoft.com/json/help/html/SerializationSettings.htm#Converters)

[![Build Status](https://dev.azure.com/AlesDo/Json.NET%20DataSet%20Converters/_apis/build/status/AlesDo.DataSetConverters?branchName=master)](https://dev.azure.com/AlesDo/Json.NET%20DataSet%20Converters/_build/latest?definitionId=1&branchName=master)
![NuGet](https://img.shields.io/nuget/v/Json.Net.DataSetConverters.svg)

DataSet [converters](https://www.newtonsoft.com/json/help/html/SerializationSettings.htm#Converters) for [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) that provide full serialization for [DataSet](https://docs.microsoft.com/en-us/dotnet/api/system.data.dataset?view=netstandard-2.0)s and [DataTable](https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netstandard-2.0)s. 
They preserve all the state like the serialization built in the standard .NET serializers. 
This enables the use of JSON serialization for all scenarios where DataSets or DataTables are used.
Since all the state is preserved it is more data heavy and should be used only when this is required.

## Why this is useful

The idea for this came of a need to modernize old WCF and ASMX web services to modern ASP.NET Core WebAPI services without the need to completely rewrite everything on the server and client side in cases where DataSets have been used to send the data to clients. This way a slow transition from older technologies to using the latest is possible. It is especially useful with .NET Core 3 that brings also WinForms and WPF client applications and you can completely move the whole stack into .NET Core. 

## How to use

The simplest way to use the converters is to add them to the json serializer default settings. This way they will override the built in converters and will always be used.

```csharp
JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
{
    Converters = new List<JsonConverter>() { new Json.Net.DataSetConverters.DataSetConverter(), new Json.Net.DataSetConverters.DataTableConverter() }
};
```

If you are using them only in specific cases you can specify the converter to use directly when you serialize or deserialize an object.

```csharp
string serializedDataSet = JsonConvert.SerializeObject(dataSet, new Json.Net.DataSetConverters.DataSetConverter());
DataSet deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(serializedDataSet, new Json.Net.DataSetConverters.DataSetConverter());

string serializedDataTable = JsonConvert.SerializeObject(dataTable, new Json.Net.DataSetConverters.DataTableConverter());
DataSet deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(serializedDataTable, new Json.Net.DataSetConverters.DataTableConverter());
```

### In ASP.NET Core

To use the converters in an ASP.NET Core applications you need to set the converters in Startup file of the application when you add MVC services.

### ASP.NET Core 2.0

```csharp
services.AddMvc().AddJsonOptions((jsonOptions) => 
{
   jsonOptions.SerializerSettings.Converters.Add(new Json.Net.DataSetConverters.DataTableConverter());
   jsonOptions.SerializerSettings.Converters.Add(new Json.Net.DataSetConverters.DataSetConverter());
});
```

### ASP.NET Core 3.0

```csharp
services.AddControllers().AddNewtonsoftJson((jsonOptions) => 
{
   jsonOptions.SerializerSettings.Converters.Add(new Json.Net.DataSetConverters.DataTableConverter());
   jsonOptions.SerializerSettings.Converters.Add(new Json.Net.DataSetConverters.DataSetConverter());
});
```

Check also the [samples solution](https://github.com/AlesDo/DataSetConverters/tree/master/Json.Net.DataSetConverters.Samples) for a working example of a Windows Forms application taking to an ASP.NET Core WebApi.

## Performance

The first measurements show that the serialization speed is better than the `BinaryFormater` and `DataContractSerializer` serialization.

In the chart below are the results of benchmarking the `DataSet` serialization. The DataSetSize is the number of rows in the main database tables. The test data set consists of 6 tables two of them have the number of row same as the size. The other tables that are related get 10 rows for each parent row. For size 10 there are two tables with 10 rows and 4 tables with 100 rows. The WithChanges parameter means if the data set had changes when it was serialized or not. When there are changes more data has to be serialized to have also all the original values. The DataSet with changes has changes in half of the rows.

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.737 (1809/October2018Update/Redstone5)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.402
  [Host] : .NET Core 2.2.7 (CoreCLR 4.6.28008.02, CoreFX 4.6.28008.03), 64bit RyuJIT DEBUG

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                      Method | DataSetSize | WithChanges |         Mean |         Error |        StdDev |       Median |      Gen 0 |     Gen 1 |     Gen 2 |    Allocated |
|---------------------------- |------------ |------------ |-------------:|--------------:|--------------:|-------------:|-----------:|----------:|----------:|-------------:|
| **JsonDotNetDataSetConverters** |           **0** |       **False** |     **185.1 us** |      **3.243 us** |      **3.034 us** |     **184.9 us** |    **29.7852** |         **-** |         **-** |    **122.92 KB** |
|             BinaryFormatter |           0 |       False |     358.4 us |      7.125 us |     12.290 us |     357.9 us |    41.0156 |         - |         - |    168.68 KB |
|      DataContractSerializer |           0 |       False |     308.3 us |      6.159 us |      8.222 us |     308.6 us |    21.4844 |         - |         - |     89.07 KB |
| **JsonDotNetDataSetConverters** |           **0** |        **True** |     **184.9 us** |      **2.314 us** |      **2.165 us** |     **185.3 us** |    **30.0293** |    **0.2441** |         **-** |    **123.55 KB** |
|             BinaryFormatter |           0 |        True |     358.0 us |      6.774 us |      7.530 us |     357.0 us |    41.5039 |         - |         - |    171.49 KB |
|      DataContractSerializer |           0 |        True |     310.9 us |      6.022 us |      6.935 us |     307.9 us |    21.9727 |         - |         - |      91.2 KB |
| **JsonDotNetDataSetConverters** |           **1** |       **False** |     **281.1 us** |      **3.442 us** |      **3.220 us** |     **280.6 us** |    **41.0156** |         **-** |         **-** |    **168.36 KB** |
|             BinaryFormatter |           1 |       False |     556.9 us |      4.106 us |      3.429 us |     556.2 us |    86.9141 |   33.2031 |   16.6016 |    372.22 KB |
|      DataContractSerializer |           1 |       False |     472.6 us |      9.187 us |     14.303 us |     471.0 us |    49.8047 |         - |         - |    205.47 KB |
| **JsonDotNetDataSetConverters** |           **1** |        **True** |     **331.6 us** |      **4.829 us** |      **4.032 us** |     **332.2 us** |    **44.4336** |         **-** |         **-** |    **182.81 KB** |
|             BinaryFormatter |           1 |        True |     624.8 us |     12.035 us |     11.257 us |     623.5 us |    95.7031 |   33.2031 |   16.6016 |    411.25 KB |
|      DataContractSerializer |           1 |        True |     543.2 us |     10.355 us |      9.179 us |     544.1 us |    55.6641 |    0.9766 |         - |     230.1 KB |
| **JsonDotNetDataSetConverters** |           **2** |       **False** |     **589.2 us** |     **11.755 us** |     **12.578 us** |     **587.9 us** |    **68.3594** |   **31.2500** |   **19.5313** |     **302.4 KB** |
|             BinaryFormatter |           2 |       False |   1,128.3 us |     12.758 us |     11.933 us |   1,127.3 us |   201.1719 |  107.4219 |   76.1719 |     922.6 KB |
|      DataContractSerializer |           2 |       False |     924.2 us |     20.510 us |     41.431 us |     914.2 us |   110.3516 |   21.4844 |         - |    456.82 KB |
| **JsonDotNetDataSetConverters** |           **2** |        **True** |     **770.3 us** |     **15.317 us** |     **16.389 us** |     **768.0 us** |    **83.9844** |   **41.0156** |   **24.4141** |    **373.68 KB** |
|             BinaryFormatter |           2 |        True |   1,522.3 us |     32.281 us |     59.834 us |   1,510.4 us |   253.9063 |  136.7188 |  105.4688 |   1196.11 KB |
|      DataContractSerializer |           2 |        True |   1,218.9 us |     24.036 us |     49.099 us |   1,206.8 us |   156.2500 |   46.8750 |   23.4375 |    673.83 KB |
| **JsonDotNetDataSetConverters** |           **5** |       **False** |   **2,560.1 us** |     **48.560 us** |     **57.807 us** |   **2,541.5 us** |   **226.5625** |  **132.8125** |   **70.3125** |   **1206.88 KB** |
|             BinaryFormatter |           5 |       False |   4,990.2 us |     97.896 us |    146.527 us |   4,940.7 us |   828.1250 |  468.7500 |  382.8125 |    5221.6 KB |
|      DataContractSerializer |           5 |       False |   4,139.2 us |     69.680 us |     61.769 us |   4,132.4 us |   593.7500 |  281.2500 |  156.2500 |    2666.4 KB |
| **JsonDotNetDataSetConverters** |           **5** |        **True** |   **3,711.5 us** |     **51.003 us** |     **45.212 us** |   **3,715.3 us** |   **304.6875** |  **210.9375** |   **89.8438** |   **1704.42 KB** |
|             BinaryFormatter |           5 |        True |   6,696.9 us |    126.284 us |    129.684 us |   6,702.9 us |  1085.9375 |  726.5625 |  500.0000 |   6104.32 KB |
|      DataContractSerializer |           5 |        True |   5,634.8 us |    106.980 us |    100.069 us |   5,616.8 us |   718.7500 |  250.0000 |  148.4375 |   3222.26 KB |
| **JsonDotNetDataSetConverters** |          **10** |       **False** |   **9,857.9 us** |    **175.129 us** |    **163.816 us** |   **9,798.0 us** |   **718.7500** |  **468.7500** |  **218.7500** |   **4435.38 KB** |
|             BinaryFormatter |          10 |       False |  18,740.8 us |    365.961 us |    375.816 us |  18,795.5 us |  2531.2500 | 1531.2500 |  968.7500 |     20396 KB |
|      DataContractSerializer |          10 |       False |  17,524.3 us |    318.714 us |    298.125 us |  17,500.9 us |  2062.5000 | 1062.5000 |  531.2500 |  10405.68 KB |
| **JsonDotNetDataSetConverters** |          **10** |        **True** |  **14,875.2 us** |    **222.638 us** |    **197.363 us** |  **14,853.4 us** |  **1046.8750** |  **656.2500** |  **296.8750** |   **6435.99 KB** |
|             BinaryFormatter |          10 |        True |  26,857.3 us |    535.243 us |    817.372 us |  26,581.0 us |  3062.5000 | 1843.7500 |  968.7500 |  23911.95 KB |
|      DataContractSerializer |          10 |        True |  24,276.8 us |    450.302 us |    442.257 us |  24,247.0 us |  2593.7500 | 1468.7500 |  500.0000 |  12611.06 KB |
| **JsonDotNetDataSetConverters** |          **20** |       **False** |  **42,643.4 us** |    **852.061 us** |  **1,014.318 us** |  **42,636.0 us** |  **2333.3333** | **1083.3333** |  **416.6667** |  **17456.33 KB** |
|             BinaryFormatter |          20 |       False |  82,315.3 us |  1,580.682 us |  1,999.056 us |  82,496.9 us |  6714.2857 | 2285.7143 | 1142.8571 |  81339.63 KB |
|      DataContractSerializer |          20 |       False |  71,389.8 us |  1,357.061 us |  1,133.207 us |  70,833.7 us |  6625.0000 | 2125.0000 | 1000.0000 |  41558.78 KB |
| **JsonDotNetDataSetConverters** |          **20** |        **True** |  **62,596.4 us** |  **1,187.188 us** |  **1,110.496 us** |  **62,710.7 us** |  **3444.4444** | **1555.5556** |  **666.6667** |  **25414.93 KB** |
|             BinaryFormatter |          20 |        True | 116,255.9 us |  1,695.100 us |  1,585.598 us | 115,564.4 us |  9000.0000 | 2200.0000 | 1200.0000 |  95496.68 KB |
|      DataContractSerializer |          20 |        True |  97,305.9 us |  1,881.012 us |  2,378.878 us |  97,368.6 us |  8666.6667 | 2000.0000 | 1000.0000 |  50444.49 KB |
| **JsonDotNetDataSetConverters** |          **50** |       **False** | **271,827.5 us** |  **5,335.898 us** |  **6,144.828 us** | **272,603.2 us** | **12500.0000** | **4500.0000** | **1000.0000** | **108795.27 KB** |
|             BinaryFormatter |          50 |       False | 539,977.1 us | 10,726.722 us | 25,906.239 us | 529,115.4 us | 38000.0000 | 2000.0000 | 1000.0000 | 471028.87 KB |
|      DataContractSerializer |          50 |       False | 453,602.6 us |  8,944.912 us |  8,367.077 us | 452,167.3 us | 39000.0000 | 3000.0000 | 2000.0000 |  223430.1 KB |
| **JsonDotNetDataSetConverters** |          **50** |        **True** | **378,880.5 us** |  **7,665.314 us** |  **8,201.800 us** | **376,318.4 us** | **18000.0000** | **6000.0000** | **1000.0000** | **158837.55 KB** |
|             BinaryFormatter |          50 |        True | 745,234.5 us | 15,690.468 us | 27,065.286 us | 736,430.3 us | 52000.0000 | 2000.0000 | 1000.0000 | 625330.23 KB |
|      DataContractSerializer |          50 |        True | 637,752.0 us |  9,188.451 us |  8,594.883 us | 636,959.9 us | 53000.0000 | 3000.0000 | 2000.0000 | 342876.66 KB |


In the following chart are the results for deserialization. Here it is visible that for big DataSets with changes the performance starts to fall behind the `BinaryFormater` and `DataContractSerializer` serialization.

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.737 (1809/October2018Update/Redstone5)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.402
  [Host] : .NET Core 2.2.7 (CoreCLR 4.6.28008.02, CoreFX 4.6.28008.03), 64bit RyuJIT DEBUG

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                      Method | DataSetSize | WithChanges |           Mean |        Error |        StdDev |         Median |      Gen 0 |      Gen 1 |     Gen 2 |    Allocated |
|---------------------------- |------------ |------------ |---------------:|-------------:|--------------:|---------------:|-----------:|-----------:|----------:|-------------:|
| **JsonDotNetDataSetConverters** |           **0** |       **False** |       **678.6 us** |     **13.17 us** |     **14.096 us** |       **675.9 us** |    **51.7578** |     **0.9766** |         **-** |    **214.19 KB** |
|             BinaryFormatter |           0 |       False |     1,116.3 us |     15.17 us |     14.192 us |     1,113.5 us |   125.0000 |    21.4844 |         - |    523.13 KB |
|      DataContractSerializer |           0 |       False |     1,209.4 us |     21.66 us |     20.264 us |     1,206.1 us |   121.0938 |    37.1094 |         - |       568 KB |
| **JsonDotNetDataSetConverters** |           **0** |        **True** |       **694.4 us** |     **10.58 us** |      **9.899 us** |       **690.9 us** |    **52.7344** |     **9.7656** |         **-** |    **217.88 KB** |
|             BinaryFormatter |           0 |        True |     1,162.1 us |     22.69 us |     21.227 us |     1,155.2 us |   123.0469 |    41.0156 |         - |    526.34 KB |
|      DataContractSerializer |           0 |        True |     1,211.5 us |     18.65 us |     17.444 us |     1,209.2 us |   121.0938 |    37.1094 |         - |    569.54 KB |
| **JsonDotNetDataSetConverters** |           **1** |       **False** |     **1,122.8 us** |     **21.88 us** |     **28.449 us** |     **1,115.2 us** |    **87.8906** |          **-** |         **-** |    **360.67 KB** |
|             BinaryFormatter |           1 |       False |     1,515.1 us |     29.45 us |     24.595 us |     1,521.4 us |   142.5781 |    35.1563 |         - |    658.44 KB |
|      DataContractSerializer |           1 |       False |     1,828.5 us |     36.57 us |     84.021 us |     1,805.3 us |   150.3906 |    50.7813 |         - |    718.66 KB |
| **JsonDotNetDataSetConverters** |           **1** |        **True** |     **1,503.9 us** |     **30.01 us** |     **28.068 us** |     **1,508.4 us** |   **109.3750** |          **-** |         **-** |    **454.96 KB** |
|             BinaryFormatter |           1 |        True |     1,684.2 us |     26.94 us |     22.496 us |     1,694.9 us |   154.2969 |    50.7813 |         - |    712.91 KB |
|      DataContractSerializer |           1 |        True |     1,928.5 us |     30.61 us |     28.636 us |     1,929.0 us |   177.7344 |     5.8594 |         - |    744.95 KB |
| **JsonDotNetDataSetConverters** |           **2** |       **False** |     **2,223.4 us** |     **44.16 us** |     **43.376 us** |     **2,223.0 us** |   **171.8750** |          **-** |         **-** |    **709.51 KB** |
|             BinaryFormatter |           2 |       False |     2,515.7 us |     49.51 us |     66.093 us |     2,517.0 us |   214.8438 |    89.8438 |   31.2500 |    971.52 KB |
|      DataContractSerializer |           2 |       False |     3,300.8 us |     55.17 us |     89.091 us |     3,297.1 us |   203.1250 |    70.3125 |         - |    982.12 KB |
| **JsonDotNetDataSetConverters** |           **2** |        **True** |     **3,721.2 us** |     **39.41 us** |     **32.912 us** |     **3,730.5 us** |   **257.8125** |    **11.7188** |         **-** |   **1065.74 KB** |
|             BinaryFormatter |           2 |        True |     3,092.3 us |     37.57 us |     33.308 us |     3,088.9 us |   253.9063 |   121.0938 |   54.6875 |   1178.73 KB |
|      DataContractSerializer |           2 |        True |     3,991.6 us |     78.23 us |     86.957 us |     4,003.0 us |   218.7500 |    70.3125 |         - |   1081.88 KB |
| **JsonDotNetDataSetConverters** |           **5** |       **False** |    **10,312.3 us** |    **199.75 us** |    **186.845 us** |    **10,345.8 us** |   **578.1250** |   **265.6250** |         **-** |   **3209.54 KB** |
|             BinaryFormatter |           5 |       False |     9,282.5 us |    163.71 us |    145.128 us |     9,317.7 us |   562.5000 |   359.3750 |  171.8750 |   3236.74 KB |
|      DataContractSerializer |           5 |       False |    13,772.2 us |    172.51 us |    161.363 us |    13,785.6 us |   500.0000 |   234.3750 |         - |   2980.27 KB |
| **JsonDotNetDataSetConverters** |           **5** |        **True** |    **20,689.0 us** |    **386.87 us** |    **361.876 us** |    **20,586.0 us** |  **1000.0000** |   **343.7500** |         **-** |   **5462.87 KB** |
|             BinaryFormatter |           5 |        True |    13,395.7 us |    266.82 us |    407.460 us |    13,304.1 us |   765.6250 |   515.6250 |  265.6250 |   4583.63 KB |
|      DataContractSerializer |           5 |        True |    18,706.9 us |    364.71 us |    638.756 us |    18,530.4 us |   656.2500 |   281.2500 |         - |   3755.72 KB |
| **JsonDotNetDataSetConverters** |          **10** |       **False** |    **40,934.8 us** |    **815.56 us** |  **1,060.462 us** |    **40,511.8 us** |  **2076.9231** |   **461.5385** |         **-** |   **12001.5 KB** |
|             BinaryFormatter |          10 |       False |    34,473.3 us |    680.60 us |    976.103 us |    34,317.1 us |  1533.3333 |   933.3333 |  333.3333 |  11236.68 KB |
|      DataContractSerializer |          10 |       False |    54,374.4 us |  1,024.34 us |  1,179.627 us |    54,357.2 us |  1700.0000 |   800.0000 |         - |   9889.88 KB |
| **JsonDotNetDataSetConverters** |          **10** |        **True** |    **85,297.8 us** |    **841.61 us** |    **702.783 us** |    **85,080.1 us** |  **3428.5714** |   **428.5714** |         **-** |  **21006.48 KB** |
|             BinaryFormatter |          10 |        True |    52,547.2 us |  1,042.03 us |  1,741.006 us |    52,461.5 us |  2200.0000 |  1400.0000 |  500.0000 |  16624.63 KB |
|      DataContractSerializer |          10 |        True |    76,321.8 us |  1,500.98 us |  2,104.165 us |    76,353.1 us |  2142.8571 |   571.4286 |         - |  13003.02 KB |
| **JsonDotNetDataSetConverters** |          **20** |       **False** |   **171,720.6 us** |  **4,517.51 us** |  **4,436.801 us** |   **170,494.0 us** |  **8333.3333** |  **2000.0000** |  **666.6667** |  **47133.91 KB** |
|             BinaryFormatter |          20 |       False |   145,894.7 us |  2,885.35 us |  7,448.008 us |   143,292.7 us |  4500.0000 |  1000.0000 |  250.0000 |  43455.83 KB |
|      DataContractSerializer |          20 |       False |   227,485.7 us |  5,147.39 us |  7,215.928 us |   225,797.1 us |  6666.6667 |  2000.0000 |  666.6667 |  37567.41 KB |
| **JsonDotNetDataSetConverters** |          **20** |        **True** |   **363,358.8 us** |  **7,172.35 us** |  **9,817.591 us** |   **360,119.4 us** | **14000.0000** |  **2000.0000** |         **-** |  **82869.92 KB** |
|             BinaryFormatter |          20 |        True |   210,288.5 us |  4,408.47 us |  4,527.171 us |   209,002.3 us |  6666.6667 |  3000.0000 |  666.6667 |   65182.7 KB |
|      DataContractSerializer |          20 |        True |   309,407.8 us |  6,020.65 us |  6,933.391 us |   307,636.2 us |  9000.0000 |  3500.0000 | 1000.0000 |  50117.69 KB |
| **JsonDotNetDataSetConverters** |          **50** |       **False** | **1,128,471.8 us** | **21,293.46 us** | **20,913.027 us** | **1,124,700.0 us** | **49000.0000** | **17000.0000** | **2000.0000** | **295707.98 KB** |
|             BinaryFormatter |          50 |       False | 1,007,059.9 us | 20,784.24 us | 44,292.908 us |   993,216.7 us | 28000.0000 | 11000.0000 | 2000.0000 | 272300.44 KB |
|      DataContractSerializer |          50 |       False | 1,587,364.6 us | 31,170.82 us | 60,055.585 us | 1,570,319.4 us | 38000.0000 | 16000.0000 | 3000.0000 | 236474.21 KB |
| **JsonDotNetDataSetConverters** |          **50** |        **True** | **2,565,057.0 us** | **36,891.88 us** | **32,703.687 us** | **2,570,743.4 us** | **91000.0000** | **26000.0000** | **2000.0000** | **521113.01 KB** |
|             BinaryFormatter |          50 |        True | 1,375,263.6 us | 21,581.35 us | 19,131.302 us | 1,377,505.1 us | 38000.0000 | 15000.0000 | 3000.0000 | 409621.34 KB |
|      DataContractSerializer |          50 |        True | 2,015,755.9 us | 30,344.04 us | 26,899.201 us | 2,016,083.8 us | 48000.0000 | 19000.0000 | 3000.0000 | 318373.57 KB |


## Why F#

To learn and experiment with it and see how it interoperates with C# code. 