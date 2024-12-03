using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Services.Users
{
    public static class Helpers
    {

        public static string CreatePassword() {

            const int MAX_SIZE = 12;

            char[] chars = new char[62];
            chars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ123456789".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider()) {
                crypto.GetNonZeroBytes(data);
                data = new byte[MAX_SIZE];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(MAX_SIZE);
            foreach (byte b in data) {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();

        }
        
        public static string HashPassword(string password) {

            string s1 = "fcChVBcs1i";
            string s2 = "bifupJSKru";
            string s3 = s1 + password + s2;

            var sha1 = new SHA1CryptoServiceProvider();
            var data = sha1.ComputeHash(Encoding.ASCII.GetBytes(s3));
            return Convert.ToBase64String(data);

        }

        public static bool IsLegalPassword(string password) {
            
            if (password == null) {
                return false;
            }

            if (password.Trim() == "") {
                return false;
            }

            if (password.Length < 5) {
                return false;
            }

            return true;

        }
    }
}
