using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Driven.Metrics.Metrics;

namespace Driven.Metrics.metrics
{
    public class NumberOfChildren : IMetricCalculator
    {
        public int MaxPassValue {get; private set;}

        public NumberOfChildren(int value)
       {
           MaxPassValue = value;
       }
         
        private int CountChildrenForType(IEnumerable<TypeDefinition> types, TypeDefinition typeDef)
        {
            int count = 0;

            foreach (TypeDefinition other_typ in types)
            {
                if (other_typ.BaseType.Name == typeDef.Name)
                {
                    count++;                    
                }
            }  
           
            return count;
        }

        public MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            var classResults = new List<ClassResult>();
            int result;

            foreach (TypeDefinition typeDefinition in types)
            {
                result = CountChildrenForType(types, typeDefinition);
                classResults.Add(new ClassResult(typeDefinition.Name, result));
            }
            return new MetricResult("Number of Children", classResults);
        }

        public MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition type)
        {
            throw new NotImplementedException();
        }
    }
}
