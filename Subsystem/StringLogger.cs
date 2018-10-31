using System;
using System.IO;

namespace Subsystem
{
    public class StringLogger
    {
        private bool wroteNewline = true;
        private int indent = 0;

        private readonly TextWriter writer;

        public StringLogger(TextWriter writer)
        {
            this.writer = writer;
        }

        public IDisposable BeginScope(string name)
        {
            if (!wroteNewline)
            {
                writer.WriteLine();
            }

            writeIndent(writer, indent);
            writer.WriteLine(name);
            writer.WriteLine();

            wroteNewline = true;
            indent += 1;

            return new ScopeDisposer(this);
        }

        public void Log(string log)
        {
            writeIndent(writer, indent);
            writer.WriteLine(log);

            wroteNewline = false;
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
                logger.indent -= 1;
            }
        }
    }
}
