namespace LANChatServer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCurrentIP = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnCheckPort = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tabControlServers = new System.Windows.Forms.TabControl();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCurrentIP
            // 
            this.lblCurrentIP.AutoSize = true;
            this.lblCurrentIP.Location = new System.Drawing.Point(16, 10);
            this.lblCurrentIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentIP.Name = "lblCurrentIP";
            this.lblCurrentIP.Size = new System.Drawing.Size(99, 15);
            this.lblCurrentIP.TabIndex = 0;
            this.lblCurrentIP.Text = "当前IP地址: ";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(115, 40);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(132, 25);
            this.txtPort.TabIndex = 1;
            // 
            // btnCheckPort
            // 
            this.btnCheckPort.Location = new System.Drawing.Point(255, 38);
            this.btnCheckPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCheckPort.Name = "btnCheckPort";
            this.btnCheckPort.Size = new System.Drawing.Size(100, 27);
            this.btnCheckPort.TabIndex = 2;
            this.btnCheckPort.Text = "检查端口";
            this.btnCheckPort.UseVisualStyleBackColor = true;
            this.btnCheckPort.Click += new System.EventHandler(this.btnCheckPort_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(15, 85);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 27);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "启动服务器";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(133, 85);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 27);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "停止服务器";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tabControlServers
            // 
            this.tabControlServers.Location = new System.Drawing.Point(13, 118);
            this.tabControlServers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControlServers.Name = "tabControlServers";
            this.tabControlServers.SelectedIndex = 0;
            this.tabControlServers.Size = new System.Drawing.Size(606, 295);
            this.tabControlServers.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "端口号: ";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 425);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControlServers);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnCheckPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblCurrentIP);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "局域网聊天服务器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblCurrentIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnCheckPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TabControl tabControlServers;
        private System.Windows.Forms.Label label1;
    }
}

