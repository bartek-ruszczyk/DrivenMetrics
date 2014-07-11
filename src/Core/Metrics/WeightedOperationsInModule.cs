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
    public class WeightedOperationsInModule : IMetricCalculator
    {
        private IEnumerable<TypeDefinition> assembly_types;
        public int MaxPassValue {get; private set;}

        public WeightedOperationsInModule(int value)
       {
           MaxPassValue = value;
       }
      
        //NOT COUNTED: abstract methods and methods with no body
        //COUNTED: all constructors (including defaults)     
        public MetricResult Calculate(IEnumerable<TypeDefinition> types)
        {
            assembly_types = types;
            var classResults = new List<ClassResult>();

            foreach (TypeDefinition typeDefinition in assembly_types)
            {
                int result = 0;
                foreach (MethodDefinition m in typeDefinition.Methods)
                {                    
                    if ((!(m.Name == "InitializeComponent" || m.Name == "Dispose")) && m.HasBody && (m.Body.CodeSize != 2))
                    {
                        result++;                       
                    }          
                }

                result = 0;
                foreach (MethodDefinition c in typeDefinition.Constructors)
                {                   
                    if (!(c.Name == ".cctor"))
                    {
                        result++;                       
                    }
                }     
                classResults.Add(new ClassResult(typeDefinition.Name, result));              
               
            }
            return new MetricResult("Weighted Operations in Module", classResults);
        }


        public MethodResult Calculate(MethodDefinition methodDefinition, TypeDefinition type)
        {
            throw new NotImplementedException();
        }
        
    }
}
