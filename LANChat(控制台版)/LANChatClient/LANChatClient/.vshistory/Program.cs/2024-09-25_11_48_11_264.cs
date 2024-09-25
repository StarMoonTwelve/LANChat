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

        TcpClient client = new TcpClient("192.168.199.159", 8888);
        NetworkStream stream = client.GetStream();
        if (client.Connected)
        {
            Console.Write("连接成功");
        }
        else
        {
            Console.Write("连接失败");
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
