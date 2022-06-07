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

        struct leaguePos
        {
            public int position;
            public int leaguePlayerCount;
        }

        public League getLeagueByCode(string code)
        {
            return dbContext.Leagues.Find(code);
        }
        public HttpResponseMessage getPage(string leagueId,string token, string salt, int amount, int page)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                List<Score> scorePage = dbContext.Scores.Where(o=>o.League_Key==leagueId).OrderByDescending(o => o.Points).Skip(page * amount).Take(amount).ToList();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(scorePage)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage getPublicLeague(string token,string salt)
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

        public HttpResponseMessage getLeaguePos(string leagueId,string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                League league;
                if(leagueId=="Public")
                {
                    league = this.dbContext.Leagues.FirstOrDefault(o => o.Championship.CurrentChamp == true && o.Unique_Key==o.Championship.Unique_Key);
                }
                else 
                {
                    league = this.dbContext.Leagues.Find(leagueId);
                }

                errors.addError("The specified league does not currently exists",league!=null);
                if(league!=null)
                {
                    Player player = tools.getPlayerByToken(token, salt);
                    List<Score> scores=dbContext.Scores.Where(o=>o.League_Key==league.Unique_Key).ToList();

                    leaguePos leaguePosPlayer = new leaguePos();

                    leaguePosPlayer.position=scores.FindIndex(o => o.Username == player.Username)+1;
                    leaguePosPlayer.leaguePlayerCount=scores.Count;

                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(leaguePosPlayer))};

                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage getPrivateLeagues(string token,string salt)
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
    }
}