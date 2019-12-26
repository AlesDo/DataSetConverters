module PropertyCollectionConverterTests

open Xunit
open System.Data
open Newtonsoft.Json
open Json.Net.DataSetConverters
open FsCheck.Xunit
open FsCheckGenerators

[<Fact>]
let ``Empty property collection serialize deserialize`` () =
    let propertyCollection = new PropertyCollection()

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    Assert.Equal(0, deserializedPropertyCollection.Count)

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``Property collection with string keys and values serialize deserialize`` (values: string[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    Assert.Equal<obj>(propertyCollection.Keys |> Seq.cast |> Seq.sort, deserializedPropertyCollection.Keys |> Seq.cast |> Seq.sort)
    let originalValues = propertyCollection.Values |> Seq.cast |> Seq.sort
    let deserializedValues = deserializedPropertyCollection.Values |> Seq.cast  |> Seq.sort
    Assert.Equal<obj>(originalValues, deserializedValues)
