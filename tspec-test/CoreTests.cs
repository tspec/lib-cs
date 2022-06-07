using System;
using System.IO;
using System.Linq;
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
        public void Simple_step()
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

            var results = spec1.Run().ToList();
            
            foreach (var r in results)
            {
                _out.WriteLine($"{r}");
            }
            
            Assert.Equal(2, results.Count);
            Assert.True(results.All(r => r.Success));
        }

        [Fact]
        public void Table_step()
        {
            var spec = @"
Simple step

* Table step1

|name|value|
|----|----|
|A|1|

No table

* Table step1

";
            var spec1 = new Spec();
            spec1.AddStepImplementation(new Steps());
            spec1.AddStepDefinition(new StringReader(spec));

            var stringWriter = new StringWriter();
            spec1.Dump(stringWriter);
            _out.WriteLine(stringWriter.ToString());

            var results = spec1.Run().ToList();
            
            _out.WriteLine("RESULTS:");
            _out.WriteLine("=========================");
            foreach (var r in results)
            {
                _out.WriteLine($"{r}");
            }
            
            Assert.Equal(2, results.Count);
            Assert.True(results.All(r => r.Success));
        }
        
        [Fact]
        public void Param_step1()
        {
            var spec = @"
* Param step1 ""str1"" ""123""
";
            ParamStepRunner(spec);
        }

        [Fact]
        public void Param_step2()
        {
            var spec = @"
* Param step2 ""$str1"" ""123"" (more ""str2"")
";
            ParamStepRunner(spec);
        }

        
        private void ParamStepRunner(string spec)
        {
            var spec1 = new Spec();
            spec1.AddStepImplementation(new Steps());
            spec1.AddStepDefinition(new StringReader(spec));

            var stringWriter = new StringWriter();
            spec1.Dump(stringWriter);
            _out.WriteLine(stringWriter.ToString());

            var results = spec1.Run().ToList();
            
            _out.WriteLine("RESULTS:");
            _out.WriteLine("=========================");
            foreach (var r in results)
            {
                _out.WriteLine($"{r}");
            }
            
            Assert.Single(results);
            Assert.True(results.All(r => r.Success));

        }
        [Fact]
        public void Simple_step_with_tear_downs()
        {
            var spec = @"
* Simple step1 with error
* Simple step2
___
* Tear-down step1
* Tear-down step2
";
            var spec1 = new Spec();
            spec1.AddStepImplementation(new Steps());
            spec1.AddStepDefinition(new StringReader(spec));

            var stringWriter = new StringWriter();
            spec1.Dump(stringWriter);
            _out.WriteLine(stringWriter.ToString());

            var results = spec1.Run().ToList();
            
            foreach (var r in results)
            {
                _out.WriteLine($"{r}");
            }
            
            Assert.Equal(3, results.Count);
            Assert.Equal(2, results.Count(r => r.Success));
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

        [Step("Simple step1 with error")]
        public void Step1Error()
        {
            throw new Exception("step error");
        }
        
        [Step("Simple step2")]
        public void Step2()
        {
        }
        
        [Step("Table step1 <table>")]
        public void TableStep1(Table table)
        {
            if (table == null)
                throw new Exception($"Table null in {nameof(TableStep1)}");
        }
        
        [Step("Table step1")]
        public void TableStep1NoTable()
        {
        }
        
        [Step("Param step1 <p1> <p2>")]
        public void ParamStep1(string p1, int p2)
        {
            Assert.Equal("str1", p1);
            Assert.Equal(123, p2);
        }
        
        [Step("Param step2 <p1> <p2> (more <p3>)")]
        public void ParamStep2(string p1, int p2, string p3)
        {
            Assert.Equal("$str1", p1);
            Assert.Equal(123, p2);
            Assert.Equal("str2", p3);
        }
        
        [Step("Tear-down step1")]
        public void TearDown1()
        {
        }
        
        [Step("Tear-down step2")]
        public void TearDown2()
        {
        }
    }
}