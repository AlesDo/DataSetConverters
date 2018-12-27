namespace Json.Net.DataSetConverters
open System.Data
open System.Collections.Generic

module DataSetExtensions =
    
    type DataSet with
        member this.GetTablesOderedByRelationshisps = 
            let iterationQueue: Queue<DataTable> = new Queue<DataTable>(
                                                    Seq.cast<DataTable> this.Tables |> 
                                                    Seq.filter<DataTable>(fun dataTable -> (isNull dataTable.ParentRelations)))
            seq { 
                while iterationQueue.Count > 0 do
                let dataTable: DataTable = iterationQueue.Dequeue()
                for childRelation: DataRelation in dataTable.ChildRelations do
                    iterationQueue.Enqueue(childRelation.ChildTable)
                yield dataTable
            }

