namespace XmlDesigner
{
    public static class GlobalKit
    {
        
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
    }
}