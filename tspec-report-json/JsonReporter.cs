using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Tspec.Core;

namespace Tspec.Report.Json
{
    public class JsonReporter
    {
        private readonly Utf8JsonWriter _writer;
        private int _passed;
        private int _failed;
        private bool _currentSuccess;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public JsonReporter(Stream stream)
        {
            _writer = new Utf8JsonWriter(stream, new JsonWriterOptions
            {
                Indented = true
            });
        }

        public void StartReport()
        {
            _stopwatch.Restart();
            _writer.WriteStartObject(); // { // root
            _writer.WriteString("timestamp", DateTime.Now.ToString("u"));
            _writer.WriteStartArray("specs"); // [
        }

        public void StartSpec(string name)
        {
            _writer.WriteStartObject(); // { // spec
            _writer.WriteString("name", name);
            _writer.WriteStartArray("steps"); // [
            _currentSuccess = true;
        }
        
        public void AddResult(Result result)
        {
            if (_currentSuccess) 
                _currentSuccess = result.Success;
            _writer.WriteStartObject();
            _writer.WriteString("text", result.Text);
            _writer.WriteBoolean("success", result.Success);
            _writer.WriteString("exception", result.Exception?.ToString() ?? "");
            _writer.WriteEndObject();
        }

        public void EndSpec()
        {
            if (_currentSuccess)
                _passed++;
            else
                _failed++;
            _writer.WriteEndArray(); // ] // steps
            _writer.WriteEndObject(); // } // spec
        }

        public void EndReport()
        {
            _stopwatch.Stop();
            _writer.WriteEndArray(); // ] // specs
            _writer.WriteString("duration", _stopwatch.Elapsed.ToString());
            _writer.WriteNumber("passed", _passed);
            _writer.WriteNumber("failed", _failed);
            _writer.WriteEndObject(); // } // root
            _writer.Flush();
        }
    }
}