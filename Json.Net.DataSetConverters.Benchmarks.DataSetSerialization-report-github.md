``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.778 (1909/November2018Update/19H2)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.201
  [Host]        : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT DEBUG
  .NET Core 3.1 : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT

Job=.NET Core 3.1  Runtime=.NET Core 3.1  

```
|                         Method | DataSetSize | WithChanges |         Mean |        Error |      StdDev |       Median |      Gen 0 |     Gen 1 |     Gen 2 |    Allocated |
|------------------------------- |------------ |------------ |-------------:|-------------:|------------:|-------------:|-----------:|----------:|----------:|-------------:|
| **Json.NET DataSet Converters** |           **0** |       **False** |     **151.3 μs** |      **1.86 μs** |     **1.74 μs** |     **151.5 μs** |    **29.7852** |         **-** |         **-** |    **122.15 KB** |
|              Binary
Formatter |           0 |       False |     251.3 μs |      3.81 μs |     3.38 μs |     250.1 μs |    40.5273 |    7.8125 |         - |    167.57 KB |
|       DataContract
Serializer |           0 |       False |     207.1 μs |      3.25 μs |     3.04 μs |     207.5 μs |    20.9961 |    0.2441 |         - |     86.22 KB |
| **Json.NET DataSet Converters** |           **0** |        **True** |     **152.0 μs** |      **2.43 μs** |     **2.03 μs** |     **151.5 μs** |    **29.7852** |         **-** |         **-** |    **122.79 KB** |
|              Binary
Formatter |           0 |        True |     256.6 μs |      3.28 μs |     2.91 μs |     256.3 μs |    41.5039 |    5.8594 |         - |    170.01 KB |
|       DataContract
Serializer |           0 |        True |     213.6 μs |      2.80 μs |     2.49 μs |     214.3 μs |    21.4844 |    2.9297 |         - |     88.24 KB |
| **Json.NET DataSet Converters** |           **1** |       **False** |     **249.8 μs** |      **4.76 μs** |     **4.68 μs** |     **248.7 μs** |    **40.5273** |    **2.9297** |         **-** |    **167.91 KB** |
|              Binary
Formatter |           1 |       False |     468.0 μs |      7.88 μs |     6.99 μs |     465.8 μs |    76.1719 |   25.3906 |   25.3906 |    368.23 KB |
|       DataContract
Serializer |           1 |       False |     341.2 μs |      4.62 μs |     4.10 μs |     341.7 μs |    47.3633 |    5.8594 |         - |    195.23 KB |
| **Json.NET DataSet Converters** |           **1** |        **True** |     **298.7 μs** |      **4.32 μs** |     **3.61 μs** |     **298.4 μs** |    **48.3398** |    **0.4883** |         **-** |    **198.38 KB** |
|              Binary
Formatter |           1 |        True |     524.4 μs |      5.92 μs |     5.54 μs |     526.8 μs |   105.4688 |   44.9219 |   22.4609 |    428.41 KB |
|       DataContract
Serializer |           1 |        True |     450.5 μs |      5.80 μs |     5.14 μs |     449.1 μs |    57.6172 |    8.7891 |         - |    239.13 KB |
| **Json.NET DataSet Converters** |           **2** |       **False** |     **660.5 μs** |     **12.37 μs** |    **10.97 μs** |     **658.3 μs** |    **65.4297** |   **32.2266** |   **32.2266** |    **303.61 KB** |
|              Binary
Formatter |           2 |       False |   1,059.6 μs |     13.06 μs |    10.91 μs |   1,056.9 μs |   125.0000 |  125.0000 |  125.0000 |     916.3 KB |
|       DataContract
Serializer |           2 |       False |     698.1 μs |     13.45 μs |    31.97 μs |     682.6 μs |   104.4922 |   25.3906 |         - |    431.76 KB |
| **Json.NET DataSet Converters** |           **2** |        **True** |     **893.4 μs** |     **17.41 μs** |    **18.63 μs** |     **891.4 μs** |    **82.0313** |   **41.0156** |   **41.0156** |    **390.86 KB** |
|              Binary
Formatter |           2 |        True |   1,260.8 μs |     12.61 μs |    11.17 μs |   1,262.5 μs |   300.7813 |  140.6250 |  138.6719 |   1278.78 KB |
|       DataContract
Serializer |           2 |        True |   1,085.8 μs |     12.93 μs |    11.46 μs |   1,085.0 μs |   179.6875 |   64.4531 |   33.2031 |     728.3 KB |
| **Json.NET DataSet Converters** |           **5** |       **False** |   **2,596.4 μs** |     **51.82 μs** |    **61.69 μs** |   **2,599.3 μs** |   **273.4375** |  **171.8750** |   **97.6563** |   **1231.66 KB** |
|              Binary
Formatter |           5 |       False |   3,887.9 μs |     59.63 μs |    55.77 μs |   3,869.5 μs |   898.4375 |  625.0000 |  476.5625 |   5191.46 KB |
|       DataContract
Serializer |           5 |       False |   3,283.8 μs |     65.38 μs |    64.21 μs |   3,278.1 μs |   605.4688 |  296.8750 |  199.2188 |    2532.6 KB |
| **Json.NET DataSet Converters** |           **5** |        **True** |   **3,681.0 μs** |     **60.52 μs** |    **56.61 μs** |   **3,677.9 μs** |   **359.3750** |  **261.7188** |  **140.6250** |   **1755.79 KB** |
|              Binary
Formatter |           5 |        True |   6,773.3 μs |     67.47 μs |    63.11 μs |   6,792.3 μs |  1164.0625 |  710.9375 |  484.3750 |   6638.47 KB |
|       DataContract
Serializer |           5 |        True |   5,662.4 μs |     69.02 μs |    64.56 μs |   5,654.5 μs |  1070.3125 |  554.6875 |  406.2500 |   4592.45 KB |
| **Json.NET DataSet Converters** |          **10** |       **False** |   **9,738.7 μs** |    **156.39 μs** |   **138.64 μs** |   **9,704.3 μs** |   **750.0000** |  **500.0000** |  **250.0000** |   **4550.62 KB** |
|              Binary
Formatter |          10 |       False |  15,446.7 μs |    230.81 μs |   204.60 μs |  15,350.5 μs |  2343.7500 | 1437.5000 |  906.2500 |  20283.87 KB |
|       DataContract
Serializer |          10 |       False |  13,030.8 μs |    153.86 μs |   143.92 μs |  13,049.7 μs |  2093.7500 | 1203.1250 |  671.8750 |    9873.2 KB |
| **Json.NET DataSet Converters** |          **10** |        **True** |  **14,993.2 μs** |    **191.87 μs** |   **170.09 μs** |  **14,989.1 μs** |  **1187.5000** |  **781.2500** |  **406.2500** |   **6614.34 KB** |
|              Binary
Formatter |          10 |        True |  26,406.3 μs |    517.80 μs |   596.30 μs |  26,533.2 μs |  3000.0000 | 1406.2500 |  531.2500 |  26027.54 KB |
|       DataContract
Serializer |          10 |        True |  22,518.5 μs |    210.12 μs |   196.55 μs |  22,526.0 μs |  3437.5000 | 1781.2500 |  968.7500 |  18101.71 KB |
| **Json.NET DataSet Converters** |          **20** |       **False** |  **43,424.9 μs** |    **624.90 μs** |   **584.53 μs** |  **43,116.6 μs** |  **2750.0000** | **1666.6667** |  **833.3333** |  **17907.37 KB** |
|              Binary
Formatter |          20 |       False |  69,051.2 μs |  1,345.66 μs | 1,495.70 μs |  68,657.6 μs |  6125.0000 | 2000.0000 | 1000.0000 |  80848.34 KB |
|       DataContract
Serializer |          20 |       False |  58,077.0 μs |  1,098.40 μs | 1,027.45 μs |  58,051.6 μs |  6666.6667 | 2555.5556 | 1555.5556 |  39350.05 KB |
| **Json.NET DataSet Converters** |          **20** |        **True** |  **63,162.1 μs** |    **656.19 μs** |   **547.95 μs** |  **63,189.5 μs** |  **3875.0000** | **1875.0000** |  **875.0000** |  **26172.53 KB** |
|              Binary
Formatter |          20 |        True | 113,092.8 μs |  1,225.16 μs | 1,086.07 μs | 112,599.2 μs | 10600.0000 | 2400.0000 | 1400.0000 | 103869.48 KB |
|       DataContract
Serializer |          20 |        True |  98,035.2 μs |  1,479.72 μs | 1,384.13 μs |  97,706.0 μs | 10666.6667 | 2500.0000 | 1500.0000 |  72259.73 KB |
| **Json.NET DataSet Converters** |          **50** |       **False** | **273,610.5 μs** |  **2,506.86 μs** | **2,093.34 μs** | **272,776.0 μs** | **12000.0000** | **4000.0000** |         **-** | **112004.51 KB** |
|              Binary
Formatter |          50 |       False | 453,981.9 μs |  6,336.04 μs | 5,616.73 μs | 452,424.0 μs | 35000.0000 | 2000.0000 | 1000.0000 | 468281.73 KB |
|       DataContract
Serializer |          50 |       False | 380,818.9 μs |  2,434.57 μs | 1,900.75 μs | 381,434.6 μs | 36000.0000 | 3000.0000 | 2000.0000 | 210263.36 KB |
| **Json.NET DataSet Converters** |          **50** |        **True** | **402,134.0 μs** |  **4,699.28 μs** | **4,395.71 μs** | **401,165.8 μs** | **18000.0000** | **6000.0000** | **1000.0000** | **162758.95 KB** |
|              Binary
Formatter |          50 |        True | 741,734.4 μs | 10,442.43 μs | 9,767.86 μs | 735,858.3 μs | 61000.0000 | 2000.0000 | 1000.0000 | 678335.83 KB |
|       DataContract
Serializer |          50 |        True | 624,375.3 μs |  9,920.87 μs | 9,279.99 μs | 621,504.3 μs | 62000.0000 | 3000.0000 | 2000.0000 | 380320.84 KB |
