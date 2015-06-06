using System.IO;

namespace Driven.Metrics.Reporting
{
    public class FileWriter : IFileWriter
    {

        public FileWriter ()
        { 
        }
		
        public void Write(string filepath, string contents)
        {
            File.WriteAllText(filepath,contents);
        }
    }
}