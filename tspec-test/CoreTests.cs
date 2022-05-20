using System.IO;
using Tspec.Core;
using Tspec.Report.Json;
using Xunit;
using Xunit.Abstractions;

namespace tspec_test
{
    public class CoreTests
    {
        private readonly ITestOutputHelper _out;

        public CoreTests(ITestOutputHelper @out)
        {
            _out = @out;
        }

        [Fact]
        public void T()
        {
            var spec = @"
Simple step

* Simple step1
* Simple step2

";
            var spec1 = new Spec();
            spec1.AddStepImplementation(new Steps());
            spec1.AddStepDefinition(new StringReader(spec));

            var stringWriter = new StringWriter();
            spec1.Dump(stringWriter);
            _out.WriteLine(stringWriter.ToString());

            foreach (var r in spec1.Run())
            {
                _out.WriteLine($"{r}");
            }
        }

        [Fact]
        public void Report()
        {
            _out.WriteLine("line");
            var memoryStream = new MemoryStream();
            var reporter = new JsonReporter(memoryStream);
            
            reporter.StartReport();
            reporter.StartSpec("Spec1");
            reporter.AddResult(new Result { Success = true, Text = "step1" });
            reporter.AddResult(new Result { Success = true, Text = "step2" });
            reporter.EndSpec();
            reporter.EndReport();
            
            memoryStream.Position = 0;
            var reader = new StreamReader(memoryStream);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                _out.WriteLine(line);
            }
        }
    }

    public class Steps
    {
        [Step("Simple step1")]
        public void Step1()
        {
        }

        [Step("Simple step2")]
        public void Step2()
        {
        }
    }
}