using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class BinaryClient
{
    private const string ServerAddress = "127.0.0.1";
    private const int Port = 5000;

    static async Task Main()
    {
        try
        {
            using TcpClient client = new TcpClient();
            Console.WriteLine("Спроба підключення до сервера...");
            await client.ConnectAsync(ServerAddress, Port);
            Console.WriteLine("Підключено до сервера. Відправка 100 повідомлень...");

            NetworkStream stream = client.GetStream();
            for (int i = 0; i < 100; i++)
            {
                string textMessage = $"Hello server! i={i}";
                byte[] data = Encoding.UTF8.GetBytes(textMessage);
                byte[] sizeBytes = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(sizeBytes, 0, sizeBytes.Length);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine($"Відправлено повідомлення #{i + 1}");
            }

            Console.WriteLine("Всі повідомлення відправлено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Клієнтська помилка: {ex.Message}");
        }
    }
}
