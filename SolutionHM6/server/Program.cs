using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class BinaryServer
{
    private const int Port = 5000;

    static async Task Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        Console.WriteLine($"BinaryServer is listening on port {Port}...");

        try
        {
            while (true)
            {
                Console.WriteLine("Очікування підключення клієнта...");
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Клієнт підключився.");
                _ = HandleClientAsync(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Серверна помилка: {ex.Message}");
        }
        finally
        {
            listener.Stop();
            Console.WriteLine("Сервер зупинено.");
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Початок обробки клієнта.");
                for (int i = 0; i < 100; i++)
                {
                    byte[] sizeBuffer = new byte[4];
                    int readBytes = await stream.ReadAsync(sizeBuffer, 0, 4);
                    if (readBytes < 4)
                    {
                        Console.WriteLine("З'єднання закрито передчасно.");
                        break;
                    }
                    int messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                    Console.WriteLine($"Розмір повідомлення: {messageSize} байт.");

                    byte[] dataBuffer = new byte[messageSize];
                    int totalRead = 0;
                    while (totalRead < messageSize)
                    {
                        int bytesRead = await stream.ReadAsync(dataBuffer, totalRead, messageSize - totalRead);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("З'єднання закрито під час читання повідомлення.");
                            return;
                        }
                        totalRead += bytesRead;
                    }
                    string message = Encoding.UTF8.GetString(dataBuffer);
                    Console.WriteLine($"Отримано повідомлення #{i + 1}: {message}");
                }
                Console.WriteLine("Обробка клієнта завершена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при обробці клієнта: {ex.Message}");
            }
        }
    }
}
