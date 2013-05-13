using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;
using Moq;
using WebFrontend.Controllers;
using WebFrontend.Handlers;
using WebFrontend.Models;
using core;
using core.Roles;
using core.Utilities;

namespace UnitTest
{
    [TestFixture]
    public class PermissionsTests
    {
        private Mock<ICloudSettingsManager> settingsMgr;

        [TestFixtureSetUp]
        public void Init()
        {
            settingsMgr = new Mock<ICloudSettingsManager>();
            settingsMgr.Setup(manager => manager.GetConfigurationSettingValue("StorageConnectionString")).Returns("UseDevelopmentStorage=true");
            settingsMgr.Setup(manager => manager.GetConfigurationSettingValue("ServiceAdministrators")).Returns("");
        }

        [Test]
        public void CreateandValidateNewUserTest()
        {
            IPermissionsUtility perm = new PermissionsUtility(new RoleTableStoreUtility(settingsMgr.Object), settingsMgr.Object);
            var core = new Core(perm, settingsMgr.Object);
            var user = new UserEntity("12345","mail@mail.com","battlelogID",true, new PermissionSetEntity("GSWAT", new List<string> {"admin"}));

            core.PermissionsUtil.AddorUpdateUser(user);

            Assert.IsTrue(core.PermissionsUtil.ValidateUser("", "mail@mail.com", new PermissionSetEntity("GSWAT", new List<string> { "admin" }), "12345"));
        }

        /*
        [Test]
        public void PermissionDeniedTest()
        {
            StartAzureDevelopmentStorage();
            IPermissionsUtility perm = new PermissionsUtility(new RoleTableStoreUtility(CloudStorageAccount.DevelopmentStorageAccount));
            Core core = new Core(perm);
            var user = new UserEntity("12345", "mail@mail.com", "battlelogID", true, new PermissionSetEntity("GSWAT", new List<string> { "admin" }));
            core.AddorUpdateUser(user);
            Assert.IsFalse(core.ValidateUser("", "mail@mail.com", new PermissionSetEntity("GSWAT", new List<string> { "anonymous" }), "12345"));
        }

        [Test]
        public void InvalidPermissionTest()
        {
            StartAzureDevelopmentStorage();
            IPermissionsUtility perm = new PermissionsUtility(new RoleTableStoreUtility(CloudStorageAccount.DevelopmentStorageAccount));
            Core core = new Core(perm);
            var user = new UserEntity("12345", "mail@mail.com", "battlelogID", true, new PermissionSetEntity("GSWAT", new List<string> { "admin" }));
            core.AddorUpdateUser(user);
            Assert.IsFalse(core.ValidateUser("", "mail@mail.com", new PermissionSetEntity("GSWAT", new List<string> { "invalidpermissionname" }), "12345"));
        }

        [Test]
        public void InvalidIDTest()
        {
            StartAzureDevelopmentStorage();
            IPermissionsUtility perm = new PermissionsUtility(new RoleTableStoreUtility(CloudStorageAccount.DevelopmentStorageAccount));
            Core core = new Core(perm);
            var user = new UserEntity("12345", "mail@mail.com", "battlelogID", true, new PermissionSetEntity("GSWAT", new List<string> { "admin" }));
            core.AddorUpdateUser(user);
            Assert.IsFalse(core.ValidateUser("", "mail@mail.com", new PermissionSetEntity("GSWAT", new List<string> { "admin" }), "11111"));
        }

        [Test]
        public void InvalidEmailTest()
        {
            StartAzureDevelopmentStorage();
            IPermissionsUtility perm = new PermissionsUtility(new RoleTableStoreUtility(CloudStorageAccount.DevelopmentStorageAccount));
            Core core = new Core(perm);
            var user = new UserEntity("12345", "mail@mail.com", "battlelogID", true, new PermissionSetEntity("GSWAT", new List<string> { "admin" }));
            core.AddorUpdateUser(user);
            Assert.IsFalse(core.ValidateUser("", "mail123@mail.com", new PermissionSetEntity("GSWAT", new List<string> { "anonymous" }), "12345"));
        }

        // Utility function to start dev storage
        private static void StartAzureDevelopmentStorage()
        {
            Int32 count = Process.GetProcessesByName("DSService").Length;

            if (count == 0)
            {
                ProcessStartInfo start = new ProcessStartInfo
                {
                    Arguments = "/devstore:start",
                    FileName = @"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun.exe"
                };
                Process proc = new Process
                {
                    StartInfo = start
                };

                proc.Start();
                proc.WaitForExit();
            }
        }
         */
    }

}
