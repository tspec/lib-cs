using System.Reflection;

namespace TSpec.Lib
{
    public class SpecImpl
    {
        public MethodInfo Method { get; set; }
        public string Pattern { get; set; }
        
        public override string ToString()
        {
            return $"[{Method.DeclaringType?.FullName}::{Method.Name}({Pattern})]";
        }
    }
}