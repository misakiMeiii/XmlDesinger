using System.Collections.Generic;

namespace CodeGenKit
{
    public abstract class CodeScope : ICodeScope
    {
        public bool Semicolon { get; set; }

        public void Gen(ICodeWriter writer)
        {
            GenFirstLine(writer);

            new OpenBraceCode().Gen(writer);

            writer.IndentCount++;

            foreach (var code in Codes)
            {
                code.Gen(writer);
            }

            writer.IndentCount--;

            new CloseBraceCode(Semicolon).Gen(writer);
        }

        protected abstract void GenFirstLine(ICodeWriter codeWriter);

        private List<ICode> _mCodes = new List<ICode>();

        public List<ICode> Codes
        {
            get { return _mCodes; }
            set { _mCodes = value; }
        }
    }
}