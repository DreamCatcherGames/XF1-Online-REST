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
        public string getToken(string salt)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            token = token.Replace("+", "").Replace("/", "");
            while (!uniqueTokenVerificator(token,salt))
            {
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                token = token.Replace("+", "").Replace("/", "");
            }
            return token;
        }
        /// <summary>
        /// Method to encrypt a token
        /// </summary>
        /// <param name="token"><see cref="string"/> object that represents the token to be encrypted</param>
        /// <returns> Encrypted token</returns>
        public string encryptToken(string token,string salt)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[10];
            rng.GetBytes(buff);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(token+salt));
            return BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
            
        }
        /// <summary>
        /// Method to verify if a token is unique inside the database
        /// </summary>
        /// <param name="token"><see cref="string"/> that represents the token needed to be verified inside the database</param>
        /// <returns><see cref="Boolean"/> object that indicates if the token provided is unique inside the database</returns>
        public Boolean uniqueTokenVerificator(string token,string salt)
        {
            string encryptedToken = encryptToken(token,salt);
            return !dbContext.Administrators.Any(o => o.Token == encryptedToken);
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
                admin.Token = encryptToken(token,admin.Salt);
                
            }
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Method to verify if an admin token is valid
        /// </summary>
        /// <param name="token"><see cref="string"/> object that represents the token needed to be verified</param>
        /// <returns><see cref="Boolean"/> object that determines if the token was found</returns>
        public Boolean verifyAdminToken(string token,string salt)
        {
            string tempToken=encryptToken(token,salt);
            return dbContext.Administrators.Any(o => o.Token == tempToken);
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
        public Boolean verifyPassword(string givenPassword, string encryptedPassword, string salt)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[10];
            rng.GetBytes(buff);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(givenPassword + salt));
            string result = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();

            return result == encryptedPassword;
        }
        /// <summary>
        /// Method designed to verify if all the dates of a race are correctly set
        /// </summary>
        /// <param name="race"><see cref="Race"/> object that contains all the  information to be analyzed</param>
        /// <returns><see cref="Boolean"/> object that tells if the dates of the date provided are Ok</returns>
        public Boolean raceDateVerifier(Race race)
        {
            Championship champ = dbContext.Championships.Find(race.Champ_Key);
            DateTime actualDate=DateTime.Now;
            Boolean validDatesCond=race.Beginning_Date>=actualDate&&race.Ending_Date>=actualDate;
            Boolean consistencyCond = race.Beginning_Date < race.Ending_Date && race.Qualification_Date < race.Competition_Date;
            Boolean insideChampionshipCond = containedDateValidation(race.Beginning_Date, race.Ending_Date,champ.Beginning_Date,champ.Ending_Date);
            Boolean insideValidDateRangeCond = containedDateValidation(race.Qualification_Date,race.Competition_Date,race.Beginning_Date,race.Ending_Date);

            if(consistencyCond&&insideChampionshipCond&&validDatesCond&&insideValidDateRangeCond)
            {
                List<Race> races = dbContext.Races.Where(o => o.Champ_Key==champ.Unique_Key).ToList<Race>();
                foreach(Race raceT in races)
                {
                    if(raceDateBumpVerification(race,raceT))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method to verify if there's any kind of intersection between two races date range.
        /// </summary>
        /// <param name="race1"><see cref="Race"/> object that contains the date range of the first race to be analyzed</param>
        /// <param name="race2"><see cref="Race"/> object that contains the date range of the second race to be analyzed</param>
        /// <returns></returns>
        public Boolean raceDateBumpVerification(Race race1,Race race2)
        {
            Boolean intersectionCond = intersectionDateValidation(race1.Beginning_Date, race1.Ending_Date, race2.Beginning_Date, race2.Ending_Date)||
                                       intersectionDateValidation(race2.Beginning_Date, race2.Ending_Date, race1.Beginning_Date, race1.Ending_Date);

            Boolean containerCond = containedDateValidation(race1.Beginning_Date, race1.Ending_Date, race2.Beginning_Date, race2.Ending_Date) ||
                                    containedDateValidation(race2.Beginning_Date, race2.Ending_Date, race1.Beginning_Date, race1.Ending_Date);

            return intersectionCond || containerCond;
        }


        /// <summary>
        /// Method designed to create a unique key of 6 characters for a championship
        /// </summary>
        /// <returns><see cref="string"/> object that represents a unique key for a championship</returns>
        public string getChampionshipKey()
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[4];
            rng.GetBytes(buff);

            string key = Convert.ToBase64String(buff);
            key = key.Replace("+", "").Replace("/", "").Substring(0,5);
            while (!uniqueChampionshipKeyVerificator(key))
            {
                buff = new byte[4];
                rng.GetBytes(buff);
                key = Convert.ToBase64String(buff);
                key = key.Replace("+", "").Replace("/", "").Substring(0,5);
            }
            return key;
        }

        /// <summary>
        /// Method designe dto determine if a key is unique among the registered championships keys
        /// </summary>
        /// <param name="key"><see cref="string"/> object that represents the key that must be verified</param>
        /// <returns><see cref="Boolean"/> value that indicates if the key provided is unique among the championship keys</returns>
        public Boolean uniqueChampionshipKeyVerificator(string key)
        {
            return !dbContext.Championships.Any(o => o.Unique_Key == key);
        }

        /// <summary>
        /// Method made to determine if the date of a new championship is valid and that it doesn't intersects with another championship dates.
        /// </summary>
        /// <param name="champ"><see cref="Championship"/> object that contains all the data of the championship to evaluate</param>
        /// <returns><see cref="Boolean"/> object that tells if the dates of the championship are valid</returns>
        public Boolean championshipDateVerifier(Championship champ)
        {
            DateTime actualDate = DateTime.Now;
            Boolean validDatesCond = !(champ.Beginning_Date <= actualDate || champ.Ending_Date < actualDate);
            Boolean consistentDatesCond= champ.Beginning_Date < champ.Ending_Date;

            if (validDatesCond && consistentDatesCond)
            {
                List<Championship> championships = dbContext.Championships.ToList();
                int length = championships.Count;

                for (int i = 0; i < length; i++)
                {
                    Championship tempChamp=championships[i];
                    Boolean intersectionCond = (intersectionDateValidation(champ.Beginning_Date, champ.Ending_Date, tempChamp.Beginning_Date, tempChamp.Ending_Date)
                                               || intersectionDateValidation(tempChamp.Beginning_Date, tempChamp.Ending_Date, champ.Beginning_Date, champ.Ending_Date));

                    Boolean containedCond=(containedDateValidation(champ.Beginning_Date, champ.Ending_Date, tempChamp.Beginning_Date, tempChamp.Ending_Date)
                                          || containedDateValidation(tempChamp.Beginning_Date, tempChamp.Ending_Date, champ.Beginning_Date, champ.Ending_Date));

                    if (intersectionCond || containedCond)
                    {
                        return false;
                    }
                }
            }
            return validDatesCond && consistentDatesCond && validChampionship(champ);
        }
        /// <summary>
        /// Method to determine if two date range intersect.
        /// </summary>
        /// <param name="date1BeginDate"><see cref="DateTime"/> object that represents the first date bound of the first date range</param>
        /// <param name="date1EndDate"><see cref="DateTime"/> object that represents the second date bound of the first date range</param>
        /// <param name="date2BeginDate"><see cref="DateTime"/> object that represents the first date bound of the second date range</param>
        /// <param name="date2EndingDate"><see cref="DateTime"/> object that represents the second date bound of the second date range</param>
        /// <returns><see cref="Boolean"/> object that tells if the two date ranges intersect</returns>
        public Boolean intersectionDateValidation(DateTime date1BeginDate,DateTime date1EndDate,DateTime date2BeginDate, DateTime date2EndDate)
        {
            return date1BeginDate >= date2BeginDate && date1BeginDate < date2EndDate;
        }

        /// <summary>
        /// Method that determines if one date range contains another.
        /// </summary>
        /// <param name="date1BeginDate"><see cref="DateTime"/> object that represents the first date bound of the first date range</param>
        /// <param name="date1EndDate"><see cref="DateTime"/> object that represents the second date bound of the first date range</param>
        /// <param name="date2BeginDate"><see cref="DateTime"/> object that represents the first date bound of the second date range</param>
        /// <param name="date2EndingDate"><see cref="DateTime"/> object that represents the second date bound of the second date range</param>
        /// <returns><see cref="Boolean"/> object that tells if the date range 2 contains the date range 1</returns>
        public Boolean containedDateValidation(DateTime date1BeginDate, DateTime date1EndDate, DateTime date2BeginDate, DateTime date2EndingDate)
        {
            return date1BeginDate >= date2BeginDate && date1EndDate <= date2EndingDate;
        }

        /// <summary>
        /// Method that determines if the beginning and ending times for a championship are valid
        /// </summary>
        /// <param name="champ"><see cref="Championship"/> object that contains the championship information</param>
        /// <returns><see cref="Boolean"/> object that tells if the championship times are correct</returns>
        public Boolean championshipTimeVerifier(Championship champ)
        {
            return champ.Beginning_Time<champ.Ending_Time;
        }

        /// <summary>
        /// Method that tells if a championship begin date is after the current championship end date
        /// </summary>
        /// <param name="champ"><see cref="Championship"/> object that contains the championship information</param>
        /// <returns><see cref="Boolean"/> object that tells if the championship times are correct</returns>
        public Boolean validChampionship(Championship champ)
        {
            Championship currentChampionship = null;
            try
            {
                currentChampionship = dbContext.Championships.First(o => o.CurrentChamp == true);
            }
            catch (Exception e)
            {
                return true;
            }
            return currentChampionship.Ending_Date <= champ.Beginning_Date;
        }
    }
}