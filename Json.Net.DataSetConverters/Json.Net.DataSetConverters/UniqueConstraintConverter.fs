namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open Newtonsoft.Json.Serialization
open ColumnSerialization

type UniqueConstraintConverter(dataTable: DataTable) =
    inherit JsonConverter()
    let dataTable = dataTable

    [<Literal>]
    static let ObjectName = "UniqueConstraint"
    [<Literal>]
    static let ConstraintName = "ConstraintName"
    [<Literal>]
    static let IsPrimaryKey = "IsPrimaryKey"
    [<Literal>]
    static let ExtendedProperties = "ExtendedProperties"
    [<Literal>]
    let Columns = "Columns"

    override this.CanConvert(objectType) =
        typeof<UniqueConstraint>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, _, _, serializer) =
        if reader.TokenType = JsonToken.Null then
            null
        else
            let columns = readColumns(reader, serializer, dataTable, Columns, ObjectName)
            let constraintName = reader.ReadPropertyFromOutput<string>(serializer, ConstraintName, ObjectName)
            let isPrimaryKey = reader.ReadPropertyFromOutput<bool>(serializer, IsPrimaryKey, ObjectName)

            let uniqueConstraint = UniqueConstraint(constraintName, List.toArray(columns), isPrimaryKey)
            reader.ReadPropertyFromOutput<PropertyCollection>(serializer, ExtendedProperties, ObjectName, uniqueConstraint.ExtendedProperties, PropertyCollectionConverter()) |> ignore
            reader.ReadAndAssert();
            reader.ValidateJsonToken(JsonToken.EndObject, ObjectName);
            reader.ReadAndAssert()
            uniqueConstraint :> obj

    override this.WriteJson(writer, value, serializer) =
        let uniqueConstraint = value :?> UniqueConstraint
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartObject()
        writeColumns(writer, serializer, resolver, uniqueConstraint.Columns, Columns)
        writer.WritePropertyToOutput(serializer, resolver, ConstraintName, uniqueConstraint.ConstraintName)
        writer.WritePropertyToOutput(serializer, resolver, IsPrimaryKey, uniqueConstraint.IsPrimaryKey)
        writer.WritePropertyToOutput(serializer, resolver, ExtendedProperties, uniqueConstraint.ExtendedProperties, PropertyCollectionConverter())
        writer.WriteEndObject()