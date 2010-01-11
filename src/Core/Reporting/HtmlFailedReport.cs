
using DrivenMetrics.Metrics;

namespace DrivenMetrics.Reporting
{

	public class HtmlFailedReport : HtmlReport
	{
		protected override string addResult(MethodResult methodResult)
        {
            string result = string.Empty;
            //return @"<td id =""fail"">" + methodResult.Result + "</td>";
            result += _emptyColumn;
            result += "<td>" + methodResult.Name + "</td>";
            result += @"<td id =""fail"">" + methodResult.Result + "</td>";
            result += "</tr>";

            return result;
        }

        protected override void inputResults(MetricResult result)
        {
            Contents += createFailedRows(result);
        }

	    private string createFailedRows(MetricResult result)
	    {
	        string htmlOutput = string.Empty;
	        
            foreach (var classResult in result.ClassResults)
	        {
	            bool addedClassHeader = false;
                string htmlClass = "<tr>";
                htmlClass += "<td>" + classResult.Name + "</td>";
                htmlClass += _emptyColumn + _emptyColumn + "</tr>";

	            foreach (var methodResult in classResult.MethodResults)
	            {
                    if (methodResult.Pass)
                        continue;

                    if (!addedClassHeader)
                    {
                        addedClassHeader = true;
                        htmlOutput += htmlClass;
                    }

	                htmlOutput += addResult(methodResult);
	            }
	        }
	        return htmlOutput;
	    }

	    public HtmlFailedReport (IFileWriter fileWriter, string filePath) : base(fileWriter, filePath)
		{
			_ReportName = "Driven Metric Failing Methods";
		}
	}
}
