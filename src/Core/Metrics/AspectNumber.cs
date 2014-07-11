using System;
using System.Collections.Generic;
using Driven.Metrics.Metrics;
using System.Text;
using PostSharp.Sdk.CodeModel;

namespace Driven.Metrics.metrics
{
    public class AspectNumber : IAOPMetricCalculator
    {
        public MetricResult Calculate(IEnumerable<TypeDefDeclaration> types)
        {
            var classResults = new List<ClassResult>();
            int aspectNum = 0;
            foreach (TypeDefDeclaration typeDefinition in types)
            {
                if ((typeDefinition.BaseType != null) && (typeDefinition.BaseType.ToString().Contains("PostSharp") == true))
                {
                    aspectNum++;
                }
            }
            classResults.Add(new ClassResult("AOPNUM", aspectNum));
            return new MetricResult("Aspect Number", classResults);
        }
    }
}

