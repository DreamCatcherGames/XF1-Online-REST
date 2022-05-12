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
    public class ChampionshipTests
    {
        ChampionshipLogic logic = new ChampionshipLogic();
        Administrator admin= new Administrator("champTestAdmin","admin1234", "dkQhxuDOS02Z1jZJ2KRpng==", "YGXMKyXDIemAKw==");
        Championship champ1 = new Championship();
        Championship champ2 = new Championship();
        /// <summary>
        /// Test method designed to assert when an unvalid token is used to create a new championship
        /// </summary>
        [TestMethod]
        public void badTokenChampionshipCreationRequestTest()
        {
            Championship champ = new Championship();
            HttpResponseMessage response = logic.championshipCreationRequest(champ,"badToken","badSalt");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        }
        
        /// <summary>
        /// Test method designed to assert when creating a championship with all his needed data in order
        /// </summary>
        [TestMethod]
        public void okChampionshipCreation()
        {
            champ1.Name = "Test1";
            champ1.Rules_Description = "Reglas 1";
            champ1.Beginning_Date = new DateTime(2023,12,25);
            champ1.Beginning_Time = new TimeSpan(7,0,0);
            champ1.Ending_Date = new DateTime(2023,12,31);
            champ1.Ending_Time = new TimeSpan(23,59,59);

            HttpResponseMessage response = logic.championshipCreationRequest(champ1, admin.Token, admin.Salt);
            logic.championshipDeletionRequest(champ1.Unique_Key, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        /// <summary>
        /// Test method designed to assert when a championship is beign made with a date before today's date
        /// </summary>
        [TestMethod]
        public void outdatedChampionshipTest()
        {
            champ1.Beginning_Date = DateTime.Now.AddDays(-2);
            champ1.Ending_Date = DateTime.Now.AddDays(-1);

            HttpResponseMessage response=logic.championshipCreationRequest(champ1, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }
        /// <summary>
        /// Test method designed to assert when the starting and ending date of a new championship are not one after the another in the correct way 
        /// </summary>
        [TestMethod]
        public void notConsistentDatesTest()
        {
            champ1.Beginning_Date = DateTime.Now.AddDays(2);
            champ1.Ending_Date = DateTime.Now.AddDays(1);

            HttpResponseMessage response=logic.championshipCreationRequest(champ1, admin.Token,admin.Salt);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        /// <summary>
        /// Test method designed to assert when  a new championship dates intersect with another championship dates
        /// </summary>
        [TestMethod]
        public void intersectionBetweenChampionshipDatesTest()
        {
            champ1.Beginning_Date= DateTime.Now.AddDays(1);
            champ1.Ending_Date=DateTime.Now.AddDays(1).AddMonths(2);

            champ2.Beginning_Date = DateTime.Now.AddDays(2);
            champ1.Ending_Date = DateTime.Now.AddDays(2).AddMonths(2);

            logic.championshipCreationRequest(champ1, admin.Token, admin.Salt);
            HttpResponseMessage response1 = logic.championshipCreationRequest(champ2, admin.Token, admin.Salt);
            logic.championshipDeletionRequest(champ1.Unique_Key, admin.Token, admin.Salt);


            champ1.Beginning_Date = DateTime.Now.AddDays(1).AddMonths(1);
            champ1.Ending_Date = DateTime.Now.AddDays(1).AddMonths(2);

            champ2.Beginning_Date = DateTime.Now.AddMonths(1);
            champ1.Ending_Date = DateTime.Now.AddMonths(2);

            logic.championshipCreationRequest(champ1, admin.Token, admin.Salt);
            HttpResponseMessage response2 = logic.championshipCreationRequest(champ2, admin.Token, admin.Salt);
            logic.championshipDeletionRequest(champ1.Unique_Key, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.Conflict, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.Conflict, response2.StatusCode);
        }

        /// <summary>
        /// Test method designed to assert when a new championship is being tried to schedule inside the date range of another championship
        /// </summary>
        [TestMethod]
        public void containedChampionshipDatesTest()
        {
            champ1.Beginning_Date = DateTime.Now.AddDays(1);
            champ1.Ending_Date = DateTime.Now.AddDays(1).AddMonths(2);

            champ2.Beginning_Date = DateTime.Now.AddDays(2);
            champ2.Ending_Date = DateTime.Now.AddMonths(2);

            logic.championshipCreationRequest(champ1, admin.Token, admin.Salt);
            HttpResponseMessage response1 = logic.championshipCreationRequest(champ2, admin.Token, admin.Salt);
            logic.championshipDeletionRequest(champ1.Unique_Key, admin.Token, admin.Salt);

            champ2.Beginning_Date = DateTime.Now.AddDays(1).AddMonths(1);
            champ2.Ending_Date = DateTime.Now.AddDays(1).AddMonths(2);

            champ1.Beginning_Date = DateTime.Now.AddMonths(1);
            champ1.Ending_Date = DateTime.Now.AddMonths(2);

            logic.championshipCreationRequest(champ1, admin.Token, admin.Salt);
            HttpResponseMessage response2 = logic.championshipCreationRequest(champ2, admin.Token, admin.Salt);
            logic.championshipDeletionRequest(champ1.Unique_Key, admin.Token, admin.Salt);

            Assert.AreEqual(HttpStatusCode.Conflict, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.Conflict, response2.StatusCode);
        }

        [TestMethod]
        public void notConsistentTimesTest()
        {
            champ1 = new Championship();

            champ1.Name = "Test1";
            champ1.Rules_Description = "Reglas 1";
            champ1.Beginning_Date = new DateTime(2022, 12, 25);
            champ1.Beginning_Time = new TimeSpan(7, 0, 0);
            champ1.Ending_Date = new DateTime(2022, 12, 31);
            champ1.Ending_Time = new TimeSpan(6, 0, 0);

            HttpResponseMessage response = logic.championshipCreationRequest(champ2, admin.Token, admin.Salt);
            logic.championshipDeletionRequest(champ1.Unique_Key, admin.Token, admin.Salt);

            Assert.AreEqual (HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
