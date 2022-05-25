using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}