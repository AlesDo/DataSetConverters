namespace Json.Net.DataSetConverters.Benchmarks

open BenchmarkDotNet.Attributes
open Json.Net.DataSetConverters.Tests.TypedDataSets
open Bogus
open System
open System.Runtime.Serialization.Formatters.Binary
open System.IO
open System.Runtime.Serialization
open Newtonsoft.Json
open Json.Net.DataSetConverters
open DataSetGenerator

[<InProcess>]
type DataSetSerialization() =
    let mutable dataSet: TestDataSet = null

    [<Params(0, 10, 20, 50, 100)>]
    member val DataSetSize = 0 with get, set
    [<Params(false, true)>]
    member val WithChanges = false with get, set

    [<GlobalSetup>]
    member this.GlobalSetup() =
        dataSet <- generateTestDataSet(this.DataSetSize)
        if (this.WithChanges) then addChanges(dataSet)

    [<Benchmark>]
    member this.JsonDotNetDataSetConverters() =
        JsonConvert.SerializeObject(dataSet, DataSetConverter()) |> ignore

    [<Benchmark>]
    member this.BinaryFormatter() =
        let binaryFormatter = new BinaryFormatter()
        use memoryStream = new MemoryStream()
        memoryStream.Position <- 0L
        binaryFormatter.Serialize(memoryStream, dataSet)

    [<Benchmark>]
    member this.DataContractSerializer() =
        let dataContractSerializer = new DataContractSerializer(typeof<TestDataSet>)
        use memoryStream = new MemoryStream()
        memoryStream.Position <- 0L
        dataContractSerializer.WriteObject(memoryStream, dataSet)


