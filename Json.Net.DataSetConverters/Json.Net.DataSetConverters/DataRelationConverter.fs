namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Data
open JsonSerializationExtensions
open Newtonsoft.Json.Serialization
open ColumnSerialization

type DataRelationConverter(dataSet: DataSet) =
    inherit JsonConverter()
    let dataSet = dataSet

    [<Literal>]
    static let ObjectName = "DataRelation"
    [<Literal>]
    static let RelationName = "RelationName"
    [<Literal>]
    static let ChildTable = "ChildTable"
    [<Literal>]
    static let ChildColumns = "ChildColumns"
    [<Literal>]
    static let ChildKeyConstraint = "ChildKeyConstraint"
    [<Literal>]
    static let ExtendedProperties = "ExtendedProperties"
    [<Literal>]
    static let ParentTable = "ParentTable"
    [<Literal>]
    static let ParentColumns = "ParentColumns"
    [<Literal>]
    static let ParentKeyConstraint = "ParentKeyConstraint"
    [<Literal>]
    static let Nested = "Nested"


    override this.CanConvert(objectType) =
        typeof<DataRelation>.IsAssignableFrom(objectType)

    override this.ReadJson(reader, _, _, serializer) =
        if reader.TokenType = JsonToken.Null then
            null
        else
            reader.ValidateJsonToken(JsonToken.StartObject, ObjectName)
            let relationName = reader.ReadPropertyFromOutput<string>(serializer, RelationName, ObjectName)
            let childTableName = reader.ReadPropertyFromOutput<string>(serializer, ChildTable, ObjectName)
            let childColumnList = readColumns(reader, serializer, dataSet.Tables.[childTableName], ChildColumns, ObjectName)
            let nested = reader.ReadPropertyFromOutput<bool>(serializer, Nested, ObjectName)
            let parentTableName = reader.ReadPropertyFromOutput<string>(serializer, ParentTable, ObjectName)
            let parentColumnList = readColumns(reader, serializer, dataSet.Tables.[parentTableName], ParentColumns, ObjectName)
            let parentConstraintName = reader.ReadPropertyFromOutput<string>(serializer, ParentKeyConstraint, ObjectName)
            reader.ReadAndAssert()
            reader.ValidatePropertyName(ChildKeyConstraint, ObjectName)
            reader.ReadAndAssert()
            let hasChildConstraint = (reader.TokenType = JsonToken.StartObject) && (reader.TokenType <> JsonToken.Null)
            let dataRelation = new DataRelation(relationName, List.toArray(parentColumnList), List.toArray(childColumnList), hasChildConstraint)
            dataSet.Relations.Add(dataRelation)
            dataRelation.Nested <- nested;
            if hasChildConstraint then
                let foreignKeyConstraintConverter = ForeignKeyConstraintConverter()
                foreignKeyConstraintConverter.ReadJson(reader, typeof<ForeignKeyConstraint>, dataRelation.ChildKeyConstraint, serializer) |> ignore

            reader.ReadPropertyFromOutput<PropertyCollection>(serializer, ExtendedProperties, ObjectName, dataRelation.ExtendedProperties, new PropertyCollectionConverter()) |> ignore
            reader.ReadAndAssert()
            reader.ValidateJsonToken(JsonToken.EndObject, ObjectName)

            dataRelation :> obj

    override this.WriteJson(writer, value, serializer) =
        let dataRelation = value :?> DataRelation
        let resolver = serializer.ContractResolver :?> DefaultContractResolver

        writer.WriteStartObject()
        writer.WritePropertyToOutput(serializer, resolver, RelationName, dataRelation.RelationName)
        writer.WritePropertyToOutput(serializer, resolver, ChildTable, dataRelation.ChildTable.TableName)
        writeColumns(writer, serializer, resolver, dataRelation.ChildColumns, ChildColumns)
        writer.WritePropertyToOutput(serializer, resolver, Nested, dataRelation.Nested)
        writer.WritePropertyToOutput(serializer, resolver, ParentTable, dataRelation.ParentTable.TableName)
        writeColumns(writer, serializer, resolver, dataRelation.ParentColumns, ParentColumns)
        if isNull(dataRelation.ParentKeyConstraint) then
            writer.WritePropertyToOutput(serializer, resolver, ParentKeyConstraint, null)
        else
            writer.WritePropertyToOutput(serializer, resolver, ParentKeyConstraint, dataRelation.ParentKeyConstraint.ConstraintName)
        if isNull(dataRelation.ChildKeyConstraint) then
            writer.WritePropertyToOutput(serializer, resolver, ChildKeyConstraint, null)
        else
            writer.WritePropertyName(match resolver with | null -> ChildKeyConstraint | _ -> resolver.GetResolvedPropertyName(ChildKeyConstraint))
            let foreignKeyConstraintConverter = ForeignKeyConstraintConverter()
            foreignKeyConstraintConverter.WriteJson(writer, dataRelation.ChildKeyConstraint, serializer)
        writer.WritePropertyToOutput(serializer, resolver, ExtendedProperties, dataRelation.ExtendedProperties, new PropertyCollectionConverter())
        writer.WriteEndObject()
