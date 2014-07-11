using System;
using System.Collections.Generic;
using Driven.Metrics.Metrics;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;

namespace Driven.Metrics.metrics
{
    public class ResponseForAModule : IMetricCalculator
    {
        /****RESPONSE FOR A MODULE*********
         * count methods in each class (only concrete methods (with body)) 
         * and all methods called from them (only the first level of call in the calling hierarchy);
         * methods called from PostSharp classes are not called, but for an aspect all methods called from it are counted
         *************************/

        private IEnumerable<TypeDefinition> assembly_types;
        
        public int MaxPassValue {get; private set;}

        public ResponseForAModule(int value)
        {
           MaxPassValue = value;
        }
        
        public MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition typeDef)
        {
            int count = 0;
            var dict = new Dictionary<string, int>();

            //properties are also included
            if (methodDefinition.Body != null)
                {                   
                    count++;
                    foreach (Instruction ins in methodDefinition.Body.Instructions)
                    {                                             
                        if (ins.OpCode.FlowControl == FlowControl.Call)
                        {
                            if (isMethodForCounting(ins))
                            {  
                                MethodReference methodCall = ins.Operand as MethodReference;
                                if ((!dict.ContainsKey(methodCall.Name)) && (methodCall.DeclaringType.Name != typeDef.Name))
                                {
                                    count++;
                                    dict.Add(methodCall.Name, count);                                   
                                }
                            }
                        }
                    }
                }

            var pass = isLessThanRecommended(count);
            var friendlyName = methodDefinition.FriendlyName();
            return new MethodResult(friendlyName, count, pass);
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
            return new MetricResult("Response For a Module", classResults);
        }

        public bool isMethodForCounting(Instruction ins)
        {
            MethodReference methodCall = ins.Operand as MethodReference;

            if ((methodCall != null) && (methodCall.Name.ToString() != ".ctor"))
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
