using System.Collections.Generic;

namespace Driven.Metrics.Metrics
{
    public class MethodResult
    {
        public string Name {get; private set;}
        public int Result {get; private set;}
        public bool Pass { get; private set; }
		
        public MethodResult(string name, int result, bool pass)
        {
            Name = name;
            Result = result;
            Pass = pass;
        }
		
    }

    public class ClassResult
    {
        public string Name {get; private set;}
        public IList<MethodResult> MethodResults {get; private set;}

        public float Result { get { return res; } }

        public float res;


        public ClassResult(string name, IList<MethodResult> methodResults)
        {
            Name = name;
            MethodResults = methodResults; 
        }

        public ClassResult(string name, float result)
        {
            Name = name;
            res = result;
        }
		
    }

    public class MetricResult
    {
        public string Name {get; private set;}
        public IList<ClassResult> ClassResults {get; private set;}
		
        public MetricResult(string name, IList<ClassResult> classResults)
        {
            Name = name;
            ClassResults = classResults;
			
        }
		
    }
}