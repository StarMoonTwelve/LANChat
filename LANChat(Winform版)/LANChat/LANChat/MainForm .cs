using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace LANChat
{
    public partial class MainForm : Form
    {
        private List<TcpClient> clients = new List<TcpClient>();
        private List<Thread> receiveThreads = new List<Thread>();
        private volatile bool isReceivingMessages = false;

        public MainForm()
        {
            InitializeComponent();
            labelLocalIP.Text = $"{GetLocalIPAddress()}";
            buttonDisconnect.Enabled = true;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string serverIP = textBoxServerIP.Text;
                int port = int.Parse(textBoxPort.Text);

                // 检查是否已经连接
                if (clients.Any(c => c.Client.RemoteEndPoint.ToString() == $"{serverIP}:{port}"))
                {
                    MessageBox.Show("已经连接到该服务器和端口。");
                    return;
                }

                TcpClient client = new TcpClient(serverIP, port);
                NetworkStream stream = client.GetStream();

                if (client.Connected)
                {
                    // 添加控件到标签页
                    // 创建新标签页
                    var tabPage = new TabPage($"{serverIP}:{port}");

                    // 创建消息显示框
                    var messagesBox = new TextBox
                    {
                        Multiline = true,
                        ReadOnly = true,
                        ScrollBars = ScrollBars.Vertical,
                        Dock = DockStyle.Fill
                    };

                    // 创建提示标签
                    var labelInput = new Label
                    {
                        Text = "输入消息(Enter发送消息,Shift+Enter换行):",
                        Dock = DockStyle.Bottom,
                        Height = 20 // 设置标签的高度
                    };

                    // 创建输入框
                    var inputBox = new TextBox
                    {
                        Multiline = true, // 支持多行输入
                        ScrollBars = ScrollBars.Vertical,
                        Dock = DockStyle.Bottom,
                        Height = 60 // 设置输入框的高度
                    };
                    inputBox.KeyDown += (s, args) =>
                    {
                        if (args.KeyCode == Keys.Enter && !args.Shift)
                        {
                            textBoxInputMessage_KeyDown(s, args, messagesBox, inputBox, client, stream);
                        }
                    };

                    // 将控件添加到标签页
                    tabPage.Controls.Add(messagesBox);
                    tabPage.Controls.Add(labelInput);
                    tabPage.Controls.Add(inputBox);
                    tabControl1.TabPages.Add(tabPage);

                    clients.Add(client);
                    isReceivingMessages = true;
                    Thread receiveThread = new Thread(() => ReceiveMessages(messagesBox, stream));
                    receiveThread.IsBackground = true;
                    receiveThreads.Add(receiveThread);
                    receiveThread.Start();

                    // 发送连接消息
                    string formattedMessage = $"{DateTime.Now} - {textBoxUsername.Text} 已连接";
                    SendMessage(formattedMessage, messagesBox, stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败: {ex.Message}");
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            try
            {
                isReceivingMessages = false;

                if (tabControl1.TabPages.Count > 0)
                {
                    var activeTab = tabControl1.SelectedTab;
                    int tabIndex = tabControl1.SelectedIndex;

                    // 关闭当前连接
                    if (tabIndex >= 0 && tabIndex < clients.Count)
                    {
                        TcpClient client = clients[tabIndex];
                        client.Close();
                        clients.RemoveAt(tabIndex);
                    }

                    if (tabIndex >= 0 && tabIndex < receiveThreads.Count)
                    {
                        receiveThreads[tabIndex].Join(1000);
                        receiveThreads.RemoveAt(tabIndex);
                    }

                    // 关闭当前标签页
                    tabControl1.TabPages.Remove(activeTab);
                }

                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"断开失败: {ex.Message}");
            }
        }

        private void ReceiveMessages(TextBox messagesBox, NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while (isReceivingMessages)
                {
                    if (stream.DataAvailable)
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            AppendMessage(messagesBox, message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"接收消息失败: {ex.Message}");
                Disconnect();
            }
        }

        private void textBoxInputMessage_KeyDown(object sender, KeyEventArgs e, TextBox messagesBox, TextBox inputBox, TcpClient client, NetworkStream stream)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string message = inputBox.Text.Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    string formattedMessage = $"{DateTime.Now} - {textBoxUsername.Text}: {message}";
                    SendMessage(formattedMessage, messagesBox, stream); // 发送消息
                    inputBox.Clear(); // 清空输入框
                }
                e.SuppressKeyPress = true; // 防止插入换行
            }
        }

        private void SendMessage(string message, TextBox messagesBox, NetworkStream stream)
        {
            if (stream != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                AppendMessage(messagesBox, message);
            }
        }

        private void AppendMessage(TextBox messagesBox, string message)
        {
            if (messagesBox.InvokeRequired)
            {
                messagesBox.Invoke(new Action<TextBox, string>(AppendMessage), messagesBox, message);
            }
            else
            {
                messagesBox.AppendText(message + Environment.NewLine);
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("未找到本机IP地址.");
        }
    }
}
