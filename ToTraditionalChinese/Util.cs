using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ToTraditionalChinese
{
    public static class Util
    {
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);
        private const int LOCALE_SYSTEM_DEFAULT = 2048;
        // private const int LCMAP_SIMPLIFIED_CHINESE = 33554432;
        private const int LCMAP_TRADITIONAL_CHINESE = 67108864;
        private static bool ContainsChinese(this string str)
        {
            return Regex.IsMatch(str, "[\\u4e00-\\u9fa5]");
        }
        public static string ToTraditionalChinese(string input)
        {
            bool flag = input.ContainsChinese();
            string result;
            if (flag)
            {
                string text = new string(' ', input.Length);
                _ = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, input, input.Length, text, input.Length);
                result = text;
            }
            else
                result = input;
            return result;
        }
    }
}
