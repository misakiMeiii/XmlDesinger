namespace XmlDesigner
{
#if UNITY_EDITOR

    using UnityEditor;

    public static class Storage
    {
        public static void SaveString(this string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public static string GetString(this string key)
        {
            return EditorPrefs.GetString(key);
        }

        public static void SaveBool(this string key, bool value)
        {
            EditorPrefs.SetBool(key, value);
        }

        public static bool GetBool(this string key)
        {
            return EditorPrefs.GetBool(key);
        }
    }
#endif
}