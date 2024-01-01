using System.Collections.Generic;

namespace CodeGenKit
{
    public interface ICodeScope : ICode
    {
        List<ICode> Codes { get; set; }
    }
}