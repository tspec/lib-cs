using System.IO;
using TSpec.Lib;
using Xunit;
using Xunit.Abstractions;

namespace tspec_test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _out;

        public UnitTest1(ITestOutputHelper @out)
        {
            _out = @out;
        }

        [Fact]
        public void Test1()
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