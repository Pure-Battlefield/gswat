namespace core.Server
{
    public interface IServerMock
    {
        // Event for mocking chat messages sent from the server
        event ChatEventHandler MessageSent;
    }
}