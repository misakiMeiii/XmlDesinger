#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace XmlDesigner
{
    public class XmlDesignerWindow : EditorWindow
    {
        private static EditorWindow _xmlDesignerWindow;
        private static bool Opening { get; set; }
        private bool _isFinishedInit;

        private static readonly string WindowTitle = "xml设计/生成器";
        private static readonly Rect WindowRect = new Rect(50, 100, 1600, 600);

        [MenuItem("XmlDesigner/Tool &T", false, 1)]
        public static void OnClickWindow()
        {
            _xmlDesignerWindow = GetWindow<XmlDesignerWindow>();
            if (Opening)
            {
                _xmlDesignerWindow.Close();
                Opening = false;
            }
            else
            {
                _xmlDesignerWindow.titleContent = new GUIContent(WindowTitle);
                _xmlDesignerWindow.position = WindowRect;
                _xmlDesignerWindow.Show();
                Opening = true;
            }
        }
    }
}
#endif