using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
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
        /// <summary>
        /// Method designed to make the request for the creation of a new championship
        /// </summary>
        /// <param name="champ"><see cref="Championship"/> object that contains the information of the championship to be registered</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
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

                        dbContext.SaveChanges();

                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Championship added successfully") };
                    }
                    return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("The times provided for this championship are not correct") };
                }
                return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("Dates provided bump into another championship dates or they are not correct") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
        /// <summary>
        /// Method designed to make the request to delete a championship
        /// </summary>
        /// <param name="champId"><see cref="Championship"/> object that contains the unique key of the championship to delete</param>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage championshipDeletionRequest(string champId,string token,string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                Championship champ = dbContext.Championships.Find(champId);
                if(champ!=null)
                {
                    dbContext.Championships.Remove(champ);
                    dbContext.SaveChanges();

                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Championship deleted succesfully") };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("The specified championship does not exists on our records") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
        public HttpResponseMessage championshipRequest(string champId,string token,string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                Championship champ= dbContext.Championships.Find(champId);
                if(champ!=null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(champ)) };
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Championship not found") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
        /// <summary>
        /// Method designed to obtain all the championships registered on the database
        /// </summary>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage allChampionshipsRequest(string token,string salt)
        {
            if(tools.verifyAdminToken(token,salt))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Championships.ToList()))};

            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }

        /// <summary>
        /// Method designed to fetch the current championship
        /// </summary>
        /// <param name="token"><see cref="string"/> object that contains the admin unique token</param>
        /// <param name="salt"><see cref="string"/> object that contains the salt needed for dencryption of the saved admin token</param>
        /// <returns><see cref="HttpResponseMessage"/> object that contains an appropiate response to the state of the request made</returns>
        public HttpResponseMessage currentChampionshipRequest(string token,string salt)
        {
            if (tools.verifyAdminToken(token, salt))
            {
                Championship currentChamp;
                try
                {
                    currentChamp = dbContext.Championships.FirstOrDefault(o => o.CurrentChamp == true);
                }
                catch (Exception ex)
                {
                    currentChamp = null;
                }

                if (currentChamp != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(currentChamp)) };
                }
                return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("There's no current championship for now") };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
        public HttpResponseMessage notCurrentChampionshipRequest(string token, string salt)
        {
            if (tools.verifyAdminToken(token, salt))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(dbContext.Championships.Where(o => o.CurrentChamp==false).ToList())) };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Invalid token") };
        }
    }
}