using System;
using System.Collections.Generic;
using Driven.Metrics.Metrics;
using System.Text;
using PostSharp.Sdk.CodeModel;

namespace Driven.Metrics.metrics 
{
      //for an aspect - the number of classes and aspects that aspects weaves into
    public class AspectCrosscuttingDegree : IAOPMetricCalculator
    {
        public MetricResult Calculate(IEnumerable<TypeDefDeclaration> types)
        {
            var classResults = new List<ClassResult>();
            foreach (TypeDefDeclaration typeDefinition in types)
            {
                List<string> usedDistinctAspects = new List<string>();
                foreach (TypeDefDeclaration innerType in typeDefinition.Types)
                {
                    string innerTypeName = innerType.ShortName.ToString();
                    if (innerTypeName.Contains("Aspects")) 
                    {
                        if (innerType.ParentType != null && innerType.Fields != null && innerType.Fields.Count > 0)
                        {
                            foreach (var field in innerType.ParentType.Fields)
                            {
                                if (field.FieldType != null)
                                {
                                    string key = field.FieldType.ToString();
                                    if (key.Contains("MethodBase"))
                                    {
                                        continue;
                                    }
                                    if (!usedDistinctAspects.Contains(key))
                                    {
                                        usedDistinctAspects.Add(key);
                                    }
                                }
                            }
                        }
                    }
                }
                classResults.Add(new ClassResult(typeDefinition.ShortName, usedDistinctAspects.Count));
            }
            return new MetricResult("Aspect Crosscutting Degree - Usage", classResults);
        }      
    }
}
