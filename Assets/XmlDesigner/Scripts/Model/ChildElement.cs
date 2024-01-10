using System.Collections.Generic;

namespace XmlDesigner
{
    public class ChildElement
    {
        public string Name;
        public bool IsAttribute;
        public ElementType ElementType;
        public BaseType ReferenceType;
        public KeyType KeyType;
        public int CustomType;
        public string DefaultValue;
    }
}