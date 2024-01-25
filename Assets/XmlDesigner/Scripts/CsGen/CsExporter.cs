using System;
using System.IO;
using System.Linq;
using CodeGenKit;

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
                    //主类
                    ns.Class(rootElement.Name, string.Empty, false, false, classScope =>
                    {
                        foreach (var childElement in rootElement.ChildElements)
                        {
                            classScope.Custom(
                                $"public {childElement.GetElementTypeName(rootElement)} {childElement.Name}{childElement.EndOfElement(rootElement)}");
                        }
                    });

                    //自定义类
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

            var filePath = folderPath + $"/{rootElement.Name}DataView.cs";
            var writer = File.CreateText(filePath);
            var codeWriter = new FileCodeWriter(writer);
            rootCode.Gen(codeWriter);
            codeWriter.Dispose();
        }

        public static void ExportSerializedClass(string folderPath, RootElement rootElement)
        {
            var rootCode = new RootCode()
                .Using("System")
                .Using("System.Collections.Generic")
                .Using("System.Xml")
                .Using("System.IO")
                .Using("UnityEngine")
                .EmptyLine()
                .Namespace(rootElement.NameSpace,
                    ns =>
                    {
                        ns.Class(rootElement.Name + "DataManager", string.Empty, false, false,
                            classScope =>
                            {
                                //从路径读取
                                classScope.CustomScope($"public {rootElement.Name} ReadFromFile(string filePath)",
                                    false,
                                    function =>
                                    {
                                        function.CustomScope("if (File.Exists(filePath))", false, ccs =>
                                        {
                                            ccs.Custom("var doc = new XmlDocument();");
                                            ccs.Custom("doc.Load(filePath);");
                                            ccs.Custom($"var rootNode = doc.SelectSingleNode(\"{rootElement.Name}\");");
                                            ccs.CustomScope("if (rootNode == null)", false, cs =>
                                            {
                                                cs.Custom($"Debug.LogError(\"找不到根节点:{rootElement.Name}\");");
                                                cs.Custom("return null;");
                                            });
                                            ccs.Custom($"return Get{rootElement.Name}Data(rootNode);");
                                        });
                                        function.Custom("Debug.LogError(\"找不到文件,路径:\" + filePath);");
                                        function.Custom("return null;");
                                    });
                                classScope.EmptyLine();
                                //从string中读取
                                classScope.CustomScope($"public {rootElement.Name} ReadFromString(string content)",
                                    false,
                                    function =>
                                    {
                                        function.CustomScope("if (!string.IsNullOrEmpty(content))", false, ccs =>
                                        {
                                            ccs.Custom("var doc = new XmlDocument();");
                                            ccs.Custom("doc.LoadXml(content);");
                                            ccs.Custom($"var rootNode = doc.SelectSingleNode(\"{rootElement.Name}\");");
                                            ccs.CustomScope("if (rootNode == null)", false, cs =>
                                            {
                                                cs.Custom($"Debug.LogError(\"找不到根节点:{rootElement.Name}\");");
                                                cs.Custom("return null;");
                                            });
                                            ccs.Custom($"return Get{rootElement.Name}Data(rootNode);");
                                        });
                                        function.Custom("Debug.LogError(\"读取的内容为空,请检查！\");");
                                        function.Custom("return null;");
                                    });
                                classScope.EmptyLine();
                                //类解析
                                CreateClassSerialize(classScope, rootElement);
                            });
                    });


            var filePath = folderPath + $"/{rootElement.Name}DataManager.cs";
            var writer = File.CreateText(filePath);
            var codeWriter = new FileCodeWriter(writer);
            rootCode.Gen(codeWriter);
            codeWriter.Dispose();
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

        private static void CreateClassSerialize(ICodeScope classScope, RootElement rootElement)
        {
            classScope.CustomScope($"public {rootElement.Name} Get{rootElement.Name}Data(XmlNode node)", false,
                function =>
                {
                    function.Custom(
                        $"var {rootElement.Name.LowerFirstLetter()} = new {rootElement.Name}();");
                    if (rootElement.ChildElements.Any(element => element.IsAttribute)) //读取属性部分
                    {
                        function.Custom("XmlAttribute attr = null;");
                        foreach (var childElement in rootElement.ChildElements.Where(
                                     element => 
                                         element.IsAttribute))
                        {
                            function.Custom(
                                $"attr = node.Attributes[\"{childElement.Name}\"];");
                            function.CustomScope("if (attr != null)", false,
                                css =>
                                {
                                    css.Custom(childElement.AttributeElementSerialize(
                                        rootElement));
                                });
                        }
                    }

                    if (rootElement.ChildElements.Any(element => !element.IsAttribute)) //读取节点部分
                    {
                        function.CustomScope("foreach (XmlNode childNode in node)", false,
                            scope =>
                            {
                                scope.CustomScope("switch (childNode.Name)", false,
                                    ccs =>
                                    {
                                        foreach (var childElement in rootElement.ChildElements
                                                     .Where(
                                                         element =>
                                                             !element.IsAttribute))
                                        {
                                            ccs.Custom($"case \"{childElement.Name}\":");
                                            ccs.NodeElementSerialize(childElement, rootElement,
                                                rootElement);
                                            ccs.TabCustom("break;");
                                        }
                                    });
                            });
                    }

                    function.Custom($"return {rootElement.Name.LowerFirstLetter()};");
                });

            foreach (var customElement in rootElement.CustomElements)
            {
                classScope.CustomScope(
                    $"public {customElement.Name} Get{customElement.Name}Data(XmlNode node)", false,
                    function =>
                    {
                        function.Custom(
                            $"var {customElement.Name.LowerFirstLetter()} = new {customElement.Name}();");
                        if (customElement.ChildElements.Any(element => element.IsAttribute)) //读取属性部分
                        {
                            function.Custom("XmlAttribute attr = null;");
                            foreach (var childElement in customElement.ChildElements.Where(
                                         element =>
                                             element.IsAttribute))
                            {
                                function.Custom(
                                    $"attr = node.Attributes[\"{childElement.Name}\"];");
                                function.CustomScope("if (attr != null)", false,
                                    css =>
                                    {
                                        css.Custom(childElement.AttributeElementSerialize(
                                            customElement));
                                    });
                            }
                        }

                        if (customElement.ChildElements.Any(element => !element.IsAttribute)) //读取节点部分
                        {
                            function.CustomScope("foreach (XmlNode childNode in node)", false,
                                scope =>
                                {
                                    scope.CustomScope("switch (childNode.Name)", false,
                                        ccs =>
                                        {
                                            foreach (var childElement in customElement.ChildElements
                                                         .Where(
                                                             element =>
                                                                 !element.IsAttribute))
                                            {
                                                ccs.Custom($"case \"{childElement.Name}\":");
                                                ccs.NodeElementSerialize(childElement, customElement,
                                                    rootElement);
                                                ccs.TabCustom("break;");
                                            }
                                        });
                                });
                        }

                        function.Custom($"return {customElement.Name.LowerFirstLetter()};");
                    });
            }
        }

        private static string AttributeElementSerialize(this ChildElement childElement, AbstractElement parentElement)
        {
            string elementStr;
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    elementStr = $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = attr.Value;";
                    break;
                case ElementType.Bool:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToBoolean(attr.Value);;";
                    break;
                case ElementType.Int:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToInt32(attr.Value);;";
                    break;
                case ElementType.Double:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToDouble(attr.Value);";
                    break;
                case ElementType.Float:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToSingle(attr.Value);";
                    break;
                default:
                    return string.Empty;
            }

            return elementStr;
        }

        private static void NodeElementSerialize(this ICodeScope self, ChildElement childElement,
            AbstractElement parentElement,
            RootElement rootElement)
        {
            var elementStr = string.Empty;
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    elementStr = $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = childNode.InnerXml;";
                    break;
                case ElementType.Bool:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToBoolean(childNode.InnerXml);";
                    break;
                case ElementType.Int:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToInt32(childNode.InnerXml);";
                    break;
                case ElementType.Double:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToDouble(childNode.InnerXml);";
                    break;
                case ElementType.Float:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Convert.ToSingle(childNode.InnerXml);";
                    break;
                case ElementType.List:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(childNode.InnerXml);";
                            break;
                        case BaseType.Bool:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToBoolean(childNode.InnerXml));";
                            break;
                        case BaseType.Int:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToInt32(childNode.InnerXml));";
                            break;
                        case BaseType.Double:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToDouble(childNode.InnerXml));";
                            break;
                        case BaseType.Float:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToSingle(childNode.InnerXml));";
                            break;
                        case BaseType.Custom:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.Queue:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(childNode.InnerXml);";
                            break;
                        case BaseType.Bool:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(Convert.ToBoolean(childNode.InnerXml));";
                            break;
                        case BaseType.Int:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(Convert.ToInt32(childNode.InnerXml));";
                            break;
                        case BaseType.Double:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(Convert.ToDouble(childNode.InnerXml));";
                            break;
                        case BaseType.Float:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(Convert.ToSingle(childNode.InnerXml));";
                            break;
                        case BaseType.Custom:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.Stack:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(childNode.InnerXml);";
                            break;
                        case BaseType.Bool:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(Convert.ToBoolean(childNode.InnerXml));";
                            break;
                        case BaseType.Int:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(Convert.ToInt32(childNode.InnerXml));";
                            break;
                        case BaseType.Double:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(Convert.ToDouble(childNode.InnerXml));";
                            break;
                        case BaseType.Float:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(Convert.ToSingle(childNode.InnerXml));";
                            break;
                        case BaseType.Custom:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.HashSet:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(childNode.InnerXml);";
                            break;
                        case BaseType.Bool:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToBoolean(childNode.InnerXml));";
                            break;
                        case BaseType.Int:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToInt32(childNode.InnerXml));";
                            break;
                        case BaseType.Double:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToDouble(childNode.InnerXml));";
                            break;
                        case BaseType.Float:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Convert.ToSingle(childNode.InnerXml));";
                            break;
                        case BaseType.Custom:
                            elementStr =
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.Dictionary:

                    string keyContent;
                    switch (childElement.KeyType)
                    {
                        case KeyType.String:
                            keyContent =
                                $"childNode.SelectSingleNode(\"{childElement.Name}Key\").InnerText;";
                            break;
                        case KeyType.Int:
                            keyContent =
                                $"Convert.ToInt32(childNode.SelectSingleNode(\"{childElement.Name}Key\").InnerText);";
                            break;
                        case KeyType.Double:
                            keyContent =
                                $"Convert.ToDouble(childNode.SelectSingleNode(\"{childElement.Name}Key\").InnerText);";
                            break;
                        case KeyType.Float:
                            keyContent =
                                $"Convert.ToSingle(childNode.SelectSingleNode(\"{childElement.Name}Key\").InnerText);";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    string valueContent;
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            valueContent = $"childNode.SelectSingleNode(\"{childElement.Name}Value\").InnerText;";
                            break;
                        case BaseType.Bool:
                            valueContent =
                                $"Convert.ToBoolean(childNode.SelectSingleNode(\"{childElement.Name}Value\").InnerText);";
                            break;
                        case BaseType.Int:
                            valueContent =
                                $"Convert.ToInt32(childNode.SelectSingleNode(\"{childElement.Name}Value\").InnerText);";
                            break;
                        case BaseType.Double:
                            valueContent =
                                $"Convert.ToDouble(childNode.SelectSingleNode(\"{childElement.Name}Value\").InnerText);";
                            break;
                        case BaseType.Float:
                            valueContent =
                                $"Convert.ToSingle(childNode.SelectSingleNode(\"{childElement.Name}Value\").InnerText);";
                            break;
                        case BaseType.Custom:
                            valueContent =
                                $"Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode.SelectSingleNode(\"{childElement.Name}Value\"));";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    self.TabCustom($"var {childElement.Name}Key = {keyContent}");
                    self.TabCustom($"var {childElement.Name}Value = {valueContent}");
                    self.TabCustom(
                        $"if ({parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ContainsKey({childElement.Name}Key))");
                    self.TabCustom("{");
                    self.TabCustom($"\tDebug.LogError(\"key重复无法插入!key:\"+{childElement.Name}Key);");
                    self.TabCustom("}");
                    self.TabCustom("else");
                    self.TabCustom("{");
                    self.TabCustom(
                        $"\t{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add({childElement.Name}Key, {childElement.Name}Value);");
                    self.TabCustom("}");

                    break;
                case ElementType.Custom:
                    elementStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode);";
                    break;
                default:
                    return;
            }

            if (childElement.ElementType != ElementType.Dictionary)
            {
                self.TabCustom(elementStr);
            }
        }
    }
}