namespace CodeGenKit
{
    /// <summary>
    ///  生成前大括号的字符串代码
    /// </summary>
    public class OpenBraceCode
    {
        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine("{");
        }
    }
}