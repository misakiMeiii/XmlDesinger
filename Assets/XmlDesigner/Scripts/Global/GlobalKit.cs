using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace XmlDesigner
{
    public static class GlobalKit
    {
        public static T ToEnum<T>(this string str)
        {
            if (string.IsNullOrEmpty(str)) return default(T);
            try
            {
                var value = (T) Enum.Parse(typeof(T), str);
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        public static T ToEnum<T>(this int value)
        {
            try
            {
                return (T) Enum.ToObject(typeof(T), value);
            }
            catch
            {
                return default(T);
            }
        }
#if UNITY_EDITOR

        public static AnimBool AnimBoolValueChangeListener(this AnimBool animBool, UnityAction call)
        {
            animBool.valueChanged.AddListener(call);
            return animBool;
        }
#endif

        public static Vector3 StringToVector3(this string str, char split = ',')
        {
            var vector3 = Vector3.zero;
            if (!string.IsNullOrEmpty(str))
            {
                var strs = str.Split(split);
                if (strs.Length >= 3)
                {
                    float.TryParse(strs[0], out vector3.x);
                    float.TryParse(strs[1], out vector3.y);
                    float.TryParse(strs[2], out vector3.z);
                }
                else
                {
                    Debug.LogError("字符传入错误:" + str);
                }
            }

            return vector3;
        }

        public static string Vector3ToString(this Vector3 vector, char split = ',')
        {
            if (vector != null)
            {
                return string.Format("{0}{1}{2}{1}{3}", vector.x, split, vector.y, vector.z);
            }

            return string.Format("0{0}0{0}0", split);
        }

        public static Vector2 StringToVector2(this string str, char split = ',')
        {
            if (string.IsNullOrEmpty(str))
            {
                return Vector2.zero;
            }

            var strs = str.Split(split);
            var vector2 = Vector2.zero;

            if (strs.Length >= 2)
            {
                float.TryParse(strs[0], out vector2.x);
                float.TryParse(strs[1], out vector2.y);
            }
            else
            {
                Debug.LogError("字符传入错误:" + str);
            }


            return vector2;
        }

        public static string Vector2ToString(this Vector2 vector, char split = ',')
        {
            if (vector != null)
            {
                return string.Format("{0}{1}{2}", vector.x, split, vector.y);
            }

            return string.Format("0{0}0", split);
        }

        public static void StringToColorAll(string s, out Color c)
        {
            var carray = s.Split(',');
            if (carray.Length != 4)
            {
                Debug.LogError("color值的格式应为x,x,x,x");
                c = Color.white;
                return;
            }

            c = new Color(float.Parse(carray[0]) / 255, float.Parse(carray[1]) / 255,
                float.Parse(carray[2]) / 255, float.Parse(carray[3]) / 255);
        }

        public static Vector4 StringToVector4(this string str, char split = ',')
        {
            if (string.IsNullOrEmpty(str))
            {
                return Vector4.zero;
            }

            var strs = str.Split(split);
            var vector4 = Vector4.zero;

            if (strs.Length >= 4)
            {
                float.TryParse(strs[0], out vector4.x);
                float.TryParse(strs[1], out vector4.y);
                float.TryParse(strs[2], out vector4.z);
                float.TryParse(strs[3], out vector4.w);
            }
            else
            {
                Debug.LogError("字符传入错误:" + str);
            }


            return vector4;
        }

        public static string Vector4ToString(this Vector4 vector, char split = ',')
        {
            if (vector != null)
            {
                return string.Format("{0}{1}{2}{1}{3}{1}{4}", vector.x, split, vector.y, vector.z, vector.w);
            }

            return string.Format("0{0}0{0}0{0}0", split);
        }

        /// <summary>
        /// 获取当前选择物体路径
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
#if UNITY_EDITOR
        public static string GetCurrentGameObjectPath(string str)
        {
            var objs = Selection.objects;
            if (objs.Length > 0)
            {
                var obj = objs[0] as GameObject;
                if (obj != null)
                {
                    var path = obj.name;
                    var parent = obj.transform.parent;
                    while (parent)
                    {
                        path = string.Format("{0}/{1}", parent.name, path);
                        parent = parent.parent;
                    }

                    return path;
                }
            }

            return str;
        }


        public static int ToInt(this string selfStr, int defaultValue = 0)
        {
            return int.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }

        public static float ToFloat(this string selfStr, float defaultValue = 0)
        {
            return float.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }

        public static double ToDouble(this string selfStr, double defaultValue = 0)
        {
            return double.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }

        public static bool ToBool(this string selfStr, bool defaultValue = false)
        {
            return bool.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }

        public static string GetCurrentGameObjectWorldPosition(string str)
        {
            var objs = Selection.objects;
            if (objs.Length > 0)
            {
                var obj = objs[0] as GameObject;
                if (obj != null)
                {
                    return obj.transform.position.Vector3ToString();
                }
            }

            return str;
        }

        public static string GetCurrentGameObjectLocalPosition(string str)
        {
            var objs = Selection.objects;
            if (objs.Length > 0)
            {
                var obj = objs[0] as GameObject;
                if (obj != null)
                {
                    return obj.transform.localPosition.Vector3ToString();
                }
            }

            return str;
        }

        public static string GetCurrentGameObjectForward(string str)
        {
            var objs = Selection.objects;
            if (objs.Length > 0)
            {
                var obj = objs[0] as GameObject;
                if (obj != null)
                {
                    return obj.transform.forward.Vector3ToString();
                }
            }

            return str;
        }

        public static string GetCurrentGameObjectScale(string str)
        {
            var objs = Selection.objects;
            if (objs.Length > 0)
            {
                var obj = objs[0] as GameObject;
                if (obj != null)
                {
                    return obj.transform.localScale.Vector3ToString();
                }
            }

            return str;
        }
#endif

        public static Color GetColorByString(this string colorstr, char split = ',')
        {
            var color = Color.black;
            if (!string.IsNullOrEmpty(colorstr))
            {
                var colorArray = colorstr.Split(split);
                float value = 0;
                if (colorArray.Length >= 3)
                {
                    if (float.TryParse(colorArray[0], out value))
                        color.r = value / 255f;
                    else
                        color.r = 0;

                    if (float.TryParse(colorArray[1], out value))
                        color.g = value / 255f;
                    else
                        color.g = 0;

                    if (float.TryParse(colorArray[2], out value))
                        color.b = value / 255f;
                    else
                        color.b = 0;
                }

                if (colorArray.Length >= 4)
                {
                    if (float.TryParse(colorArray[3], out value))
                        color.a = value / 255f;
                    else
                        color.a = 0;
                }

                return color;
            }

            return Color.white;
        }

        public static string GetStringFromColor(this Color color, char split = ',')
        {
            if (color != null)
            {
                var r = ((int) (color.r * 255)).ToString();
                var g = ((int) (color.g * 255)).ToString();
                var b = ((int) (color.b * 255)).ToString();
                var a = ((int) (color.a * 255)).ToString();
                return string.Format("{0}{4}{1}{4}{2}{4}{3}", r, g, b, a, split);
            }

            return string.Format("255{0}255{0}255{0}255", split);
        }


        // 元素前移方法
        public static void MoveElementForward<T>(this int index, List<T> list)
        {
            if (index < 1 || index >= list.Count)
            {
                return;
            }

            var temp = list[index];
            list.RemoveAt(index);
            list.Insert(index - 1, temp);
        }

        // 元素后移方法
        public static void MoveElementBackward<T>(this int index, List<T> list)
        {
            if (index < 0 || index >= list.Count - 1)
            {
                return;
            }

            var temp = list[index];
            list.RemoveAt(index);
            list.Insert(index + 1, temp);
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="originStr"></param>
        /// <param name="title"></param>
        /// <param name="directory"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
#if UNITY_EDITOR
        public static string GetNameByOpenFile(this string originStr, string title, string directory, string extension)
        {
            var fullPath = EditorUtility.OpenFilePanel(title, directory, extension);
            // 最后一个"/" 占据整个路径字符串的索引
            var lastPath = fullPath.LastIndexOf("/", StringComparison.Ordinal);
            //Debug.Log("Unity xzy :lastpath index:" + lastpath);
            // 最后一个"." 占据整个路径字符串的索引
            var lastDot = fullPath.LastIndexOf(".", StringComparison.Ordinal);

            // 纯文件名字长度
            var length = lastDot - lastPath - 1;

            // 文件目录字符串 xx/xx/xx/
            //string beginpart = FullPath.Substring(0, lastpath + 1);
            //  纯文件名字
            if (!string.IsNullOrEmpty(fullPath))
            {
                return fullPath.Substring(lastPath + 1, length);
            }

            return originStr;
        }

        public static string GetSelectedFolderPath(this string originStr)
        {
            // 打开一个文件夹选择对话框，并返回选定的文件夹路径
            var path = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            return string.IsNullOrEmpty(path) ? originStr : path;
        }
#endif

        /// <summary>
        /// 组合键判断
        /// </summary>
        /// <param name="prekey"></param>
        /// <param name="postkey"></param>
        /// <param name="postkeyevent">监听类型</param>
        /// <returns></returns>
        public static bool IsCombinationKey(EventModifiers prekey, KeyCode postkey, EventType postkeyevent)
        {
            if (prekey != EventModifiers.None)
            {
                var eventDown = (Event.current.modifiers & prekey) != 0;
                if (eventDown && Event.current.rawType == postkeyevent && Event.current.keyCode == postkey)
                {
                    Event.current.Use();

                    // if (postkey != KeyCode.None)
                    //     Debug.Log( string .Format( "{0} + {1}" , prekey.ToString(), postkey.ToString()));
                    // else
                    //     Debug.Log( string .Format( "{0} + {1}" , prekey.ToString(), postkeyevent.ToString()));
                    //
                    return true;
                }
            }
            else
            {
                if (Event.current.rawType == postkeyevent && Event.current.keyCode == postkey)
                {
                    Event.current.Use();

                    // if (postkey != KeyCode.None)
                    //     Debug.Log( string .Format( "{0}" , postkey.ToString()));
                    // else
                    //     Debug.Log( string .Format( "{0}" , postkeyevent.ToString()));
                    //
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        public static string UpperFirstLetter(this string originStr)
        {
            if (string.IsNullOrEmpty(originStr))
                return originStr;

            var firstChar = originStr[0];

            if (char.IsLetter(firstChar) && char.IsLower(firstChar))
            {
                string capitalized = char.ToUpper(firstChar) + originStr.Substring(1);
                return capitalized;
            }

            return originStr;
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        public static string LowerFirstLetter(this string originStr)
        {
            if (string.IsNullOrEmpty(originStr))
                return originStr;

            var firstChar = originStr[0];

            if (char.IsLetter(firstChar) && char.IsUpper(firstChar))
            {
                string capitalized = char.ToLower(firstChar) + originStr.Substring(1);
                return capitalized;
            }

            return originStr;
        }
    }
}