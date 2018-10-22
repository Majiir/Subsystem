using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Subsystem
{
    public class StringLogger
    {
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        public StringLogger()
        {
            scopes.Push(new Scope());
        }

        public IDisposable BeginScope(string name)
        {
            var scope = scopes.Peek();
            var newScope = scope.CreateScope(name);
            scopes.Push(newScope);
            return new ScopeDisposer(this);
        }

        public void Log(string log)
        {
            scopes.Peek().AddLog(log);
        }

        public string GetLog()
        {
            var writer = new StringWriter();
            WriteLog(writer);
            return writer.ToString();
        }

        public void WriteLog(TextWriter writer)
        {
            writeLog(writer, scopes.Peek(), indent: 0);
        }

        private void writeLog(TextWriter writer, Scope scope, int indent)
        {
            foreach (var logEntry in scope.LogEntries)
            {
                writeIndent(writer, indent);
                writer.WriteLine(logEntry.Log);
            }

            if (scope.LogEntries.Any())
            {
                writer.WriteLine();
            }

            foreach (var kvp in scope.Scopes)
            {
                var childScopeName = kvp.Key;
                var childScope = kvp.Value;

                writeIndent(writer, indent);
                writer.WriteLine(childScopeName);
                writer.WriteLine();

                writeLog(writer, childScope, indent + 1);
            }
        }

        private static void writeIndent(TextWriter writer, int indent)
        {
            for (var i = 0; i < indent; i++)
            {
                writer.Write("  ");
            }
        }

        private class ScopeDisposer : IDisposable
        {
            private readonly StringLogger logger;

            public ScopeDisposer(StringLogger logger)
            {
                this.logger = logger;
            }

            public void Dispose()
            {
                logger.scopes.Pop();
            }
        }

        private class Scope
        {
            public IList<KeyValuePair<string, Scope>> Scopes
            {
                get { return new ReadOnlyCollection<KeyValuePair<string, Scope>>(scopes); }
            }

            public IList<LogEntry> LogEntries
            {
                get { return new ReadOnlyCollection<LogEntry>(logEntries); }
            }

            private readonly List<KeyValuePair<string, Scope>> scopes = new List<KeyValuePair<string, Scope>>();
            private readonly List<LogEntry> logEntries = new List<LogEntry>();

            public void AddLog(string log)
            {
                logEntries.Add(new LogEntry(log));
            }

            public Scope CreateScope(string name)
            {
                var scope = new Scope();
                scopes.Add(new KeyValuePair<string, Scope>(name, scope));
                return scope;
            }
        }

        private class LogEntry
        {
            public string Log { get; private set; }

            public LogEntry(string log)
            {
                Log = log;
            }
        }
    }
}
