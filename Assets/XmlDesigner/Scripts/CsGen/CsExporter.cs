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
                                classScope.CustomScope(
                                    $"public static {rootElement.Name} ReadFromFile(string filePath)",
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
                                classScope.CustomScope(
                                    $"public static {rootElement.Name} ReadFromString(string content)",
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

                                //写入xml
                                classScope.CustomScope(
                                    $"public static void ExportXml({rootElement.Name} {rootElement.Name.LowerFirstLetter()}, string exportFolder, string xmlName)",
                                    false,
                                    function =>
                                    {
                                        function.CustomScope($"if ({rootElement.Name.LowerFirstLetter()} == null)",
                                            false,
                                            ccs =>
                                            {
                                                ccs.Custom(
                                                    $"Debug.LogError(\"导出失败,{rootElement.Name.LowerFirstLetter()}为空,请检查！\");");
                                                ccs.Custom("return;");
                                            });
                                        function.Custom("var doc = new XmlDocument();")
                                            .Custom("doc.CreateXmlDeclaration(\"1.0\", \"utf - 8\", null);")
                                            .Custom(
                                                $"doc.AppendChild(GetNodeFrom{rootElement.Name}(doc, {rootElement.Name.LowerFirstLetter()}));")
                                            .Custom(
                                                "doc.Save(exportFolder + (xmlName.EndsWith(\".xml\") ? xmlName : xmlName + \".xml\"));");
                                    });

                                //类解析
                                CreateClassSerialize(classScope, rootElement);
                                //类写入
                                CreateClassWriter(classScope, rootElement);
                            });
                    });


            var filePath = folderPath + $"/{rootElement.Name}DataManager.cs";
            var writer = File.CreateText(filePath);
            var codeWriter = new FileCodeWriter(writer);
            rootCode.Gen(codeWriter);
            codeWriter.Dispose();
        }

        #region 解析方法

        private static string GetElementTypeName(this ChildElement childElement, RootElement rootElement)
        {
            string elementStr;
            switch (childElement.ElementType)
            {
                case <= ElementType.Float:
                    elementStr = childElement.ElementType.ToString().ToLower();
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
                    endStr = string.IsNullOrEmpty(childElement.DefaultValue)
                        ? ";"
                        : $" = \"{childElement.DefaultValue}\";";
                    break;
                case ElementType.Bool:
                    endStr = childElement.DefaultValue != "True" ? ";" : " = true;";
                    break;
                case ElementType.Int:
                    endStr = childElement.DefaultValue.ToInt() == 0 ? ";" : $" = {childElement.DefaultValue};";
                    break;
                case ElementType.Double:
                    endStr = childElement.DefaultValue.ToDouble() == 0 ? ";" : $" = {childElement.DefaultValue};";
                    break;
                case ElementType.Float:
                    endStr = childElement.DefaultValue.ToFloat() == 0 ? ";" : $" = {childElement.DefaultValue};";
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
            classScope.CustomScope($"private static {rootElement.Name} Get{rootElement.Name}Data(XmlNode node)", false,
                function =>
                {
                    function.Custom(
                        $"var {rootElement.Name.LowerFirstLetter()} = new {rootElement.Name}();");
                    if (rootElement.ChildElements.Any(element => element.IsAttribute)) //读取属性部分
                    {
                        function.Custom("XmlAttribute attr;");
                        foreach (var childElement in rootElement.ChildElements.Where(
                                     element =>
                                         element.IsAttribute))
                        {
                            function.Custom(
                                $"attr = node.Attributes[\"{childElement.Name}\"];");
                            function.CustomScope("if (attr != null)", false,
                                css => { css.AttributeElementSerialize(childElement, rootElement); });
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
                    $"private static {customElement.Name} Get{customElement.Name}Data(XmlNode node)", false,
                    function =>
                    {
                        function.Custom(
                            $"var {customElement.Name.LowerFirstLetter()} = new {customElement.Name}();");
                        if (customElement.ChildElements.Any(element => element.IsAttribute)) //读取属性部分
                        {
                            function.Custom("XmlAttribute attr;");
                            foreach (var childElement in customElement.ChildElements.Where(
                                         element =>
                                             element.IsAttribute))
                            {
                                function.Custom(
                                    $"attr = node.Attributes[\"{childElement.Name}\"];");
                                function.CustomScope("if (attr != null)", false,
                                    css => { css.AttributeElementSerialize(childElement, customElement); });
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

        private static void AttributeElementSerialize(this ICodeScope self, ChildElement childElement,
            AbstractElement parentElement)
        {
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    self.Custom($"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = attr.Value;");
                    break;
                case >= ElementType.Bool and < ElementType.List:
                    self.Custom(
                            $"if(!{childElement.ElementType.ToString().ToLower()}.TryParse(attr.Value, out {parentElement.Name.LowerFirstLetter()}.{childElement.Name}))")
                        .TabCustom(
                            $"Debug.LogError($\"无法将 {childElement.Name} 转换为 {childElement.ElementType.ToString().ToLower()}: {{attr.Value}}. 出错节点: {{node.Name}}, 完整 XML: {{node.OuterXml}}\");");

                    break;
                case >= ElementType.List:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void NodeElementSerialize(this ICodeScope self, ChildElement childElement,
            AbstractElement parentElement,
            RootElement rootElement)
        {
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    self.TabCustom(
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = childNode.InnerXml;");
                    break;
                case >= ElementType.Bool and < ElementType.List:
                    self.TabCustom(
                            $"if(!{childElement.ElementType.ToString().ToLower()}.TryParse(childNode.InnerXml, out {parentElement.Name.LowerFirstLetter()}.{childElement.Name}))")
                        .TabCustom(
                            $"\tDebug.LogError($\"无法将 {childElement.Name} 转换为 {childElement.ElementType.ToString().ToLower()}: {{childNode.InnerXml}}. 出错节点: {{childNode.Name}}, 完整 XML: {{childNode.OuterXml}}\");")
                        ;
                    break;
                case ElementType.List:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(childNode.InnerXml);");
                            break;
                        case >= BaseType.Bool and < BaseType.Custom:
                            self.TabCustom(
                                    $"if(!{childElement.ReferenceType.ToString().ToLower()}.TryParse(childNode.InnerXml, out var {childElement.Name.LowerFirstLetter()}))")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\tDebug.LogError($\"无法将 {childElement.Name} 转换为 {childElement.ReferenceType.ToString().ToLower()}: {{childNode.InnerXml}}. 出错节点: {{childNode.Name}}, 完整 XML: {{childNode.OuterXml}}\");")
                                .TabCustom("}")
                                .TabCustom("else")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\t{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add({childElement.Name.LowerFirstLetter()});")
                                .TabCustom("}");
                            break;
                        case BaseType.Custom:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.Queue:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(childNode.InnerXml);");
                            break;
                        case >= BaseType.Bool and < BaseType.Custom:
                            self.TabCustom(
                                    $"if(!{childElement.ReferenceType.ToString().ToLower()}.TryParse(childNode.InnerXml, out var {childElement.Name.LowerFirstLetter()}))")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\tDebug.LogError($\"无法将 {childElement.Name} 转换为 {childElement.ReferenceType.ToString().ToLower()}: {{childNode.InnerXml}}. 出错节点: {{childNode.Name}}, 完整 XML: {{childNode.OuterXml}}\");")
                                .TabCustom("}")
                                .TabCustom("else")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\t{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue({childElement.Name.LowerFirstLetter()});")
                                .TabCustom("}");
                            break;
                        case BaseType.Custom:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Enqueue(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.Stack:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(childNode.InnerXml);");
                            break;
                        case >= BaseType.Bool and < BaseType.Custom:
                            self.TabCustom(
                                    $"if(!{childElement.ReferenceType.ToString().ToLower()}.TryParse(childNode.InnerXml, out var {childElement.Name.LowerFirstLetter()}))")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\tDebug.LogError($\"无法将 {childElement.Name} 转换为 {childElement.ReferenceType.ToString().ToLower()}: {{childNode.InnerXml}}. 出错节点: {{childNode.Name}}, 完整 XML: {{childNode.OuterXml}}\");")
                                .TabCustom("}")
                                .TabCustom("else")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\t{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push({childElement.Name.LowerFirstLetter()});")
                                .TabCustom("}");
                            break;
                        case BaseType.Custom:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Push(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ElementType.HashSet:
                    switch (childElement.ReferenceType)
                    {
                        case BaseType.String:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(childNode.InnerXml);");
                            break;
                        case >= BaseType.Bool and < BaseType.Custom:
                            self.TabCustom(
                                    $"if(!{childElement.ReferenceType.ToString().ToLower()}.TryParse(childNode.InnerXml, out var {childElement.Name.LowerFirstLetter()}))")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\tDebug.LogError($\"无法将 {childElement.Name} 转换为 {childElement.ReferenceType.ToString().ToLower()}: {{childNode.InnerXml}}. 出错节点: {{childNode.Name}}, 完整 XML: {{childNode.OuterXml}}\");")
                                .TabCustom("}")
                                .TabCustom("else")
                                .TabCustom("{")
                                .TabCustom(
                                    $"\t{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add({childElement.Name.LowerFirstLetter()});")
                                .TabCustom("}");
                            break;
                        case BaseType.Custom:
                            self.TabCustom(
                                $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add(Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode));");
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

                    self.TabCustom($"var {childElement.Name.LowerFirstLetter()}Key = {keyContent}");
                    self.TabCustom($"var {childElement.Name.LowerFirstLetter()}Value = {valueContent}");
                    self.TabCustom(
                        $"if ({parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ContainsKey({childElement.Name.LowerFirstLetter()}Key))");
                    self.TabCustom("{");
                    self.TabCustom(
                        $"\tDebug.LogError(\"key重复无法插入!key:\" + {childElement.Name.LowerFirstLetter()}Key);");
                    self.TabCustom("}");
                    self.TabCustom("else");
                    self.TabCustom("{");
                    self.TabCustom(
                        $"\t{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.Add({childElement.Name.LowerFirstLetter()}Key, {childElement.Name.LowerFirstLetter()}Value);");
                    self.TabCustom("}");

                    break;
                case ElementType.Custom:
                    self.TabCustom(
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name} = Get{rootElement.CustomElementNames[childElement.CustomType]}Data(childNode);");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 导出xml方法

        private static void CreateClassWriter(ICodeScope classScope, RootElement rootElement)
        {
            classScope.CustomScope(
                $"private static XmlNode GetNodeFrom{rootElement.Name}(XmlDocument doc, {rootElement.Name} {rootElement.Name.LowerFirstLetter()})",
                false,
                function =>
                {
                    //根节点
                    function.Custom(
                            $"var {rootElement.Name.LowerFirstLetter()}Node = doc.CreateElement(\"{rootElement.Name}\");")
                        .EmptyLine();
                    foreach (var childElement in rootElement.ChildElements.Where(
                                 element =>
                                     element.IsAttribute))
                    {
                        function.Custom(
                                $"var {childElement.Name.LowerFirstLetter()}Ab = doc.CreateAttribute(\"{childElement.Name}\");")
                            .Custom(
                                $"{childElement.Name.LowerFirstLetter()}Ab.InnerText = {childElement.EndOfAttributeElement(rootElement)}")
                            .Custom(
                                $"{rootElement.Name.LowerFirstLetter()}Node.SetAttributeNode({childElement.Name.LowerFirstLetter()}Ab);")
                            .EmptyLine();
                    }

                    foreach (var childElement in rootElement.ChildElements.Where(
                                 element =>
                                     !element.IsAttribute))
                    {
                        function.EndOfNodeElement(childElement, rootElement, rootElement).EmptyLine();
                    }

                    function.Custom($"return {rootElement.Name.LowerFirstLetter()}Node;");
                });

            foreach (var customElement in rootElement.CustomElements)
            {
                classScope.CustomScope(
                    $"private static XmlNode GetNodeFrom{customElement.Name}(XmlDocument doc, {customElement.Name} {customElement.Name.LowerFirstLetter()})",
                    false,
                    function =>
                    {
                        function.Custom(
                                $"var {customElement.Name.LowerFirstLetter()}Node = doc.CreateElement(\"{customElement.Name}\");")
                            .EmptyLine();
                        foreach (var childElement in customElement.ChildElements.Where(
                                     element =>
                                         element.IsAttribute))
                        {
                            function.Custom(
                                    $"var {childElement.Name.LowerFirstLetter()}Ab = doc.CreateAttribute(\"{childElement.Name}\");")
                                .Custom(
                                    $"{childElement.Name.LowerFirstLetter()}Ab.InnerText = {childElement.EndOfAttributeElement(customElement)}")
                                .Custom(
                                    $"{customElement.Name.LowerFirstLetter()}Node.SetAttributeNode({childElement.Name.LowerFirstLetter()}Ab);")
                                .EmptyLine();
                        }

                        foreach (var childElement in customElement.ChildElements.Where(
                                     element =>
                                         !element.IsAttribute))
                        {
                            function.EndOfNodeElement(childElement, customElement, rootElement).EmptyLine();
                        }

                        function.Custom($"return {customElement.Name.LowerFirstLetter()}Node;");
                    });
            }
        }

        private static string EndOfAttributeElement(this ChildElement childElement, AbstractElement parentElement)
        {
            string endStr;
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    endStr = $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name};";
                    break;
                case ElementType.Bool:
                    endStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ToString();";
                    break;
                case ElementType.Int:
                    endStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ToString();";
                    break;
                case ElementType.Double:
                    endStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ToString();";
                    break;
                case ElementType.Float:
                    endStr =
                        $"{parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ToString();";
                    break;
                case ElementType.List:
                    endStr = "\"\";";
                    break;
                case ElementType.Queue:
                    endStr = "\"\";";
                    break;
                case ElementType.Stack:
                    endStr = "\"\";";
                    break;
                case ElementType.HashSet:
                    endStr = "\"\";";
                    break;
                case ElementType.Dictionary:
                    endStr = "\"\";";
                    break;
                case ElementType.Custom:
                    endStr = "\"\";";
                    break;
                default:
                    endStr = string.Empty;
                    break;
            }

            return endStr;
        }

        private static ICodeScope EndOfNodeElement(this ICodeScope self, ChildElement childElement,
            AbstractElement parentElement, RootElement rootElement)
        {
            switch (childElement.ElementType)
            {
                case ElementType.String:
                    self.Custom(
                            $"var {childElement.Name.LowerFirstLetter()}Node = doc.CreateElement(\"{childElement.Name}\");")
                        .Custom(
                            $"{childElement.Name.LowerFirstLetter()}Node.InnerText = {parentElement.Name.LowerFirstLetter()}.{childElement.Name};")
                        .Custom(
                            $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild({childElement.Name.LowerFirstLetter()}Node);");
                    break;
                case >= ElementType.Bool and <= ElementType.Float:
                    self.Custom(
                            $"var {childElement.Name.LowerFirstLetter()}Node = doc.CreateElement(\"{childElement.Name}\");")
                        .Custom(
                            $"{childElement.Name.LowerFirstLetter()}Node.InnerText = {parentElement.Name.LowerFirstLetter()}.{childElement.Name}.ToString();")
                        .Custom(
                            $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild({childElement.Name.LowerFirstLetter()}Node);");
                    break;
                case >= ElementType.List and <= ElementType.HashSet:
                    self.CustomScope(
                        $"foreach (var {childElement.Name.LowerFirstLetter()}Value in {parentElement.Name.LowerFirstLetter()}.{childElement.Name})",
                        false, ccs =>
                        {
                            switch (childElement.ReferenceType)
                            {
                                case BaseType.String:
                                    ccs.Custom(
                                            $"var {childElement.Name.LowerFirstLetter()}Node = doc.CreateElement(\"{childElement.Name}\");")
                                        .Custom(
                                            $"{childElement.Name.LowerFirstLetter()}Node.InnerText = {childElement.Name.LowerFirstLetter()}Value;")
                                        .Custom(
                                            $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild({childElement.Name.LowerFirstLetter()}Node);");
                                    break;
                                case > BaseType.String and < BaseType.Custom:
                                    ccs.Custom(
                                            $"var {childElement.Name.LowerFirstLetter()}Node = doc.CreateElement(\"{childElement.Name}\");")
                                        .Custom(
                                            $"{childElement.Name.LowerFirstLetter()}Node.InnerText = {childElement.Name.LowerFirstLetter()}Value.ToString();")
                                        .Custom(
                                            $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild({childElement.Name.LowerFirstLetter()}Node);");
                                    break;
                                case BaseType.Custom:
                                    ccs.Custom(
                                        $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild(GetNodeFrom{rootElement.CustomElementNames[childElement.CustomType]}(doc, {childElement.Name.LowerFirstLetter()}Value));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        });
                    break;
                case ElementType.Dictionary:
                    self.CustomScope(
                        $"foreach(var kvp in {parentElement.Name.LowerFirstLetter()}.{childElement.Name})",
                        false, ccs =>
                        {
                            ccs.Custom(
                                    $"var {childElement.Name.LowerFirstLetter()}KeyNode = doc.CreateElement(\"{childElement.Name}Key\");")
                                .Custom(
                                    $"{childElement.Name.LowerFirstLetter()}KeyNode.InnerText = kvp.Key" +
                                    (childElement.KeyType == KeyType.String
                                        ? ";"
                                        : ".ToString();"));

                            ccs.Custom(
                                $"var {childElement.Name.LowerFirstLetter()}ValueNode = doc.CreateElement(\"{childElement.Name}Value\");");
                            switch (childElement.ReferenceType)
                            {
                                case BaseType.String:
                                    ccs.Custom(
                                        $"{childElement.Name.LowerFirstLetter()}ValueNode.InnerText = kvp.Value;");
                                    break;
                                case > BaseType.String and < BaseType.Custom:
                                    ccs.Custom(
                                        $"{childElement.Name.LowerFirstLetter()}ValueNode.InnerText = kvp.Value");
                                    break;
                                case BaseType.Custom:
                                    ccs.Custom(
                                        $"{childElement.Name.LowerFirstLetter()}ValueNode.AppendChild(GetNodeFrom{rootElement.CustomElementNames[childElement.CustomType]}(doc, kvp.Value));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            ccs.Custom(
                                    $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild({childElement.Name.LowerFirstLetter()}KeyNode);")
                                .Custom(
                                    $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild({childElement.Name.LowerFirstLetter()}ValueNode);");
                        });

                    break;
                case ElementType.Custom:
                    self.Custom(
                        $"{parentElement.Name.LowerFirstLetter()}Node.AppendChild(GetNodeFrom{rootElement.CustomElementNames[childElement.CustomType]}(doc, {parentElement.Name.LowerFirstLetter()}.{childElement.Name}));");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return self;
        }

        #endregion
    }
}