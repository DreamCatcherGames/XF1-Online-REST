using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using XF1_Online_REST.LogicScripts;

namespace XF1_Online_REST.LogicScript
{
    public class RaceLogic
    {
        private XF1OnlineEntities dbContext;
        private Tools tools;

        public RaceLogic()
        {
            this.dbContext = new XF1OnlineEntities();
            this.tools = new Tools();
        }

        public HttpResponseMessage raceCreationRequest(Race race,string token)
        {
            if(tools.verifyAdminToken(token))
            {
                if (tools.raceDateVerifier(race))
                {
                    dbContext.Races.Add(race);
                    dbContext.SaveChangesAsync();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Race added successfully") };
                }
                return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("The dates given for this race are wrong or they are not inside the championship date range")};
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
    }
}