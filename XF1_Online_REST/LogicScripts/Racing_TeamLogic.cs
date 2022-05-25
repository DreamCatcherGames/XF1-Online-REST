using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class Racing_TeamLogic
    {
        public XF1OnlineEntities dbContext;
        public Tools tools;

        public Racing_TeamLogic()
        {
            dbContext = new XF1OnlineEntities();
            tools = new Tools();
        }

        public HttpResponseMessage getAllTeams(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Racing_Team)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage addRacingTeam(string token, string salt, Racing_Team team)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                errors.addError("The name provided is already taken", tools.verifyTeamName(team));
                errors.purgeErrorsList();
                if (!errors.hasErrors())
                {
                    dbContext.Racing_Team.Add(team);
                    dbContext.SaveChanges();

                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Racing team addedd succesfully") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage deleteRacingTeam(string token, string salt, Racing_Team team)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                errors.addError("The specified team does not exists", !tools.verifyTeamName(team));
                errors.purgeErrorsList();
                if (!errors.hasErrors())
                {
                    dbContext.Racing_Team.Remove(team);
                    dbContext.SaveChanges();

                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Racing team addedd succesfully") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public HttpResponseMessage getPage(string token, string salt, int amount, int page)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator")||tools.verifyToken(token,salt,"Player"));
            if (!errors.hasErrors())
            {
                List<Racing_Team> teamsPage=dbContext.Racing_Team.OrderByDescending(o =>o.Price).Skip(page*amount).Take(amount).ToList();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(teamsPage))};
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
    }
}