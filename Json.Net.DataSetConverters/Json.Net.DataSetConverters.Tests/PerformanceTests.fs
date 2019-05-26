module PerformanceTests

open Xunit.Abstractions
open System.Diagnostics
open Newtonsoft.Json
open Json.Net.DataSetConverters
open Xunit
open System.Runtime.Serialization
open System.IO
open System.Runtime.Serialization.Formatters.Binary
open Json.Net.DataSetConverters.Tests.TypedDataSets
open Bogus
open System.Data

type PerformanceTester(testOutputHelper: ITestOutputHelper) =
    let generateTestDataSet() =
       let testDataSet = new TestDataSet()
       let testDataTable1RowGenerator =
           Faker<TestDataSet.DataTable1Row>()
               .CustomInstantiator(fun _f -> testDataSet.DataTable1.NewDataTable1Row())
               .RuleFor<string>((fun row -> row.Name), fun (f:Faker) -> f.Name.FullName())
               .RuleFor<sbyte>((fun row -> row.SByteValue), fun (f:Faker) -> f.Random.SByte())
               .RuleFor<uint16>((fun row -> row.UInt16Value), fun (f:Faker) -> f.Random.UShort())
               .RuleFor<uint32>((fun row -> row.UInt32Value), fun (f:Faker) -> f.Random.UInt())
               .RuleFor<uint64>((fun row -> row.UInt64Value), fun (f:Faker) -> f.Random.ULong())
       let testDataTable2RowGenerator =
           Faker<TestDataSet.DataTable2Row>()
               .CustomInstantiator(fun _f -> testDataSet.DataTable2.NewDataTable2Row())
               .RuleFor<string>((fun row -> row.Name), fun (f:Faker) -> f.Name.FullName())
               .RuleFor<bool>((fun row -> row.BooleanValue), fun (f:Faker) -> f.Random.Bool())
               //.RuleFor<obj>((fun row -> row.ObjectValue), fun (f:Faker) -> f.Random.Uuid())

       for _rowNumber = 1 to 1000 do
           let newRow = testDataTable1RowGenerator.Generate()
           testDataSet.DataTable1.AddDataTable1Row(newRow)
           for _x = 1 to 10 do
               let newDataTable2Row = testDataTable2RowGenerator.Generate()
               newDataTable2Row.DataTable1Id <- newRow.Id
               testDataSet.DataTable2.AddDataTable2Row(newDataTable2Row)

       testDataSet.AcceptChanges()
       testDataSet

    member private this.TestOutputHelper = testOutputHelper

    [<Fact>]
    member this.``serialize deserialize empty data set JSON DataSetConverters 10000 times``() =
        let dataSet = new DataSet()

        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 10000 do
            let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
            JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter()) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize DataSet JSON DataSetConverters {0} ms.", stopwatch.ElapsedMilliseconds)

    [<Fact>]
    member this.``serialize deserialize empty data set DataContractSerializer 10000 times``() =
        let dataSet = new DataSet()

        let dataContractSerializer = new DataContractSerializer(typeof<DataSet>)
        use memoryStream = new MemoryStream()
        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 10000 do
            memoryStream.Position <- 0L
            dataContractSerializer.WriteObject(memoryStream, dataSet)
            memoryStream.Position <- 0L
            dataContractSerializer.ReadObject(memoryStream) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize DataSet JSON DataSetConverters {0} ms.", stopwatch.ElapsedMilliseconds)

    [<Fact>]
    member this.``serialize deserialize empty data set BinaryFormatter 10000 times``() =
        let dataSet = new DataSet()

        let binaryFormatter = new BinaryFormatter()
        use memoryStream = new MemoryStream()
        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 10000 do
            memoryStream.Position <- 0L
            binaryFormatter.Serialize(memoryStream, dataSet)
            memoryStream.Position <- 0L
            binaryFormatter.Deserialize(memoryStream) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize DataSet JSON BinaryFormatter {0} ms.", stopwatch.ElapsedMilliseconds)


    [<Fact>]
    member this.``serialize deserialize big data set JSON DataSetConverters 1000 times``() =
        let testDataSet = generateTestDataSet()

        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 1000 do
            let jsonDataSet = JsonConvert.SerializeObject(testDataSet, DataSetConverter())
            JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter()) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize Large DataSet JSON DataSetConverters {0} ms.", stopwatch.ElapsedMilliseconds)

    [<Fact>]
    member this.``serialize deserialize big data set JSON DataContractSerializer 1000 times``() =
        let testDataSet = generateTestDataSet()

        let dataContractSerializer = new DataContractSerializer(typeof<TestDataSet>)
        use memoryStream = new MemoryStream()
        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 1000 do
            memoryStream.Position <- 0L
            dataContractSerializer.WriteObject(memoryStream, testDataSet)
            memoryStream.Position <- 0L
            dataContractSerializer.ReadObject(memoryStream) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize Large DataSet JSON DataContractSerializer {0} ms.", stopwatch.ElapsedMilliseconds)

    [<Fact>]
    member this.``serialize deserialize big data set JSON BinaryFormatter 1000 times``() =
        let testDataSet = generateTestDataSet()

        let binaryFormatter = new BinaryFormatter()
        use memoryStream = new MemoryStream()
        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 1000 do
            memoryStream.Position <- 0L
            binaryFormatter.Serialize(memoryStream, testDataSet)
            memoryStream.Position <- 0L
            binaryFormatter.Deserialize(memoryStream) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize Large DataSet JSON BinaryFormatter {0} ms.", stopwatch.ElapsedMilliseconds)

