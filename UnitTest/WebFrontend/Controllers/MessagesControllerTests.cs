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
using core.TableStoreEntities;

namespace UnitTest
{
    [TestFixture]
    public class MessagesControllerTests
    {
        [Test]
        public void GetFromTimeReturnsMessagesTest()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var messagesHandler = new Mock<IMessagesHandler>();
            var returnList = new List<ChatMessageEntity>();
            messagesHandler.Setup(x => x.RetrieveAllMessagesFromTime(It.IsAny<DateTimeInfo>()))
                           .Returns(returnList);
            var classUnderTest = new MessagesController(messagesHandler.Object);

            var response = classUnderTest.Get(request, 0, "getfromtime");

            messagesHandler.Verify(x=>x.RetrieveAllMessagesFromTime(It.IsAny<DateTimeInfo>()),Times.Once());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }

}
