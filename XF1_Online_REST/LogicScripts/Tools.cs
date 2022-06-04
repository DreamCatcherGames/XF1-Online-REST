using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        /// Method to get an unique token for a Player or Administrator
        /// </summary>
        /// <returns><see cref="string"/> object that represents a unique token </returns>
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
        /// Method to get an unique token
        /// </summary>
        /// <returns><see cref="string"/> object that represents a unique token </returns>
        public string generateVerificationToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            token = token.Replace("+", "").Replace("/", "").Replace("=", "");
            while (dbContext.Verification_Request.Any(o=>o.Token==token))
            {
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                token = token.Replace("+", "").Replace("/", "").Replace("=","");
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
            return !dbContext.Administrators.Any(o => o.Token == encryptedToken)&&!dbContext.Players.Any(o=>o.Token==encryptedToken);
        }

        /// <summary>
        /// Method to assign a unique token to a <see cref="Administrator"/>/<see cref="User"/> inside the database
        /// </summary>
        /// <param name="user"><see cref="Object"/> that contains the credentials of the <see cref="Administrator"/>/<see cref="Player"/> that the token has to be assigned to</param>
        /// <param name="token"><see cref="string"/> that represents the token needed to be assigned</param>
        public void assignToken(Object user,string token)
        {
            if(user is Administrator)
            {
                Administrator admin= (Administrator)user;
                admin = dbContext.Administrators.Find(admin.Username);
                admin.Token = encryptToken(token,admin.Salt);
                
            }
            else if (user is Player)
            {
                Player player= (Player)user;
                player = dbContext.Players.Find(player.Username);
                player.Token = encryptToken(token,player.Salt);
            }
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Method to verify if an admin token is valid
        /// </summary>
        /// <param name="token"><see cref="string"/> object that represents the token needed to be verified</param>
        /// <returns><see cref="Boolean"/> object that determines if the token was found</returns>
        public Boolean verifyToken(string token,string salt,string type)
        {
            string tempToken=encryptToken(token,salt);
            if (type == "Administrator")
            {
                return dbContext.Administrators.Any(o => o.Token == tempToken);
            }
            return dbContext.Players.Any(o => o.Token == tempToken);
        }

        public Player getPlayerByToken(string token,string salt)
        {
            string tempToken = encryptToken(token, salt);

            return dbContext.Players.FirstOrDefault(o => o.Token == tempToken);
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
            salt=salt.Replace("+", "").Replace("/", "");

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
        public Error_List raceDateVerifier(Race race)
        {
            Championship champ = dbContext.Championships.Find(race.Champ_Key);

            Error_List errors = new Error_List();

            DateTime actualDate=DateTime.Now;
            Boolean validDatesCond=race.Beginning_Date>=actualDate&&race.Ending_Date>=actualDate;
            Boolean insideChampionshipCond = containedDateValidation(race.Beginning_Date, race.Ending_Date,champ.Beginning_Date,champ.Ending_Date);
            Boolean insideValidDateRangeCond = containedDateValidation(race.Qualification_Date,race.Competition_Date,race.Beginning_Date,race.Ending_Date);

            errors.addError("The beggining/ending date of the race is past to the present date", validDatesCond);
            errors.addError("The ending date of the race is past it's beggining date", race.Beginning_Date < race.Ending_Date);
            errors.addError("The Qualification date of the race is past it's Competition date", race.Qualification_Date < race.Competition_Date);
            errors.addError("The race beginning/ending date is outside it's championship timeline", insideChampionshipCond);
            errors.addError("The race qualification/competition date is outside it's championship timeline",insideValidDateRangeCond);

            errors.purgeErrorsList();

            if(!errors.hasErrors())
            {
                List<Race> races = dbContext.Races.Where(o => o.Champ_Key==champ.Unique_Key).ToList<Race>();
                foreach(Race raceT in races)
                {
                    Error_List raceDateBumpErrors = raceDateBumpVerification(race, raceT);
                    raceDateBumpErrors.purgeErrorsList();
                    if (raceDateBumpErrors.hasErrors())
                    {
                        errors.fuse(raceDateBumpErrors);
                        break;
                    }
                }
            }
            errors.purgeErrorsList();
            return errors;
        }
        /// <summary>
        /// Method to verify if there's any kind of intersection between two races date range.
        /// </summary>
        /// <param name="race1"><see cref="Race"/> object that contains the date range of the first race to be analyzed</param>
        /// <param name="race2"><see cref="Race"/> object that contains the date range of the second race to be analyzed</param>
        /// <returns></returns>
        public Error_List raceDateBumpVerification(Race race1,Race race2)
        {
            Error_List errors= new Error_List();
            Boolean intersectionCond = intersectionDateValidation(race1.Beginning_Date, race1.Ending_Date, race2.Beginning_Date, race2.Ending_Date)||
                                       intersectionDateValidation(race2.Beginning_Date, race2.Ending_Date, race1.Beginning_Date, race1.Ending_Date);

            Boolean containerCond = containedDateValidation(race1.Beginning_Date, race1.Ending_Date, race2.Beginning_Date, race2.Ending_Date) ||
                                    containedDateValidation(race2.Beginning_Date, race2.Ending_Date, race1.Beginning_Date, race1.Ending_Date);

            errors.addError("The race beginning/ending date is inside another race timeline", intersectionCond);
            errors.addError("The race timeline is inside another race timeline or it contains another race timeline", containerCond);

            return errors;
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
            return !dbContext.Championships.Any(o => o.Unique_Key == key)&&!dbContext.Leagues.Any(o=> o.Unique_Key==key);
        }

        /// <summary>
        /// Method made to determine if the date of a new championship is valid and that it doesn't intersects with another championship dates.
        /// </summary>
        /// <param name="champ"><see cref="Championship"/> object that contains all the data of the championship to evaluate</param>
        /// <returns><see cref="Error_List"/> object that contains all the possible errors related to a championship dates</returns>
        public Error_List championshipDateVerifier(Championship champ)
        {
            DateTime actualDate = DateTime.Now;

            Error_List errors = new Error_List();

            Boolean validDatesCond = !(champ.Beginning_Date <= actualDate || champ.Ending_Date < actualDate);
            Boolean consistentDatesCond= champ.Beginning_Date < champ.Ending_Date;
            Boolean validChampionshipCond= validChampionship(champ);

            errors.addError("The beginning/ending date of the championship is past to the present date",validDatesCond);
            errors.addError("The ending date of the championship is past to it's beginning date", consistentDatesCond);
            errors.addError("The championship that is being created is before the ending date of the current championship",validChampionshipCond);

            if (!errors.hasErrors())
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
                        errors.addError("The championship beginning/ending date is inside of the timeline of another registered championship",!intersectionCond);
                        errors.addError("The championship timeline is totally inside of another championship timeline or contains another championship timeline", !containedCond);
                    }
                }
            }
            return errors;
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

        public void sendEmail(Player player)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("xf1online2022@gmail.com", "XF1 Team");
            mail.To.Add(player.Email);

            Verification_Request request= new Verification_Request();
            request.Username=player.Username;
            request.Token = generateVerificationToken();

            mail.Subject = "Email Confirmation";
            mail.Body = "Hi " + player.First_Name + "! We are thrilled that you're joining us to experience the ultimate F1 fantasy game: XF1 Online!\n\n" +
                        "Click on the next link to confirm your email: \n\n" +
                        "http://dreamcatchergames.github.io/XF1-online/emailVerification/" + request.Token+"\n\n" +
                        "See you on the track!\n\n" +
                        "XF1 Team.";
            
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("xf1online2022@gmail.com", "W4<b^UZd9");
            smtpServer.EnableSsl = true;

            smtpServer.SendMailAsync(mail);

            dbContext.Verification_Request.Add(request);
            dbContext.SaveChanges();
        }

        public Boolean verifyTeamName(Racing_Team team)
        {
            return !dbContext.Racing_Team.Any(o => o.Name == team.Name);
        }

        public Boolean verifyPilotName(Pilot pilot)
        {
            return !dbContext.Pilots.Any(o => o.Name == pilot.Name);
        }

        public Boolean playerPrivateLeaguesVerification(Player player)
        {
            List<Score> scores= dbContext.Scores.Where(o=>o.Username==player.Username).ToList();
            int leagues = 0;
            foreach(Score score in scores)
            {
                Championship championship = dbContext.Leagues.Find(score.League_Key).Championship;

                if (championship.CurrentChamp) {leagues++;}
                
            }
            return leagues < 2;

        }

        public string getLeagueKey()
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[8];
            rng.GetBytes(buff);

            string key = Convert.ToBase64String(buff);
            key = key.Replace("+", "").Replace("/", "").Substring(0, 10);
            while (!uniqueChampionshipKeyVerificator(key))
            {
                buff = new byte[4];
                rng.GetBytes(buff);
                key = Convert.ToBase64String(buff);
                key = key.Replace("+", "").Replace("/", "").Substring(0, 10);
            }
            return key;
        }
    }
}