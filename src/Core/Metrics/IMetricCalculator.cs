using System.Collections.Generic;
using Driven.Metrics.Metrics;
using Mono.Cecil;
using PostSharp.Sdk.CodeModel;


namespace Driven.Metrics.metrics
{
    public interface IMetricCalculator
    {
        int MaxPassValue {get;}
		MetricResult Calculate(IEnumerable<TypeDefinition> types);
        MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition type);

    }
}