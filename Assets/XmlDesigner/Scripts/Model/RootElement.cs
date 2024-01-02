using System.Collections.Generic;

namespace XmlDesigner
{
    public class RootElement
    {
        public string Name;
        public string NameSpace;
        public List<ChildElement> ChildElements = new List<ChildElement>();
        public List<CustomElement> CustomElements = new List<CustomElement>();
    }
}