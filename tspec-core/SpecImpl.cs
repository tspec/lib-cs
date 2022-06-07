using System.Reflection;

namespace Tspec.Core
{
    public class SpecImpl
    {
        public MethodInfo Method { get; set; }
        public string Pattern { get; set; }
        public string Text { get; set; }
        
        public override string ToString()
        {
            return $"[{Method.DeclaringType?.FullName}::{Method.Name}({Pattern})]";
        }
    }
}