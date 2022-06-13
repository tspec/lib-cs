using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Tspec.Core;

namespace tspec_example
{
    public class Demo2Steps
    {
        [Step("Hello <message>")]
        public void StepHello(string message)
        {
            Console.WriteLine(message);
        }

        [Step("Step table <table>")]
        public void StepTable(Table table)
        {
            Console.WriteLine(table);
        }
    }
}