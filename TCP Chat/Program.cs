namespace TCP_Chat
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ServerObject server = new ServerObject();
            await server.ListenAsync();
        }
    }
}