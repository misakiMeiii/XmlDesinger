using System.Collections.Generic;
using System.Linq;

namespace XmlDesigner
{
    public class RootElement : AbstractElement
    {
        public string NameSpace;
        public string[] CustomElementNames => CustomElements.Select(element => element.Name).ToArray();
        public List<ChildElement> ChildElements = new List<ChildElement>();
        public List<CustomElement> CustomElements = new List<CustomElement>();
    }
}