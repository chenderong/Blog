using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XNetCoreCommon
{
    public static class HtmlTool
    {
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = Regex.Replace(html, "<[^>]+>", "");
            strText = Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }
    }
}
