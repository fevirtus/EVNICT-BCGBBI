using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BI_Project.Helpers.Security
{
    public static class SaltGenerator
    {
        private const int SALT_SIZE = 24; // ((24 * 8)/6 =32 ) Kết quả trả về cần là 32 byte mà lại khai báo 24 là do phương thức ToBase64String (xử lý từ Mail Server. Mail Server chỉ sử dụng 6 bit cho 1 ký tự : A->Z, a->z, 0->9, + /)

        // Random Number Generator
        private static RNGCryptoServiceProvider rng = null;
        static SaltGenerator()
        {
            rng = new RNGCryptoServiceProvider();
        }
        public static string GetSaltGenerate()
        {
            byte[] bytes = new byte[SALT_SIZE];
            rng.GetNonZeroBytes(bytes);
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}