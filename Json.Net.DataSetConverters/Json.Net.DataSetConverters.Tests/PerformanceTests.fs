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
    let generateTestDataSet(numberOfMainTableRows: int) =
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
        for _rowNumber = 1 to numberOfMainTableRows do
            let newRow = testDataTable1RowGenerator.Generate()
            testDataSet.DataTable1.AddDataTable1Row(newRow)
            let newDataTable6Row = testDataTable6RowGenerator.Generate()
            newDataTable6Row.ParentId <- parentDataTable6Row.Id;
            testDataSet.DataTable6.AddDataTable6Row(newDataTable6Row);
            parentDataTable6Row <- newDataTable6Row
            for _x = 1 to (numberOfMainTableRows / 10) do
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

    let generateTestDataSetWithChanges(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSet(numberOfMainTableRows)
        let faker = Faker()
        Seq.cast<TestDataSet.DataTable1Row> testDataSet.DataTable1.Rows |> Seq.iteri(fun i dataTable1Row -> if i % 2 = 0 then dataTable1Row.SByteValue <- faker.Random.SByte())
        Seq.cast<TestDataSet.DataTable2Row> testDataSet.DataTable2.Rows |> Seq.iteri(fun i dataTable2Row -> if i % 2 = 0 then dataTable2Row.Name <- faker.Name.FullName())
        Seq.cast<TestDataSet.DataTable3Row> testDataSet.DataTable3.Rows |> Seq.iteri(fun i dataTable3Row -> if i % 2 = 0 then dataTable3Row.GuidValue <- faker.Random.Guid())
        Seq.cast<TestDataSet.DataTable4Row> testDataSet.DataTable4.Rows |> Seq.iteri(fun i dataTable4Row -> if i % 2 = 0 then dataTable4Row.UInt64Value <- faker.Random.ULong())
        Seq.cast<TestDataSet.DataTable5Row> testDataSet.DataTable5.Rows |> Seq.iteri(fun i dataTable5Row -> if i % 2 = 0 then dataTable5Row.DecimalValue <- faker.Random.Decimal())
        Seq.cast<TestDataSet.DataTable6Row> testDataSet.DataTable6.Rows |> Seq.iteri(fun i dataTable6Row -> if i % 2 = 0 then dataTable6Row.DateTimeOffsetValue <- DateTimeOffset.Now)

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


    [<Theory>]
    [<InlineData(20)>]
    [<InlineData(50)>]
    [<InlineData(100)>]
    [<InlineData(200)>]
    member this.``serialize deserialize big data set JSON DataSetConverters 1000 times``(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSet(numberOfMainTableRows)

        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 1000 do
            let jsonDataSet = JsonConvert.SerializeObject(testDataSet, DataSetConverter())
            JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter()) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize Large DataSet JSON DataSetConverters {0} ms.", stopwatch.ElapsedMilliseconds)

    [<Theory>]
    [<InlineData(20)>]
    [<InlineData(50)>]
    [<InlineData(100)>]
    [<InlineData(200)>]
    member this.``serialize deserialize big data set JSON DataContractSerializer 1000 times``(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSet(numberOfMainTableRows)

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

    [<Theory>]
    [<InlineData(20)>]
    [<InlineData(50)>]
    [<InlineData(100)>]
    [<InlineData(200)>]
    member this.``serialize deserialize big data set JSON BinaryFormatter 1000 times``(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSet(numberOfMainTableRows)

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

    [<Theory>]
    [<InlineData(20)>]
    [<InlineData(50)>]
    [<InlineData(100)>]
    [<InlineData(200)>]
    member this.``serialize deserialize big data set with changes JSON DataSetConverters 1000 times``(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSetWithChanges(numberOfMainTableRows)

        let stopwatch = Stopwatch.StartNew()
        for _count = 1 to 1000 do
            let jsonDataSet = JsonConvert.SerializeObject(testDataSet, DataSetConverter())
            JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter()) |> ignore

        stopwatch.Stop()
        this.TestOutputHelper.WriteLine("Serialize deserialize Large DataSet JSON DataSetConverters {0} ms.", stopwatch.ElapsedMilliseconds)

    [<Theory>]
    [<InlineData(20)>]
    [<InlineData(50)>]
    [<InlineData(100)>]
    [<InlineData(200)>]
    member this.``serialize deserialize big data set with changes JSON DataContractSerializer 1000 times``(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSetWithChanges(numberOfMainTableRows)

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

    [<Theory>]
    [<InlineData(20)>]
    [<InlineData(50)>]
    [<InlineData(100)>]
    [<InlineData(200)>]
    member this.``serialize deserialize big data set with changes JSON BinaryFormatter 1000 times``(numberOfMainTableRows: int) =
        let testDataSet = generateTestDataSetWithChanges(numberOfMainTableRows)

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
