global using System;
global using System.Reflection;
global using BenchmarkDotNet.Attributes;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

using DotNetVerify.SealedClass;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

//var customConfig = ManualConfig
//    .Create(DefaultConfig.Instance)
//    .AddValidator(JitOptimizationsValidator.FailOnError)
//    .AddDiagnoser(MemoryDiagnoser.Default)
//    .AddColumn(StatisticColumn.AllStatistics)
//    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
//    .AddExporter(DefaultExporters.Markdown);

//BenchmarkRunner.Run<SealedClassBenchmark>(customConfig);