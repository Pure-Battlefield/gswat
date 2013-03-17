using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using NUnit.Framework;
using Moq;
using WebFrontend.Controllers;
using WebFrontend.Handlers;
using WebFrontend.Models;
using core.Roles;

namespace UnitTest
{
    [TestFixture]
    public class PermissionsTests
    {
        [Test]
        public void LoadFromConfigFileTest()
        {
            PermissionsUtility util = new PermissionsUtility();
            var permissionList = new List<string>();
            permissionList.Add("admin");
            util.ValidateToken("testtoken", new PermissionSetEntity("GSWAT", permissionList));
        }
    }

}
