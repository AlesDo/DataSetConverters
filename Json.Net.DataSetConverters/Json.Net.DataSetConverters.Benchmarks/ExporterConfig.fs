module ExporterConfig

open BenchmarkDotNet.Configs
open BenchmarkDotNet.Exporters.Csv
open BenchmarkDotNet.Exporters

type PlotExporterConfig() as this =
   inherit ManualConfig()
   do
       this.AddExporter(CsvMeasurementsExporter.Default, RPlotExporter.Default) |> ignore
