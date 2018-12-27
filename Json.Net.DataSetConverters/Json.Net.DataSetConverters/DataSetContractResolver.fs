namespace Json.Net.DataSetConverters
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Data

type DataSetContractResolver =
    inherit DefaultContractResolver

    override this.CreateProperty(memberInfo, memberSerialization) =
        let property: JsonProperty = base.CreateProperty(memberInfo, memberSerialization)
        if property.DeclaringType = typeof<DataColumn> && property.PropertyType = typeof<DataTable> then
            property.Ignored <- true
        
        property
