module PropertyCollectionConverterTests

open Xunit
open System.Data
open Newtonsoft.Json
open Json.Net.DataSetConverters
open FsCheck.Xunit
open FsCheckGenerators
open System
open Bogus
open Newtonsoft.Json.Linq

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
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value.ToUniversalTime(), value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, deserializedPropertyCollection.[value.ToUniversalTime()] :?> DateTime))

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``Property collection with object keys and values serialize deserialize`` (values: TestClass[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value.Key, value))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonPropertyCollection, PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value.Key, (deserializedPropertyCollection.[value.Key] :?> TestClass).Key))
    values |> Seq.iter (fun value -> Assert.Equal(value.Value, (deserializedPropertyCollection.[value.Key] :?> TestClass).Value))

[<Property>]
let ``Property collection with unknown value type deserializes a JObject`` (values: int[]) =
    let propertyCollection = new PropertyCollection()
    values |> Seq.iter (fun value -> if not(propertyCollection.ContainsKey(value)) then propertyCollection.Add(value,  {| Key = value; Value = value.ToString(); |}))

    let jsonPropertyCollection = JsonConvert.SerializeObject(propertyCollection, PropertyCollectionConverter())
    let jsonJArray = JArray.Parse(jsonPropertyCollection)
    jsonJArray |> Seq.iter (fun jToken -> jToken.["ValueType"] <- new JValue(jsonJArray.First.Value<string>("ValueType").Replace("AnonymousType", "UnknownType"))) // because the anonymous type is known at deserialization we have to change to somthing that does not exist
    let deserializedPropertyCollection = JsonConvert.DeserializeObject<PropertyCollection>(jsonJArray.ToString(), PropertyCollectionConverter())

    Assert.NotNull(deserializedPropertyCollection)
    values |> Seq.iter (fun value -> Assert.Equal(value, (deserializedPropertyCollection.[value] :?> JObject).Value<int>("Key")))
    values |> Seq.iter (fun value -> Assert.Equal(value.ToString(), (deserializedPropertyCollection.[value] :?> JObject).Value<string>("Value")))
