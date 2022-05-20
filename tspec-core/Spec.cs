using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("tspec-test")]

namespace Tspec.Core
{
    public class Spec
    {
        private readonly List<SpecDef> _defs = new List<SpecDef>();
        private readonly List<SpecImpl> _impls = new List<SpecImpl>();
        private readonly Dictionary<Type, object> _objs = new Dictionary<Type, object>();

        public void AddStepImplementationAssembly(Assembly assembly)
        {
            _impls.AddRange(FindIn(assembly));
        }

        public void AddStepImplementation(object steps)
        {
            var type = steps.GetType();
            _objs[type] = steps;
            _impls.AddRange(FindIn(type));
        }

        public void AddStepDefinition(TextReader text)
        {
            _defs.AddRange(FindIn(text));
        }

        private IEnumerable<SpecImpl> FindIn(Type type)
        {
            foreach (var m in type.GetMethods())
            {
                var pattern = m.GetCustomAttribute<StepAttribute>()?.Pattern;
                if (string.IsNullOrWhiteSpace(pattern)) continue;
                foreach (var p in m.GetParameters())
                {
                    var t = p.ParameterType;

                    var exp = t == typeof(int) ||
                              t == typeof(long) ||
                              t == typeof(short) ||
                              t == typeof(byte) ||
                              t == typeof(float) ||
                              t == typeof(double) ? @"[\d\.]+"
                        : t == typeof(string) ? @"(?:[^""\\]|\\.)*"
                        : t == typeof(char) ? @"(?:\\""|[^""])"
                        : null;

                    if (exp != null)
                        pattern = pattern.Replace($"<{p.Name}>", $"\"(?<{p.Name}>{exp})\"");
                    else
                        pattern = pattern.Replace($"<{p.Name}>", "");

                    pattern = pattern.Trim();
                }

                yield return new SpecImpl
                {
                    Method = m,
                    Pattern = pattern
                };
            }
        }

        private IEnumerable<SpecImpl> FindIn(Assembly assembly)
        {
            return assembly.GetTypes().SelectMany(FindIn);
        }

        private IEnumerable<SpecDef> FindIn(TextReader text)
        {
            SpecDef current = null;
            while (true)
            {
                var line = text.ReadLine();
                if (line == null) break;

                // Any line starting with '*' is definition
                if (Regex.IsMatch(line, @"^\s*\*\s*.+"))
                {
                    current = new SpecDef { Text = Regex.Match(line, @"^\s*\*(.+?)$").Groups[1].Value.Trim() };
                    yield return current;
                }

                // Add any tables to the current spec
                if (current != null && Regex.IsMatch(line, @"^\s*\|"))
                {
                    var strings = line.Split('|');

                    // not a table
                    if (strings.Length < 3) continue;

                    // remove first and last elements and trim
                    var values = strings.Skip(1).Take(strings.Length - 2)
                        .Select(x => x.Trim()).ToArray();

                    // ignore table lines
                    if (values.All(s => s.All(c => c == '-'))) continue;

                    if (current.Table == null)
                        // No table yet. Assume header
                        current.Table = new Table { Columns = values };
                    else
                        // ..and the values
                        current.Table.AddRow(values);
                }
            }
        }

        public IEnumerable<Result> Run()
        {
            foreach (var specDef in _defs)
            {
                var step = _impls.FirstOrDefault(s => Regex.IsMatch(specDef.Text, s.Pattern));
                if (step == null)
                {
                    Console.WriteLine($"ERROR: Did not match: {specDef.Text}");
                    continue;
                }

                var match = Regex.Match(specDef.Text, step.Pattern);

                var t = step.Method.DeclaringType;
                if (!_objs.TryGetValue(t, out var obj)) _objs[t] = obj = Activator.CreateInstance(t);

                var parameters = new List<object>();

                foreach (var parameterInfo in step.Method.GetParameters())
                    if (parameterInfo.ParameterType == typeof(Table))
                    {
                        parameters.Add(specDef.Table);
                    }
                    else
                    {
                        var strValue = match.Groups[parameterInfo.Name].Value;
                        var value = Convert.ChangeType(strValue, parameterInfo.ParameterType);
                        parameters.Add(value);
                    }

                Exception exception = null;
                try
                {
                    step.Method.Invoke(obj, parameters.ToArray());
                }
                catch (Exception e)
                {
                    exception = e;
                }

                yield return new Result
                {
                    Success = exception == null,
                    Text = specDef.Text,
                    Exception = exception
                };
            }
        }

        public void Dump(TextWriter @out)
        {
            foreach (var impl in _impls) @out.WriteLine(impl);

            foreach (var def in _defs)
            {
                @out.WriteLine(def);
                if (def.Table != null) @out.WriteLine(def.Table);
            }
        }
    }
}