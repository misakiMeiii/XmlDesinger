using System.Xml;
using UnityEngine;

namespace XmlDesigner
{
    public static class DesignerReader
    {
        public static RootElement ReadDesignFile(string filePath)
        {
            var rootElement = new RootElement();
            var doc = new XmlDocument();
            doc.Load(filePath);
            var rootNode = doc.SelectSingleNode("RootClass");
            if (rootNode == null)
            {
                Debug.LogError("读取错误，文件根节点必须为RootClass");
                return null;
            }

            foreach (XmlNode node in rootNode)
            {
                switch (node.Name)
                {
                    case "Name":
                        rootElement.Name = node.InnerText;
                        break;
                    case "NameSpace":
                        rootElement.NameSpace = node.InnerText;
                        break;
                    case "ChildElement":
                        rootElement.ChildElements.Add(node.CreateChildElement());
                        break;
                    case "CustomElement":
                        rootElement.CustomElements.Add(node.CreateCustomElement());
                        break;
                }
            }

            return rootElement;
        }

        private static CustomElement CreateCustomElement(this XmlNode node)
        {
            var customElement = new CustomElement();

            if (node.Attributes?["name"] != null)
            {
                customElement.Name = node.Attributes["name"].Value;
            }

            foreach (XmlNode childNode in node)
            {
                if (childNode.Name == "ChildElement")
                {
                    customElement.ChildElements.Add(childNode.CreateChildElement());
                }
            }

            return customElement;
        }

        private static ChildElement CreateChildElement(this XmlNode node)
        {
            var childElement = new ChildElement();

            if (node.Attributes?["name"] != null)
            {
                childElement.Name = node.Attributes["name"].Value;
            }

            if (node.Attributes?["isAttribute"] != null)
            {
                childElement.IsAttribute = node.Attributes["isAttribute"].Value.ToBool();
            }

            if (node.Attributes?["elementType"] != null)
            {
                childElement.ElementType = node.Attributes["elementType"].Value.ToEnum<ElementType>();
            }

            if (node.Attributes?["referenceType"] != null)
            {
                childElement.ReferenceType = node.Attributes["referenceType"].Value.ToEnum<BaseType>();
            }

            if (node.Attributes?["keyType"] != null)
            {
                childElement.KeyType = node.Attributes["keyType"].Value.ToEnum<KeyType>();
            }

            if (node.Attributes?["customType"] != null)
            {
                childElement.CustomType = node.Attributes["customType"].Value.ToInt();
            }

            if (node.Attributes?["defaultValue"] != null)
            {
                childElement.DefaultValue = node.Attributes["defaultValue"].Value;
            }

            return childElement;
        }
    }
}