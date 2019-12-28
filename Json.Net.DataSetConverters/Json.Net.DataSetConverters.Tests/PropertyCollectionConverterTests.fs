module PropertyCollectionConverterTests

open Xunit
open System.Data
open Newtonsoft.Json
open Json.Net.DataSetConverters
open FsCheck.Xunit
open FsCheckGenerators
open System

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
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> string))

[<Property>]
let ``Property collection with integer keys and values serialize deserialize`` (values: int[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> int))

[<Property>]
let ``Property collection with float keys and values serialize deserialize`` (values: float[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> float))

[<Property>]
let ``Property collection with double keys and values serialize deserialize`` (values: double[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> double))

[<Property>]
let ``Property collection with decimal keys and values serialize deserialize`` (values: decimal[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> decimal))

[<Property>]
let ``Property collection with boolean keys and values serialize deserialize`` (values: bool[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> bool))

[<Property>]
let ``Property collection with date keys and values serialize deserialize`` (values: DateTime[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value] :?> DateTime))
