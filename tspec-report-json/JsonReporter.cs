using System;
using System.IO;
using System.Text.Json;
using Tspec.Core;

namespace Tspec.Report.Json
{
    public class JsonReporter
    {
        private readonly Utf8JsonWriter _writer;

        public JsonReporter(Stream stream)
        {
            _writer = new Utf8JsonWriter(stream, new JsonWriterOptions
            {
                Indented = true
            });
        }

        public void StartReport()
        {
            _writer.WriteStartObject(); // { // root
            _writer.WriteStartArray("specs"); // [
        }

        public void StartSpec(string name)
        {
            _writer.WriteStartObject(); // { // spec
            _writer.WriteString("name", name);
            _writer.WriteStartArray("steps"); // [
        }
        
        public void AddResult(Result result)
        {
            _writer.WriteStartObject();
            _writer.WriteString("text", result.Text);
            _writer.WriteBoolean("success", result.Success);
            _writer.WriteString("exception", result.Exception?.ToString() ?? "");
            _writer.WriteEndObject();
        }

        public void EndSpec()
        {
            _writer.WriteEndArray(); // ] // steps
            _writer.WriteEndObject(); // } // spec
        }

        public void EndReport()
        {
            _writer.WriteEndArray(); // ] // specs
            _writer.WriteEndObject(); // } // root
            _writer.Flush();
        }
    }
}