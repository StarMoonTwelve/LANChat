using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    static void Main(string[] args)
    {
        Console.Write("请输入您的用户名: ");
        string username = Console.ReadLine();

        Console.Write("请输入服务器的IP地址: ");
        string serverIp = Console.ReadLine();

        Console.Write("请输入服务器的端口号: ");
        int port = int.Parse(Console.ReadLine());

        TcpClient client = new TcpClient(serverIp, port);
        NetworkStream stream = client.GetStream();

        Thread receiveThread = new Thread(() => ReceiveMessages(stream));
        receiveThread.Start();

        Console.WriteLine("输入您的消息（输入exit退出）：");
        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit") break;

            string formattedMessage = $"{username}: {message}";
            byte[] data = Encoding.UTF8.GetBytes(formattedMessage);
            stream.Write(data, 0, data.Length);
        }

        client.Close();
    }

    private static void ReceiveMessages(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0) break;

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);
        }
    }
}
