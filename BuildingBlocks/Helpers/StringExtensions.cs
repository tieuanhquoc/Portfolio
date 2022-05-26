using System.Text;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Helpers
{
    public static class StringExtensions
    {
        private static Regex _convertToUnSignRg;
        private static readonly Regex HtmlReg = new("<[^>]+>", RegexOptions.IgnoreCase);

        public static string RemoveUnicode(this string str)
        {
            _convertToUnSignRg ??= new Regex("\\p{IsCombiningDiacriticalMarks}+");
            var temp = str.Normalize(NormalizationForm.FormD);
            return _convertToUnSignRg.Replace(temp, string.Empty).Replace("đ", "d").Replace("Đ", "D");
        }

        public static string ReplaceSpace(this string str)
        {
            return str.Replace(" ", "_").Trim();
        }

        public static string UpCaseTitle(this string str)
        {
            char[] temp;
            return string.Join(' ', str.RemoveSpaceDuplicate().Split(' ').Select(x =>
                {
                    temp = x.ToCharArray();
                    temp[0] = char.ToUpper(temp[0]);
                    x = string.Join("", temp);
                    return x;
                }
            ));
        }

        public static string RemoveSpace(this string str)
        {
            return !string.IsNullOrEmpty(str) ? str.Replace(" ", "").Trim() : str;
        }

        public static string RemoveSpaceDuplicate(this string str)
        {
            return Regex.Replace(str, @"\s+", " ").Trim();
        }

        public static string RemoveEmail(this string str)
        {
            return str.Contains("@") ? Regex.Replace(str, @"(@.*)", "") : str;
        }

        public static string RemoveHtml(this string text)
        {
            const string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);
            text = regexCss.Replace(text, string.Empty);
            text = Regex.Replace(text, htmlTagPattern, string.Empty);
            text = Regex.Replace(text, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            text = text.Replace("&nbsp;", string.Empty);
            return HtmlReg.Replace(text, "");
        }

        public static string RemoveSpecialCharacters(this string text)
        {
            return Regex.Replace(text, "[^A-Za-z0-9 ]", "");
        }

        public static string GetShortContent(this string text)
        {
            text = HtmlReg.Replace(text, "");
            var arr = text.Split(" ");
            return $"{string.Join(" ", arr.Length < 30 ? arr : arr[..30])}...";
        }

        public static bool EqualNoUnicode(this string parent, string child)
        {
            parent = parent.Trim().RemoveUnicode().ToLower().RemoveSpaceDuplicate();
            child = child.Trim().RemoveUnicode().ToLower().RemoveSpaceDuplicate();
            return parent.Contains(child) || child.Contains(parent);
        }

        public static bool IsNullOrEmptyCustom(this string text, string text2)
        {
            if (text == null)
                return true;
            return text.Length == 0 || text2.Contains(text);
        }
    }
}