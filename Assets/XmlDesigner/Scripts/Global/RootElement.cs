using System.Collections.Generic;

namespace XmlDesigner
{
    public class RootElement
    {
        public bool IsRoot;
        public string Name;
        public List<ChildElement> ChildElements = new List<ChildElement>();
    }
}