using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace XFIA_REST.Models
{
    public class MiscelaneousFunctions
    {
        public MiscelaneousFunctions()
        {

        }

        public string getToken()
        {
            string g = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            g = g.Replace("+", "");
            return g.Replace("/", "");
        }
        public List<string> passwordEncryptor(string password)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[10];
            rng.GetBytes(buff);

            string salt = Convert.ToBase64String(buff);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(password + salt));
            string result = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
            return new List<string>(new string[] { result, salt });
        }
    }
}