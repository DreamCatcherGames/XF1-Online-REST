using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using XF1_Online_REST;

namespace XF1_Online_REST.LogicScripts
{
    public class Tools
    {
        private XF1OnlineEntities dbContext;
        public Tools()
        {
            dbContext = new XF1OnlineEntities();
        }

        /// <summary>
        /// Method to get an unique token
        /// </summary>
        /// <returns><see cref="string"/> object that represents a unique code </returns>
        public string getToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            token = token.Replace("+", "").Replace("/", "");
            while (!uniqueTokenVerificator(token))
            {
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                token = token.Replace("+", "").Replace("/", "");
            }
            return token;
        }
        /// <summary>
        /// Method to verify if a token is unique inside the database
        /// </summary>
        /// <param name="token"><see cref="string"/> that represents the token needed to be verified inside the database</param>
        /// <returns><see cref="Boolean"/> object that indicates if the token provided is unique inside the database</returns>
        public Boolean uniqueTokenVerificator(string token)
        {
            return !dbContext.Administrators.Any(o => o.Token == token);
        }

        /// <summary>
        /// Method to assign a unique token to a <see cref="Administrator"/>/<see cref="User"/> inside the database
        /// </summary>
        /// <param name="user"><see cref="Object"/> that contains the credentials of the <see cref="Administrator"/>/<see cref="User"/> that the token has to be assigned to</param>
        /// <param name="token"><see cref="string"/> that represents the token needed to be assigned</param>
        public void assignToken(Object user,string token)
        {
            if(user is Administrator)
            {
                Administrator admin= (Administrator)user;
                admin = dbContext.Administrators.Find(admin.Username);
                admin.Token = token;
                
            }
            dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Method to encrypt a password using MD5 encryption method
        /// </summary>
        /// <param name="password"><see cref="string"/> object that represents the password to encrypt</param>
        /// <returns><see cref="List{string}"/> object that contains the encrypted password and the salt component needed it's decryption </returns>
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
        /// Method to verify if a given password matches a MD5 encrypted password
        /// </summary>
        /// <param name="givenPassword"><see cref="string"/> object that represents the uncrypted password to be verified</param>
        /// <param name="encryptedPassword"><see cref="string"/> object that represents the encrypted password to be verified</param>
        /// <param name="salt"><see cref="string"/> object that represents the salt component needed for the verification</param>
        /// <returns></returns>
        public Boolean verifyPassword(string givenPassword,string encryptedPassword,string salt)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[10];
            rng.GetBytes(buff);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(givenPassword + salt));
            string result = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();

            return result == encryptedPassword;
        }
    }
}