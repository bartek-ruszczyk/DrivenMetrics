using System.Collections.Generic;
using Driven.Metrics.Metrics;
using Mono.Cecil;
using PostSharp.Sdk.CodeModel;


namespace Driven.Metrics.metrics
{
    public interface IAOPMetricCalculator
    {        
        MetricResult Calculate(IEnumerable<TypeDefDeclaration> types);        

    }
}