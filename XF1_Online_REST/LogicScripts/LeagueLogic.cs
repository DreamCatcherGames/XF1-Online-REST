using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class LeagueLogic
    {

        private XF1OnlineEntities dbContext;
        private Tools tools;

        public LeagueLogic()
        {
            this.dbContext = new XF1OnlineEntities();
            this.tools = new Tools();
        }


        public League getLeagueByCode(string code)
        {
            return dbContext.Leagues.Find(code);
        }
        public HttpResponseMessage getPage(string leagueId, string token, string salt, int amount, int page)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                Boolean existingLeague = dbContext.Leagues.Find(leagueId) != null;
                errors.addError("The given ID is not related to any of the registered leagues",existingLeague);
                if(existingLeague)
                {
                    List<Score> scorePage = dbContext.Scores.Where(o => o.League_Key == leagueId).OrderByDescending(o => o.Points).Skip(page * amount).Take(amount).ToList();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(scorePage)) };
                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage getPublicLeague(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                Championship champ = dbContext.Championships.FirstOrDefault(o => o.CurrentChamp == true);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Leagues.Find(champ.Unique_Key))) };

            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage getLeagueById(League league, string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                League tempLeague = dbContext.Leagues.Find(league.Unique_Key);
                errors.addError("The given identifier does not correspond to any of the registered leagues", tempLeague != null);
                if(tempLeague!=null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(tempLeague))};
                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage createPrivateLeague(League league, string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                League tempLeague=dbContext.Leagues.FirstOrDefault(o=>o.Name==league.Name&& o.Championship.CurrentChamp==true);
                errors.addError("The specified name for the league is already taken", tempLeague==null);
                if(tempLeague==null)
                {
                    Player owner = tools.getPlayerByToken(token, salt);
                    Boolean privateLeaguesVerification = tools.playerPrivateLeaguesVerification(owner);
                    errors.addError("The user already owns or belongs to a private league",privateLeaguesVerification);
                    if(privateLeaguesVerification)
                    {
                        league.Unique_Key = tools.getLeagueKey();
                        league.Type = "Private";
                        league.OwnerUsername = owner.Username;
                        dbContext.Leagues.Add(league);

                        dbContext.SaveChanges();

                        Score score = new Score();
                        score.League_Key = league.Unique_Key;
                        score.Username = owner.Username;

                        dbContext.Scores.Add(score);

                        dbContext.SaveChanges();
                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(league.Unique_Key) };

                    }
                    

                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
    }
}