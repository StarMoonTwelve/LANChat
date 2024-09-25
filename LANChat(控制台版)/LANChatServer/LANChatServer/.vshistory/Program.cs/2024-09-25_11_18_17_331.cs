using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    private static List<(TcpClient client, string username)> clients = new List<(TcpClient, string)>();

    static void Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("聊天服务器已启动...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        string username = "";

        // 接收用户名
        try
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            username = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            clients.Add((client, username));
            Console.WriteLine($"{username} 已连接.");
        }
        catch
        {
            return; // 如果接收失败则直接返回
        }

        // 广播用户已连接信息
        Broadcast($"{username} 加入了聊天！", client);

        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = $"{username}: {Encoding.UTF8.GetString(buffer, 0, bytesRead)}";
                Console.WriteLine(message);
                Broadcast(message, client);
            }
            catch
            {
                break;
            }
        }

        clients.RemoveAll(c => c.client == client);
        Console.WriteLine($"{username} 已断开连接.");
        Broadcast($"{username} 离开了聊天！", client);
        client.Close();
    }

    private static void Broadcast(string message, TcpClient sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            if (client.client != sender)
            {
                NetworkStream stream = client.client.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
