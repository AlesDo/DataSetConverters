module DataSetConverterTests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open System.Data
open Json.Net.DataSetConverters
open Newtonsoft.Json
open System.Globalization
open System.Text.RegularExpressions
open FsCheck
open System.Collections.Generic
open System.Linq
open TestHelpers
open Json.Net.DataSetConverters.Tests.TypedDataSets

[<Fact>]
let ``Empty DataSet serialize deserialize`` () =
    let dataSet = new DataSet()

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.NotNull(deserializedDataSet)
    Assert.Equal(0, deserializedDataSet.Tables.Count);

[<Property>]
let ``DataSet serialize deserialize validate CaseSensitive`` (caseSensitive: bool) = 
    let dataSet = new DataSet()
    dataSet.CaseSensitive <- caseSensitive

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.CaseSensitive, deserializedDataSet.CaseSensitive)

[<Property(Arbitrary =[| typeof<MyGenerators> |])>]
let ``DataSet serialize deserialize validate DataSetName`` (dataSetName: string) = 
    let dataSet = new DataSet()
    dataSet.DataSetName <- dataSetName

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.DataSetName, deserializedDataSet.DataSetName)

[<Property>]
let ``DataSet serialize deserialize validate EnforceConstraints`` (enforceConstraints: bool) = 
    let dataSet = new DataSet()
    dataSet.EnforceConstraints <- enforceConstraints

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.EnforceConstraints, deserializedDataSet.EnforceConstraints)

[<Property>]
let ``DataSet serialize deserialize validate Locale`` (locale: CultureInfo) = 
    let dataSet = new DataSet()
    dataSet.Locale <- locale

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.Locale, deserializedDataSet.Locale)

[<Property>]
let ``DataSet serialize deserialize validate Namespace`` (``namespace``: string) = 
    let dataSet = new DataSet()
    dataSet.Namespace <- ``namespace``

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.Namespace, deserializedDataSet.Namespace)

[<Property(Arbitrary =[| typeof<PrefixGenerators> |])>]
let ``DataSet serialize deserialize validate Prefix`` (prefix: string) = 
    let dataSet = new DataSet()
    dataSet.Prefix <- prefix

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.Prefix, deserializedDataSet.Prefix)

[<Property>]
let ``DataSet serialize deserialize validate RemotingFormat`` (remotingFormat: SerializationFormat) = 
    let dataSet = new DataSet()
    dataSet.RemotingFormat <- remotingFormat

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.Equal(dataSet.RemotingFormat, deserializedDataSet.RemotingFormat)

//[<Property>]
//let ``DataSet serialize deserialize validate SchemaSerializationMode`` (schemaSerializationMode: SchemaSerializationMode) =
//    let dataSet = new DataSet()
//    dataSet.SchemaSerializationMode <- schemaSerializationMode
//
//    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
//    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())
//
//    Assert.Equal(dataSet.SchemaSerializationMode, deserializedDataSet.SchemaSerializationMode)

[<Fact>]
let ``DataSet serialize deserialize two empty unrelated tables`` () =
    let dataSet = new DataSet()
    dataSet.Tables.Add("table1") |> ignore
    dataSet.Tables.Add("table2") |> ignore

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.NotNull(deserializedDataSet)
    Assert.Equal(2, deserializedDataSet.Tables.Count)
    Assert.Equal("table1", deserializedDataSet.Tables.[0].TableName)
    Assert.Equal("table2", deserializedDataSet.Tables.[1].TableName)

[<Fact>]
let ``DataSet serialize deserialize two empy related tables`` () =
    let dataSet = new DataSet()
    let table1 = dataSet.Tables.Add("table1")
    let table1Id = table1.Columns.Add("Id", typeof<int>)
    let table2 = dataSet.Tables.Add("table2")
    table2.Columns.Add("Id", typeof<int>) |> ignore
    let table2Table1Id = table2.Columns.Add("Table1Id", typeof<int>)
    dataSet.Relations.Add(table1Id, table2Table1Id) |> ignore

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.NotNull(deserializedDataSet)
    Assert.Equal(2, deserializedDataSet.Tables.Count)
    Assert.Equal("table1", deserializedDataSet.Tables.[0].TableName)
    Assert.Equal("table2", deserializedDataSet.Tables.[1].TableName)
    Assert.Equal(1, deserializedDataSet.Relations.Count)
    Assert.Equal("Id", deserializedDataSet.Relations.[0].ParentColumns.[0].ColumnName)
    Assert.Equal("Table1Id", deserializedDataSet.Relations.[0].ChildColumns.[0].ColumnName)
    Assert.Equal("table1", deserializedDataSet.Relations.[0].ParentTable.TableName)
    Assert.Equal("table2", deserializedDataSet.Relations.[0].ChildTable.TableName)

[<Fact>]
let ``DataSet serialize deserialize multiple empy related tables`` () =
    let dataSet = new DataSet()
    let table1 = dataSet.Tables.Add("table1")
    let table1Id = table1.Columns.Add("Id", typeof<int>)
    let table2 = dataSet.Tables.Add("table2")
    let table2Id = table2.Columns.Add("Id", typeof<int>)
    let table2Table1Id = table2.Columns.Add("Table1Id", typeof<int>)
    dataSet.Relations.Add(table1Id, table2Table1Id) |> ignore
    let table3 = dataSet.Tables.Add("table3")
    let table3Id = table3.Columns.Add("Id", typeof<int>)
    let table3Table1Id = table3.Columns.Add("Table1Id", typeof<int>)
    dataSet.Relations.Add(table1Id, table3Table1Id) |> ignore
    let table4 = dataSet.Tables.Add("table4")
    table4.Columns.Add("Id", typeof<int>) |> ignore
    let table4Table1Id = table4.Columns.Add("Table3Id", typeof<int>)
    dataSet.Relations.Add(table3Id, table4Table1Id) |> ignore
    let table4Table2Id = table4.Columns.Add("Table2Id", typeof<int>)
    dataSet.Relations.Add(table2Id, table4Table2Id) |> ignore

    let jsonDataSet = JsonConvert.SerializeObject(dataSet, DataSetConverter())
    let deserializedDataSet = JsonConvert.DeserializeObject<DataSet>(jsonDataSet, DataSetConverter())

    Assert.NotNull(deserializedDataSet)
    Assert.Equal(4, deserializedDataSet.Tables.Count)
    Assert.Equal("table1", deserializedDataSet.Tables.[0].TableName)
    Assert.Equal("table2", deserializedDataSet.Tables.[1].TableName)
    Assert.Equal("table3", deserializedDataSet.Tables.[2].TableName)
    Assert.Equal("table4", deserializedDataSet.Tables.[3].TableName)
    Assert.Equal(4, deserializedDataSet.Relations.Count)
    Assert.Equal("Id", deserializedDataSet.Relations.[0].ParentColumns.[0].ColumnName)
    Assert.Equal("Table1Id", deserializedDataSet.Relations.[0].ChildColumns.[0].ColumnName)
    Assert.Equal("table1", deserializedDataSet.Relations.[0].ParentTable.TableName)
    Assert.Equal("table2", deserializedDataSet.Relations.[0].ChildTable.TableName)
    Assert.Equal("Id", deserializedDataSet.Relations.[1].ParentColumns.[0].ColumnName)
    Assert.Equal("Table1Id", deserializedDataSet.Relations.[1].ChildColumns.[0].ColumnName)
    Assert.Equal("table1", deserializedDataSet.Relations.[1].ParentTable.TableName)
    Assert.Equal("table3", deserializedDataSet.Relations.[1].ChildTable.TableName)
    Assert.Equal("Id", deserializedDataSet.Relations.[2].ParentColumns.[0].ColumnName)
    Assert.Equal("Table3Id", deserializedDataSet.Relations.[2].ChildColumns.[0].ColumnName)
    Assert.Equal("table3", deserializedDataSet.Relations.[2].ParentTable.TableName)
    Assert.Equal("table4", deserializedDataSet.Relations.[2].ChildTable.TableName)
    Assert.Equal("Id", deserializedDataSet.Relations.[3].ParentColumns.[0].ColumnName)
    Assert.Equal("Table2Id", deserializedDataSet.Relations.[3].ChildColumns.[0].ColumnName)
    Assert.Equal("table2", deserializedDataSet.Relations.[3].ParentTable.TableName)
    Assert.Equal("table4", deserializedDataSet.Relations.[3].ChildTable.TableName)

[<Fact>]
let ``DataSet serialize deserialize empty typed DataSet`` () =
   let typedDataSet = new TestDataSet()

   let jsonDataSet = JsonConvert.SerializeObject(typedDataSet, DataSetConverter())
   let deserializedDataSet = JsonConvert.DeserializeObject<TestDataSet>(jsonDataSet, DataSetConverter())

   Assert.NotNull(deserializedDataSet)

[<Fact>]
let ``DataSet serialize deserialize typed DataSet with Data`` () =
   let typedDataSet = new TestDataSet()
   let dataTable1Row1 = typedDataSet.DataTable1.AddDataTable1Row("DataTable1Row1", SByte.MaxValue, UInt16.MaxValue, UInt32.MaxValue, UInt64.MaxValue)
   typedDataSet.AcceptChanges()

   let jsonDataSet = JsonConvert.SerializeObject(typedDataSet, DataSetConverter())
   let deserializedDataSet = JsonConvert.DeserializeObject<TestDataSet>(jsonDataSet, DataSetConverter())

   Assert.NotNull(deserializedDataSet)
   Assert.Equal<DataRow>(dataTable1Row1, deserializedDataSet.DataTable1.Rows.[0], dataRowComparer<DataRow>);
