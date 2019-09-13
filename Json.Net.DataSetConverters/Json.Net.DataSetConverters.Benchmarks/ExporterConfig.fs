module ExporterConfig

open BenchmarkDotNet.Configs
open BenchmarkDotNet.Exporters.Csv
open BenchmarkDotNet.Exporters

type PlotExporterConfig() as this =
   inherit ManualConfig()
   do
       this.Add(CsvMeasurementsExporter.Default)
       this.Add(RPlotExporter.Default)
