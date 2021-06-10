using System;

namespace TSpec.Lib
{
    public class StepAttribute : Attribute
    {
        public StepAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; }
    }
}