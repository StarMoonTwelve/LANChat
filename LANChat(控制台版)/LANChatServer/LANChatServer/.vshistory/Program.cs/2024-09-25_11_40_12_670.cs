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

        while ((bytesRead = client.Receive(buffer)) > 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("收到消息: " + message);
            Broadcast(message, client);
        }

        clients.Remove(client);
        client.Close();
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
