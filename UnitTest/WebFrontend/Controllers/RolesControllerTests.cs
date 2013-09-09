using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Moq;
using NUnit.Framework;
using WebFrontend.Controllers;
using WebFrontend.Models;
using core;
using core.Roles;
using core.Utilities;

namespace UnitTest.WebFrontend.Controllers
{
    [TestFixture]
    public class RolesControllerTests
    {
        #region SetUp / TearDown

        [SetUp]
        public void Init()
        { }

        [TearDown]
        public void Dispose()
        { }

        #endregion

        #region Tests

        [Test]
        public void DoesReturnForbiddenOnUnauthorizedUser()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var fakeCore = new Mock<ICore>();
            fakeCore.Setup(
                x => x.PermissionsUtil.ValidateUser(
                    It.IsAny<string>(), 
                    It.IsAny<PermissionSetEntity>(),
                    It.IsAny<string>())).Returns(false);
            var fakeRoleUtil = new Mock<IRoleTableStoreUtility>();
            var fakeMailer = new Mock<IMailer>();
            var classUnderTest = new RolesController(fakeCore.Object, fakeRoleUtil.Object, fakeMailer.Object);
            var requestingUser = new AuthenticatedUser
                                     {
                                         Token = "test"
                                     };

            var response = classUnderTest.Put(request, requestingUser, null);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        #endregion
    }
}
