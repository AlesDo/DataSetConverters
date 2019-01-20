namespace Json.Net.DataSetConverters
open System.Data
open System.Collections

module PropertyCollectionExtensions =
    
    
    type PropertyCollection with
        member this.ReplaceItems(sourcePropertyCollection: PropertyCollection) =
           this.Clear()
           for extendedProperty in Seq.cast<DictionaryEntry> sourcePropertyCollection do
               this.Add(extendedProperty.Key, extendedProperty.Value)
