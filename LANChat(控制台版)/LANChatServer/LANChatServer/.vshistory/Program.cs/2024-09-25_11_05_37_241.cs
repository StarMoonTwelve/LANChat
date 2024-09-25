using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    private static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("聊天服务器已启动...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("新用户已连接.");
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        string clientName = $"用户{clients.IndexOf(client) + 1}";

        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = $"{clientName}: {Encoding.UTF8.GetString(buffer, 0, bytesRead)}";
                Console.WriteLine(message);
                Broadcast(message, client);
            }
            catch
            {
                break;
            }
        }

        clients.Remove(client);
        Console.WriteLine($"{clientName} 已断开连接.");
        client.Close();
    }

    private static void Broadcast(string message, TcpClient sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
