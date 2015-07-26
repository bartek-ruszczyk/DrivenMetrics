using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ISet<string> coupledTypes;
        private float coupling;

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
                            if (!coupledTypes.Contains(field.DeclaringType.Name))
                            {
                                coupledTypes.Add(field.DeclaringType.Name);
                                coupling++;
                            }
                        }
                    }                    
                    if (ins.OpCode.FlowControl == FlowControl.Call)
                    {
                        if (isMethodForCounting(ins))
                        {
                            MethodReference methodCall = ins.Operand as MethodReference;
                            //methods called from the same class are not counted
                            if ((!coupledTypes.Contains(methodCall.GetOriginalMethod().DeclaringType.Name)) && (methodCall.DeclaringType.Name != type.Name))
                            {
                                coupledTypes.Add(methodCall.GetOriginalMethod().DeclaringType.Name);
                                coupling++;
                            }
                        }
                    }
                }
            }

            // method result are not important, we count coupling for a whole type
            var pass = isLessThanRecommended(0);

            var friendlyName = methodDefinition.FriendlyName();
            return new MethodResult(friendlyName, 0, pass);
        }

        public MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            var classResults = new List<ClassResult>();
            assembly_types = types;
                   
            foreach (TypeDefinition typeDefinition in types)
            {
                coupledTypes = new HashSet<string>();
                coupling = 0;

                foreach (MethodDefinition method in typeDefinition.Methods.WithBodys())
                {
                    Calculate(method, typeDefinition);
                }
                
                classResults.Add(new ClassResult(typeDefinition.Name, coupling));
            }
            return new MetricResult("Coupling Between Modules", classResults);
        }
    }
}

