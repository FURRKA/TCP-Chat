using System.Net.Sockets;

namespace TCP_Chat
{
    public class ServerObject
    {
        private TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 8888);
        private List<ClientObject> clients = new List<ClientObject>();

        public async Task ListenAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    ClientObject clientObj = new ClientObject(tcpClient, this);
                    clients.Add(clientObj);
                    Task.Run(clientObj.ProcessAsync);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            { Disconect(); }
        }

        public async Task BroadcastMessageAsync(string message, string ID)
        {
            foreach (var client in clients)
            {
                if (client.Id != ID)
                {
                    await client.Writer.WriteAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        public void RemoveConnection(string Id)
        {
            ClientObject? client = clients.FirstOrDefault(client => client.Id == Id);
            if (client != null) clients.Remove(client);
            client?.Close();
        }

        public void Disconect()
        {
            foreach (ClientObject client in clients)
            {
                client.Close();
            }
            listener.Stop();
        }

    }
}
