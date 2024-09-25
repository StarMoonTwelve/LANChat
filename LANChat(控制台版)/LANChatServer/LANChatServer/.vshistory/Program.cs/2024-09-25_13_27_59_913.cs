using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private static List<Socket> clients = new List<Socket>();

    static void Main()
    {
        Console.WriteLine("启动服务器...");
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();

        while (true)
        {
            Socket client = server.AcceptSocket();
            clients.Add(client);
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void HandleClient(Socket client)
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = client.Receive(buffer)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("收到消息: " + message);
                LogMessage(message);  // 记录消息到文件
                Broadcast(message, client);
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("客户端已断开连接。");
        }
        catch (Exception ex)
        {
            Console.WriteLine("发生错误: " + ex.Message);
        }
        finally
        {
            clients.Remove(client);
            client.Close();
        }
    }

    // 记录消息到文件的方法
    private static void LogMessage(string message)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");  // 获取当前日期
        string filePath = $"chatlog_{date}.txt";  // 使用日期作为文件名的一部分

        using (StreamWriter writer = new StreamWriter(filePath, true))  // 以追加方式打开文件
        {
            writer.WriteLine($"{DateTime.Now}: {message}");  // 写入时间戳和消息
        }
    }


    private static void Broadcast(string message, Socket sender)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        foreach (Socket client in clients)
        {
            if (client != sender)
            {
                client.Send(buffer);
            }
        }
    }
}
