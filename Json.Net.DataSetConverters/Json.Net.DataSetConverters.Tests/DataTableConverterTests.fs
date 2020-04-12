module DataTableConverterTests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open System.Data
open Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Globalization
open System.Linq
open FsCheckGenerators
open Comparers
open Newtonsoft.Json.Linq

[<Fact>]
let ``Empty table serialize deserialize`` () =
    let dataTable = new DataTable()

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

    Assert.Equal(dataTable.TableName, deserializedDataTable.TableName)
    
[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataTable serialize deserialize validate TableName`` (tableName: string) = 
    let dataTable = new DataTable(tableName)

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

    Assert.Equal(dataTable.TableName, deserializedDataTable.TableName)

[<Property>]
let ``DataTable serialize deserialize validate CaseSensitive`` (caseSensitive: bool) = 
    let dataTable = new DataTable()
    dataTable.CaseSensitive <- caseSensitive

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

    Assert.Equal(dataTable.CaseSensitive, deserializedDataTable.CaseSensitive)

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataTable serialize deserialize validate DisplayExpression`` (displayExpression: string) = 
    let dataTable = new DataTable()
    dataTable.Columns.Add(displayExpression) |> ignore
    dataTable.DisplayExpression <- String.Format("Convert([{0}], 'System.String')", displayExpression)

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

    Assert.Equal(dataTable.DisplayExpression, deserializedDataTable.DisplayExpression)

[<Property>]
let ``DataTable serialize deserialize validate Locale`` (locale: CultureInfo) = 
    let dataTable = new DataTable()
    dataTable.Locale <- locale

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

    Assert.Equal(dataTable.Locale, deserializedDataTable.Locale)

[<Property>]
let ``DataTable serialize deserialize validate MinimumCapacity`` (minimumCapacity: NonNegativeInt) = 
    let dataTable = new DataTable()
    dataTable.MinimumCapacity <- minimumCapacity.Get

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())
    
    Assert.Equal(dataTable.MinimumCapacity, deserializedDataTable.MinimumCapacity)

[<Property>]
let ``DataTable serialize deserialize validate Namespace`` (namespaceName: string) = 
    let dataTable = new DataTable()
    dataTable.Namespace <- namespaceName

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())
    
    Assert.Equal(dataTable.Namespace, deserializedDataTable.Namespace)

[<Property(Arbitrary =[| typeof<PrefixGenerators> |])>]
let ``DataTable serialize deserialize validate Prefix`` (prefix: string) = 
    let dataTable = new DataTable()
    dataTable.Prefix <- prefix

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())
    
    Assert.Equal(dataTable.Prefix, deserializedDataTable.Prefix)

[<Property>]
let ``DataTable serialize deserialize validate RemotingFormat`` (remotingFormat: SerializationFormat) = 
    let dataTable = new DataTable()
    dataTable.RemotingFormat <- remotingFormat

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())
    
    Assert.Equal(dataTable.RemotingFormat, deserializedDataTable.RemotingFormat)

let columnDataGenerator = (Arb.Default.String(), Gen.elements [typeof<int>, typeof<int16>, typeof<string>, typeof<bool>, typeof<decimal>, typeof<float>, typeof<double>, typeof<byte[]>])

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataTable serialize deserialize columns`` (dataColumns: DataColumn[]) =
   let distinctDataColumns = dataColumns |> Seq.distinctBy(fun dc -> dc.ColumnName.ToLowerInvariant())
   let dataTable = new DataTable()
   dataTable.Columns.AddRange(distinctDataColumns |> Seq.toArray)

   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())
   let deserializedColumns: seq<DataColumn> = Seq.cast deserializedDataTable.Columns
   Assert.Equal<string>(distinctDataColumns |> Seq.map(fun dc -> dc.ColumnName), deserializedColumns |> Seq.map(fun dc -> dc.ColumnName))
   Assert.Equal<Type>(distinctDataColumns |> Seq.map(fun dc -> dc.DataType), deserializedColumns |> Seq.map(fun dc -> dc.DataType))

[<Property>]
let ``DataTable serialize deserialize rows with integer values`` (byteValue: byte, sbyteValue: sbyte, uint16Value: uint16, int16Value: int16, uint32Value: uint32, int32Value: int32, uint64Value: uint64, int64Value: int64) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("byte", typeof<byte>) |> ignore
   dataTable.Columns.Add("sbyte", typeof<sbyte>) |> ignore
   dataTable.Columns.Add("uint16", typeof<uint16>) |> ignore
   dataTable.Columns.Add("int16", typeof<int16>) |> ignore
   dataTable.Columns.Add("uint32", typeof<uint32>) |> ignore
   dataTable.Columns.Add("int32", typeof<int32>) |> ignore
   dataTable.Columns.Add("uint64", typeof<uint64>) |> ignore
   dataTable.Columns.Add("int64", typeof<int64>) |> ignore

   let columnValues: obj [] = [| byteValue; sbyteValue; uint16Value; int16Value; uint32Value; int32Value; uint64Value; int64Value |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows with string values`` (string1: string, string2: string, string3: string) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("string1", typeof<string>) |> ignore
   dataTable.Columns.Add("string2", typeof<string>) |> ignore
   dataTable.Columns.Add("string3", typeof<string>) |> ignore

   let columnValues: obj [] = [| nullToDbNull string1; nullToDbNull string2; nullToDbNull string3 |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows with decimal and float values`` (decimalValue: decimal, floatValue: float, doubleValue: double) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("decimal", typeof<decimal>) |> ignore
   dataTable.Columns.Add("float", typeof<float>) |> ignore
   dataTable.Columns.Add("double", typeof<double>) |> ignore

   let columnValues: obj [] = [| decimalValue; floatValue; doubleValue |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows boolean values`` (booleanValue: bool, nullableBooleanValue: bool option) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("boolean", typeof<bool>) |> ignore
   dataTable.Columns.Add("nullable boolean", typeof<bool>) |> ignore

   let columnValues: obj [] = [| booleanValue; optionToDbNull nullableBooleanValue |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataTable serialize deserialize rows char values`` (char1: char, char2: char option) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("char1", typeof<char>) |> ignore
   dataTable.Columns.Add("char2", typeof<char>) |> ignore

   let columnValues: obj [] = [| char1; optionToDbNull char2 |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows guid values`` (guid1: Guid, guid2: Guid option) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("guid1", typeof<Guid>) |> ignore
   dataTable.Columns.Add("guid2", typeof<Guid>) |> ignore

   let columnValues: obj [] = [| guid1; optionToDbNull guid2 |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows DateTime values`` (dateTime1: DateTime, dateTime2: DateTime option) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("dateTime1", typeof<DateTime>) |> ignore
   dataTable.Columns.Add("dateTime2", typeof<DateTime>) |> ignore

   let columnValues: obj [] = [| dateTime1; optionToDbNull dateTime2 |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows TimeSpan values`` (timeSpan1: TimeSpan, timeSpan2: TimeSpan option) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("timeSpan1", typeof<TimeSpan>) |> ignore
   dataTable.Columns.Add("TimeSpan2", typeof<TimeSpan>) |> ignore

   let columnValues: obj [] = [| timeSpan1; optionToDbNull timeSpan2 |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows byte[] values`` (byteArray1: byte[], byteArray2: byte[] option) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("byteArray1", typeof<byte array>) |> ignore
   dataTable.Columns.Add("byteArray2", typeof<byte array>) |> ignore

   let columnValues: obj [] = [| byteArray1; optionToDbNull byteArray2 |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, arrayEqualityComparer)

[<Property>]
let ``DataTable serialize deserialize rows preserves row state and original values`` () =
   let dataTable = new DataTable()
   dataTable.Columns.Add("string1", typeof<string>) |> ignore
   dataTable.Columns.Add("string2", typeof<string>) |> ignore
   dataTable.Columns.Add("string3", typeof<string>) |> ignore

   let columnValues: obj [] = [| "string1"; "string2"; "string3" |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   dataTable.AcceptChanges()
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   dataTable.Rows.[1].["string2"] <- "changedValue"
   dataTable.Rows.[2].Delete()
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<DataRowState>([| DataRowState.Unchanged; DataRowState.Modified; DataRowState.Deleted; DataRowState.Added |] :> seq<DataRowState>, deserializedDataTable.Rows.Cast<DataRow>() |> Seq.map(fun dataRow -> dataRow.RowState))
   Assert.Equal("string2", deserializedDataTable.Rows.[1].["string2", DataRowVersion.Original] :?> string)

[<Property>]
let ``DataTable serialize deserialize auto increment column with changed row`` (numberOfRows: uint16) = 
    let dataTable = new DataTable()
    dataTable.Columns.Add(new DataColumn (ColumnName = "Id", DataType = typeof<int>, AutoIncrement = true, Unique = true, ReadOnly = true))
    dataTable.Columns.Add("Value", typeof<int>) |> ignore
    dataTable.PrimaryKey <- [| dataTable.Columns.[0] |]
    for _count = 1 to numberOfRows |> int do
        dataTable.Rows.Add() |> ignore
    dataTable.AcceptChanges()

    dataTable.Rows.OfType<DataRow>() |> Seq.iteri(fun index row -> if index % 2 = 0 then row.["Value"] <- 1)

    let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
    let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

    Assert.Equal(dataTable, deserializedDataTable, dataTableComparer)

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataTable serialize deserialize object column with known object type`` (value1: TestClass, value2: TestClass) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("objectValue1", typeof<TestClass>) |> ignore
   dataTable.Columns.Add("objectValue2", typeof<TestClass>) |> ignore

   let columnValues: obj [] = [| value1; value2; |]
   dataTable.LoadDataRow(columnValues, LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal<obj>(columnValues :> seq<obj>, deserializedDataTable.Rows.Item(0).ItemArray :> seq<obj>, objectEqualityComparer)

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataTable serialize deserialize object column with object type`` (value1: TestClass, value2: TestClass) =
   let dataTable = new DataTable()
   dataTable.Columns.Add("objectValue1", typeof<Object>) |> ignore
   dataTable.Columns.Add("objectValue2", typeof<Object>) |> ignore

   let columnValues: TestClass[] = [| value1; value2; |]
   dataTable.LoadDataRow(Seq.toArray<obj>(columnValues |> Seq.cast<obj>), LoadOption.Upsert) |> ignore
   let jsonDataTable = JsonConvert.SerializeObject(dataTable, DataTableConverter())
   let deserializedDataTable = JsonConvert.DeserializeObject<DataTable>(jsonDataTable, DataTableConverter())

   Assert.Equal(columnValues.[0].Key, (deserializedDataTable.Rows.Item(0).ItemArray.[0] :?> JObject).Value<int>("Key"))
   Assert.Equal(columnValues.[0].Value, (deserializedDataTable.Rows.Item(0).ItemArray.[0] :?> JObject).Value<string>("Value"))
   Assert.Equal(columnValues.[1].Key, (deserializedDataTable.Rows.Item(0).ItemArray.[1] :?> JObject).Value<int>("Key"))
   Assert.Equal(columnValues.[1].Value, (deserializedDataTable.Rows.Item(0).ItemArray.[1] :?> JObject).Value<string>("Value"))
   
