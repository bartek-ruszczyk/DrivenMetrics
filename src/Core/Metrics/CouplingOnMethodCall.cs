using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Driven.Metrics.Metrics;
using Mono.Cecil.Extensions;

namespace Driven.Metrics.metrics
{
    class CouplingOnMethodCall : IMetricCalculator
    {
        private IEnumerable<TypeDefinition> assembly_types;
        
        public int MaxPassValue {get; private set;}

        public CouplingOnMethodCall(int value)
        {
           MaxPassValue = value;
        }

        public MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition typeDef)
        {           
            var lst = new List<string>();

            foreach (Instruction ins in methodDefinition.Body.Instructions)
            {
                if (isMethodForCounting(ins))
                {
                    MethodReference methodCall = ins.Operand as MethodReference;
                    //do not count methods called from the same class
                    if ((!lst.Contains(methodCall.GetOriginalMethod().DeclaringType.Name)) && (methodCall.DeclaringType.Name != typeDef.Name))
                    {
                        lst.Add(methodCall.GetOriginalMethod().DeclaringType.Name);
                    }
                }
            }            
            var value = lst.Count;
            var pass = isLessThanRecommended(value);
            var friendlyName = methodDefinition.FriendlyName();
            return new MethodResult(friendlyName, value, pass);
        }

        private bool isLessThanRecommended(int value)
        {
            if (value > MaxPassValue)
                return false;

            return true;
        }

        public MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            var classResults = new List<ClassResult>();
            assembly_types = types;

            foreach (TypeDefinition typeDefinition in types)
            {
                var results = new List<MethodResult>();                 
                foreach (MethodDefinition method in typeDefinition.Methods.WithBodys())
                {
                    var methodResult = Calculate(method, typeDefinition);
                    results.Add(methodResult);
                }

                if (results.Count == 0)
                    continue;

                classResults.Add(new ClassResult(typeDefinition.Name, results));
            }

            return new MetricResult("Coupling on Method Calls", classResults);
        }

        public bool isMethodForCounting(Instruction ins)
        {
            MethodReference methodCall = ins.Operand as MethodReference;

            if ((methodCall != null))
            {
                foreach (TypeDefinition t in assembly_types)
                {
                    if (t.Name == methodCall.DeclaringType.Name) return true;
                }
            }
            return false;
        }
    }
}
