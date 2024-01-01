using System;

namespace CodeGenKit
{
    public class NamespaceCodeScope : CodeScope
    {
        private string NameSpace { get; set; }
        
        public NamespaceCodeScope(string ns)
        {
            NameSpace = ns;
        }

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            codeWriter.WriteLine(string.Format("namespace {0}", NameSpace));
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Namespace(this ICodeScope self, string ns,
            Action<NamespaceCodeScope> namespaceCodeScopeSetting)
        {
            var namespaceCodeScope = new NamespaceCodeScope(ns);
            namespaceCodeScopeSetting(namespaceCodeScope);
            self.Codes.Add(namespaceCodeScope);
            return self;
        }
    }
}