using System;
using System.Collections.Generic;
using Driven.Metrics.Metrics;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;

namespace Driven.Metrics.metrics
{
    
    //for each class the number of used classes
    //the number of aspects used is not included in this number, but it is counter separately
    public class CouplingBetweenModules : IMetricCalculator
    {
        public int MaxPassValue { get; private set; }
        private IEnumerable<TypeDefinition> assembly_types;

        public CouplingBetweenModules(int maxValue)
        {
            MaxPassValue = maxValue;
        }

        public bool isFieldForCounting(Instruction ins)
        {        
            FieldReference field = ins.Operand as FieldReference;
            foreach (TypeDefinition t in assembly_types)
            {              
                if (t.Name == field.DeclaringType.Name) return true;
            }
            return false;
        }

        public static bool isFieldUsage(Instruction ins)
        {
            if (ins.OpCode == OpCodes.Stfld || ins.OpCode == OpCodes.Stsfld || ins.OpCode == OpCodes.Ldfld
                                            || ins.OpCode == OpCodes.Ldsfld || ins.OpCode == OpCodes.Ldsflda)
            { return true; }
            return false;
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

        private bool isLessThanRecommended(int value)
        {
            if (value > MaxPassValue)
                return false;

            return true;
        }

        public MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition type) 
        {
            var lst = new List<string>();  

            if (methodDefinition.Body != null)
            {
                foreach (Instruction ins in methodDefinition.Body.Instructions)
                {
                    if (isFieldUsage(ins))
                    {                      
                        FieldReference field = ins.Operand as FieldReference;
                        if ((field != null) && (field.DeclaringType.Name != type.Name)
                            //do not count fields called from aspects
                                            && isFieldForCounting(ins))
                        {
                            if (!lst.Contains(field.DeclaringType.Name))
                            {
                                lst.Add(field.DeclaringType.Name);
                            }
                        }
                    }                    
                    if (ins.OpCode.FlowControl == FlowControl.Call)
                    {
                        if (isMethodForCounting(ins))
                        {
                            MethodReference methodCall = ins.Operand as MethodReference;
                            //methods called from the same class are not counted
                            if ((!lst.Contains(methodCall.GetOriginalMethod().DeclaringType.Name)) && (methodCall.DeclaringType.Name != type.Name))
                            {
                                lst.Add(methodCall.GetOriginalMethod().DeclaringType.Name);

                            }
                        }
                    }
                }
            }

            var coupling = lst.Count;
            var pass = isLessThanRecommended(coupling);

            var friendlyName = methodDefinition.FriendlyName();
            return new MethodResult(friendlyName, coupling, pass);
        }

        public MetricResult Claculate(IEnumerable<TypeDefinition> types)
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
            return new MetricResult("Coupling Between Modules", classResults);
        }
    }
}

