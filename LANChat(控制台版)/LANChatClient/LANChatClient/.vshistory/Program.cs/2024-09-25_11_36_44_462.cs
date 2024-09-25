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

        Console.Write("请输入服务器的IP地址-端口号-验证码（格式：IP-端口-验证码）: ");
        string input = Console.ReadLine();
        string[] serverInfo = input?.Split('-');

        if (serverInfo == null || serverInfo.Length != 3)
        {
            Console.WriteLine("输入格式错误，请使用：IP-端口-验证码");
            return;
        }

        string serverIp = serverInfo[0];
        if (!int.TryParse(serverInfo[1], out int port))
        {
            Console.WriteLine("端口号无效。");
            return;
        }
        string verificationCode = serverInfo[2];

        try
        {
            TcpClient client = new TcpClient(serverIp, port);
            NetworkStream stream = client.GetStream();

            // 发送用户名
            byte[] usernameData = Encoding.UTF8.GetBytes(username);
            stream.Write(usernameData, 0, usernameData.Length);

            // 发送验证码
            byte[] codeData = Encoding.UTF8.GetBytes(verificationCode);
            stream.Write(codeData, 0, codeData.Length);

            // 检查是否连接成功
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string responseMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (responseMessage.Contains("连接被拒绝"))
            {
                Console.WriteLine(responseMessage);
                client.Close();
                return;
            }

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
        catch (Exception ex)
        {
            Console.WriteLine($"连接失败: {ex.Message}");
        }
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
