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

        public HttpResponseMessage loginRequest(Administrator admin)
        {
            Administrator testAdmin = dbContext.Administrators.Find(admin.Username);
            if (testAdmin==null)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Incorrect username or password") };
            }
            if (tools.verifyPassword(admin.Password, testAdmin.Password, testAdmin.Salt))
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