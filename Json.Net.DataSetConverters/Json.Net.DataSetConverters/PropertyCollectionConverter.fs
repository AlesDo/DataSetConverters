namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open System.Collections
open Newtonsoft.Json.Serialization
open System

type PropertyCollectionConverter() =
    inherit JsonConverter()

    [<Literal>]
    static let ObjectName = "PropertyCollection"
    [<Literal>]
    static let Key = "Key"
    [<Literal>]
    static let KeyType = "KeyType"
    [<Literal>]
    static let Value = "Value"
    [<Literal>]
    static let ValueType = "ValueType"
    
    static let readExtendedProperty(reader: JsonReader, serializer: JsonSerializer, propertyCollection: PropertyCollection) = 
        reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
        propertyCollection.Add(
            reader.ReadTypedPropertyFromOutput(serializer, Key, reader.ReadPropertyFromOutput(serializer, KeyType, ObjectName), ObjectName),
            reader.ReadTypedPropertyFromOutput(serializer, Value, reader.ReadPropertyFromOutput(serializer, ValueType, ObjectName), ObjectName))
        reader.ReadAndAssert()
        reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)
        reader.ReadAndAssert()

    static let writeExtendedProperty(writer: JsonWriter, serializer: JsonSerializer, resolver: DefaultContractResolver, extendedProperty: DictionaryEntry) =
        writer.WriteStartObject()
        let keyType = if isNull(extendedProperty.Key) then null else extendedProperty.Key.GetType()
        writer.WritePropertyToOutput(serializer, resolver, KeyType, if isNull(keyType) then null else keyType.FullName)
        if isNull(keyType) then
            writer.WritePropertyToOutput(serializer, resolver, Key, extendedProperty.Key)
        else
            writer.WriteTypedPropertyToOutput(serializer, resolver, Key, keyType, extendedProperty.Key)
        let valueType = if isNull(extendedProperty.Value) then null else extendedProperty.Value.GetType()
        writer.WritePropertyToOutput(serializer, resolver, ValueType, if isNull(valueType) then null else valueType.FullName)
        if isNull(valueType) then
            writer.WritePropertyToOutput(serializer, resolver, Value, extendedProperty.Value)
        else
            writer.WriteTypedPropertyToOutput(serializer, resolver, Value, valueType, extendedProperty.Value)
        writer.WriteEndObject()


    override this.CanConvert(objectType) =
        typeof<PropertyCollection>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, objectType, existingValue, serializer) =
        if not (this.CanConvert(objectType)) then raise (new ArgumentOutOfRangeException("objectType", "Invalid object type"))
        if reader.TokenType = JsonToken.Null then
            null
        else
            let propertyCollection = 
                match existingValue with
                    | null -> new PropertyCollection() 
                    | _ -> existingValue :?> PropertyCollection
            reader.ValidateJsonToken(JsonToken.StartArray, ObjectName)
            reader.ReadAndAssert()
            while reader.TokenType <> JsonToken.EndArray do
                readExtendedProperty(reader, serializer, propertyCollection)
            
            propertyCollection :> obj

    override this.WriteJson(writer, value, serializer) =
        let propertyCollection = value :?> PropertyCollection
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartArray()
        for extendedProperty in Seq.cast<DictionaryEntry> propertyCollection do
            writeExtendedProperty(writer, serializer, resolver, extendedProperty)

        writer.WriteEndArray()

