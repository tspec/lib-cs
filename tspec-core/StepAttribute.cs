using System;

namespace Tspec.Core
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