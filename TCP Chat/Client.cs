using System.Net.Sockets;

namespace TCP_Chat
{
    internal class ClientObject
    {
        private string _id;
        private StreamReader _reader;
        private StreamWriter _writer;

        private TcpClient _TcpClient;
        private ServerObject _server;

        public string Id => _id;
        public StreamReader Reader => _reader;
        public StreamWriter Writer => _writer;
        public ClientObject (TcpClient tcpClient, ServerObject server)
        {
            _id = Guid.NewGuid().ToString();
            _TcpClient = tcpClient;
            _server = server;
            var stream = _TcpClient.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                string? userName = await _reader.ReadLineAsync();
                string? message = $"{userName} подключился к чату";

                await _server.BroadcastMessageAsync(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = await _reader.ReadLineAsync();
                        if (message == null) continue;
                        message = $"{userName}: {message}";
                        Console.WriteLine(message);
                        await _server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        message = $"{userName} покинул чат";
                        Console.WriteLine(message);
                        await _server.BroadcastMessageAsync(message, Id);
                        break;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            { _server.RemoveConnection(Id); }
        }

        public void Close()
        {
            _TcpClient.Close();
            _reader.Close();
            _writer.Close();
        }
    }
}
