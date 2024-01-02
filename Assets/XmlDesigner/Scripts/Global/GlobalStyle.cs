using UnityEngine;

namespace XmlDesigner
{
    public class GlobalStyle
    {
        public static GUIStyle RedFontStyle
        {
            get
            {
                RedFont.normal.textColor = Color.red;
                return RedFont;
            }
        }

        private static readonly GUIStyle RedFont = new GUIStyle(GUI.skin.label);

        public static string Box => "box";

        public static string AddStyle => "OL Plus";

        public static string RemoveStyle => "OL Minus";

        public static string OutLineHighLight => "ControlHighlight";

        public static string SelectionRect => "SelectionRect";

        public static string FlowNode1 => "flow node 1";

        public static string FlowNode2 => "flow node 2";

        public static string FlowNode3 => "flow node 3";

        public static string FlowNode4 => "flow node 4";

        public static string FlowNode5 => "flow node 5";

        public static string BoldToggle => "BoldToggle";

        public static string ButtonLeft => "ButtonLeft";

        public static string ButtonMid => "ButtonMid";

        public static string ButtonRight => "ButtonRight";

        public static string GridList => "GridList";

        public static string Dopesheetkeyframe => "Dopesheetkeyframe";

        public static string LargeButtonLeft => "LargeButtonLeft";

        public static string LargeButtonMid => "LargeButtonMid";

        public static string LargeButtonRight => "LargeButtonRight";

        public static string PreDropDown => "PreDropDown";
    }
}