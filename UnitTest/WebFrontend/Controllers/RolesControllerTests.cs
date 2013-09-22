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
using core.Roles.Models;
using core.Utilities;

namespace UnitTest.WebFrontend.Controllers
{
    [TestFixture]
    public class RolesControllerTests
    {
        #region SetUp / TearDown

        #endregion

        #region Tests
        public class ValidatesRequestingUserRolesControllerTests : RolesControllerTests
        {
            private HttpRequestMessage _request;
            private Mock<ICore> _fakeCore;
            private Mock<IRoleTableStoreUtility> _fakeRoleUtil;
            private Mock<IMailer> _fakeMailer;
            private AuthenticatedUser _requestingUser;

            public ValidatesRequestingUserRolesControllerTests()
            {
                _request = new HttpRequestMessage();
                _request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                _fakeCore = new Mock<ICore>();
                _fakeCore.Setup(
                    x => x.PermissionsUtil.ValidateUser(
                        It.IsAny<IValidatableUser>(),
                        It.IsAny<PermissionSetEntity>())).Returns(false);
                _fakeRoleUtil = new Mock<IRoleTableStoreUtility>();
                _fakeMailer = new Mock<IMailer>();
                
                _requestingUser = new AuthenticatedUser
                                      {
                                          Token = "test"
                                      };
            }
            [Test]
            public void DoesReturnForbiddenOnUnauthorizedUser()
            {
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, null);

                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            }

            [Test]
            public void DoesReturnForbiddenOnNullUser()
            {
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, null, null);

                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }


        public class ValidUserToAddRolesControllerTests : RolesControllerTests
        {
            private HttpRequestMessage _request;
            private Mock<ICore> _fakeCore;
            private Mock<IRoleTableStoreUtility> _fakeRoleUtil;
            private Mock<IMailer> _fakeMailer;
            private AuthenticatedUser _requestingUser;

            public ValidUserToAddRolesControllerTests()
            {
                _request = new HttpRequestMessage();
                _request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                _fakeCore = new Mock<ICore>();
                _fakeCore.Setup(
                    x => x.PermissionsUtil.ValidateUser(
                        It.IsAny<IValidatableUser>(),
                        It.IsAny<PermissionSetEntity>()))
                        .Returns(true);
                _fakeRoleUtil = new Mock<IRoleTableStoreUtility>();
                _fakeMailer = new Mock<IMailer>();

                _requestingUser = new AuthenticatedUser
                {
                    Token = "test"
                };
            }

            [Test]
            public void RejectsNullUserToAdd()
            {
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, null);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void RejectsNullEmailToAdd()
            {
                var user = new User
                               {
                                   Email = null
                               };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void RejectsEmptyEmailToAdd()
            {
                var user = new User
                {
                    Email = ""
                };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void RejectsSpacesOnlyEmailToAdd()
            {
                var user = new User
                {
                    Email = "   "
                };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Test]
            public void RejectsNullNamespaceToAdd()
            {
                var user = new User
                {
                    Email = "x",
                    Namespace = null
                };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
            [Test]
            public void RejectsEmptyNamespaceToAdd()
            {
                var user = new User
                {
                    Email = "x",
                    Namespace = ""
                };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
            [Test]
            public void RejectsSpacesOnlyNamespaceToAdd()
            {
                var user = new User
                {
                    Email = "x",
                    Namespace = "   "
                };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
            [Test]
            public void RejectsNullPermissionsToAdd()
            {
                var user = new User
                {
                    Email = "x",
                    Namespace = "x",
                    Permissions = null
                };
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Put(_request, _requestingUser, user);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
        

        #endregion
    }
}
