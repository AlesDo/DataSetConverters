namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open Newtonsoft.Json.Serialization

module ColumnSerialization =
    let readColumns(reader: JsonReader, serializer: JsonSerializer, dataTable: DataTable, propertyName: string, objectName: string) = 
        reader.ReadAndAssert()
        reader.ValidatePropertyName(propertyName, objectName)
        reader.ReadAndAssert()
        reader.ValidateJsonToken(JsonToken.StartArray, objectName)
        reader.ReadAndAssert()
        let mutable constraintColumns: DataColumn list = []
        while reader.TokenType <> JsonToken.EndArray do
            constraintColumns <- dataTable.Columns.[serializer.Deserialize<string>(reader)] :: constraintColumns
            reader.ReadAndAssert()
        constraintColumns

    let writeColumns(writer: JsonWriter, serializer: JsonSerializer, resolver: DefaultContractResolver, columns: seq<DataColumn>, propertyName: string) =
        writer.WritePropertyName(match resolver with | null -> propertyName | _ -> resolver.GetResolvedPropertyName(propertyName))
        writer.WriteStartArray()
        for column in columns do
            serializer.Serialize(writer, column.ColumnName)
        writer.WriteEndArray()
