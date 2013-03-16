using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using WebFrontend.Handlers;
using core;
using core.TableStoreEntities;

namespace UnitTest.WebFrontend.Handlers
{
    [TestFixture]
    class MessagesHandlerTests
    {
        [Test]
        public void RetrieveAllMessagesTest()
        {
            var coreMock = new Mock<ICore>();
            var classUnderTest = new MessagesHandler(coreMock.Object);
            var queue = new Queue<ChatMessageEntity>();
            queue.Enqueue(new ChatMessageEntity());
            queue.Enqueue(new ChatMessageEntity());
            coreMock.Setup(x => x.GetMessageQueue()).Returns(queue);

            var result = classUnderTest.RetrieveAllMessages();
            var sum = result.Count();
            Assert.AreEqual(2, sum, "All messages not returned.");
        }
    }
}
