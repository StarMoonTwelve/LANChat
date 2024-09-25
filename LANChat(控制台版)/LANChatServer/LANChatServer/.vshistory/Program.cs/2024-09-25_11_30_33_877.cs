using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    private static List<(TcpClient client, string username)> clients = new List<(TcpClient, string username)>();
    private const string VerificationCode = "1234"; // 设定的验证码

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

        // 接收用户名和验证码
        try
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            username = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            // 接收验证码
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string enteredCode = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            // 验证码检查
            if (enteredCode != VerificationCode)
            {
                byte[] denyMessage = Encoding.UTF8.GetBytes("验证码错误，连接被拒绝。");
                stream.Write(denyMessage, 0, denyMessage.Length);
                client.Close();
                return;
            }

            clients.Add((client, username));
            Console.WriteLine($"{username} 已连接.");
        }
        catch
        {
            return; // 如果接收失败则直接返回
        }

        Broadcast($"{username} 加入了聊天室！", client);

        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string formattedMessage = $"{username}: {message}";
                Console.WriteLine(formattedMessage);
                Broadcast(formattedMessage, client);
            }
            catch
            {
                break;
            }
        }

        clients.RemoveAll(c => c.client == client);
        Console.WriteLine($"{username} 已断开连接.");
        Broadcast($"{username} 离开了聊天室！", client);
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
