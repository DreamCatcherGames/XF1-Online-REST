using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public HttpResponseMessage championshipCreationRequest(Championship champ,string token,string salt)
        {
            if (tools.verifyAdminToken(token,salt))
            {
                if (tools.championshipDateVerifier(champ))
                {
                    if (tools.championshipTimeVerifier(champ))
                    {
                        champ.Unique_Key = tools.getChampionshipKey();
                        champ.Public_League_Name = champ.Name + " Public League";
                        champ.CurrentChamp = true;

                        try
                        {
                            dbContext.Championships.First(o => o.CurrentChamp == true).CurrentChamp = false;
                        }
                        catch (Exception ex)
                        {
                            
                        }
                        dbContext.Championships.Add(champ);

                        dbContext.SaveChangesAsync();

                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Championship added successfully") };
                    }
                    return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("The times provided for this championship are not correct") };
                }
                return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("Dates provided bump into another championship dates or they are not correct") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }

    }
}