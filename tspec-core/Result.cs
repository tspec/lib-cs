using System;

namespace Tspec.Core
{
    public class Result
    {
        public bool Success { get; internal set; }
        public string Text { get; internal set; }
        public Exception Exception { get; internal set; }

        public override string ToString()
        {
            var r = Success ? "PASS" : "FAIL";
            var error = Exception == null ? string.Empty : $": {Exception.Message}";
            return $"{r} ({Text}){error}";
        }
    }
}