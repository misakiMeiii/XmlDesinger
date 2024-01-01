using System;

namespace CodeGenKit
{
    public class ClassCodeScope : CodeScope
    {
        public ClassCodeScope(string className, string parentClassName, bool isPartial, bool isStatic)
        {
            _className = className;
            _parentClassName = parentClassName;
            _isPartial = isPartial;
            _isStatic = isStatic;
        }

        private string _className;
        private string _parentClassName;
        private bool _isPartial;
        private bool _isStatic;

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            var staticKey = _isStatic ? " static" : string.Empty;
            var partialKey = _isPartial ? " partial" : string.Empty;
            var parentClassKey = !string.IsNullOrEmpty(_parentClassName) ? " : " + _parentClassName : string.Empty;

            codeWriter.WriteLine(string.Format("public{0}{1} class {2}{3}", staticKey, partialKey, _className,
                parentClassKey));
        }
    }

    public static partial class CodeScopeExtensions
    {
        public static ICodeScope Class(this ICodeScope self, string className, string parentClassName, bool isPartial,
            bool isStatic, Action<ClassCodeScope> classCodeScopeSetting)
        {
            var classCodeScope = new ClassCodeScope(className, parentClassName, isPartial, isStatic);
            classCodeScopeSetting(classCodeScope);
            self.Codes.Add(classCodeScope);
            return self;
        }
    }
}
