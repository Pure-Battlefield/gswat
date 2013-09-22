using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using core.Roles;
using core.Roles.Models;
using core.Utilities;

namespace UnitTest.core
{
    [TestFixture]
    public class RolesTests
    {
        private Mock<ICloudSettingsManager> settingsMgr;

        [TestFixtureSetUp]
        public void Init()
        {
            settingsMgr = new Mock<ICloudSettingsManager>();
            settingsMgr.Setup(manager => manager.GetConfigurationSettingValue("StorageConnectionString")).Returns("UseDevelopmentStorage=true");
            settingsMgr.Setup(manager => manager.GetConfigurationSettingValue("ServiceAdministrators")).Returns("");
        }
        public class ConfirmEmailTokenTests : RolesTests
        {
            private Mock<IValidatableUser> _user;
            private string _userId;
            private Guid _goodPermissionSetGuid;
            private UnboundPermissionSetEntity _unboundPermissions;

            [TestFixtureSetUp]
            public void Init()
            {
                _goodPermissionSetGuid = Guid.NewGuid();
                _userId = "12345";
                _user = new Mock<IValidatableUser>();
                _user.Setup(x => x.GetGoogleId())
                    .Returns(_userId);
                _unboundPermissions = new UnboundPermissionSetEntity
                                          {
                                              Namespace = "GSWAT",
                                              Guid = _goodPermissionSetGuid.ToString(),
                                              Permissions =
                                                  new PermissionSetEntity("GSWAT", new List<string>{"admin"})
                                          };
                _unboundPermissions.ETag = "*";
            }

            [Test]
            public void ThrowsIfUserIsNull()
            {
                var classUnderTest = new RoleTableStoreUtility(settingsMgr.Object);

                Assert.Throws<ArgumentException>(() => classUnderTest.ConfirmEmailToken(null, Guid.NewGuid()));
            }
            [Test]
            public void ReturnsFalseIfPermissionsTokenNotInTableStore()
            {
                var classUnderTest = new RoleTableStoreUtility(settingsMgr.Object);

                Assert.False(classUnderTest.ConfirmEmailToken(_user.Object, new Guid()));
            }

            [Test]
            public void ThrowsIfNoGoogleId()
            {
                var classUnderTest = new RoleTableStoreUtility(settingsMgr.Object);
                var user = new Mock<IValidatableUser>();
                user.Setup(x => x.GetGoogleId())
                    .Returns((string)null);

                Assert.Throws<ArgumentException>(() => classUnderTest.ConfirmEmailToken(user.Object, new Guid()));
            }

            [Test]
            public void ReturnsTrueIfPermissionsExist()
            {
                var classUnderTest = new RoleTableStoreUtility(settingsMgr.Object);
                classUnderTest.AddOrUpdateUnboundPermission(_unboundPermissions);
                try
                {
                    Assert.True(classUnderTest.ConfirmEmailToken(_user.Object, _goodPermissionSetGuid));
                }
                finally
                {
                    classUnderTest.DeleteUnboundPermission(_unboundPermissions);
                }
            }

            [Test]
            public void GrantsUserPermissionsIfValidPermissionSet()
            {
                var classUnderTest = new RoleTableStoreUtility(settingsMgr.Object);
                classUnderTest.AddOrUpdateUnboundPermission(_unboundPermissions);

                try
                {
                    classUnderTest.ConfirmEmailToken(_user.Object, _goodPermissionSetGuid);
                    var createdUser = classUnderTest.GetUserEntity("GSWAT", _userId);
                    Assert.NotNull(createdUser);
                    Assert.AreEqual(_unboundPermissions.Permissions, createdUser.Permissions);
                }
                finally
                {
                    var userEntity = new UserEntity
                                            {
                                                PartitionKey = "GSWAT",
                                                RowKey = _userId
                                            };
                    userEntity.ETag = "*";
                    classUnderTest.DeleteUserEntity(userEntity);
                    classUnderTest.DeleteUnboundPermission(_unboundPermissions);
                }
            }

            [Test]
            public void DoesntNukeExistingUserPermissions()
            {
                var classUnderTest = new RoleTableStoreUtility(settingsMgr.Object);
                var userEntity = new UserEntity(_userId, "", "", true,
                                                new PermissionSetEntity("GSWAT", new List<string> {"samplePermission"}));
                classUnderTest.SetUserEntity(userEntity);
                classUnderTest.AddOrUpdateUnboundPermission(_unboundPermissions);
                try
                {
                    classUnderTest.ConfirmEmailToken(_user.Object, _goodPermissionSetGuid);
                    var user = classUnderTest.GetUserEntity("GSWAT", _userId);
                    Assert.NotNull(user);
                    Assert.Contains("samplePermission", (ICollection) user.Permissions.GetPermissionSet());
                    foreach (var permission in _unboundPermissions.Permissions.GetPermissionSet())
                    {
                        Assert.Contains(permission, (ICollection) user.Permissions.GetPermissionSet());
                    }
                }
                finally
                {
                    userEntity = new UserEntity
                    {
                        PartitionKey = "GSWAT",
                        RowKey = _userId
                    };
                    userEntity.ETag = "*";
                    classUnderTest.DeleteUserEntity(userEntity);
                    classUnderTest.DeleteUnboundPermission(_unboundPermissions);
                }
            }
        }
    }
}
