namespace Json.Net.DataSetConverters.Benchmarks

open System.Xml.Serialization
open System

type User(firstName: string, lastName: string) =
    member val public FirstName = firstName with get, set
    member val public LastName = lastName with get, set

    new() = User(null, null)

    interface IXmlSerializable with
        member this.GetSchema(): Xml.Schema.XmlSchema = 
            null
        member this.ReadXml(xmlReader) =
            xmlReader.MoveToContent() |> ignore
            this.FirstName <- xmlReader.GetAttribute("FirstName")
            this.LastName <- xmlReader.GetAttribute("LastName")
            xmlReader.Read() |> ignore
        member this.WriteXml(xmlWriter) =
            xmlWriter.WriteAttributeString("FirstName", this.FirstName)
            xmlWriter.WriteAttributeString("LastName", this.LastName)
