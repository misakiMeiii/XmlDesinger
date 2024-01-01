namespace CodeGenKit
{
    /// <summary>
    /// 引用代码生成
    /// </summary>
    public class UsingCode : ICode
    {
        private string _namespace { get; set; }

        public UsingCode(string ns)
        {
            _namespace = ns;
        }
        
        public void Gen(ICodeWriter writer)
        {
            writer.WriteFormatLine("using {0};", _namespace);
        }
    }
    
    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Using(this ICodeScope self,string ns)
        {
            self.Codes.Add(new UsingCode(ns));
            return self;
        }
    }
}