using Driven.Metrics.Interfaces;
using Driven.Metrics.metrics;
using Driven.Metrics.Metrics;
using Driven.Metrics.Reporting;
using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Driven.Metrics
{
    public class DrivenMetrics
    {
        public readonly IAssemblySearcher _assemblySearcher;
        public readonly IReport Report;
        public readonly IMetricCalculator[] _metricCalculators;
        public readonly IEnumerable<IAOPMetricCalculator> AopMetrics;

        public DrivenMetrics(IAssemblySearcher methodFinder, IReport report, IMetricCalculator[] metricCalculators,
            IEnumerable<IAOPMetricCalculator> aopMetrics)
        {
            _assemblySearcher = methodFinder;
            Report = report;
            _metricCalculators = metricCalculators;
            AopMetrics = aopMetrics;
        }

        /*    public void CalculateLinesOfCode()
        {
            foreach (TypeDefinition typeInAsm in _assemblyDefinition.MainModule.Types)
            {
                if (!typeInAsm.IsClass)
                    continue;
                
                Console.WriteLine("Type: " + typeInAsm);

                foreach (MethodDefinition method in typeInAsm.Methods)
                {
                    if (method.IsConstructor || method.IsSetter || method.IsGetter || method.IsAbstract)
                        continue;
                }
            }
        }
      

        public int CalculateCyclomicComplexity(string methodName)
        {
            var methodDefinition = getMethod(methodName);
            
            var cyclomicCompexity = new ILCyclomicComplextityCalculator();
            return cyclomicCompexity.Calculate(methodDefinition);
            return 5;
        }*/

        public DrivenMetrics Create(string[] assemblyNames)
            {
                throw new Exception();
            }

        public void RunAllMetricsAndGenerateReport()
        {
            var metricResults = new List<MetricResult>();

            foreach (var calculator in _metricCalculators)
            {
                var result = calculator.Calculate(_assemblySearcher.GetAllTypes());
                metricResults.Add(result);
            }

            foreach (IAOPMetricCalculator aopMetric in AopMetrics)
            {
                MetricResult result = aopMetric.Calculate(AssemblyLoader.all_valid_types);
                metricResults.Add(result);
            }

            Report.Generate(metricResults);
        }
        
        
        public class Factory
        {
            public DrivenMetrics Create(string assemblyName,  string reportFilePath)
            {
                return Create(new[] {assemblyName}, reportFilePath);
            }
			
			public DrivenMetrics Create(string[] assemblyNames, string reportFilePath)
            {
                var assemblies = new List<AssemblyDefinition>();

                foreach (var assemblyName in assemblyNames)
                {
                    var assemblyLoader = new AssemblyLoader(assemblyName);
                    var assembly = assemblyLoader.Load();    
                    assemblies.Add(assembly);
                }

                var methodFinder = new AssemblySearcher(assemblies.ToArray());
                //var htmlReport = new HtmlReport(new FileWriter(), reportFilePath);
                var htmlReport = new HtmlFailedReport(new FileWriter(), reportFilePath);
                var numberOfLines = new NumberOfLinesCalculator(20);
                var cyclomicCompexity = new ILCyclomicComplextityCalculator(20);

                var drivenMetric = new DrivenMetrics(methodFinder, htmlReport, new IMetricCalculator[] { numberOfLines, cyclomicCompexity }, new List<IAOPMetricCalculator>());

                return drivenMetric;
            }
			
			public DrivenMetrics Create(string[] assemblyNames, IMetricCalculator[] metrics, IEnumerable<IAOPMetricCalculator> aopMetrics, string reportFilePath, IReport htmlReport)
			{
				 var assemblies = new List<AssemblyDefinition>();

                foreach (var assemblyName in assemblyNames)
                {
                    var assemblyLoader = new AssemblyLoader(assemblyName);
                    var assembly = assemblyLoader.Load();    
                    assemblies.Add(assembly);
                    //AssemblyLoader.LoadAssemblyPostSharp(assemblyName);
                }

                var methodFinder = new AssemblySearcher(assemblies.ToArray());
				var drivenMetric = new DrivenMetrics(methodFinder, htmlReport, metrics, aopMetrics);

                return drivenMetric;	
				
			}
        }
    }
}
