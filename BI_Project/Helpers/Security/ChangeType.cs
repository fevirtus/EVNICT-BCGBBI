using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BI_Project.Helpers.Security
{
    public static class ChangeType
    {

        public static byte[] GetBytes(String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)]; // Tong so byte = so ky tu * so byte
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)]; // Tong so byte = so ky tu * so byte
            Buffer.BlockCopy(bytes, 0, chars, 0, chars.Length);
            return new string(chars);
        }
    }
}