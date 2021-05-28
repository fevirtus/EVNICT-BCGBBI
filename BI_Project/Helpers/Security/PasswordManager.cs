using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BI_Project.Helpers.Security
{
    public class PasswordManager
    {

        //HashAlgorithm doHash = new SHA256Managed();
        HashAlgorithm doHash = new SHA512Managed();
        public string GetPasswordHashedAndGetSalt(string plaintextPassword, out string salt)
        {

            salt = SaltGenerator.GetSaltGenerate();

            string finalStr = plaintextPassword + salt;
            string rets = Convert.ToBase64String(doHash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(finalStr)));
            return rets;
        }

        public string IsMatch(string plaintextPassword, string salt)
        {
            string finalStr = plaintextPassword + salt;
            string rets = Convert.ToBase64String(doHash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(finalStr)));
            return rets;
        }
    }

}