using System.Collections.Generic;

namespace CodeGenKit
{
    public class RootCode : ICodeScope
    {
        private List<ICode> _codes = new List<ICode>();

        public List<ICode> Codes
        {
            get { return _codes; }
            set { _codes = value; }
        }


        public void Gen(ICodeWriter writer)
        {
            foreach (var code in Codes)
            {
                code.Gen(writer);
            }
        }
    }
}