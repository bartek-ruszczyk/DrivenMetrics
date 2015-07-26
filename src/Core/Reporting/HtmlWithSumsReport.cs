using Driven.Metrics.Metrics;
using System.Linq;
using System.Web;

namespace Driven.Metrics.Reporting
{
    public class HtmlWithSumsReport : HtmlReport
    {
        private const string RowFormat = "<tr><td>{0}</td><td>{1}</td></tr>";

        public HtmlWithSumsReport(IFileWriter fileWriter, string filePath) : base(fileWriter, filePath)
        {

        }

        protected override void inputResults(MetricResult result)
        {
            float metricResult = 0;
            foreach (ClassResult classResult in result.ClassResults)
            {
                metricResult += classResult.Result;
                Contents += string.Format(RowFormat, HttpUtility.HtmlEncode(classResult.Name), classResult.Result);
            }

            Contents += string.Format("<tr><td><b>{0}</b></td><td><b>{1}</b></td></tr>",
                HttpUtility.HtmlEncode(result.Name), metricResult);
        }

        protected override void createTableHeader(MetricResult result)
        {
            Contents += "<h2>" + result.Name + "</h2>";
            Contents += @"<table border=""1"">
							<tr>
							<th>Name</th>
							<th>Result</th>
							</tr>";
        }
    }
}
