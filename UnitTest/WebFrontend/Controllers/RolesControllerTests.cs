using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using WebFrontend.Controllers;
using WebFrontend.Exceptions;
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

                Assert.Throws<UnauthorizedException>(() => classUnderTest.Put(_request, _requestingUser, null));
            }

            [Test]
            public void DoesReturnForbiddenOnNullUser()
            {
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                Assert.Throws<UnauthorizedException>(() => classUnderTest.Put(_request, null, null));
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
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

                Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
            }
        }

        public class RolesControllerGetTests : RolesControllerTests
        {
            private HttpRequestMessage _request;
            private Mock<ICore> _fakeCore;
            private Mock<IRoleTableStoreUtility> _fakeRoleUtil;
            private AuthenticatedUser _requestingUser;
            private Mock<IMailer> _fakeMailer;

            public RolesControllerGetTests()
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
            public void DoesNotPermitNullAuthorizedUser()
            {
                var classUnderTest = new RolesController(_fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                Assert.Throws<UnauthorizedException>(() => classUnderTest.Get(null));
            }

            [Test]
            public void DoesNotPermitUnauthorizedUser()
            {
                var fakeCore = new Mock<ICore>();
                fakeCore.Setup(
                    x => x.PermissionsUtil.ValidateUser(
                        It.IsAny<IValidatableUser>(),
                        It.IsAny<PermissionSetEntity>()))
                    .Returns(false);
                var classUnderTest = new RolesController(fakeCore.Object, _fakeRoleUtil.Object, _fakeMailer.Object);

                Assert.Throws<UnauthorizedException>(() => classUnderTest.Get(_requestingUser));
            }

            [Test]
            public void GetsIndividualUserIfIdSpecified()
            {
                const int googleId = 1234;
                const string email = "test@example.com";
                const string usrNamespace = "gswat";
                var permissions = new List<string> {"ex1", "ex2"};

                var fakeRoleUtil = new Mock<IRoleTableStoreUtility>();
                fakeRoleUtil.Setup(x => x.GetUserEntity(
                    "gswat", googleId.ToString(CultureInfo.InvariantCulture)))
                    .Returns(new UserEntity(
                        googleId.ToString(CultureInfo.InvariantCulture),
                        email,
                        "foobar",
                        true,
                        new PermissionSetEntity("gswat", permissions)));

                var classUnderTest = new RolesController(_fakeCore.Object, fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Get(_requestingUser, googleId.ToString(CultureInfo.InvariantCulture));

                var output = response.Data as User;
                Assert.NotNull(output);
                Assert.AreEqual(googleId, output.GoogleId);
                Assert.AreEqual(email, output.Email);
                Assert.AreEqual(usrNamespace, output.Namespace);
                foreach (var perm in permissions)
                {
                    Assert.Contains(perm, output.Permissions);
                }
            }

            [Test]
            public void ThrowsIfNoUserFound()
            {
                var fakeRoleUtil =
                    new Mock<IRoleTableStoreUtility>();
                fakeRoleUtil.Setup(
                        x => x.GetUserEntity(
                            It.IsAny<string>(), 
                            It.IsAny<string>()))
                        .Returns((UserEntity)null);

                var classUnderTest = new RolesController(_fakeCore.Object, fakeRoleUtil.Object, _fakeMailer.Object);

                Assert.Throws<UserNotFoundException>(() => classUnderTest.Get(_requestingUser, "1234"));
            }

            [Test]
            public void ReturnsListOfUsersWhenNoIdSpecified()
            {
                const int googleId = 1234;
                const string email = "test@example.com";
                var permissions = new List<string> {"ex1", "ex2"};
                var fakeRoleUtil =
                    new Mock<IRoleTableStoreUtility>();
                var user1 = new UserEntity(
                    googleId.ToString(CultureInfo.InvariantCulture),
                    email,
                    "foobar",
                    true,
                    new PermissionSetEntity("gswat", permissions));
                var users = new List<UserEntity> {user1};
                fakeRoleUtil.Setup(x => x.GetUserEntitiesInNamespace("gswat"))
                    .Returns(users);
                var classUnderTest = new RolesController(_fakeCore.Object, fakeRoleUtil.Object, _fakeMailer.Object);

                var response = classUnderTest.Get(_requestingUser);

                var output = response.Data as List<User>;

            }
        }

        #endregion
    }
}
