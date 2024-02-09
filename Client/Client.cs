using System.Net.Sockets;

namespace Client
{
    internal class Client
    {
        private string _ip;
        private int _port;
        private string? _username;
        private StreamReader? _reader;
        private StreamWriter? _writter;

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public async void Run()
        {
            Console.Write("Введите ваш никнейм в чате: ");
            _username = Console.ReadLine();
            Console.WriteLine($"Добро пожаловать {_username}");

            try
            {
                var client = new TcpClient();
                client.Connect(_ip, _port);
                Console.WriteLine("Успешное подключение");

                var stream = client.GetStream();
                _reader = new StreamReader(stream);
                _writter = new StreamWriter(stream);

                if (_writter == null || _reader == null)
                {
                    Console.WriteLine("Не робит");
                    return;
                }

                Task.Run(() =>ReceiveMessage(_reader));
                await SendMessage(_writter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _reader?.Close();
                _writter?.Close(); 
            }
        }

        private async Task SendMessage(StreamWriter writter)
        {
            await writter.WriteLineAsync(_username);
            await writter.FlushAsync();

            while (true)
            {
                string? message = Console.ReadLine();
                await writter.WriteLineAsync(message);
                await writter.FlushAsync();
            }
        }

        private async Task ReceiveMessage(StreamReader reader)
        {
            while (true)
            {
                try
                {
                    string? message = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;
                    Console.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
    }
}
