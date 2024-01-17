using System.Text;
using CodeGenKit;
using UnityEngine;

namespace XmlDesigner
{
    public static class CsExporter
    {
        public static void ExportDesignClass(string folderPath, RootElement rootElement)
        {
            var rootCode = new RootCode()
                .Using("System")
                .Using("System.Collections.Generic")
                .EmptyLine()
                .Namespace(rootElement.NameSpace, ns =>
                {
                    for (var index = 0; index < rootElement.CustomElements.Count; index++)
                    {
                        var customElement = rootElement.CustomElements[index];
                        ns.Class(customElement.Name, string.Empty, false, false, classScope =>
                        {
                            foreach (var childElement in customElement.ChildElements)
                            {
                                classScope.Custom(
                                    $"public {childElement.GetElementTypeName(rootElement)} {childElement.Name}{childElement.EndOfElement(rootElement)}");
                            }
                        });
                        if (index < rootElement.CustomElements.Count - 1)
                        {
                            ns.EmptyLine();
                        }
                    }
                });

            var stringBuilder = new StringBuilder();
            var codeWriter = new StringCodeWriter(stringBuilder);
            rootCode.Gen(codeWriter);
            Debug.Log(stringBuilder.ToString());
        }

        private static string GetElementTypeName(this ChildElement childElement, RootElement rootElement)
        {
            string elementStr;
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    elementStr = "string";
                    break;
                case ElementType.Bool:
                    elementStr = "bool";
                    break;
                case ElementType.Int:
                    elementStr = "int";
                    break;
                case ElementType.Double:
                    elementStr = "double";
                    break;
                case ElementType.Float:
                    elementStr = "float";
                    break;
                case ElementType.List:
                    elementStr = $"List<{childElement.GetReferenceTypeName(rootElement)}>";
                    break;
                case ElementType.Queue:
                    elementStr = $"Queue<{childElement.GetReferenceTypeName(rootElement)}>";
                    break;
                case ElementType.Stack:
                    elementStr = $"Stack<{childElement.GetReferenceTypeName(rootElement)}>";
                    break;
                case ElementType.HashSet:
                    elementStr = $"HashSet<{childElement.GetReferenceTypeName(rootElement)}>";
                    break;
                case ElementType.Dictionary:
                    elementStr =
                        $"Dictionary<{childElement.KeyType.ToString().ToLower()}, {childElement.GetReferenceTypeName(rootElement)}>";
                    break;
                case ElementType.Custom:
                    elementStr = rootElement.CustomElementNames[childElement.CustomType];
                    break;
                default:
                    elementStr = string.Empty;
                    break;
            }

            return elementStr;
        }


        private static string EndOfElement(this ChildElement childElement, RootElement rootElement)
        {
            string endStr;
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    endStr = string.IsNullOrEmpty(childElement.DefaultValue) ? ";" : $" = {childElement.DefaultValue};";
                    break;
                case ElementType.Bool:
                    endStr = childElement.DefaultValue != "True" ? ";" : " = true;";
                    break;
                case ElementType.Int:
                    endStr = childElement.DefaultValue.ToInt() == 0 ? ";" : $"= {childElement.DefaultValue};";
                    break;
                case ElementType.Double:
                    endStr = childElement.DefaultValue.ToDouble() == 0 ? ";" : $"= {childElement.DefaultValue};";
                    break;
                case ElementType.Float:
                    endStr = childElement.DefaultValue.ToFloat() == 0 ? ";" : $"= {childElement.DefaultValue};";
                    break;
                case ElementType.List:
                    endStr = $" = new List<{childElement.GetReferenceTypeName(rootElement)}>();";
                    break;
                case ElementType.Queue:
                    endStr = $" = new Queue<{childElement.GetReferenceTypeName(rootElement)}>();";
                    break;
                case ElementType.Stack:
                    endStr = $" = new Stack<{childElement.GetReferenceTypeName(rootElement)}>();";
                    break;
                case ElementType.HashSet:
                    endStr = $" = new HashSet<{childElement.GetReferenceTypeName(rootElement)}>();";
                    break;
                case ElementType.Dictionary:
                    endStr =
                        $" = new Dictionary<{childElement.KeyType.ToString().ToLower()}, {childElement.GetReferenceTypeName(rootElement)}>();";
                    break;
                case ElementType.Custom:
                    endStr = $" = new {rootElement.CustomElementNames[childElement.CustomType]}();";
                    break;
                default:
                    endStr = string.Empty;
                    break;
            }

            return endStr;
        }

        private static string GetReferenceTypeName(this ChildElement childElement, RootElement rootElement)
        {
            if (childElement.ReferenceType != BaseType.Custom)
            {
                return childElement.ReferenceType.ToString().ToLower();
            }

            if (childElement.CustomType >= 0 && childElement.CustomType < rootElement.CustomElementNames.Length)
            {
                return rootElement.CustomElementNames[childElement.CustomType];
            }

            return string.Empty;
        }
    }
}