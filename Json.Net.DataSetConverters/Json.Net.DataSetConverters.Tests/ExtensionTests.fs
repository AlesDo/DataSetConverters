module ExtensionTests

open Xunit
open FsCheck.Xunit
open Json.Net.DataSetConverters.JsonSerializationExtensions
open Newtonsoft.Json


[<Fact>]
let ``First Test`` () =
    Assert.True(true)

[<Fact>]
let ``Second Test`` () =
    Assert.True(true)

[<Fact>]
let ``ReadAndAssert Successfull Test`` () =
    let jsonReader = { new Newtonsoft.Json.JsonReader() with member this.Read() = true}
    jsonReader.ReadAndAssert()
    Assert.True(true)

[<Fact>]
let ``ReadAndAssert Fail Test`` () =
    let textReader = { 
        new System.IO.TextReader() with
        member this.Read() = 20
        } 
    let jsonReader = { 
        new Newtonsoft.Json.JsonTextReader(textReader) with 
            member this.Read() = false 
        interface Newtonsoft.Json.IJsonLineInfo with 
            member this.HasLineInfo() = true
            member this.LineNumber = 1
            member this.LinePosition = 2
         }
    Assert.Throws<Newtonsoft.Json.JsonSerializationException>(fun () -> jsonReader.ReadAndAssert())
    
[<Property>]
let ``ValidateJsonToken Successfull Test`` (jsonToken: JsonToken) = 
    let jsonReader = {
        new JsonReader() with
        member this.TokenType = jsonToken
        member this.Read() = true
    }
    jsonReader.ValidateJsonToken(jsonToken, "TestObject")

[<Fact>]
let ``ValidateJsonToken Failed Test`` () = 
    let textReader = { 
        new System.IO.TextReader() with
        member this.Read() = 20
        } 
    let jsonReader = { 
        new Newtonsoft.Json.JsonTextReader(textReader) with 
            member this.Read() = false
            member this.TokenType = JsonToken.Float
        interface Newtonsoft.Json.IJsonLineInfo with 
            member this.HasLineInfo() = true
            member this.LineNumber = 1
            member this.LinePosition = 2
         }
    Assert.Throws<Newtonsoft.Json.JsonSerializationException>(fun () -> jsonReader.ValidateJsonToken(JsonToken.Boolean, "TestObject"))

[<Property()>]
let ``ValidatePropertyName Successfull test`` (propertyName: string) =
    let textReader = { 
        new System.IO.TextReader() with
        member this.Read() = 20
        } 
    let jsonReader = { 
        new Newtonsoft.Json.JsonTextReader(textReader) with 
            member this.Read() = true
            member this.TokenType = JsonToken.PropertyName
            member this.Value = propertyName :> obj
        interface Newtonsoft.Json.IJsonLineInfo with 
            member this.HasLineInfo() = true
            member this.LineNumber = 1
            member this.LinePosition = 2
         }
    jsonReader.ValidatePropertyName(propertyName, "TestObject")

[<Fact>]
let ``ValidatePropertyName Failed test`` () =
    let textReader = { 
        new System.IO.TextReader() with
        member this.Read() = 20
        } 
    let jsonReader = { 
        new Newtonsoft.Json.JsonTextReader(textReader) with 
            member this.Read() = true
            member this.TokenType = JsonToken.PropertyName
            member this.Value = "propertyName" :> obj
        interface Newtonsoft.Json.IJsonLineInfo with 
            member this.HasLineInfo() = true
            member this.LineNumber = 1
            member this.LinePosition = 2
         }
    Assert.Throws<Newtonsoft.Json.JsonSerializationException>(fun () -> jsonReader.ValidatePropertyName("propertyName1", "TestObject"))
