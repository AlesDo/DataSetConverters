module FsCheckGenerators

open FsCheck
open System.Text.RegularExpressions
open System.Data
open System

let filter (testString: string) =
    testString.Length > 10

type MyGenerators =
    static member String() = (Arb.Default.String() |> Arb.filter (fun s -> match s with | null -> false | _ -> Regex.Match(s, "^[\w\d]+$").Success))
    static member DataColumn() = 
      let genDataColumn = gen {  let! columnName = MyGenerators.String().Generator
                                 let! dataType = Gen.elements [typeof<int>; typeof<int16>; typeof<string>; typeof<bool>; typeof<decimal>; typeof<float>; typeof<double>; typeof<byte[]>]
                                 return new DataColumn(columnName, dataType)}
      Arb.fromGenShrink (genDataColumn, fun  s -> Seq.empty)
    static member Char() = (Arb.Default.Char() |> Arb.filter (fun c -> not (Char.IsControl(c))))

type PrefixGenerators =
    static member String() = (Arb.Default.String() |> Arb.filter (fun s -> match s with | null -> false | _ -> Regex.Match(s, "^[a-zA-Z]+\z").Success))
