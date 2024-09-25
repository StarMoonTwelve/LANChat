using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    static void Main(string[] args)
    {
        // 在这里替换为你的本地 IP 地址
        string serverIp = "你的本地IP地址"; // e.g., "192.168.1.100"
        TcpClient client = new TcpClient(serverIp, 5000);
        NetworkStream stream = client.GetStream();

        Thread receiveThread = new Thread(() => ReceiveMessages(stream));
        receiveThread.Start();

        Console.WriteLine("输入您的消息（输入exit退出）：");
        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit") break;

            byte[] data = Encoding.UTF8.GetBytes(message);
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
