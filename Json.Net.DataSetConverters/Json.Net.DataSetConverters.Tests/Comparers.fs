module Comparers

open System
open System.Data
open System.Collections.Generic
open System.Collections
open System.Linq

let nullToDbNull value = if isNull value then DBNull.Value :> obj else value :> obj
let optionToDbNull value = if Option.isNone value then DBNull.Value :> obj else Option.get value :> obj
let equalObjects (o1: obj, o2: obj) = if (isNull o1) || (isNull o2) then o1 = o2 else o1.Equals(o2)
let equalSequences (a1: seq<obj>, a2: seq<obj>) = if (isNull a1) || (isNull a2) then a1 = a2 else (Seq.compareWith(fun e1 e2 -> if equalObjects(e1, e2) then 0 else 1) a1 a2) = 0

let objectEqualityComparer = {
    new IEqualityComparer<obj> with
    member this.Equals(x, y) = equalObjects(x , y)
    member this.GetHashCode(o) = o.GetHashCode()
}

let arrayEqualityComparer = {
    new IEqualityComparer<obj> with
    member this.Equals(x, y) = if (isNull x) || (x = (DBNull.Value :> obj)) || (isNull y) || (y = (DBNull.Value :> obj)) then x = y else equalSequences((x :?> Array).Cast<obj>(), (y :?> Array).Cast<obj>())
    member this.GetHashCode(o) = o.GetHashCode()
}

let dictionaryEntryComparer = {
   new IEqualityComparer<DictionaryEntry> with
   member this.Equals(dictionaryEntry1, distionaryEntry2) = equalObjects(dictionaryEntry1.Key, distionaryEntry2.Key) && equalObjects(dictionaryEntry1.Value, distionaryEntry2.Value)
   member this.GetHashCode(row) = row.GetHashCode()
}

let compareDictionaryEntries = Seq.compareWith(fun dictionaryEntry1 dictionaryEntry2 -> if dictionaryEntryComparer.Equals(dictionaryEntry1, dictionaryEntry2) then 0 else 1)

let dataRowComparer<'T when 'T :> DataRow and 'T : equality and 'T : null> = {
    new IEqualityComparer<'T> with
    member this.Equals(row1, row2) = if (isNull row1) || (isNull row2) then row1 = row2 else (row1.RowState = row2.RowState) && ((row1.RowState = DataRowState.Deleted) || equalSequences(row1.ItemArray, row2.ItemArray))
    member this.GetHashCode(row) = row.GetHashCode()
}

let compareRows<'T when 'T :> DataRow and 'T : equality and 'T : null> = Seq.compareWith(fun row1 row2 -> if dataRowComparer.Equals(row1, row2) then 0 else 1)

let hashTableComparer<'T when 'T :> Hashtable and 'T : equality and 'T : null> = {
    new IEqualityComparer<'T> with
    member this.Equals(hashtable1, hashtable2) = (compareDictionaryEntries (Seq.cast hashtable1) (Seq.cast hashtable2) = 0)
    member this.GetHashCode(hashtable) = hashtable.GetHashCode()
}

let dataColumnComparer = {
    new IEqualityComparer<DataColumn> with
    member this.Equals(column1, column2) =
        column1.AllowDBNull = column2.AllowDBNull && column1.AutoIncrement = column2.AutoIncrement && column1.AutoIncrementSeed = column2.AutoIncrementSeed &&
        column1.AutoIncrementStep = column2.AutoIncrementStep && column1.Caption = column2.Caption && column1.ColumnMapping = column2.ColumnMapping &&
        column1.ColumnName = column2.ColumnName && column1.DataType = column2.DataType && column1.DateTimeMode = column2.DateTimeMode &&
        column1.DefaultValue = column2.DefaultValue && column1.Expression = column2.Expression && column1.MaxLength = column2.MaxLength &&
        column1.Namespace = column2.Namespace && column1.Ordinal = column2.Ordinal && column1.Prefix = column2.Prefix && column1.ReadOnly = column2.ReadOnly &&
        column1.Unique = column2.Unique && hashTableComparer.Equals(column1.ExtendedProperties, column2.ExtendedProperties)
    member this.GetHashCode(column) = column.GetHashCode()
}

let compareColumns<'T when 'T :> DataColumn and 'T : equality and 'T : null> = Seq.compareWith(fun column1 column2 -> if dataColumnComparer.Equals(column1, column2) then 0 else 1)

let dataRelationComparer = {
    new IEqualityComparer<DataRelation> with
    member this.Equals(dataRelation1, dataRelation2) =
        (compareColumns (Seq.cast dataRelation1.ChildColumns) (Seq.cast dataRelation2.ChildColumns) = 0) &&
        dataRelation1.ChildTable.TableName = dataRelation2.ChildTable.TableName &&
        dataRelation1.Nested = dataRelation2.Nested &&
        (compareColumns (Seq.cast dataRelation1.ParentColumns) (Seq.cast dataRelation2.ParentColumns) = 0) &&
        dataRelation1.ParentTable.TableName = dataRelation2.ParentTable.TableName &&
        dataRelation1.RelationName = dataRelation2.RelationName && hashTableComparer.Equals(dataRelation1.ExtendedProperties, dataRelation2.ExtendedProperties)
    member this.GetHashCode(dataRelation) = dataRelation.GetHashCode()
}

let compareRelations<'T when 'T :> DataRelation and 'T : equality and 'T : null> = Seq.compareWith(fun dataRelation1 dataRelation2 -> if dataRelationComparer.Equals(dataRelation1, dataRelation2) then 0 else 1)

let dataTableComparer<'T when 'T :> DataTable and 'T : equality and 'T : null> = {
    new IEqualityComparer<'T> with
    member this.Equals(dataTable1, dataTable2) =
        (compareRows (Seq.cast dataTable1.Rows)  (Seq.cast dataTable2.Rows) = 0) && (compareColumns (Seq.cast dataTable1.Columns) (Seq.cast dataTable2.Columns) = 0) &&
        dataTable1.TableName = dataTable2.TableName && dataTable1.CaseSensitive = dataTable2.CaseSensitive && dataTable1.DisplayExpression = dataTable2.DisplayExpression &&
        dataTable1.HasErrors = dataTable2.HasErrors && dataTable1.IsInitialized = dataTable2.IsInitialized && dataTable1.Locale = dataTable2.Locale && dataTable1.MinimumCapacity = dataTable2.MinimumCapacity &&
        dataTable1.Namespace = dataTable2.Namespace && (compareColumns (Seq.cast dataTable1.PrimaryKey) (Seq.cast dataTable2.PrimaryKey) = 0) && dataTable1.Prefix = dataTable2.Prefix &&
        dataTable1.RemotingFormat = dataTable2.RemotingFormat && hashTableComparer.Equals(dataTable1.ExtendedProperties, dataTable2.ExtendedProperties)
    member this.GetHashCode(dataTable) = dataTable.GetHashCode()
}

let compareTables<'T when 'T :> DataTable and 'T : equality and 'T : null> = Seq.compareWith(fun dataTable1 dataTable2 -> if dataTableComparer.Equals(dataTable1, dataTable2) then 0 else 1)

let dataSetComparer<'T when 'T :> DataSet and 'T : equality and 'T : null> = {
   new IEqualityComparer<'T> with
   member this.Equals(dataSet1, dataSet2) =
        dataSet1.CaseSensitive = dataSet2.CaseSensitive && dataSet1.DataSetName = dataSet2.DataSetName && dataSet1.EnforceConstraints = dataSet2.EnforceConstraints &&
        dataSet1.HasErrors = dataSet2.HasErrors && dataSet1.IsInitialized = dataSet2.IsInitialized && dataSet1.Locale = dataSet2.Locale && dataSet1.Namespace = dataSet2.Namespace &&
        dataSet1.Prefix = dataSet2.Prefix && dataSet1.RemotingFormat = dataSet2.RemotingFormat && dataSet1.SchemaSerializationMode = dataSet2.SchemaSerializationMode &&
        hashTableComparer.Equals(dataSet1.ExtendedProperties, dataSet2.ExtendedProperties) && (compareRelations (Seq.cast dataSet1.Relations) (Seq.cast dataSet2.Relations) = 0) &&
        (compareTables (Seq.cast dataSet1.Tables) (Seq.cast dataSet2.Tables) = 0)
   member this.GetHashCode(row) = row.GetHashCode()
}
