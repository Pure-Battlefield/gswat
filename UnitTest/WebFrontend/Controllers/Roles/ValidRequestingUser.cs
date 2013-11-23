using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Moq;
using NUnit.Framework;
using WebFrontend.Controllers;
using WebFrontend.Exceptions;
using core;
using core.Roles;
using core.Roles.Models;
using core.Utilities;

namespace UnitTest.WebFrontend.Controllers.Roles
{
    [TestFixture]
    public class ValidatesRequestingUserRolesControllerTests
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
}
