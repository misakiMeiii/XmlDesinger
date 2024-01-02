using System.Collections.Generic;

namespace XmlDesigner
{
    public class ChildElement
    {
        public string Name;
        public ElementType ElementType;
        public bool IsAttribute;
        public List<ChildElement> ChildElements = new List<ChildElement>();
    }
}