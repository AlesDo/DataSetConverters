module TestHelpers

open FsCheck
open System.Text.RegularExpressions
open System.Data
open System
open System.Collections.Generic
open System.Linq

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

let nullToDbNull value = if isNull value then DBNull.Value :> obj else value :> obj
let optionToDbNull value = if Option.isNone value then DBNull.Value :> obj else Option.get value :> obj
let equalObjects (o1: obj, o2: obj) = if (isNull o1) || (isNull o2) then o1 = o2 else o1.Equals(o2)
let equalSequences (a1: seq<obj>, a2: seq<obj>) = if (isNull a1) || (isNull a2) then a1 = a2 else (Seq.compareWith(fun e1 e2 -> if equalObjects(e1, e2) then 0 else 1) a1 a2) = 0

let objectEqualityComparer = {
      new IEqualityComparer<obj> with
      member this.Equals(x, y) = equalObjects(x , y)
      member this.GetHashCode(o) = o.GetHashCode()
   }

let arrayEqualityComparer = {
      new IEqualityComparer<obj> with
      member this.Equals(x, y) = if (isNull x) || (x = (DBNull.Value :> obj)) || (isNull y) || (y = (DBNull.Value :> obj)) then x = y else equalSequences((x :?> Array).Cast<obj>(), (y :?> Array).Cast<obj>())
      member this.GetHashCode(o) = o.GetHashCode()
   }