namespace CodeGenKit
{
    public class TabCustomCode : ICode
    {
        private string _line;

        public TabCustomCode(string line)
        {
            _line = line;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine("\t" + _line);
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope TabCustom(this ICodeScope self, string line)
        {
            self.Codes.Add(new TabCustomCode(line));
            return self;
        }
    }
}