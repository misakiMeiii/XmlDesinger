using System.Text;
using System.Xml;
using UnityEngine;

namespace XmlDesigner
{
    public static class DesignerExporter
    {
        public static void ExportDesignFile(string filePath, RootElement rootElement)
        {
            var doc = new XmlDocument();
            doc.CreateXmlDeclaration("1.0", "utf - 8", null);

            if (rootElement == null)
            {
                Debug.LogError("内容为空，请检查！");
                return;
            }

            WriteRootNode(doc, rootElement);

            var xmlTextWriter = new XmlTextWriter(filePath, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            doc.Save(xmlTextWriter);
            xmlTextWriter.Close();
        }

        private static void WriteRootNode(XmlDocument doc, RootElement rootElement)
        {
            //创建根节点
            var rootNode = doc.CreateElement("RootClass");
            //名称
            var rootNameNode = doc.CreateElement("Name");
            rootNameNode.InnerText = rootElement.Name;
            rootNode.AppendChild(rootNameNode);
            //命名空间
            var rootNameSpaceNode = doc.CreateElement("NameSpace");
            rootNameSpaceNode.InnerText = rootElement.NameSpace;
            rootNode.AppendChild(rootNameSpaceNode);
            //根节点子元素
            foreach (var childElement in rootElement.ChildElements)
            {
                var childNode = childElement.CreateChildNode(doc);
                rootNode.AppendChild(childNode);
            }

            //自定义类
            foreach (var customElement in rootElement.CustomElements)
            {
                var customNode = doc.CreateElement("CustomElement");
                var nameAb = doc.CreateAttribute("name");
                nameAb.InnerText = customElement.Name;
                customNode.SetAttributeNode(nameAb);

                foreach (var childElement in customElement.ChildElements)
                {
                    var childNode = childElement.CreateChildNode(doc);
                    customNode.AppendChild(childNode);
                }

                rootNode.AppendChild(customNode);
            }

            //添加根节点
            doc.AppendChild(rootNode);
        }

        private static XmlNode CreateChildNode(this ChildElement childElement, XmlDocument doc)
        {
            var childNode = doc.CreateElement("ChildElement");
            
            var nameAb = doc.CreateAttribute("name");
            nameAb.InnerText = childElement.Name;
            childNode.SetAttributeNode(nameAb);

            var isAttributeAb = doc.CreateAttribute("isAttribute");
            isAttributeAb.InnerText = childElement.IsAttribute.ToString();
            childNode.SetAttributeNode(isAttributeAb);

            var elementTypeAb = doc.CreateAttribute("elementType");
            elementTypeAb.InnerText = childElement.ElementType.ToString();
            childNode.SetAttributeNode(elementTypeAb);

            var referenceTypeAb = doc.CreateAttribute("referenceType");
            referenceTypeAb.InnerText = childElement.ReferenceType.ToString();
            childNode.SetAttributeNode(referenceTypeAb);

            var keyTypeAb = doc.CreateAttribute("keyType");
            keyTypeAb.InnerText = childElement.KeyType.ToString();
            childNode.SetAttributeNode(keyTypeAb);

            var customTypeAb = doc.CreateAttribute("customType");
            customTypeAb.InnerText = childElement.CustomType.ToString();
            childNode.SetAttributeNode(customTypeAb);

            var defaultValueAb = doc.CreateAttribute("defaultValue");
            defaultValueAb.InnerText = childElement.DefaultValue;
            childNode.SetAttributeNode(defaultValueAb);

            return childNode;
        }
    }
}