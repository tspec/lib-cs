using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Tspec.Core;
using Tspec.Report.Json;

namespace tspec_example
{
    class Program
    {
        static int Main()
        {
            var file = "Demo.tspec.md";
            using var textReader = File.OpenText(file);

            var stream = new MemoryStream();
            var report = new JsonReporter(stream);
            report.StartReport();
            
            report.StartSpec(file);
            var spec = new Spec();
            spec.AddStepDefinition(textReader);
            spec.AddStepImplementationAssembly(Assembly.GetExecutingAssembly());

            var results = spec.Run().ToList();
            
            foreach (var result in results)
            {
                report.AddResult(result);
                Console.WriteLine(result);
            }
            
            report.EndSpec();
            report.EndReport();

            stream.Position = 0;
            var reader = new StreamReader(stream);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                Console.WriteLine(line);
            }

            if (!report.Success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error running example.");
                Console.ResetColor();
                return 1;
            }

            return 0;
        }
    }
}