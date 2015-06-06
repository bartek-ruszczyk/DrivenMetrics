using Driven.Metrics.Metrics;
using PostSharp.Sdk.CodeModel;
using System.Collections.Generic;


namespace Driven.Metrics.metrics
{
    public interface IAOPMetricCalculator
    {        
        MetricResult Calculate(IEnumerable<TypeDefDeclaration> types);
    }
}