using System;
using System.IO;
using System.Reflection;
using TSpec.Lib;

namespace tspec_example
{
    class Program
    {
        static void Main(string[] args)
        {
            var spec = new Spec();
            spec.AddStepDefinition(File.OpenText("Demo.spc.md"));
            spec.AddStepImplementationAssembly(Assembly.GetExecutingAssembly());
            foreach (var result in spec.Run())
            {
                Console.WriteLine(result);
            }
        }
    }
}