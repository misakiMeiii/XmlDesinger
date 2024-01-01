using System.IO;
using System.Text;

namespace CodeGenKit
{
    public class FileCodeWriter : ICodeWriter
    {
        private readonly StreamWriter _mWriter;

        public FileCodeWriter(StreamWriter writer)
        {
            _mWriter = writer;
        }

        public int IndentCount { get; set; }

        private string Indent
        {
            get
            {
                var builder = new StringBuilder();

                for (var i = 0; i < IndentCount; i++)
                {
                    builder.Append("\t");
                }

                return builder.ToString();
            }
        }

        public void WriteFormatLine(string format, params object[] args)
        {
            _mWriter.WriteLine(Indent + format, args);
        }

        public void WriteLine(string code = null)
        {
            _mWriter.WriteLine(Indent + code);
        }

        public void Dispose()
        {
            _mWriter?.Dispose();
        }
    }
}