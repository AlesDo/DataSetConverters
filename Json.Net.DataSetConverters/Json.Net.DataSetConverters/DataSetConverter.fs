namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open System.Collections
open Newtonsoft.Json.Serialization
open System.Globalization
open System

type DataSetConverter() =
    inherit JsonConverter()

    [<Literal>]
    static let ObjectName = "DataSet"
    [<Literal>]
    static let CaseSensitive = "CaseSensitive"
    [<Literal>]
    static let DataSetName = "DataSetName"
    [<Literal>]
    static let EnforceConstraints = "EnforceConstraints"
    [<Literal>]
    static let ExtendedProperties = "ExtendedProperties"
    [<Literal>]
    static let Locale = "Locale"
    [<Literal>]
    static let Namespace = "Namespace"
    [<Literal>]
    static let Prefix = "Prefix"
    [<Literal>]
    static let Relations = "Realtions"
    [<Literal>]
    static let RemotingFormat = "RemotingFormat"
    [<Literal>]
    static let SchemaSerializationMode = "SchemaSerializationMode"
    [<Literal>]
    static let Tables = "Tables"

    static let readTables(reader: JsonReader, dataSet: DataSet, serializer: JsonSerializer) =
        let dataTableConverter = new DataTableConverter()
        reader.ReadAndAssert()
        reader.ValidatePropertyName(Tables, ObjectName)
        reader.ReadAndAssert()
        reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
        reader.ReadAndAssert()
        while reader.TokenType <> JsonToken.EndObject do
            let dataTable = dataSet.Tables.[reader.Value :?> string]
            reader.ReadAndAssert()
            let exists = dataTable <> null
            let dataTable = dataTableConverter.ReadJson(reader, (if dataTable <> null then dataTable.GetType() else typeof<DataTable>), dataTable, serializer) :?> DataTable
            if not exists then dataSet.Tables.Add(dataTable)
            reader.ReadAndAssert()
            
        reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)

    static let readRelations(reader: JsonReader, dataSet: DataSet, serializer: JsonSerializer) =
        reader.ReadAndAssert();
        reader.ValidatePropertyName(Relations, ObjectName)
        reader.ReadAndAssert();
        reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
        reader.ReadAndAssert();
        let dataRelationConverter = new DataRelationConverter(dataSet)
        while reader.TokenType <> JsonToken.EndObject do
            let dataRelation = dataSet.Relations.[reader.Value :?> string]
            reader.ReadAndAssert()
            dataRelationConverter.ReadJson(reader, typeof<DataRelation>, dataRelation, serializer) |> ignore
            reader.ReadAndAssert()

        reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)

    static let writeTables(writer: JsonWriter, dataSet: DataSet, serializer: JsonSerializer) =
        let dataTableConverter = new DataTableConverter()
        writer.WritePropertyName(Tables)
        writer.WriteStartObject()
        for dataTable in dataSet.Tables do
            writer.WritePropertyName(dataTable.TableName)
            dataTableConverter.WriteJson(writer, dataTable, serializer)
        writer.WriteEndObject()

    static let writeRelations(writer: JsonWriter, dataSet: DataSet, serializer: JsonSerializer) =
        let dataRelationConverter = new DataRelationConverter(dataSet)
        writer.WritePropertyName(Relations)
        writer.WriteStartObject()
        for dataRelation in dataSet.Relations do
            writer.WritePropertyName(dataRelation.RelationName)
            dataRelationConverter.WriteJson(writer, dataRelation, serializer)
        writer.WriteEndObject()

    override this.CanConvert(objectType) =
        typeof<DataSet>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, objectType, _, serializer) =
        if reader.TokenType = JsonToken.Null then
            null
        else
            let dataSet = if objectType = typeof<DataSet> then new DataSet() else Activator.CreateInstance(objectType) :?> DataSet
            reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
            dataSet.CaseSensitive <- reader.ReadPropertyFromOutput<bool>(serializer, CaseSensitive, ObjectName)
            dataSet.DataSetName <- reader.ReadPropertyFromOutput<string>(serializer, DataSetName, ObjectName)
            dataSet.EnforceConstraints <- reader.ReadPropertyFromOutput<bool>(serializer, EnforceConstraints, ObjectName)
            reader.ReadPropertyFromOutput(serializer, ExtendedProperties, ObjectName, dataSet.ExtendedProperties, PropertyCollectionConverter()) |> ignore
            dataSet.Locale <- CultureInfo(reader.ReadPropertyFromOutput<string>(serializer, Locale, ObjectName))
            dataSet.Namespace <- reader.ReadPropertyFromOutput<string>(serializer, Namespace, ObjectName)
            dataSet.Prefix <- reader.ReadPropertyFromOutput<string>(serializer, Prefix, ObjectName)
            dataSet.RemotingFormat <- reader.ReadPropertyFromOutput<SerializationFormat>(serializer, RemotingFormat, ObjectName)
            dataSet.SchemaSerializationMode <- reader.ReadPropertyFromOutput<SchemaSerializationMode>(serializer, SchemaSerializationMode, ObjectName)
            readTables(reader, dataSet, serializer)
            readRelations(reader, dataSet, serializer)
            reader.ReadAndAssert()
            reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)

            dataSet :> obj

    override this.WriteJson(writer, value, serializer) =
        let dataSet = value :?> DataSet
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartObject()
        writer.WritePropertyToOutput(serializer, resolver, CaseSensitive, dataSet.CaseSensitive)
        writer.WritePropertyToOutput(serializer, resolver, DataSetName, dataSet.DataSetName)
        writer.WritePropertyToOutput(serializer, resolver, EnforceConstraints, dataSet.EnforceConstraints)
        writer.WritePropertyToOutput(serializer, resolver, ExtendedProperties, dataSet.ExtendedProperties, PropertyCollectionConverter())
        writer.WritePropertyToOutput(serializer, resolver, Locale, dataSet.Locale.Name)
        writer.WritePropertyToOutput(serializer, resolver, Namespace, dataSet.Namespace)
        writer.WritePropertyToOutput(serializer, resolver, Prefix, dataSet.Prefix)
        writer.WritePropertyToOutput(serializer, resolver, RemotingFormat, dataSet.RemotingFormat)
        writer.WritePropertyToOutput(serializer, resolver, SchemaSerializationMode, dataSet.SchemaSerializationMode)
        writeTables(writer, dataSet, serializer)
        writeRelations(writer, dataSet, serializer)
        writer.WriteEndObject()