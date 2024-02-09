namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 8888);
            client.Run();
        }
    }
}