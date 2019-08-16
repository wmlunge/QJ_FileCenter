using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QJ_FileCenter
{
    public static class MD5Util
    {
        public static string GetMd5(string text)
        {
            var md5Bytes = MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));

            return GetMd5Text(md5Bytes);
        }

        public static string GetMd5(byte[] buffer)
        {
            var md5Bytes = MD5.Create().ComputeHash(buffer);

            return GetMd5Text(md5Bytes);
        }

        private static string GetMd5Text(byte[] md5Bytes)
        {
            return BitConverter.ToString(md5Bytes).Replace("-", "").ToLower();
        }

    }
}
