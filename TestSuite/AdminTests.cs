using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using XF1_Online_REST.LogicScripts;
using System.Net;
using XF1_Online_REST;

namespace TestSuite
{
    [TestClass]
    public class AdminTests
    {
        AdminLogic logic= new AdminLogic();

        [TestMethod]
        public void okLoginTest()
        {
            Administrator admin = new Administrator();
            admin.Username = "loginTestAdmin";
            admin.Password = "admin1234";

            HttpResponseMessage response=logic.loginRequest(admin);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [TestMethod]
        public void wrongloginTest()
        {
            Administrator admin = new Administrator();
            admin.Username = "loginTestAdmin";
            admin.Password = "admin123";


            HttpResponseMessage response = logic.loginRequest(admin);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
