namespace Json.Net.DataSetConverters.Benchmarks

open BenchmarkDotNet.Attributes
open Json.Net.DataSetConverters.Tests.TypedDataSets
open System.Runtime.Serialization.Formatters.Binary
open System.IO
open System.Runtime.Serialization
open Newtonsoft.Json
open Json.Net.DataSetConverters
open DataSetGenerator
open ExporterConfig

[<InProcess; MemoryDiagnoser; Config(typeof<PlotExporterConfig>); RPlotExporter>]
type DataSetDeSerialization() =

    let binaryFormatterDataSet = new MemoryStream()
    let dataContractSerializerDataSet = new MemoryStream()
    let mutable jsonDataSet: string = null

    let serializeWithBinaryFormatter(dataSet: System.Data.DataSet, memoryStream: MemoryStream) =
        let binaryFormatter = new BinaryFormatter()
        memoryStream.Position <- 0L
        binaryFormatter.Serialize(memoryStream, dataSet)

    let serializeWithDataContractSerializer(dataSet: System.Data.DataSet, memoryStream: MemoryStream) =
       let dataContractSerializer = new DataContractSerializer(typeof<TestDataSet>)
       memoryStream.Position <- 0L
       dataContractSerializer.WriteObject(memoryStream, dataSet)

    [<Params(0, 1, 2, 5, 10, 20, 50)>]
    member val DataSetSize = 0 with get, set
    [<Params(false, true)>]
    member val WithChanges = false with get, set

    [<GlobalSetup>]
    member this.GlobalSetup() =
        let testDataSet = generateTestDataSet(this.DataSetSize)
        if this.WithChanges then
            addChanges(testDataSet)
        serializeWithBinaryFormatter(testDataSet, binaryFormatterDataSet)
        serializeWithDataContractSerializer(testDataSet, dataContractSerializerDataSet)
        jsonDataSet <- JsonConvert.SerializeObject(testDataSet, DataSetConverter())

    [<GlobalCleanup>]
    member this.GlobalCleanup() =
        binaryFormatterDataSet.Close()
        dataContractSerializerDataSet.Close()

    [<Benchmark>]
    member this.JsonDotNetDataSetConverters() =
        JsonConvert.DeserializeObject<TestDataSet>(jsonDataSet, DataSetConverter()) |> ignore

    [<Benchmark>]
    member this.BinaryFormatter() =
        let binaryFormatter = new BinaryFormatter()
        binaryFormatterDataSet.Position <- 0L
        binaryFormatter.Deserialize(binaryFormatterDataSet) |> ignore

    [<Benchmark>]
    member this.DataContractSerializer() =
        let dataContractSerializer = new DataContractSerializer(typeof<TestDataSet>)
        dataContractSerializerDataSet.Position <- 0L
        dataContractSerializer.ReadObject(dataContractSerializerDataSet) |> ignore


