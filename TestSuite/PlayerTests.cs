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
        Player player1 = new Player() 
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

        PlayerLogic logic = new PlayerLogic();
        Tools tools= new Tools();
        XF1OnlineEntities dbContext= new XF1OnlineEntities();

        [TestMethod]
        public void AddPlayerToPublicLeagueTest()
        {
            logic.registerRequest(player1);
            Verification_Request request = dbContext.Verification_Request.FirstOrDefault(o => o.Username == player1.Username);
            logic.verificationRequest(request);
            player1 = dbContext.Players.Find(player1.Username);
            
            League league = dbContext.Leagues.FirstOrDefault(o => o.Championship.CurrentChamp&&o.Unique_Key==o.Championship.Unique_Key);
            Score score = dbContext.Scores.FirstOrDefault(o => o.Username == player1.Username);

            dbContext.SP_Delete_Player(player1.Username);
            resetPlayers();

            Assert.AreEqual<String>(league.Unique_Key, score.League_Key);
            
        }

        [TestMethod]
        public void duplicateEmailTest()
        {
            logic.registerRequest(player1);
            player2.Email = player1.Email;
            
            HttpResponseMessage response = logic.registerRequest(player2);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);

            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            resetPlayers();
        }

        [TestMethod]
        public void duplicateUsernameTest()
        {
            logic.registerRequest(player1);
            player2.Username = player1.Username;

            HttpResponseMessage response = logic.registerRequest(player2);

            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);

            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
            resetPlayers();
        }

        [TestMethod]
        public void teamsUpdateTest()
        {
            logic.registerRequest(player1);
            logic.registerRequest(player2);

            Verification_Request request1 = dbContext.Verification_Request.FirstOrDefault(o => o.Username == player1.Username);
            logic.verificationRequest(request1);

            Verification_Request request2 = dbContext.Verification_Request.FirstOrDefault(o => o.Username == player2.Username);
            logic.verificationRequest(request2);

            player1 = new Player()
            {
                Username = player1.Username
            };


            List<Pilot> pilots1 = dbContext.Pilots.OrderByDescending(o => o.Price).Take(5).ToList();
            Team team1= new Team();
            Team team2= new Team();
            Team team3 = new Team();
            Team team4 = new Team();

            team1.Username = player1.Username;
            team1.Name = "Team 1";

            team2.Username = player1.Username;
            team2.Name = "Team 2";


            foreach (Pilot pilot in pilots1)
            {
                team1.Pilots.Add(pilot);
                team2.Pilots.Add(pilot);
                team3.Pilots.Add(pilot);
                team4.Pilots.Add(pilot);
            }

            player1.Teams.Clear();
            player1.Teams.Add(team1);
            player1.Teams.Add(team2);

            player2.Teams.Clear();
            player2.Teams.Add(team3);
            player2.Teams.Add(team4);



            logic.updatePlayerTeams(player1);

            List<Pilot> pilots2 = dbContext.Pilots.OrderByDescending(o => o.Price).Skip(5).Take(5).ToList();
            team1.Pilots.Clear();
            team2.Pilots.Clear();

            foreach (Pilot pilot in pilots2)
            {
                team1.Pilots.Add(pilot);
                team2.Pilots.Add(pilot);
            }

            logic.updatePlayerTeams(player1);

            player1 = dbContext.Players.Find(player1.Username);
            int ind = 0;
            foreach(Team team in player2.Teams)
            { 
                Team team1Test = player1.Teams.ElementAt(ind);
                int ind2 = 0;
                foreach(Pilot pilot in team.Pilots)
                {
                    Assert.AreNotEqual(pilot.Name, team1Test.Pilots.ElementAt(ind2));
                    ind2++;
                }
                ind++;
            }
            dbContext.SP_Delete_Player(player1.Username);
            dbContext.SP_Delete_Player(player2.Username);
            resetPlayers();
        }

        public void resetPlayers()
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
        }

    }
}
