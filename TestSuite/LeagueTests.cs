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
    public class LeagueTests
    {
        Player player1 = new Player()
        {
            Username = "Testing1",
            Active = false,
            Country = "China",
            First_Name = "Ligma",
            Last_Name = "Balls",
            Email = "Testing123@mail.com",
            Money = 10000000,
            Encrypted_Password = "1234"
    };

        Player player2 = new Player()
        {
            Username = "Testing2",
            Active = false,
            Country = "China",
            First_Name = "Ligma",
            Last_Name = "Balls",
            Email = "Testing456@mail.com",
            Money = 10000000,
            Encrypted_Password = "1234"

        };

        string token1 = "";
        string token2 = "";

        LeagueLogic leagueLogic = new LeagueLogic();
        PlayerLogic playerLogic = new PlayerLogic();

        Tools tools = new Tools();
        XF1OnlineEntities dbContext = new XF1OnlineEntities();



        [TestMethod]
        public void createLeagueTest()
        {
            assignTokens();

            createLeague(true);

            League league = dbContext.Leagues.FirstOrDefault(o => o.OwnerUsername == player1.Username);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();

            Assert.IsNotNull(league);

        }

        [TestMethod]
        public void failedCreateLeagueAlreadyBelongsToPrivateLeagueTest()
        {
            assignTokens();
            joinLeague(null);

            League league = createLeague(false);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();

            Assert.IsNotNull(league);
        }

        [TestMethod]
        public void failedCreateLeagueSameName()
        {
            assignTokens();
            createLeague(false);

            League league=createLeague(true);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();

            Assert.IsNotNull(league);
        }

        [TestMethod]
        public void joinToLeagueTest()
        {
            assignTokens();
            Score score=joinLeague(null);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();

            Assert.IsNotNull(score);

        }

        [TestMethod]
        public void failedJoinToLeagueAlreadyBelongsToPrivateLeagueTest()
        {
            assignTokens();
            joinLeague(null);

            Score score = joinLeague(null);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();

            Assert.IsNull(score);
        }

        [TestMethod]
        public void failedJoinToLeagueWrongCode()
        {
            assignTokens();
            Score score = joinLeague("123");

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();

            Assert.IsNull(score);
        }
        public void assignTokens()
        {
            playerLogic.registerRequest(player1);
            playerLogic.registerRequest(player2);

            token1 = tools.getToken(player1.Salt);
            token2 = tools.getToken(player2.Salt);

            player1 = dbContext.Players.Find(player1.Username);
            player2 = dbContext.Players.Find(player2.Username);

            player1.Token = tools.encryptToken(token1, player1.Salt);
            player2.Token = tools.encryptToken(token2, player2.Salt);

            dbContext.SaveChanges();
        }
        public void resetPlayers()
        {
            player1 = new Player()
            {
                Username = "Testing1",
                Active = false,
                Country = "China",
                First_Name = "Ligma",
                Last_Name = "Balls",
                Email = "Testing123@mail.com",
                Money = 10000000,
                Encrypted_Password = "1234"
            };

            player2 = new Player()
            {
                Username = "Testing2",
                Active = false,
                Country = "China",
                First_Name = "Ligma",
                Last_Name = "Balls",
                Email = "Testing456@mail.com",
                Money = 10000000,
                Encrypted_Password = "1234"
            };
        }

        public Score joinLeague(String joinCode)
        {
            League league = createLeague(true);
            if (joinCode != null)
            {
                league = dbContext.Leagues.Find(joinCode);

            }
            HttpStatusCode code= leagueLogic.joinLeagueRequest(league, token2, player2.Salt).StatusCode;

            if (code.Equals(HttpStatusCode.OK))
            {
                Notification notification = dbContext.Notifications.Where(o => o.Username == player1.Username).ToList().ElementAt(0);

                

                leagueLogic.approveJoin(notification, token1, player1.Salt, true);

                return dbContext.Scores.FirstOrDefault(o => o.Username == player2.Username && o.League_Key == league.Unique_Key);
            }
            return null;
        }
        public League createLeague(Boolean player1Cond)
        {
            League league = new League()
            {
                Name = "Testing League 1"
            };

            if (player1Cond)
            {
                leagueLogic.createPrivateLeague(league, token1, player1.Salt);
            }
            else
            {
                leagueLogic.createPrivateLeague(league, token2, player2.Salt);
            }

            return league;
        }
    }
}
