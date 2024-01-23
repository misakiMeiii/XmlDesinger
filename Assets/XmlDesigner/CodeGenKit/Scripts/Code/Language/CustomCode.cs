namespace CodeGenKit
{
    /// <summary>
    /// 自定义代码生成带缩进
    /// </summary>
    public class CustomCode : ICode
    {
        private string _line;

        public CustomCode(string line)
        {
            _line = line;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine(_line);
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Custom(this ICodeScope self, string line)
        {
            self.Codes.Add(new CustomCode(line));
            return self;
        }
    }
}