namespace Json.Net.DataSetConverters

open System
open System.Globalization
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Newtonsoft.Json.Serialization

module JsonSerializationExtensions = 
    let FormatMessage(lineInfo: IJsonLineInfo, path: string, message: string) =
        let mutable mutableMessage: string = message
        if mutableMessage.EndsWith(Environment.NewLine, StringComparison.Ordinal) then
            mutableMessage <- mutableMessage.Trim()
            if not (mutableMessage.EndsWith(".")) then
                mutableMessage <- mutableMessage + "."
            mutableMessage <- mutableMessage + " "

        mutableMessage <- mutableMessage + String.Format("Path '{0}'", CultureInfo.InvariantCulture, path)
        if not (isNull lineInfo) && lineInfo.HasLineInfo() then
            mutableMessage <- mutableMessage + String.Format(", line {0}, position {1}", CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition)
        mutableMessage <- mutableMessage + "."
        mutableMessage

    let resolveType(assemblyQualifiedName: string) =
        let typeName = assemblyQualifiedName.Substring(0, assemblyQualifiedName.IndexOf(','));
        let resolvedType = Type.GetType(typeName)
        if isNull(resolvedType) then
            Type.GetType(assemblyQualifiedName)
        else
            resolvedType

    type JsonReader with

        member this.ReadAndAssert() =
            if not (this.Read()) then
                raise (this.CreateSerializationException("Unexpected end when reading JSON."))

        member this.ValidateJsonToken(jsonToken: JsonToken, objectName: string) =
            if this.TokenType <> jsonToken then
                raise (this.CreateSerializationException(String.Format(CultureInfo.InvariantCulture, "Unexpected JSON token when reading {0}. Expected {1}, got {2}.", objectName, jsonToken, this.TokenType)))

        member this.ValidatePropertyName(propertyName: string, objectName: string) =
            this.ValidateJsonToken(JsonToken.PropertyName, objectName)
            if not (String.Equals(this.Value, propertyName)) then
                raise (this.CreateSerializationException(String.Format(CultureInfo.InvariantCulture, "Unexpected PropertyName whean reading {0}. Expected {1}, got {2}.", objectName, propertyName, this.TokenType)))

        member this.ReadPropertyFromOutput<'T>(serializer: JsonSerializer, propertyName: string, objectName: string) =
            this.ReadAndAssert()
            this.ValidatePropertyName(propertyName, objectName)
            this.ReadAndAssert()
            serializer.Deserialize<'T>(this)

        member this.ReadRowValuePropertyFromOutput(serializer: JsonSerializer, propertyName: string, propertyType: Type, objectName: string) =
            let propertyValue = this.ReadTypedPropertyFromOutput(serializer, propertyName, propertyType, objectName)
            if isNull(propertyValue) then DBNull.Value :> obj else propertyValue

        member this.ReadTypedPropertyFromOutput(serializer: JsonSerializer, propertyName: string, propertyType: Type, objectName: string) =
            this.ReadAndAssert()
            this.ValidatePropertyName(propertyName, objectName)
            this.ReadAndAssert()
            if this.TokenType = JsonToken.Null then
                null
            else
                serializer.Deserialize(this, propertyType)

        member this.ReadPropertyFromOutput<'T>(serializer: JsonSerializer, propertyName: string, objectName: string, existingValue: obj, converter: JsonConverter) =
            this.ReadAndAssert()
            this.ValidatePropertyName(propertyName, objectName)
            this.ReadAndAssert()
            converter.ReadJson(this, typedefof<'T>, existingValue, serializer) :?> 'T

        member private this.DowncastToIJsonLineInfo =
            match this with
                | :? JsonTextReader as textReader -> textReader :> IJsonLineInfo
                | :? JTokenReader as jTokenReader -> jTokenReader :> IJsonLineInfo
                | _ -> raise (InvalidCastException())

        member private this.CreateSerializationException(message: string) =
            JsonSerializationException(FormatMessage(this.DowncastToIJsonLineInfo, this.Path, message))

    type JsonWriter with

        member this.WritePropertyToOutput(serializer: JsonSerializer, contractResolver: DefaultContractResolver, propertyName: string, propertyValue: obj) =
            if serializer.NullValueHandling <> NullValueHandling.Ignore || not(isNull(propertyValue)) then
                this.WritePropertyName(if not (isNull contractResolver) then contractResolver.GetResolvedPropertyName(propertyName) else propertyName)
                serializer.Serialize(this, propertyValue)

        member this.WritePropertyToOutput(serializer: JsonSerializer, contractResolver: DefaultContractResolver, propertyName: string, propertyValue: obj, converter: JsonConverter) =
            if serializer.NullValueHandling <> NullValueHandling.Ignore || not(isNull(propertyValue)) then
                this.WritePropertyName(if not (isNull contractResolver) then contractResolver.GetResolvedPropertyName(propertyName) else propertyName)
                converter.WriteJson(this, propertyValue, serializer)

        member this.WriteTypedPropertyToOutput(serializer: JsonSerializer, contractResolver: DefaultContractResolver, propertyName: string, propertyType: Type, propertyValue: obj) =
            if serializer.NullValueHandling <> NullValueHandling.Ignore || not(isNull(propertyValue)) then
                this.WritePropertyName(if not (isNull contractResolver) then contractResolver.GetResolvedPropertyName(propertyName) else propertyName)
                match propertyType with
                | t when t = typeof<decimal> && not (isNull propertyValue) -> serializer.Serialize(this, (propertyValue :?> decimal).ToString("F28", CultureInfo.InvariantCulture))
                | _ -> serializer.Serialize(this, propertyValue)

        member this.WriteRowValuePropertyToOutput(serializer: JsonSerializer, contractResolver: DefaultContractResolver, propertyName: string, propertyType: Type, propertyValue: obj) =
            let serializeValue = if propertyValue = (DBNull.Value :> obj) then null else propertyValue
            this.WriteTypedPropertyToOutput(serializer, contractResolver, propertyName, propertyType, serializeValue)
