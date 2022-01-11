global using System;
global using System.Reflection;
global using BenchmarkDotNet.Attributes;

using BenchmarkDotNet.Running;


BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
