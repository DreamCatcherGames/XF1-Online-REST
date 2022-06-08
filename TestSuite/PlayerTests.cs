using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using XF1_Online_REST.LogicScripts;
using System.Net;
using XF1_Online_REST;
using XF1_Online_REST.LogicScript;
using System.Linq;
using System.Collections.Generic;

namespace TestSuite
{
    [TestClass]
    public class PlayerTests
    {
        Player player = new Player() 
        {
            Username="Testing1",
            Active=false,
            Country="China",
            First_Name="Ligma",
            Last_Name="Balls",
            Email="Testing123@mail.com",
            Money=10000000,
            Encrypted_Password="1234"
        };

        PlayerLogic logic = new PlayerLogic();
        Tools tools= new Tools();
        XF1OnlineEntities dbContext= new XF1OnlineEntities();

        [TestMethod]
        public void AddPlayerToPublicLeagueTest()
        {
            logic.registerRequest(player);
            Verification_Request request = dbContext.Verification_Request.FirstOrDefault(o => o.Username == player.Username);
            logic.verificationRequest(request);
            player = dbContext.Players.Find(player.Username);
            
            League league = dbContext.Leagues.FirstOrDefault(o => o.Championship.CurrentChamp&&o.Unique_Key==o.Championship.Unique_Key);
            Score score = dbContext.Scores.FirstOrDefault(o => o.Username == player.Username);

            dbContext.SP_Delete_Player(player.Username);

            Assert.AreEqual<String>(league.Unique_Key, score.League_Key);
            
        }
    }
}
