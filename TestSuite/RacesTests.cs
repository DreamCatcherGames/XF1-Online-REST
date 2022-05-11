using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using XF1_Online_REST.LogicScripts;
using System.Net;
using XF1_Online_REST;
using XF1_Online_REST.LogicScript;

namespace TestSuite
{
    [TestClass]
    public class RacesTests
    {
        RaceLogic logic = new RaceLogic();
    
        Administrator admin = new Administrator("champTestAdmin", "admin1234", "dkQhxuDOS02Z1jZJ2KRpng==", "YGXMKyXDIemAKw==");
        Race race1 = new Race();

        /// <summary>
        /// Test method designed to assert when an unvalid token is used to create a new race
        /// </summary>
        [TestMethod]
        public void badTokenRaceCreationRequestTest()
        {
            Race race1 = new Race();
            HttpResponseMessage response = logic.raceCreationRequest(race1, "badToken", "badSalt");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        /// <summary>
        /// Test method dsigned to assert when a race is succesfully created
        /// </summary>
        [TestMethod]
        public void okRaceCreation()
        {
            race1 = new Race();

            race1.Name = "GolloCumbia Race";
            race1.Champ_Key="VkZfD"; //Siempre hay que actualizarlo antes de correr el TEST
            race1.Country = "Costa Rica";
            race1.Track_Name="Circuito SUR";
            race1.Beginning_Date = new DateTime(2022,12,25);
            race1.Ending_Date = new DateTime(2022,12,26);
            race1.Beginning_Time = new TimeSpan(7,0,0);
            race1.Ending_Time = new TimeSpan(15,0,0);
            race1.Qualification_Date = new DateTime(2022,12,25);
            race1.Competition_Date = new DateTime(2022, 12, 26);

            HttpResponseMessage response = logic.raceCreationRequest(race1, admin.Token, admin.Salt);
            logic.raceDeletionRequest(race1, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Test method designed to assert when a race is being created in the past
        /// </summary>
        [TestMethod]
        public void notConsistentDatesTest()
        {
            race1 = new Race();
            race1.Champ_Key = "VkZfD"; //Siempre hay que actualizarlo antes de correr el TEST

            race1.Beginning_Date = DateTime.Now.AddDays(-2);
            race1.Ending_Date= DateTime.Now.AddDays(1);

            HttpResponseMessage response1 = logic.raceCreationRequest(race1, admin.Token, admin.Salt);

            race1.Beginning_Date = DateTime.Now.AddDays(3);
            race1.Ending_Date = DateTime.Now.AddDays(1);

            HttpResponseMessage response2 = logic.raceCreationRequest(race1, admin.Token, admin.Salt);

            race1.Beginning_Date = DateTime.Now.AddDays(1);
            race1.Ending_Date = DateTime.Now.AddDays(3);
            race1.Qualification_Date = DateTime.Now.AddDays(1);
            race1.Competition_Date = DateTime.Now.AddDays(2);

            HttpResponseMessage response3 = logic.raceCreationRequest(race1, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.Conflict, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.Conflict, response2.StatusCode);
            Assert.AreEqual(HttpStatusCode.Conflict, response3.StatusCode);
              
        }

        /// <summary>
        /// Test method designed to assert when a new race is not in the date range of its championship
        /// </summary>
        [TestMethod]
        public void notInsideChampionshipDateTest()
        {
            race1 = new Race();
            race1.Champ_Key = "VkZfD"; //Siempre hay que actualizarlo antes de correr el TEST

            race1.Beginning_Date = new DateTime(2022,11,25);
            race1.Ending_Date = new DateTime(2022,12,26);

            HttpResponseMessage response1=logic.raceCreationRequest(race1, admin.Token, admin.Salt);

            race1.Beginning_Date = new DateTime(2022, 12, 26);
            race1.Ending_Date = new DateTime(2023, 01, 25);

            HttpResponseMessage response2 = logic.raceCreationRequest(race1, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.Conflict, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.Conflict, response2.StatusCode);
        }

        /// <summary>
        /// Test method designed to assert when the qualification or competition date is not inside the date range of the race
        /// </summary>
        [TestMethod]
        public void notInsideRaceDateTest()
        {
            race1= new Race();
            race1.Champ_Key = "VkZfD"; //Siempre hay que actualizarlo antes de correr el TEST

            race1.Beginning_Date = new DateTime(2022, 12, 25);
            race1.Ending_Date = new DateTime(2022, 12, 26);
            
            race1.Qualification_Date = new DateTime(2022, 12, 25);
            race1.Competition_Date = new DateTime(2022, 12, 27);

            HttpResponseMessage response1 = logic.raceCreationRequest(race1, admin.Token, admin.Salt);
            Assert.AreEqual(HttpStatusCode.Conflict, response1.StatusCode);
        }
    }
}
