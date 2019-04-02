module TestHelpers

open FsCheck
open System.Text.RegularExpressions
open System.Data
open System
open System.Collections.Generic
open System.Linq
open System.Collections

let filter (testString: string) =
    testString.Length > 10

type MyGenerators =
    static member String() = (Arb.Default.String() |> Arb.filter (fun s -> match s with | null -> false | _ -> Regex.Match(s, "^[\w\d]+$").Success))
    static member DataColumn() = 
      let genDataColumn = gen {  let! columnName = MyGenerators.String().Generator
                                 let! dataType = Gen.elements [typeof<int>; typeof<int16>; typeof<string>; typeof<bool>; typeof<decimal>; typeof<float>; typeof<double>; typeof<byte[]>]
                                 return new DataColumn(columnName, dataType)}
      Arb.fromGenShrink (genDataColumn, fun  s -> Seq.empty)
    static member Char() = (Arb.Default.Char() |> Arb.filter (fun c -> not (Char.IsControl(c))))

type PrefixGenerators =
    static member String() = (Arb.Default.String() |> Arb.filter (fun s -> match s with | null -> false | _ -> Regex.Match(s, "^[a-zA-Z]+\z").Success))

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

let dataRowComparer<'T when 'T :> DataRow and 'T : equality and 'T : null> = {
    new IEqualityComparer<'T> with
    member this.Equals(row1, row2) = if (isNull row1) || (isNull row2) then row1 = row2 else (row1.RowState = row2.RowState) && equalSequences(row1.ItemArray, row2.ItemArray)
    member this.GetHashCode(row) = row.GetHashCode()
}

let compareRows<'T when 'T :> DataRow and 'T : equality and 'T : null> = Seq.compareWith(fun row1 row2 -> if dataRowComparer.Equals(row1, row2) then 0 else 1)

let hashTableComparer<'T when 'T :> Hashtable and 'T : equality and 'T : null> = {
    new IEqualityComparer<'T> with
    member this.Equals(hashtable1, hashtable2) = equalSequences((Seq.cast hashtable1.Keys), (Seq.cast hashtable2.Keys))
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
        column1.Unique = column2.Unique // add extended properties
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
        dataRelation1.RelationName = dataRelation2.RelationName
    member this.GetHashCode(dataRelation) = dataRelation.GetHashCode()
}

let dataTableComparer<'T when 'T :> DataTable and 'T : equality and 'T : null> = {
    new IEqualityComparer<'T> with
    member this.Equals(table1, table2) =
        (compareRows (Seq.cast table1.Rows)  (Seq.cast table2.Rows) = 0) && (compareColumns (Seq.cast table1.Columns) (Seq.cast table2.Columns) = 0) &&
        table1.TableName = table2.TableName && table1.CaseSensitive = table2.CaseSensitive && table1.DisplayExpression = table2.DisplayExpression &&
        table1.HasErrors = table2.HasErrors && table1.IsInitialized = table2.IsInitialized && table1.Locale = table2.Locale && table1.MinimumCapacity = table2.MinimumCapacity &&
        table1.Namespace = table2.Namespace && (compareColumns (Seq.cast table1.PrimaryKey) (Seq.cast table2.PrimaryKey) = 0) && table1.Prefix = table2.Prefix &&
        table1.RemotingFormat = table2.RemotingFormat
    member this.GetHashCode(row) = row.GetHashCode()
}
