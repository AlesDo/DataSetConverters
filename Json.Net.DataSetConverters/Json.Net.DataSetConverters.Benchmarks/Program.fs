// Learn more about F# at http://fsharp.org

open System
open BenchmarkDotNet.Running
open Json.Net.DataSetConverters.Benchmarks
open BenchmarkDotNet.Configs

[<EntryPoint>]
let main argv =
    BenchmarkRunner.Run<DataSetSerialization>() |> ignore
    BenchmarkRunner.Run<DataSetDeSerialization>() |> ignore
    0 // return an integer exit code
