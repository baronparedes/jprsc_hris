﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Infrastructure.MediatR
{
    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/Runner.cs
    public class WrappingWriter : TextWriter
    {
        private readonly TextWriter _innerWriter;
        private readonly StringBuilder _stringWriter = new StringBuilder();

        public WrappingWriter(TextWriter innerWriter)
        {
            _innerWriter = innerWriter;
        }

        public override void Write(char value)
        {
            _stringWriter.Append(value);
            _innerWriter.Write(value);
        }

        public override Task WriteLineAsync(string value)
        {
            _stringWriter.AppendLine(value);
            return _innerWriter.WriteLineAsync(value);
        }

        public override Encoding Encoding => _innerWriter.Encoding;

        public string Contents => _stringWriter.ToString();
    }
}