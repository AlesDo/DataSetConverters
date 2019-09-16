namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open Newtonsoft.Json.Serialization
open System
open System.Globalization

type DataRowConverter(dataTable: DataTable) =
    inherit JsonConverter()
    let dataTable = dataTable

    [<Literal>]
    static let ObjectName = "DataRow"
    [<Literal>]
    static let OriginalRow = "OriginalRow"
    [<Literal>]
    static let RowState = "RowState"

    static let readColumnValues(reader: JsonReader, serializer: JsonSerializer, dataRow: DataRow, dataTable: DataTable) = 
        for dataColumn in dataTable.Columns do
            if dataRow.RowState = DataRowState.Detached then
                dataRow.[dataColumn] <- reader.ReadPropertyFromOutput(serializer, dataColumn.ColumnName, dataColumn.DataType, ObjectName)
            else
                if not dataColumn.ReadOnly then
                    dataRow.[dataColumn] <- reader.ReadPropertyFromOutput(serializer, dataColumn.ColumnName, dataColumn.DataType, ObjectName)
                else
                    reader.ReadPropertyFromOutput(serializer, dataColumn.ColumnName, dataColumn.DataType, ObjectName) |> ignore
  

    static let writeColumnValue(writer: JsonWriter, serializer: JsonSerializer, resolver: DefaultContractResolver, dataRow: DataRow, dataColumn: DataColumn, dataRowVersion: DataRowVersion) =
        let columnValue = dataRow.[dataColumn, dataRowVersion]
        if serializer.NullValueHandling <> NullValueHandling.Ignore || (columnValue <> (DBNull.Value :> obj) && (not(isNull(columnValue)))) then
            writer.WritePropertyName(match resolver with | null -> dataColumn.ColumnName | _ -> resolver.GetResolvedPropertyName(dataColumn.ColumnName))
            match dataColumn.DataType with
            | t when t = typeof<decimal> -> serializer.Serialize(writer, (columnValue :?> decimal).ToString("F28", CultureInfo.InvariantCulture))
            | _ -> serializer.Serialize(writer, columnValue)

    override this.CanConvert(objectType) =
        typeof<DataRow>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, _, existingValue, serializer) =
        if reader.TokenType = JsonToken.Null then
            null
        else
            let dataRow = 
                match existingValue with
                    | null -> dataTable.NewRow()
                    | _ -> existingValue :?> DataRow
            
            reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
            reader.ReadAndAssert()
            reader.ValidateJsonToken(JsonToken.PropertyName, ObjectName)
            if reader.Value.Equals(OriginalRow) then
                reader.ReadAndAssert()
                if reader.TokenType <> JsonToken.Null then
                    reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
                    readColumnValues(reader, serializer, dataRow, dataTable)
                    reader.ReadPropertyFromOutput<DataRowState>(serializer, RowState, ObjectName) |> ignore
                    reader.ReadAndAssert()
                    reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)
                    dataTable.Rows.Add(dataRow)
                    dataRow.AcceptChanges()
        
            readColumnValues(reader, serializer, dataRow, dataTable)
            let originalRowState = reader.ReadPropertyFromOutput<DataRowState>(serializer, RowState, ObjectName) 
            if dataRow.RowState = DataRowState.Detached then
                dataTable.Rows.Add(dataRow)

            if dataRow.RowState <> originalRowState then
                match originalRowState with
                    | DataRowState.Added -> 
                        dataRow.AcceptChanges()
                        dataRow.SetAdded()
                    | DataRowState.Modified -> 
                        dataRow.AcceptChanges()
                        dataRow.SetModified()
                    | DataRowState.Deleted -> 
                        dataRow.Delete()
                    | DataRowState.Unchanged -> 
                        dataRow.AcceptChanges()
                    | _ -> ()

            reader.ReadAndAssert()
            reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)
            reader.ReadAndAssert()
            dataRow :> obj

    override this.WriteJson(writer, value, serializer) =
        let dataRow = value :?> DataRow
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartObject()
        writer.WritePropertyName(match resolver with | null -> OriginalRow | _ -> resolver.GetResolvedPropertyName(OriginalRow))
        if dataRow.RowState = DataRowState.Modified || dataRow.RowState = DataRowState.Deleted then
            writer.WriteStartObject()
            for dataColumn in dataRow.Table.Columns do
                writeColumnValue(writer, serializer, resolver, dataRow, dataColumn, DataRowVersion.Original)

            writer.WritePropertyToOutput(serializer, resolver, RowState, dataRow.RowState)
            writer.WriteEndObject()
        else
            if serializer.NullValueHandling <> NullValueHandling.Ignore then
                writer.WriteNull()

        for dataColumn in dataRow.Table.Columns do
            writeColumnValue(writer, serializer, resolver, dataRow, dataColumn, match dataRow.RowState with DataRowState.Deleted -> DataRowVersion.Original | _ -> DataRowVersion.Current)

        writer.WritePropertyToOutput(serializer, resolver, RowState, dataRow.RowState)
        writer.WriteEndObject()
