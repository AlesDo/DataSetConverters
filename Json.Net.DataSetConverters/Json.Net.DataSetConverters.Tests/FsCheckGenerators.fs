module FsCheckGenerators

open FsCheck
open System.Text.RegularExpressions
open System.Data
open System
open Bogus

let filter (testString: string) =
    testString.Length > 10

type TestClass() =
    member val public Key: int = 0 with get, set
    member val public Value: string = null with get,set
    override this.Equals(obj: Object) =
        if obj :? TestClass then (obj :?> TestClass).Value = this.Value && (obj :?> TestClass).Key = this.Key else false

type MyGenerators =
    static member String() = (Arb.Default.String() |> Arb.filter (fun s -> match s with | null -> false | _ -> Regex.Match(s, "^[\w\d]+$").Success))
    static member DataColumn() = 
      let genDataColumn = gen {  let! columnName = MyGenerators.String().Generator
                                 let! dataType = Gen.elements [typeof<int>; typeof<int16>; typeof<string>; typeof<bool>; typeof<decimal>; typeof<float>; typeof<double>; typeof<byte[]>]
                                 return new DataColumn(columnName, dataType)}
      Arb.fromGenShrink (genDataColumn, fun  s -> Seq.empty)
    static member Char() = (Arb.Default.Char() |> Arb.filter (fun c -> not (Char.IsControl(c))))
    static member TestClass() =
        let testClassFaker =
            Faker<TestClass>()
                .RuleFor<int>((fun testClass -> testClass.Key), fun (f:Faker) -> f.UniqueIndex)
                .RuleFor<string>((fun testClass -> testClass.Value), fun (f:Faker) -> f.Random.String())
        let genTestClass = gen { return testClassFaker.Generate() }
        Arb.fromGenShrink (genTestClass, fun  s -> Seq.empty)

type PrefixGenerators =
    static member String() = (Arb.Default.String() |> Arb.filter (fun s -> match s with | null -> false | _ -> Regex.Match(s, "^[a-zA-Z]+\z").Success))

