using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace XFIA_REST.Models
{
    public class MiscelaneousFunctions
    {
        private XF1OnlineEntities context;
        public MiscelaneousFunctions()
        {
            this.context = new XF1OnlineEntities();
        }
        /// <summary>
        /// Generates a random token 
        /// </summary>
        /// <returns>A generated random token</returns>
        public string getToken()
        {
            string g = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            g = g.Replace("+", "");
            return g.Replace("/", "");
        }
        /// <summary>
        /// Encrypts a given <see cref="System.String"/> password using MD5 encryption method.
        /// </summary>
        /// <param name="password">The <see cref="System.String"/> password to encrypt</param>
        /// <returns></returns>
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
        /// <summary>
        /// Verifies if a given set of credentials are the same registered on the database
        /// </summary>
        /// <param name="user"><see cref="Administrator"> object that contains the credential atributes as password and username</param>
        /// <returns><see cref="bool"> object that indicates if the credentials are valid</returns>
        public bool passwordVerifier(Object user)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes;
            string testPassword = "";
            string storedPassword= "";
            string storedSalt = "";
            if(user is Administrator)
            {
                Administrator credentials = (Administrator)user;
                Administrator testAdmin = context.Administrators.Find(credentials.Username);
                if(testAdmin==null)
                {
                    return false;
                }
                testPassword = credentials.Password;
                storedPassword = testAdmin.Password;
                storedSalt = testAdmin.Salt;
            }
            
            bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(testPassword + storedSalt));
            string result = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();

            return result.Equals(storedPassword);
        }
        public void assignToken(Object user,string token)
        {
            if(user is Administrator)
            {
                context.Administrators.Find(((Administrator)user).Username).Token = token;
                context.SaveChanges();
            }

        }
    }
}