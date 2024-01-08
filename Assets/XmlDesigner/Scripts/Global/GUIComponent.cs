#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace XmlDesigner
{
    public class GUIComponent
    {
        public static string CompactTextField(string title, string field, float totalLength = 200,
            float fieldLength = 150)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                field = EditorGUILayout.TextField(field, GUILayout.Width(fieldLength));
            }
            GUILayout.EndHorizontal();
            return field;
        }

        public static int CompactIntField(string title, int field, float totalLength = 200, float fieldLength = 150)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                field = EditorGUILayout.IntField(field, GUILayout.Width(fieldLength));
            }
            GUILayout.EndHorizontal();
            return field;
        }

        public static float CompactFloatField(string title, float field, float totalLength = 200,
            float fieldLength = 150)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                field = EditorGUILayout.FloatField(field, GUILayout.Width(fieldLength));
            }
            GUILayout.EndHorizontal();
            return field;
        }

        public static double CompactDoubleField(string title, double field, float totalLength = 200,
            float fieldLength = 150)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                field = EditorGUILayout.DoubleField(field, GUILayout.Width(fieldLength));
            }
            GUILayout.EndHorizontal();
            return field;
        }

        public static bool CompactToggle(string title, bool field, float totalLength = 200,
            float fieldLength = 150)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                field = EditorGUILayout.Toggle(field, GUILayout.Width(fieldLength));
            }
            GUILayout.EndHorizontal();
            return field;
        }

        public static Enum CompactEnumPopup(string title, Enum @enum, float totalLength = 200,
            float fieldLength = 150)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(200));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                @enum = EditorGUILayout.EnumPopup(@enum);
            }
            GUILayout.EndHorizontal();
            return @enum;
        }

        public static void TipButton(string tip, UnityAction action = null, float size = 16)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(size));
            {
                if (GUILayout.Button(new GUIContent("i",
                            tip),
                        GUILayout.Width(size), GUILayout.Height(size)))
                {
                    if (action != null) action.Invoke();
                }
            }
            GUILayout.EndHorizontal();
        }

        public static void DisplayTipButton(string tip, float size = 15)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("i", GUILayout.Width(size), GUILayout.Height(size)))
                {
                    EditorUtility.DisplayDialog("提示", tip, "确定");
                }
            }
            GUILayout.EndHorizontal();
        }


        public static string Vector2Field(string title, string originStr, float totalLength = 300,
            float fieldLength = 250)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                originStr = EditorGUILayout
                    .Vector2Field("", originStr.StringToVector2(), GUILayout.Width(fieldLength))
                    .Vector2ToString();
            }
            GUILayout.EndHorizontal();

            return originStr;
        }

        public static string Vector3Field(string title, string originStr, float totalLength = 300,
            float fieldLength = 250)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                originStr = EditorGUILayout
                    .Vector3Field("", originStr.StringToVector3(), GUILayout.Width(fieldLength))
                    .Vector3ToString();
            }
            GUILayout.EndHorizontal();

            return originStr;
        }

        public static string Vector4Field(string title, string originStr, float totalLength = 300,
            float fieldLength = 250)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(totalLength));
            {
                GUILayout.Label(title);
                GUILayout.Space(5);
                originStr = EditorGUILayout
                    .Vector4Field("", originStr.StringToVector4(), GUILayout.Width(fieldLength))
                    .Vector4ToString();
            }
            GUILayout.EndHorizontal();

            return originStr;
        }

        public static string GetObjPathBtn(string path)
        {
            if (GUILayout.Button("获取物体路径", GUILayout.Width(80)))
            {
                path = GlobalKit.GetCurrentGameObjectPath(path);
            }

            return path;
        }

        public static string GetWorldPosBtn(string pos)
        {
            if (GUILayout.Button("获取世界坐标", GUILayout.Width(80)))
            {
                pos = GlobalKit.GetCurrentGameObjectWorldPosition(pos);
            }

            return pos;
        }
    }
}
#endif