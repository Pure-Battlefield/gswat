using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using core.Utilities;

namespace UnitTest.core
{
    [TestFixture]
    class RolesTests
    {
        private Mock<ICloudSettingsManager> settingsMgr;

        [TestFixtureSetUp]
        public void Init()
        {
            settingsMgr = new Mock<ICloudSettingsManager>();
            settingsMgr.Setup(manager => manager.GetConfigurationSettingValue("StorageConnectionString")).Returns("UseDevelopmentStorage=true");
            settingsMgr.Setup(manager => manager.GetConfigurationSettingValue("ServiceAdministrators")).Returns("");
        }
    }
}
