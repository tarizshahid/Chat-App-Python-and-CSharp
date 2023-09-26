using System;
using System.Net.Sockets;
using System.Text;

class ChatClient
{
    static void Main()
    {
        Console.Write("Enter the server IP address: ");
        string serverIp = Console.ReadLine();

        Console.Write("Enter the server port: ");
        int serverPort = int.Parse(Console.ReadLine());

        TcpClient client = new TcpClient(serverIp, serverPort);
        NetworkStream stream = client.GetStream();

        Console.Write("Enter your name: ");
        string? name = Console.ReadLine();
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        stream.Write(nameBytes, 0, nameBytes.Length);

        Console.WriteLine($"Connected to {serverIp}:{serverPort}");
        Console.WriteLine($"Welcome, {name}!");

        // Start a thread to receive messages from the server
        var receiveThread = new System.Threading.Thread(() =>
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Server has disconnected.");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
        });
        receiveThread.Start();

        // Send messages to the server
        while (true)
        {
            string message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }
    }
}
