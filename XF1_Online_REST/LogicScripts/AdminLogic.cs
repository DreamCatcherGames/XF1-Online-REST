using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class AdminLogic
    {
        private XF1OnlineEntities dbContext;
        private Tools tools;

        public AdminLogic()
        {
            this.dbContext = new XF1OnlineEntities();
            this.tools = new Tools();
        }
        /// <summary>
        /// Function designed to implement the login process of an Administrator
        /// </summary>
        /// <param name="admin"><see cref="Administrator"> object that contains the login credentials</param>
        /// <returns><see cref="HttpStatusCode"/> object that contains the response and status code </returns>
        public HttpResponseMessage loginRequest(Administrator admin)
        {
            Administrator testAdmin = dbContext.Administrators.Find(admin.Username);
            if (testAdmin==null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Incorrect username or password") };
            }
            if (tools.verifyPassword(admin.Encrypted_Password, testAdmin.Encrypted_Password, testAdmin.Salt))
            {
                string token = tools.getToken(admin.Salt);
                tools.assignToken(admin, token);
                admin = dbContext.Administrators.Find(admin.Username);
                admin.Token = token;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(admin)) };
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Incorrect username or password") };
        }


    }
}