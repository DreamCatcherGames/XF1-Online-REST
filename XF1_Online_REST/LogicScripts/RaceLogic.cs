using Newtonsoft.Json;
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
        /// <summary>
        /// Method designed to retrive all the races of a specific championship
        /// </summary>
        /// <param name="champId"><see cref="string"/> object that contains the id of the championship</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage getRacesByChampionshipRequest(string champId,string token,string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                Championship champ= dbContext.Championships.Find(champId);
                if(champ != null)
                {
                    List<Race> races = dbContext.Races.Where(r => r.Champ_Key == champId).ToList();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(races)) };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Championship not found") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }

        /// <summary>
        /// Method designed to fetch a race by its key values
        /// </summary>
        /// <param name="champId"><see cref="string"/> object that represents the key of the championship of the race</param>
        /// <param name="raceName"><see cref="string"/> object that represents the name of the race</param>
        /// <param name="country"><see cref="string"/> object that represents the country of the race</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage getRaceByIdRequest(string champId, string raceName, string country, string token, string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                Championship champ = dbContext.Championships.Find(champId);
                if (champ != null)
                {
                    Race race = dbContext.Races.Where(r => r.Champ_Key == champId && r.Name==raceName && r.Country==country).ToList()[0];
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(race)) };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Championship not found") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }

        /// <summary>
        /// Method designed to make the process of registering a new race
        /// </summary>
        /// <param name="race"><see cref="Race"/> object that contains the information of the race to be registered</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage raceCreationRequest(Race race,string token,string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                if (tools.raceDateVerifier(race))
                {
                    if(race.Status==null)
                    {
                        race.Status = "Pendiente";
                    }
                    dbContext.Races.Add(race);
                    dbContext.SaveChanges();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Race added successfully") };
                }
                return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("The dates given for this race are wrong or bump into another race. Check if it's inisde the championship date range")};
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
        /// <summary>
        /// Method designed to make the process of deleting a race
        /// </summary>
        /// <param name="race"><see cref="Race"/> object that contains the information of the race to be deleted</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage raceDeletionRequest(Race race,string token,string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                dbContext.Races.Remove(race);
                dbContext.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Race deleted succesfully") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };

        }
    }
}