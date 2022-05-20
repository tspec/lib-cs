using System;
using System.IO;
using System.Reflection;
using Tspec.Core;
using Tspec.Report.Json;

namespace tspec_example
{
    class Program
    {
        static void Main(string[] args)
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
            foreach (var result in spec.Run())
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
        }
    }
}