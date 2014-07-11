using System;
using System.Collections.Generic;
using Driven.Metrics.Metrics;
using System.Text;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Driven.Metrics.metrics
{
    public class LackOfCohesionInOperations : IMetricCalculator
    {        
        public int MaxPassValue {get; private set;}

        //TODO
        public LackOfCohesionInOperations(int value)
       {
           MaxPassValue = value;
       }

        private List<string> CreateClassFieldsList(TypeDefinition typeDef)
        {
            var lstClassFields = new List<string>();
            foreach (FieldDefinition fd in typeDef.Fields)
            {
                lstClassFields.Add(fd.Name);
            }
            return lstClassFields;
        }

        private List<string> CompareWithClassField(TypeDefinition typeDef, MethodDefinition met)
        {
            var lstClassFields = CreateClassFieldsList(typeDef);
            var lstMethodFields = new List<string>();
            
            foreach (Instruction ins in met.Body.Instructions)
            {
                    if (ins.OpCode == OpCodes.Stfld || ins.OpCode == OpCodes.Stsfld || ins.OpCode == OpCodes.Ldfld
                                                    || ins.OpCode == OpCodes.Ldsfld || ins.OpCode == OpCodes.Ldsflda)
                    {
                        FieldReference field = ins.Operand as FieldReference;                     

                        if (field != null)
                        {
                            if (lstClassFields.Contains(field.Name))
                            {
                                lstMethodFields.Add(field.Name);
                            }
                        }

                    }
               
            }
            return lstMethodFields;
        }

       
        private int CreateCombinations(List<List<string>> lst)
        {
            IEnumerable<string> intersection;
            var br = 0;
            var nonEmpty = 0;
            var isEmpty = 0;            

            var lst_copy = new List<List<string>>();
            lst_copy.AddRange(lst);
            
            if (lst.Count == 0) return 0;

            foreach (var l in lst)
            {
                if (lst_copy.Count != 0)
                {
                    lst_copy.RemoveAt(0);

                    foreach (var l2 in lst_copy)
                    {
                        intersection = l.Intersect(l2);
                        br = intersection.Count();                                          
                        if (br != 0) nonEmpty++;
                        else isEmpty++;
                    }
                }
                else break;

            }

            var diff = isEmpty - nonEmpty;

            if (diff > 0) return diff;
            else return 0;


        }

        private int CalculateCohesion(TypeDefinition typeDef)
        {
            var lstMethodFieldsLst = new List<List<string>>();

             //including getters and setters
            foreach (MethodDefinition m in typeDef.Methods)
            {
                var lstMethodFields = new List<string>();
                if (m.HasBody)
                {
                    lstMethodFields = CompareWithClassField(typeDef, m);
                    lstMethodFieldsLst.Add(lstMethodFields);
                } 
            }          
            return CreateCombinations(lstMethodFieldsLst); 
        }

        public MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            var classResults = new List<ClassResult>();
            int result;

            foreach (TypeDefinition typeDefinition in types)
            {
                result = CalculateCohesion(typeDefinition);
                classResults.Add(new ClassResult(typeDefinition.Name, result));
            }
            return new MetricResult("Lack of Cohesion in Operations", classResults);
        }

        //TODO
        public MethodResult Calculate(MethodDefinition method, TypeDefinition type)
        {
            throw new NotImplementedException();
        }

    }
}
