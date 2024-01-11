using System.Text;
using System.Xml;
using UnityEngine;

namespace XmlDesigner
{
    public class DesignerExporter
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
            rootNameSpaceNode.InnerText = rootElement.Name;
            rootNode.AppendChild(rootNameSpaceNode);

            //添加根节点
            doc.AppendChild(rootNode);
        }
    }
}