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
        public static string Compiling = "编译中...";

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

        private void OnDestroy()
        {
            Opening = false;
        }

        private void Init()
        {
        }

        private Vector2 _scrollPos;

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.Label(Compiling);
                return;
            }

            if (!_isFinishedInit)
            {
                Init();
                _isFinishedInit = true;
            }

            DrawView();
        }

        private RootElement _rootElement;

        private void DrawView()
        {
            if (_rootElement == null)
            {
                GUILayout.BeginHorizontal();
                {
                    CreateAddRootBtn();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                _scrollPos = GUILayout.BeginScrollView(_scrollPos, GlobalStyle.Box);
                {
                    DrawRootElement(_rootElement);
                }
                GUILayout.EndScrollView();
            }
        }

        private void CreateAddRootBtn()
        {
            if (GUILayout.Button("添加根节点", GUILayout.Width(100), GUILayout.Height(30)))
            {
                _rootElement = new RootElement
                {
                    Name = "Root",
                    NameSpace = "XML"
                };
            }
        }

        private void DrawRootElement(RootElement rootElement)
        {
            if (rootElement == null) return;
            var rootRect = EditorGUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal(GUILayout.Width(400));
                {
                    //根节点
                    GUILayout.Label("根节点类名:");
                    rootElement.Name = GUILayout.TextField(rootElement.Name, GUILayout.Width(150)).UpperFirstLetter();
                    if (string.IsNullOrEmpty(rootElement.Name))
                    {
                        GUILayout.Label("请输入类名！", GlobalStyle.RedFontStyle);
                    }

                    GUILayout.Label("命名空间:");
                    rootElement.NameSpace = GUILayout.TextField(rootElement.NameSpace, GUILayout.Width(150));
                    if (string.IsNullOrEmpty(rootElement.NameSpace))
                    {
                        GUILayout.Label("请输入命名空间！", GlobalStyle.RedFontStyle);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            var lineRect =
                new Rect(
                    new Vector2(rootRect.x, rootRect.y + rootRect.height + 15f),
                    new Vector2(_xmlDesignerWindow.position.width, 1f));
            GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture);
            GUILayout.Space(15);
            
        }
    }
}
#endif