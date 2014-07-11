using System;
using System.Collections.Generic;
using Driven.Metrics.Metrics;
using Mono.Cecil.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Driven.Metrics.metrics
{
    public class DepthOfInheritance : IMetricCalculator
    {
      
        private int getDepth(TypeDefinition typeDef)
        {
            int depth = 0;
            TypeDefinition basetype = typeDef.BaseType as TypeDefinition;

            while (basetype != null && basetype.Name != "Object")
            {
                depth++;
                basetype = basetype.BaseType as TypeDefinition;
            }
            return depth;
        }


        public MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            var classResults = new List<ClassResult>();
            int result;

            foreach (TypeDefinition typeDefinition in types)
            {
                result = getDepth(typeDefinition);
                classResults.Add(new ClassResult(typeDefinition.Name, result));
            }

            return new MetricResult("Depth Of Inheritance", classResults);
        }
        
    }


}
