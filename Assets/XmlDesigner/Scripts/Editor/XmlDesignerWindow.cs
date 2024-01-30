#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace XmlDesigner
{
    [InitializeOnLoad]
    public class XmlDesignerWindow : EditorWindow
    {
        private static EditorWindow _xmlDesignerWindow;
        public static bool Opening { get; set; }
        public bool IsFinishedInit { get; private set; }
        private const string Compiling = "编译中...";
        private bool _autoRead;
        private string _lastLoadDesignFilePath;

        private static readonly string WindowTitle = "xml设计/生成器";
        private static readonly Rect WindowRect = new Rect(50, 100, 1600, 600);

        private RootElement _rootElement;

        private string _exportXmlDesignPath;
        private string _exportCsPath;

        static XmlDesignerWindow()
        {
            _xmlDesignerWindow = null;
        }

        [MenuItem("XmlDesigner/Tool &T", false, 1)]
        public static void OnClickWindow()
        {
            _xmlDesignerWindow = GetWindow<XmlDesignerWindow>();
            if (Opening)
            {
                Opening = false;
                _xmlDesignerWindow.Close();
            }
            else
            {
                _xmlDesignerWindow.titleContent = new GUIContent(WindowTitle);
                _xmlDesignerWindow.position = WindowRect;
                Opening = true;
                _xmlDesignerWindow.Show();
            }
        }

        [DidReloadScripts]
        public static void Reload()
        {
            if (Opening)
            {
                _xmlDesignerWindow = GetWindow<XmlDesignerWindow>();
                _xmlDesignerWindow.titleContent = new GUIContent(WindowTitle);
            }
        }
        
        public static bool IsMyCustomEditorWindowOpen()
        {
            var window = GetWindow<XmlDesignerWindow>(false); // false参数表示不创建新窗口
            return window != null && window.hasFocus;
        }

        private void OnDestroy()
        {
            Opening = false;
        }


        private void Init()
        {
            _exportXmlDesignPath = string.IsNullOrEmpty(nameof(_exportXmlDesignPath).GetString())
                ? Application.dataPath
                : nameof(_exportXmlDesignPath).GetString();
            _exportCsPath = string.IsNullOrEmpty(nameof(_exportCsPath).GetString())
                ? Application.dataPath
                : nameof(_exportCsPath).GetString();
            _lastLoadDesignFilePath = nameof(_lastLoadDesignFilePath).GetString();
            _autoRead = nameof(_autoRead).GetBool();
            if (_autoRead)
            {
                LoadDesignFile(_lastLoadDesignFilePath);
            }
        }


        private Vector2 _scrollPos;


        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                GUILayout.Label(Compiling);
                IsFinishedInit = false;
                return;
            }

            if (!IsFinishedInit)
            {
                Init();
                IsFinishedInit = true;
            }

            if (!IsFinishedInit) return;

            DrawView();
        }


        private void DrawView()
        {
            if (_rootElement == null)
            {
                GUILayout.BeginHorizontal();
                {
                    CreateAddRootBtn("添加根节点");
                    CreateXmlDesignerReader();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                _scrollPos = GUILayout.BeginScrollView(_scrollPos, GlobalStyle.Box);
                {
                    GUILayout.BeginHorizontal();
                    {
                        CreateAddRootBtn("新建根节点");
                        CreateXmlDesignerReader();
                    }
                    GUILayout.EndHorizontal();
                    CreateCsExporter(_rootElement);
                    CreateXmlDesignerExporter(_rootElement);
                    DrawRootElement(_rootElement);
                }
                GUILayout.EndScrollView();
            }
        }

        private void CreateAddRootBtn(string titleStr)
        {
            if (GUILayout.Button(titleStr, GUILayout.Width(100), GUILayout.Height(30)))
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

                DrawChildElement(rootElement.ChildElements); //绘制子元素

                CreateChildElementBtn(rootElement.ChildElements);
            }
            EditorGUILayout.EndVertical();

            var lineRect =
                new Rect(
                    new Vector2(rootRect.x, rootRect.y + rootRect.height + 15f),
                    new Vector2(_xmlDesignerWindow.position.width, 1f));
            GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture); //分割线

            GUILayout.Space(20);

            DrawCustomElement(rootElement.CustomElements);

            CreateCustomElementBtn(rootElement.CustomElements); //添加自定义类按钮
        }

        private void DrawChildElement(List<ChildElement> childElements)
        {
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < childElements.Count; i++)
                {
                    var childElement = childElements[i];
                    GUILayout.BeginHorizontal(GUILayout.Width(300));
                    {
                        if (GUILayout.Button("", GlobalStyle.RemoveStyle, GUILayout.Width(20)))
                        {
                            childElements.RemoveAt(i);
                        }

                        if (childElement == null) return;


                        if (GUILayout.Button("↑", GUILayout.Width(20)))
                        {
                            i.MoveElementForward(childElements);
                            GUIUtility.keyboardControl = 0;
                        }

                        if (GUILayout.Button("↓", GUILayout.Width(20)))
                        {
                            i.MoveElementBackward(childElements);
                            GUIUtility.keyboardControl = 0;
                        }

                        childElement.Name = GUIComponent.CompactTextField("字段名称:", childElement.Name);
                        if (string.IsNullOrEmpty(childElement.Name))
                        {
                            GUILayout.Label("字段名称不能为空！", GlobalStyle.RedFontStyle);
                        }

                        childElement.IsAttribute =
                            GUIComponent.CompactToggle("是否是属性:", childElement.IsAttribute, 40, 20);
                        if (childElement.ElementType > ElementType.Float && childElement.IsAttribute)
                        {
                            GUILayout.Label("不支持作为属性！", GlobalStyle.RedFontStyle);
                        }

                        childElement.ElementType =
                            (ElementType) GUIComponent.CompactEnumPopup("字段类型:", childElement.ElementType);

                        switch (childElement.ElementType)
                        {
                            case ElementType.String:
                                childElement.DefaultValue =
                                    GUIComponent.CompactTextField("默认值(可以不设置):", childElement.DefaultValue);
                                break;
                            case ElementType.Bool:
                                childElement.DefaultValue =
                                    GUIComponent.CompactToggle("默认值(可以不设置):", childElement.DefaultValue.ToBool())
                                        .ToString();
                                break;
                            case ElementType.Int:
                                childElement.DefaultValue =
                                    GUIComponent.CompactIntField("默认值(可以不设置):", childElement.DefaultValue.ToInt())
                                        .ToString();
                                break;
                            case ElementType.Double:
                                childElement.DefaultValue =
                                    GUIComponent.CompactDoubleField("默认值(可以不设置):", childElement.DefaultValue.ToDouble())
                                        .ToString();
                                break;
                            case ElementType.Float:
                                childElement.DefaultValue =
                                    GUIComponent.CompactFloatField("默认值(可以不设置):", childElement.DefaultValue.ToFloat())
                                        .ToString();
                                break;
                            case ElementType.List:
                                childElement.ReferenceType =
                                    (BaseType) GUIComponent.CompactEnumPopup("泛型:", childElement.ReferenceType);
                                if (childElement.ReferenceType == BaseType.Custom)
                                {
                                    childElement.CustomType = GUIComponent.CompactPopup("类:", childElement.CustomType,
                                        _rootElement.CustomElementNames);
                                }

                                break;
                            case ElementType.Queue:
                                childElement.ReferenceType =
                                    (BaseType) GUIComponent.CompactEnumPopup("泛型:", childElement.ReferenceType);
                                if (childElement.ReferenceType == BaseType.Custom)
                                {
                                    childElement.CustomType = GUIComponent.CompactPopup("类:", childElement.CustomType,
                                        _rootElement.CustomElementNames);
                                }

                                break;
                            case ElementType.Stack:
                                childElement.ReferenceType =
                                    (BaseType) GUIComponent.CompactEnumPopup("泛型:", childElement.ReferenceType);
                                if (childElement.ReferenceType == BaseType.Custom)
                                {
                                    childElement.CustomType = GUIComponent.CompactPopup("类:", childElement.CustomType,
                                        _rootElement.CustomElementNames);
                                }

                                break;
                            case ElementType.HashSet:
                                childElement.ReferenceType =
                                    (BaseType) GUIComponent.CompactEnumPopup("泛型:", childElement.ReferenceType);
                                if (childElement.ReferenceType == BaseType.Custom)
                                {
                                    childElement.CustomType = GUIComponent.CompactPopup("类:", childElement.CustomType,
                                        _rootElement.CustomElementNames);
                                }

                                break;
                            case ElementType.Dictionary:
                                childElement.KeyType =
                                    (KeyType) GUIComponent.CompactEnumPopup("Key的类型:", childElement.KeyType);
                                childElement.ReferenceType =
                                    (BaseType) GUIComponent.CompactEnumPopup("Value的类型:", childElement.ReferenceType);
                                if (childElement.ReferenceType == BaseType.Custom)
                                {
                                    childElement.CustomType = GUIComponent.CompactPopup("类:", childElement.CustomType,
                                        _rootElement.CustomElementNames);
                                }

                                break;
                            case ElementType.Custom:
                                childElement.CustomType = GUIComponent.CompactPopup("类:", childElement.CustomType,
                                    _rootElement.CustomElementNames);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void CreateChildElementBtn(List<ChildElement> childElements)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            {
                if (GUILayout.Button("", GlobalStyle.AddStyle, GUILayout.Width(20)))
                {
                    childElements.Add(new ChildElement());
                }

                GUILayout.Label("添加子元素");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawCustomElement(List<CustomElement> customElements)
        {
            for (int i = 0; i < customElements.Count; i++)
            {
                var customElement = customElements[i];
                if (customElement == null) return;
                var singleRect = EditorGUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(50));
                    {
                        if (GUILayout.Button("", GlobalStyle.RemoveStyle))
                        {
                            customElements.RemoveAt(i);
                        }

                        GUILayout.Label("删除类");
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        customElement.Name =
                            GUIComponent.CompactTextField("类名:", customElement.Name).UpperFirstLetter();
                        if (string.IsNullOrEmpty(customElement.Name))
                        {
                            GUILayout.Label("类名不能为空！", GlobalStyle.RedFontStyle);
                        }
                    }
                    GUILayout.EndHorizontal();
                    DrawChildElement(customElement.ChildElements); //绘制子元素
                    CreateChildElementBtn(customElement.ChildElements);
                }
                EditorGUILayout.EndVertical();

                var lineRect =
                    new Rect(
                        new Vector2(singleRect.x, singleRect.y + singleRect.height + 15f),
                        new Vector2(_xmlDesignerWindow.position.width, 1f));
                GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture); //分割线
                GUILayout.Space(30);
            }
        }

        private void CreateCustomElementBtn(List<CustomElement> customElements)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            {
                if (GUILayout.Button("", GlobalStyle.AddStyle, GUILayout.Width(20)))
                {
                    customElements.Add(new CustomElement());
                }

                GUILayout.Label("添加自定义类");
            }
            GUILayout.EndHorizontal();
        }

        private void CreateXmlDesignerExporter(RootElement rootElement)
        {
            GUILayout.BeginHorizontal();
            {
                _exportXmlDesignPath = GUIComponent.CompactTextField("设计文件路径:", _exportXmlDesignPath, 500, 450);
                GUILayout.Space(5);
                if (GUILayout.Button("选择路径", GUILayout.Width(80)))
                {
                    _exportXmlDesignPath = _exportXmlDesignPath.GetSelectedFolderPath();
                    nameof(_exportXmlDesignPath).SaveString(_exportXmlDesignPath);
                }

                if (GUILayout.Button("导出设计", GUILayout.Width(80)))
                {
                    var filePath = _exportXmlDesignPath + $"/{rootElement.Name}Design.xml";
                    DesignerExporter.ExportDesignFile(filePath, rootElement);
                    Process.Start(_exportXmlDesignPath);
                    nameof(_exportXmlDesignPath).SaveString(_exportXmlDesignPath);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void CreateXmlDesignerReader()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("读取设计文件", GUILayout.Width(100), GUILayout.Height(30)))
                {
                    var filePath = EditorUtility.OpenFilePanel("读取设计文件", "", "xml");
                    LoadDesignFile(filePath);
                }

                CreateAutoReadToggle();
            }
            GUILayout.EndHorizontal();
        }

        private void LoadDesignFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                _rootElement = DesignerReader.ReadDesignFile(filePath);
                nameof(_lastLoadDesignFilePath).SaveString(filePath);
            }
        }

        private void CreateAutoReadToggle()
        {
            _autoRead = GUILayout.Toggle(_autoRead, "自动读取上次选择的设计文件", GUILayout.Width(200));
            nameof(_autoRead).SaveBool(_autoRead);
        }

        private void CreateCsExporter(RootElement rootElement)
        {
            GUILayout.BeginHorizontal();
            {
                _exportCsPath = GUIComponent.CompactTextField("解析脚本路径:", _exportCsPath, 500, 450);
                GUILayout.Space(5);
                if (GUILayout.Button("选择路径", GUILayout.Width(80)))
                {
                    _exportCsPath = _exportCsPath.GetSelectedFolderPath();
                    nameof(_exportCsPath).SaveString(_exportCsPath);
                }

                if (GUILayout.Button("导出脚本", GUILayout.Width(80)))
                {
                    CsExporter.ExportDesignClass(_exportCsPath, rootElement);
                    CsExporter.ExportSerializedClass(_exportCsPath, rootElement);
                    _xmlDesignerWindow.Close();
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif