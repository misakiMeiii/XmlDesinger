namespace CodeGenKit
{
    /// <summary>
    /// 生成后大括号的字符串代码
    /// </summary>
    public class CloseBraceCode : ICode
    {
        private readonly bool _mSemicolon;

        public CloseBraceCode(bool semicolon)
        {
            _mSemicolon = semicolon;
        }

        public void Gen(ICodeWriter writer)
        {
            var semicolonKey = _mSemicolon ? ";" : string.Empty;
            writer.WriteLine("}" + semicolonKey);
        }
    }
}