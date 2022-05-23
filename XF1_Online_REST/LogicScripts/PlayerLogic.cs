using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class PlayerLogic
    {
        private XF1OnlineEntities dbContext;
        private Tools tools;

        public PlayerLogic()
        {
            this.dbContext = new XF1OnlineEntities();
            this.tools = new Tools();
        }

        /// <summary>
        /// Function designed to implement the login process of a Player
        /// </summary>
        /// <param name="player"><see cref="Player"> object that contains the login credentials</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>
        public HttpResponseMessage loginRequest(Player player)
        {
            Player testPlayer = dbContext.Players.Find(player.Username);
            if (testPlayer == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Incorrect username or password") };
            }
            if (tools.verifyPassword(player.Encrypted_Password, testPlayer.Encrypted_Password, testPlayer.Salt))
            {
                string token = tools.getToken(player.Salt);
                tools.assignToken(player, token);
                player = dbContext.Players.Find(player.Username);
                player.Token = token;
                player.Encrypted_Password = "";
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(player)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Incorrect username or password") };
        }

        /// <summary>
        /// Function designed to implement the registration process of a Player
        /// </summary>
        /// <param name="player"><see cref="Player"> object that contains the player information</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>
        public HttpResponseMessage registerRequest(Player player)
        {
            if(!dbContext.Players.Any(o=>o.Username==player.Username))
            {
                if(!dbContext.Players.Any(o=>o.Email==player.Email))
                {
                    player.Active = false;
                    player.Money = 10000000;

                    List<string> passwordComponents = tools.passwordEncryptor(player.Encrypted_Password);

                    player.Encrypted_Password = passwordComponents[0];
                    player.Salt=passwordComponents[1];
                    player.Token=tools.getToken(player.Salt);

                    dbContext.Players.Add(player);
                    dbContext.SaveChanges();

                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Player registered succesfully! Please check your email")};
                }
                return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("Email already taken!") };
            }
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent("Username already taken!") };
        }
    }
}