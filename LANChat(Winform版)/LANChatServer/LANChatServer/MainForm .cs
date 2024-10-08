﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace LANChatServer
{
    public partial class MainForm : Form
    {
        private Dictionary<int, TcpListener> servers = new Dictionary<int, TcpListener>();
        private Dictionary<int, TextBox> serverConsoles = new Dictionary<int, TextBox>();
        private Dictionary<int, bool> serverStates = new Dictionary<int, bool>(); // 记录端口状态
        private Dictionary<int, List<Socket>> clientSockets = new Dictionary<int, List<Socket>>(); // 管理每个端口的客户端套接字

        public MainForm()
        {
            InitializeComponent();
            lblCurrentIP.Text = $"当前IP地址: {GetLocalIPAddress()}"; // 获取并显示当前IP
        }

        private void btnCheckPort_Click(object sender, EventArgs e)
            {
            if (int.TryParse(txtPort.Text, out int port))
            {
                bool isUsed = IsPortInUse(port);
                MessageBox.Show(isUsed ? "端口已被占用" : "端口可用");
            }
            else
            {
                MessageBox.Show("请输入有效的端口号");
            }
        }

        private bool IsPortInUse(int port)
        {
            bool result = false;
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
            }
            catch (SocketException)
            {
                result = true; // 端口已被占用
            }
            finally
            {
                listener?.Stop();
            }
            return result;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPort.Text, out int port) && !servers.ContainsKey(port))
            {
                var server = new TcpListener(IPAddress.Any, port);
                server.Start();
                servers[port] = server;
                serverStates[port] = true; // 设置端口为活动状态
                clientSockets[port] = new List<Socket>(); // 初始化客户端列表

                Thread serverThread = new Thread(() => ListenForClients(server, port));
                serverThread.Start();

                AddServerTab(port);
                MessageBox.Show($"聊天服务器 {port} 已启动");
            }
            else if (servers.ContainsKey(port))
            {
                MessageBox.Show($"聊天服务器 {port} 已在运行中");
            }
            else
            {
                MessageBox.Show("请输入有效的端口号");
            }
        }

        private void ListenForClients(TcpListener server, int port)
        {
            try
            {
                while (serverStates[port])
                {
                    var client = server.AcceptSocket();
                    if (serverStates[port])
                    {
                        clientSockets[port].Add(client); // 添加客户端套接字
                        Thread clientThread = new Thread(() => HandleClient(client, port));
                        clientThread.Start();
                    }
                }
            }
            catch (SocketException) { }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPort.Text, out int port) && servers.ContainsKey(port))
            {
                StopServer(port);
            }
            else
            {
                MessageBox.Show("请输入有效的端口号或服务器未启动");
            }
        }

        private void StopServer(int port)
        {
            if (servers.TryGetValue(port, out var server))
            {
                serverStates[port] = false; // 设置端口为非活动状态
                server.Stop();

                // 关闭所有客户端连接
                foreach (var client in clientSockets[port])
                {
                    client.Close();
                }
                clientSockets[port].Clear(); // 清空客户端列表

                // 删除对应的标签页
                if (serverConsoles.ContainsKey(port))
                {
                    var tabPage = tabControlServers.TabPages.Cast<TabPage>().FirstOrDefault(t => t.Text == $"服务器: {port}");
                    if (tabPage != null)
                    {
                        tabControlServers.TabPages.Remove(tabPage);
                        serverConsoles.Remove(port);
                    }
                }

                // 移除端口和服务器的记录
                servers.Remove(port);
                MessageBox.Show($"聊天服务器 {port} 已停止");
            }
        }

        private void AddServerTab(int port)
        {
            TabPage newTab = new TabPage($"服务器: {port}");
            TextBox consoleTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };
            newTab.Controls.Add(consoleTextBox);
            tabControlServers.TabPages.Add(newTab);
            serverConsoles[port] = consoleTextBox; // 将端口与控制台关联
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
            throw new Exception("未找到本地IP地址");
        }

        private void HandleClient(Socket client, int port)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = client.Receive(buffer)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    UpdateConsole(port, message); // 更新对应控制台
                    Broadcast(message, client, port);
                    LogMessage(port, message);
                }
            }
            catch (SocketException) { }
            catch (Exception ex) { }
            finally
            {
                clientSockets[port].Remove(client); // 从列表中移除客户端
                client.Close();
            }
        }

        private void UpdateConsole(int port, string message)
        {
            if (serverConsoles.TryGetValue(port, out var consoleTextBox))
            {
                // 在UI线程更新控制台内容
                Invoke(new Action(() =>
                {
                    consoleTextBox.AppendText($"收到消息: {message}{Environment.NewLine}");
                }));
            }
        }

        private void Broadcast(string message, Socket sender, int port)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            foreach (var client in clientSockets[port]) // 获取当前端口的所有客户端套接字
            {
                if (client != sender)
                {
                    client.Send(buffer);
                }
            }
        }

        /// <summary>
        /// 记录消息到文件的方法
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="message">信息</param>
        private void LogMessage(int port,string message)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath); // 创建文件夹
            }

            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = Path.Combine(folderPath, $"chatlog_{GetLocalIPAddress()}_{port}_{date}.txt"); // 避免使用冒号

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8)) // 指定编码
                {
                    writer.WriteLine($"{message}");
                }
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show($"不支持的路径格式: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"没有权限写入文件: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"写入日志时出错: {ex.Message}");
            }
        }
        /// <summary>
        /// 添加关闭卡控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 检查 Dictionary 是否为空
            if (servers.Count > 0)
            {
                var result = MessageBox.Show("目前还存在使用的服务，是否需要全部关闭？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    e.Cancel = true; // 取消关闭

                }
                else
                {
                    // 如果用户选择是，可以在这里关闭所有 TcpListener
                    foreach (var listener in servers.Values)
                    {
                        listener.Stop(); // 停止每个 TcpListener
                    }
                }
            }
        }
    }
}
