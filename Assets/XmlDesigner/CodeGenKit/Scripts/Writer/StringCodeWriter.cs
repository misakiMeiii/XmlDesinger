using System.Text;

namespace CodeGenKit
{
    public class StringCodeWriter : ICodeWriter
    {
        private readonly StringBuilder _writer;

        public StringCodeWriter(StringBuilder writer)
        {
            _writer = writer;
        }

        public int IndentCount { get; set; }

        private string Indent
        {
            get
            {
                var builder = new StringBuilder();

                for (var i = 0; i < IndentCount; i++)
                {
                    builder.Append("\t"); //缩进
                }

                return builder.ToString();
            }
        }

        public void WriteFormatLine(string format, params object[] args)
        {
            _writer.AppendFormat(Indent + format, args).AppendLine();
        }

        public void WriteLine(string code = null)
        {
            _writer.AppendLine(Indent + code);
        }

        public void Dispose()
        {
            _writer?.Clear();
        }
    }
}