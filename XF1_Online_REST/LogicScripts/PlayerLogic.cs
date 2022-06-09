using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class PlayerLogic
    {
        private XF1OnlineEntities dbContext;
        private Tools tools;

        public PlayerLogic()
        {
            this.dbContext = new XF1OnlineEntities();
            this.tools = new Tools();
        }

        /// <summary>
        /// Function designed to implement the login process of a Player
        /// </summary>
        /// <param name="player"><see cref="Player"> object that contains the login credentials</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>
        public HttpResponseMessage loginRequest(Player player)
        {
            Player testPlayer = dbContext.Players.Find(player.Username);

            Error_List errors = new Error_List();
            errors.addError("Incorrect username or password", testPlayer != null);

            if (testPlayer != null)
            {
                Boolean correctPasswordCond = tools.verifyPassword(player.Encrypted_Password, testPlayer.Encrypted_Password, testPlayer.Salt);
                errors.addError("Incorrect username or password", correctPasswordCond);
                if (!errors.hasErrors())
                {
                    string token = tools.getToken(player.Salt);
                    tools.assignToken(player, token);
                    player = dbContext.Players.Find(player.Username);
                    player.Token = token;
                    player.Encrypted_Password = "";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(player)) };
                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        /// <summary>
        /// Function designed to implement the registration process of a Player
        /// </summary>
        /// <param name="player"><see cref="Player"> object that contains the player information</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>
        public HttpResponseMessage registerRequest(Player player)
        {
            Error_List errors = new Error_List();
            Boolean avaliableUsernameCond = !dbContext.Players.Any(o => o.Username == player.Username);
            Boolean avaliableEmailCond = !dbContext.Players.Any(o => o.Email == player.Email);

            errors.addError("Email already taken!", avaliableEmailCond);
            errors.addError("Username already taken!", avaliableUsernameCond);
            errors.purgeErrorsList();

            if (!errors.hasErrors())
            {
                player.Active = false;
                player.Money = 10000000;

                List<string> passwordComponents = tools.passwordEncryptor(player.Encrypted_Password);

                player.Encrypted_Password = passwordComponents[0];
                player.Salt = passwordComponents[1];
                player.Token = tools.getToken(player.Salt);

                dbContext.Players.Add(player);
                dbContext.SaveChanges();

                tools.sendEmail(player);

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Player registered succesfully! Please check your email") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage verificationRequest(Verification_Request ver_request)
        {
            Error_List errors = new Error_List();
            Verification_Request temprequest = dbContext.Verification_Request.FirstOrDefault(o => o.Token == ver_request.Token);
            errors.addError("Invalid token", temprequest != null);
            if (!errors.hasErrors())
            {
                Player player = dbContext.Players.Find(temprequest.Username);
                player.Active = true;
                dbContext.Verification_Request.Remove(temprequest);
                
                createInitialTeam(player, "1");
                createInitialTeam(player, "2");

                try
                {
                    Championship champ = dbContext.Championships.FirstOrDefault(o => o.CurrentChamp == true);
                    Score score = new Score();
                    score.League_Key = champ.Unique_Key;
                    score.Points = 0;
                    score.Username=player.Username;
                    dbContext.Scores.Add(score);
                }
               
                catch (Exception ex)
                {

                }

                dbContext.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Player verified succesfully!") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public void createInitialTeam(Player player, string teamNo)
        {
            Team team = new Team();
            team.Name = "Team " + teamNo;
            player.Teams.Add(team);

            dbContext.SaveChanges();
        }

        public HttpResponseMessage getProfile(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(tools.getPlayerByToken(token, salt))) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage updateProfile(string token, string salt, Player player)
        {
            Error_List errors = new Error_List();
            Player tempPlayer = tools.getPlayerByToken(token,salt);
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player")&&tempPlayer.Token==tools.encryptToken(token,salt));

            if (!errors.hasErrors())
            {
                tempPlayer = dbContext.Players.Find(player.Username);
                tempPlayer.First_Name = player.First_Name;
                tempPlayer.Last_Name = player.Last_Name;
                tempPlayer.Country = player.Country;
                tempPlayer.Money = player.Money;
                dbContext.SaveChanges();

                updatePlayerTeams(player);

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Profile updated succesfully!") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public void updatePlayerTeams(Player player)
        {
            Player tempPlayer=dbContext.Players.Find(player.Username);

            tempPlayer.Teams.ToList()[0].Pilots.Clear();
            tempPlayer.Teams.ToList()[1].Pilots.Clear();

            dbContext.SP_AddRacing_TeamPlayer("Team 1", player.Username, player.Teams.ToList()[0].Racing_Team_Name);
            dbContext.SP_AddRacing_TeamPlayer("Team 2", player.Username, player.Teams.ToList()[1].Racing_Team_Name);

            foreach (Team team in player.Teams)
            {
                foreach(Pilot pilot in team.Pilots)
                {
                    dbContext.SP_AddPilotsPlayer(team.Name, team.Username, pilot.Name);
                }
                
            }

        }

        public HttpResponseMessage hasNotifications(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));

            if (!errors.hasErrors())
            {
                Player tempPlayer = tools.getPlayerByToken(token, salt);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tempPlayer.HasNotifications.ToString()) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage getNotifications(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));

            if (!errors.hasErrors())
            {
                Player tempPlayer = tools.getPlayerByToken(token, salt);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(tempPlayer.UserNotifications)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage deleteNotification(Notification notification, string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));

            if (!errors.hasErrors())
            {
                tools.deleteNotification(notification);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Notification deleted succesfully")};
                
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage getPrivateLeague(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));

            if (!errors.hasErrors())
            {
                Player player = tools.getPlayerByToken(token, salt);
                List<Score> scores = dbContext.Scores.Where(o => o.Username == player.Username).ToList();
                foreach (Score score in scores)
                {
                    League league = dbContext.Leagues.Find(score.League_Key);
                    if (league.Championship.CurrentChamp && league.Type == "Private")
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(league)) };
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(null)) };

            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage deletePlayer(string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));

            if (!errors.hasErrors())
            {
                Player player = tools.getPlayerByToken(token, salt);
                dbContext.Players.Remove(player);
                dbContext.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Player deleted succesfully") };

            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
    }
}