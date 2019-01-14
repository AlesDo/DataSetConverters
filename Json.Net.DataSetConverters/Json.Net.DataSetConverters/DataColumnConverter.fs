namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open System.Collections
open Newtonsoft.Json.Serialization

type DataColumnConverter() =
    inherit JsonConverter()
    [<Literal>]
    static let ObjectName = "DataColumn"
    [<Literal>]
    static let AllowDBNull = "AllowDBNull"
    [<Literal>]
    static let AutoIncrement = "AutoIncrement"
    [<Literal>]
    static let AutoIncrementSeed = "AutoIncrementSeed"
    [<Literal>]
    static let AutoIncrementStep = "AutoIncrementStep"
    [<Literal>]
    static let Caption = "Caption"
    [<Literal>]
    static let ColumnMapping = "ColumnMapping"
    [<Literal>]
    static let ColumnName = "ColumnName"
    [<Literal>]
    static let DataType = "DataType"
    [<Literal>]
    static let DateTimeMode = "DateTimeMode"
    [<Literal>]
    static let DefaultValue = "DefaultValue"
    [<Literal>]
    static let Expression = "Expression"
    [<Literal>]
    static let ExtendedProperties = "ExtendedProperties"
    [<Literal>]
    static let MaxLength = "MaxLength"
    [<Literal>]
    static let NamespaceName = "Namespace"
    [<Literal>]
    static let Prefix = "Prefix"
    [<Literal>]
    static let ReadOnly = "ReadOnly"



    override this.CanConvert(objectType) =
        typeof<DataColumn>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, _, existingValue, serializer) =
        if reader.TokenType = JsonToken.Null then
            null
        else
            let dataColumn = 
                match existingValue with
                    | null -> new DataColumn() 
                    | _ -> existingValue :?> DataColumn
            reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
            dataColumn.AllowDBNull <- reader.ReadPropertyFromOutput(serializer, AllowDBNull, ObjectName)
            dataColumn.AutoIncrement <- reader.ReadPropertyFromOutput(serializer, AutoIncrement, ObjectName)
            dataColumn.AutoIncrementSeed <- reader.ReadPropertyFromOutput(serializer, AutoIncrementSeed, ObjectName)
            dataColumn.AutoIncrementStep <- reader.ReadPropertyFromOutput(serializer, AutoIncrementStep, ObjectName)
            dataColumn.Caption <- reader.ReadPropertyFromOutput(serializer, Caption, ObjectName)
            dataColumn.ColumnMapping <- reader.ReadPropertyFromOutput(serializer, ColumnMapping, ObjectName)
            dataColumn.ColumnName <- reader.ReadPropertyFromOutput(serializer, ColumnName, ObjectName)
            dataColumn.DataType <- reader.ReadPropertyFromOutput(serializer, DataType, ObjectName)
            dataColumn.DateTimeMode <- reader.ReadPropertyFromOutput(serializer, DateTimeMode, ObjectName)
            if dataColumn.AutoIncrement then
               reader.ReadPropertyFromOutput(serializer, DefaultValue, ObjectName) |> ignore
            else
               dataColumn.DefaultValue <- reader.ReadPropertyFromOutput(serializer, DefaultValue, ObjectName)
            dataColumn.Expression <- reader.ReadPropertyFromOutput(serializer, Expression, ObjectName)
            reader.ReadPropertyFromOutput(serializer, ExtendedProperties, ObjectName, dataColumn.ExtendedProperties, PropertyCollectionConverter()) |> ignore
            dataColumn.MaxLength <- reader.ReadPropertyFromOutput(serializer, MaxLength, ObjectName)
            dataColumn.Namespace <- reader.ReadPropertyFromOutput(serializer, NamespaceName, ObjectName)
            dataColumn.Prefix <- reader.ReadPropertyFromOutput(serializer, Prefix, ObjectName)
            dataColumn.ReadOnly <- reader.ReadPropertyFromOutput(serializer, ReadOnly, ObjectName)
            reader.ReadAndAssert();
            reader.ValidateJsonToken(JsonToken.EndObject, ObjectName);
            
            dataColumn :> obj

    override this.WriteJson(writer, value, serializer) =
        let dataColumn = value :?> DataColumn
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartObject()
        writer.WritePropertyToOutput(serializer, resolver, AllowDBNull, dataColumn.AllowDBNull)
        writer.WritePropertyToOutput(serializer, resolver, AutoIncrement, dataColumn.AutoIncrement)
        writer.WritePropertyToOutput(serializer, resolver, AutoIncrementSeed, dataColumn.AutoIncrementSeed)
        writer.WritePropertyToOutput(serializer, resolver, AutoIncrementStep, dataColumn.AutoIncrementStep)
        writer.WritePropertyToOutput(serializer, resolver, Caption, dataColumn.Caption)
        writer.WritePropertyToOutput(serializer, resolver, ColumnMapping, dataColumn.ColumnMapping)
        writer.WritePropertyToOutput(serializer, resolver, ColumnName, dataColumn.ColumnName)
        writer.WritePropertyToOutput(serializer, resolver, DataType, dataColumn.DataType)
        writer.WritePropertyToOutput(serializer, resolver, DateTimeMode, dataColumn.DateTimeMode)
        writer.WritePropertyToOutput(serializer, resolver, DefaultValue, dataColumn.DefaultValue)
        writer.WritePropertyToOutput(serializer, resolver, Expression, dataColumn.Expression)
        writer.WritePropertyToOutput(serializer, resolver, ExtendedProperties, dataColumn.ExtendedProperties, PropertyCollectionConverter())
        writer.WritePropertyToOutput(serializer, resolver, MaxLength, dataColumn.MaxLength)
        writer.WritePropertyToOutput(serializer, resolver, NamespaceName, dataColumn.Namespace)
        writer.WritePropertyToOutput(serializer, resolver, Prefix, dataColumn.Prefix)
        writer.WritePropertyToOutput(serializer, resolver, ReadOnly, dataColumn.ReadOnly) 
        writer.WriteEndObject()