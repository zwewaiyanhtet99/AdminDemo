using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ABankAdmin.Core.Utils
{
    public class PasswordHash
    {
        #region SHA256 Password HashString
        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }

        //public static string SHA256HexHashString(string stringIn)
        //{
        //    string hashString;
        //    using (var sha256 = SHA256Managed.Create())
        //    {
        //        var hash = sha256.ComputeHash(Encoding.Default.GetBytes(stringIn));
        //        hashString = ToHex(hash, false);
        //    }

        //    return hashString;
        //}
        public static string SHA256HexHashString(string stringIn, string username)
        {

            string saltedcode = EncodedbySalted(username);//username
            string hashString;
            using (var sha256 = SHA256Managed.Create())
            {
                var hash = sha256.ComputeHash(Encoding.Default.GetBytes(stringIn + saltedcode));
                hashString = ToHex(hash, false);
            }

            return hashString;
        }

        public static string EncodedbySalted(string decodestring)
        {

            decodestring = decodestring.ToLower().Replace("a", "@").Replace("i", "!").Replace("l", "1").Replace("e", "3").Replace("o", "0").Replace("s", "$").Replace("n", "&");
            return decodestring;


        }
        #endregion
    }
}
