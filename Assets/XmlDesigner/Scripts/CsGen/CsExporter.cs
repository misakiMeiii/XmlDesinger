using CodeGenKit;

namespace XmlDesigner
{
    public static class CsExporter
    {
        public static void ExportDesignClass(string folderPath, RootElement rootElement)
        {
            var rootCode = new RootCode()
                .Using("System")
                .EmptyLine()
                .Namespace(rootElement.NameSpace , ns =>
                {
                    foreach (var customElement in rootElement.CustomElements)
                    {
                        ns.Class(customElement.Name, string.Empty, false, false, classScope =>
                        {
                            foreach (var childElement in customElement.ChildElements)
                            {
                                classScope.Custom("public ");
                            }
                        });
                    }
                });
        }

        // public static string GetElementTypeName()
        // {
        //     
        // }
    }
}