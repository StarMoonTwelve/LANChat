using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Write("请输入您的昵称: ");
        string nickname = Console.ReadLine();

        Console.Write("请输入IP地址及端口号(格式:192.168.0.1-8888): ");
        string Url = Console.ReadLine();
        string[] parts = Url.Split('-');

        TcpClient client = new TcpClient(parts[0], Convert.ToInt32(parts[1]));
        NetworkStream stream = client.GetStream();
        if (client.Connected)
        {
            Console.WriteLine("连接成功,可以愉快的聊天了...");
        }
        else
        {
            Console.WriteLine("连接失败...");
            return;
        }
        Thread receiveThread = new Thread(() => ReceiveMessages(stream));
        receiveThread.Start();

        while (true)
        {
            string message = Console.ReadLine();
            string formattedMessage = $"{nickname}: {message}";
            byte[] buffer = Encoding.UTF8.GetBytes(formattedMessage);
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    private static void ReceiveMessages(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);
        }
    }
}
