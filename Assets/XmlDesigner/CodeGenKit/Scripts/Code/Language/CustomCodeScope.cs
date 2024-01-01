using System;

namespace CodeGenKit
{
    public class CustomCodeScope : CodeScope
    {
        private string FirstLine { get; set; }

        public CustomCodeScope(string firstLine)
        {
            FirstLine = firstLine;
        }
        
        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            codeWriter.WriteLine(FirstLine);
        }
    }
    
    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope CustomScope(this ICodeScope self,string firstLine,bool semicolon, Action<CustomCodeScope> customCodeScopeSetting)
        {
            var custom = new CustomCodeScope(firstLine);
            custom.Semicolon = semicolon;
            customCodeScopeSetting(custom);
            self.Codes.Add(custom);
            return self;
        }
    }
}