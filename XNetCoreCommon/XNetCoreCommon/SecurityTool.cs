using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace XNetCoreCommon
{

    /// <summary>
    /// 加密工具
    /// </summary>
    public sealed class SecurityTool
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}
