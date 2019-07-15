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
open System
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
                //.RuleFor<obj>((fun row -> row.ObjectValue), fun (f:Faker) -> f.Person.FullName)
        let testDataTable3RowGenerator =
            Faker<TestDataSet.DataTable3Row>()
                .CustomInstantiator(fun _f -> testDataSet.DataTable3.NewDataTable3Row())
                .RuleFor<string>((fun row -> row.Name), fun (f:Faker) -> f.Name.FullName())
                .RuleFor<char>((fun row -> row.CharValue), fun (f:Faker) -> f.Random.Char('A', 'z'))
                .RuleFor<Guid>((fun row -> row.GuidValue), fun (f:Faker) -> f.Random.Guid())
        let testDataTable4RowGenerator =
            Faker<TestDataSet.DataTable4Row>()
                .CustomInstantiator(fun _f -> testDataSet.DataTable4.NewDataTable4Row())
                .RuleFor<string>((fun row -> row.Name), fun (f:Faker) -> f.Name.FullName())
                .RuleFor<byte>((fun row -> row.ByteValue), fun (f:Faker) -> f.Random.Byte())
                .RuleFor<uint16>((fun row -> row.UInt16Value), fun (f:Faker) -> f.Random.UShort())
                .RuleFor<uint64>((fun row -> row.UInt64Value), fun (f:Faker) -> f.Random.ULong())
                .RuleFor<uint32>((fun row -> row.UInt32Value), fun (f:Faker) -> f.Random.UInt())
        let testDataTable5RowGenerator =
            Faker<TestDataSet.DataTable5Row>()
                .CustomInstantiator(fun _f -> testDataSet.DataTable5.NewDataTable5Row())
                .RuleFor<Guid>((fun row -> row.Key), fun (f:Faker) -> f.Random.Guid())
                .RuleFor<string>((fun row -> row.Name), fun (f:Faker) -> f.Name.FullName())
                .RuleFor<decimal>((fun row -> row.DecimalValue), fun (f:Faker) -> f.Random.Decimal())
                .RuleFor<double>((fun row -> row.DoubleValue), fun (f:Faker) -> f.Random.Double())
                .RuleFor<single>((fun row -> row.SingleValue), fun (f:Faker) -> f.Random.Float())
        let testDataTable6RowGenerator =
           Faker<TestDataSet.DataTable6Row>()
               .CustomInstantiator(fun _f -> testDataSet.DataTable6.NewDataTable6Row())
               .RuleFor<string>((fun row -> row.Name), fun (f:Faker) -> f.Name.FullName())
               .RuleFor<DateTimeOffset>((fun row -> row.DateTimeOffsetValue), fun (f:Faker) -> DateTimeOffset.Now)
               .RuleFor<DateTime>((fun row -> row.DateTimeValue), fun (f:Faker) -> f.Date.Past())
               .RuleFor<TimeSpan>((fun row -> row.TimeSpanValue), fun (f:Faker) -> f.Date.Timespan())

        let mutable parentDataTable6Row = testDataTable6RowGenerator.Generate()
        testDataSet.DataTable6.AddDataTable6Row(parentDataTable6Row);
        for _rowNumber = 1 to 100 do
            let newRow = testDataTable1RowGenerator.Generate()
            testDataSet.DataTable1.AddDataTable1Row(newRow)
            let newDataTable6Row = testDataTable6RowGenerator.Generate()
            newDataTable6Row.ParentId <- parentDataTable6Row.Id;
            testDataSet.DataTable6.AddDataTable6Row(newDataTable6Row);
            parentDataTable6Row <- newDataTable6Row
            for _x = 1 to 10 do
                let newDataTable2Row = testDataTable2RowGenerator.Generate()
                newDataTable2Row.DataTable1Id <- newRow.Id
                testDataSet.DataTable2.AddDataTable2Row(newDataTable2Row)
                let newDataTable3Row = testDataTable3RowGenerator.Generate()
                newDataTable3Row.DataTable1Id <- newRow.Id
                testDataSet.DataTable3.AddDataTable3Row(newDataTable3Row)
                let newDataTable4Row = testDataTable4RowGenerator.Generate()
                newDataTable4Row.DataTable1Id <- newRow.Id
                testDataSet.DataTable4.AddDataTable4Row(newDataTable4Row)
                let newDataTable5Row = testDataTable5RowGenerator.Generate()
                newDataTable5Row.DataTable2Id <- newDataTable2Row.Id
                newDataTable5Row.DataTable3Id <- newDataTable3Row.Id
                newDataTable5Row.DataTable4Id <- newDataTable4Row.Id
                testDataSet.DataTable5.AddDataTable5Row(newDataTable5Row)

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

