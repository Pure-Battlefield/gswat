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

namespace UnitTest.WebFrontend.Controllers.Roles
{
    [TestFixture]
    public class RolesControllerGetTests
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
            var permissions = new List<string> { "ex1", "ex2" };

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
            var permissions = new List<string> { "ex1", "ex2" };
            var fakeRoleUtil =
                new Mock<IRoleTableStoreUtility>();
            var user1 = new UserEntity(
                googleId.ToString(CultureInfo.InvariantCulture),
                email,
                "foobar",
                true,
                new PermissionSetEntity("gswat", permissions));
            var users = new List<UserEntity> { user1 };
            fakeRoleUtil.Setup(x => x.GetUserEntitiesInNamespace("gswat"))
                .Returns(users);
            var classUnderTest = new RolesController(_fakeCore.Object, fakeRoleUtil.Object, _fakeMailer.Object);

            var response = classUnderTest.Get(_requestingUser);

            var output = response.Data as List<User>;
            Assert.NotNull(output);
            Assert.AreEqual(users.Count, output.Count);
            Assert.AreEqual(Int64.Parse(user1.GoogleIDNumber), output[0].GoogleId);
        }
    }
}
