using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class PilotLogic
    {
        public XF1OnlineEntities dbContext;
        public Tools tools;

        public PilotLogic()
        {
            dbContext = new XF1OnlineEntities();
            tools = new Tools();
        }

        public HttpResponseMessage getAllPilots(string token, string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Pilots)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage addPilot(string token, string salt, Pilot pilot)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                errors.addError("The name provided is already taken", tools.verifyPilotName(pilot));
                errors.purgeErrorsList();
                if (!errors.hasErrors())
                {
                    dbContext.Pilots.Add(pilot);
                    dbContext.SaveChanges();

                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Pilot addedd succesfully") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage deletePilot(string token, string salt, Pilot pilot)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                errors.addError("The specified pilot does not exists", !tools.verifyPilotName(pilot));
                errors.purgeErrorsList();
                if (!errors.hasErrors())
                {
                    dbContext.Pilots.Remove(pilot);
                    dbContext.SaveChanges();

                }
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Racing team addedd succesfully") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage getPage(string token, string salt, int amount, int page)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                List<Pilot> pilotsPage = dbContext.Pilots.OrderByDescending(o => o.Price).Skip(page * amount).Take(amount).ToList();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(pilotsPage)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }

        public HttpResponseMessage searchPilots(string token, string salt, int amount, int page,string name,string racing_team)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator") || tools.verifyToken(token, salt, "Player"));
            if (!errors.hasErrors())
            {
                
                var pilotsPage = dbContext.SP_filterPilots(name,racing_team).OrderByDescending(o => o.Price).Skip(page * amount).Take(amount).ToList();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(pilotsPage)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
    }
}