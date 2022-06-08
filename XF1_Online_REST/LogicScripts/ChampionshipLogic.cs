using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.LogicScript
{
    public class ChampionshipLogic
    {
        public XF1OnlineEntities dbContext;
        public Tools tools;

        public ChampionshipLogic()
        {
            dbContext = new XF1OnlineEntities();
            tools = new Tools();
        }
        /// <summary>
        /// Method designed to make the request for the creation of a new championship
        /// </summary>
        /// <param name="champ"><see cref="Championship"/> object that contains the information of the championship to be registered</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage championshipCreationRequest(Championship champ,string token,string salt,Boolean dummy)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                errors.fuse(tools.championshipDateVerifier(champ));
                errors.purgeErrorsList();

                if (!errors.hasErrors())
                {
                    champ.Unique_Key = tools.getChampionshipKey();
                    if (!dummy)
                    {
                        champ.CurrentChamp = true;
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    try
                    {
                        if (!dummy)
                        {
                            dbContext.Championships.First(o => o.CurrentChamp == true).CurrentChamp = false;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
#pragma warning restore CS0168 // Variable is declared but never used
                    dbContext.Championships.Add(champ);

                    League publicLeague = new League();
                    publicLeague.Name = champ.Name + " Public League";
                    publicLeague.Champ_Key = champ.Unique_Key;
                    publicLeague.Unique_Key = champ.Unique_Key;
                    publicLeague.Type = "Public";
                    dbContext.Leagues.Add(publicLeague);
                    dbContext.SaveChanges();
                    addAllPlayersToCurrentPLeague(publicLeague);
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Championship added successfully") };
                }
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        void addAllPlayersToCurrentPLeague(League league)
        {
            foreach(Player player in dbContext.Players)
            {
                Score score = new Score();
                score.Points = 0;
                score.Username = player.Username;
                score.League_Key=league.Unique_Key;
                dbContext.Scores.Add(score);
            }
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Method designed to make the request to delete a championship
        /// </summary>
        /// <param name="champId"><see cref="Championship"/> object that contains the unique key of the championship to delete</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage championshipDeletionRequest(string champId,string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                Championship champ = dbContext.Championships.Find(champId);
                errors.addError("The specified championship does not exists on our records", champ != null);
                if(champ != null)
                {
                    dbContext.Championships.Remove(champ);
                    dbContext.SaveChanges();

                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Championship deleted succesfully") };
                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage championshipRequest(string champId,string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            { 
                Championship champ= dbContext.Championships.Find(champId);
                errors.addError("Championship not found", champ != null);
                if(champ!=null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(champ)) };
                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        /// <summary>
        /// Method designed to obtain all the championships registered on the database
        /// </summary>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage allChampionshipsRequest(string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Championships.ToList()))};

            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        /// <summary>
        /// Method designed to fetch the current championship
        /// </summary>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage currentChampionshipRequest(string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                Championship currentChamp;
                try
                {
                    currentChamp = dbContext.Championships.FirstOrDefault(o => o.CurrentChamp == true);
                }
                catch (Exception ex)
                {
                    currentChamp = null;
                }

                errors.addError("There's no current championship for now", currentChamp != null);
                if (currentChamp != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(currentChamp)) };
                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage notCurrentChampionshipRequest(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Championships.Where(o => o.CurrentChamp==false).ToList())) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
    }
}