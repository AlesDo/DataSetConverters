namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open Newtonsoft.Json.Serialization
open ColumnSerialization

type ForeignKeyConstraintConverter() =
    inherit JsonConverter()

    [<Literal>]
    static let ObjectName = "ForeignKeyConstraint"
    [<Literal>]
    static let ConstraintName = "ConstraintName"
    [<Literal>]
    static let AcceptRejectRule = "AcceptRejectRule"
    [<Literal>]
    static let DeleteRule = "DeleteRule"
    [<Literal>]
    static let UpdateRule = "UpdateRule"
    [<Literal>]
    static let ExtendedProperties = "ExtendedProperties"

    override this.CanConvert(objectType) =
        typeof<ForeignKeyConstraint>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, _, existingValue, serializer) =
        if reader.TokenType = JsonToken.Null then
            null
        else
            let foreignKeyConstraint = existingValue :?> ForeignKeyConstraint
            reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
            if isNull(existingValue) then
               reader.ReadPropertyFromOutput<AcceptRejectRule>(serializer, AcceptRejectRule, ObjectName) |> ignore
               reader.ReadPropertyFromOutput<string>(serializer, ConstraintName, ObjectName) |> ignore
               reader.ReadPropertyFromOutput<Rule>(serializer, DeleteRule, ObjectName) |> ignore
               reader.ReadPropertyFromOutput<Rule>(serializer, UpdateRule, ObjectName) |> ignore
               reader.ReadPropertyFromOutput<PropertyCollection>(serializer, ExtendedProperties, ObjectName, new PropertyCollection(), new PropertyCollectionConverter()) |> ignore
            else
               foreignKeyConstraint.AcceptRejectRule <- reader.ReadPropertyFromOutput<AcceptRejectRule>(serializer, AcceptRejectRule, ObjectName)
               foreignKeyConstraint.ConstraintName <- reader.ReadPropertyFromOutput<string>(serializer, ConstraintName, ObjectName)
               foreignKeyConstraint.DeleteRule <- reader.ReadPropertyFromOutput<Rule>(serializer, DeleteRule, ObjectName)
               foreignKeyConstraint.UpdateRule <- reader.ReadPropertyFromOutput<Rule>(serializer, UpdateRule, ObjectName)
               reader.ReadPropertyFromOutput<PropertyCollection>(serializer, ExtendedProperties, ObjectName, foreignKeyConstraint.ExtendedProperties, new PropertyCollectionConverter()) |> ignore
            reader.ReadAndAssert()
            reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)
            foreignKeyConstraint :> obj
            

    override this.WriteJson(writer, value, serializer) =
        let foreignKeyConstraint = value :?> ForeignKeyConstraint
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartObject()
        writer.WritePropertyToOutput(serializer, resolver, AcceptRejectRule, foreignKeyConstraint.AcceptRejectRule)
        writer.WritePropertyToOutput(serializer, resolver, ConstraintName, foreignKeyConstraint.ConstraintName)
        writer.WritePropertyToOutput(serializer, resolver, DeleteRule, foreignKeyConstraint.DeleteRule)
        writer.WritePropertyToOutput(serializer, resolver, UpdateRule, foreignKeyConstraint.UpdateRule)
        writer.WritePropertyToOutput(serializer, resolver, ExtendedProperties, foreignKeyConstraint.ExtendedProperties, new PropertyCollectionConverter())
        writer.WriteEndObject()

