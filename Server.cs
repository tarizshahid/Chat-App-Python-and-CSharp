using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    private static readonly Dictionary<Socket, string> clients = new Dictionary<Socket, string>();
    private static readonly object clientLock = new object();

    static void Main()
    {

        // Listen on any available IP address on port 12345
        IPAddress ipAddress = IPAddress.Any;
        int port = 1025;
        Console.WriteLine($"Chat Server is running... on port :{port}");

        // Create a TCP/IP socket
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the IP address and port
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        serverSocket.Bind(endPoint);

        // Start listening for incoming connections
        serverSocket.Listen(10);

        while (true)
        {
            Console.WriteLine("Waiting for a connection...");
            Socket clientSocket = serverSocket.Accept();

            // Ask for the user's name
            byte[] nameBuffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(nameBuffer);
            string name = Encoding.UTF8.GetString(nameBuffer, 0, receivedBytes);

            lock (clientLock)
            {
                clients[clientSocket] = name;
            }

            string welcomeMessage = $"Welcome, {name}!";
            byte[] welcomeMessageBuffer = Encoding.UTF8.GetBytes(welcomeMessage);
            clientSocket.Send(welcomeMessageBuffer);

            // Create a new thread to handle the client
            Thread clientThread = new Thread(() => HandleClient(clientSocket, name));
            clientThread.Start();
        }
    }

    private static void HandleClient(Socket clientSocket, string name)
    {
        try
        {
            while (true)
            {
                byte[] messageBuffer = new byte[1024];
                int receivedBytes = clientSocket.Receive(messageBuffer);
                if (receivedBytes == 0)
                {
                    break; // Client has disconnected
                }

                string message = Encoding.UTF8.GetString(messageBuffer, 0, receivedBytes);
                string timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
                string formattedMessage = $"{timestamp}{name}: {message}";

                lock (clientLock)
                {
                    foreach (var client in clients.Keys)
                    {
                        if (client != clientSocket)
                        {
                            client.Send(Encoding.UTF8.GetBytes(formattedMessage));
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // Handle any exceptions (e.g., client disconnects)
        }
        finally
        {
            lock (clientLock)
            {
                clients.Remove(clientSocket);
            }

            clientSocket.Close();
            Console.WriteLine($"{name} has left the chat.");
        }
    }
}
