using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Driven.Metrics.Metrics;
using Mono.Cecil.Extensions;

namespace Driven.Metrics.metrics
{
    /*******************WEIGHTED OPERATIONS IN MODULE********************
     * PostSharp methods injected (woven) into class are not counted -
     * - they are only counted as methods of an aspect
     * ****/
    public class WeightedOperationsWithCComplexity : IMetricCalculator
    {
        private IEnumerable<TypeDefinition> assembly_types;

        public int MaxPassValue {get; private set;}

        public WeightedOperationsWithCComplexity(int value)
        {
            MaxPassValue = value;           
        }

        public  MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            var classResults = new List<ClassResult>();
            assembly_types = types;

            foreach (TypeDefinition typeDefinition in assembly_types)
            {
                int result = 0;
                var comlexity = new ILCyclomicComplextityCalculator(MaxPassValue);
                var count = 0;

                foreach (MethodDefinition m in typeDefinition.Methods)
                {
                    if ((!(m.Name == "InitializeComponent" || m.Name == "Dispose")) && m.HasBody && (m.Body.CodeSize != 2))
                    {
                        var res = comlexity.Calculate(m, typeDefinition);
                        result += res.Result;
                        count++;
                    }
                }
                foreach (MethodDefinition c in typeDefinition.Constructors)
                {                  
                    if (!(c.Name == ".cctor"))
                    {
                        result++;
                        count++;
                    }
                }
                float classResult = 0;
                if (count != 0)
                {
                    classResult = (float)(result / count);
                }
                classResults.Add(new ClassResult(typeDefinition.Name, classResult));
            }
            return new MetricResult("Weighted Operations in Module - Cyclomatic Complexity", classResults);
        }

        public MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition type)
        {
            throw new NotImplementedException();
        }

    }
}


