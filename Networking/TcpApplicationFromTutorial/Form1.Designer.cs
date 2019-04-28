namespace TcpApplicationFromTutorial
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnServerStart = new System.Windows.Forms.Button();
            this.TxtServerPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtServerIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BtnSendMessage = new System.Windows.Forms.Button();
            this.TxtMessage = new System.Windows.Forms.TextBox();
            this.TxtSummary = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BtnClientConnect = new System.Windows.Forms.Button();
            this.TxtClientPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtClientIP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BtnServerStart);
            this.groupBox1.Controls.Add(this.TxtServerPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TxtServerIP);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(684, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // BtnServerStart
            // 
            this.BtnServerStart.Location = new System.Drawing.Point(599, 39);
            this.BtnServerStart.Name = "BtnServerStart";
            this.BtnServerStart.Size = new System.Drawing.Size(75, 23);
            this.BtnServerStart.TabIndex = 2;
            this.BtnServerStart.Text = "Start";
            this.BtnServerStart.UseVisualStyleBackColor = true;
            this.BtnServerStart.Click += new System.EventHandler(this.BtnServerStart_Click);
            // 
            // TxtServerPort
            // 
            this.TxtServerPort.Location = new System.Drawing.Point(369, 39);
            this.TxtServerPort.Name = "TxtServerPort";
            this.TxtServerPort.ReadOnly = true;
            this.TxtServerPort.Size = new System.Drawing.Size(212, 22);
            this.TxtServerPort.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(316, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port";
            // 
            // TxtServerIP
            // 
            this.TxtServerIP.Location = new System.Drawing.Point(89, 39);
            this.TxtServerIP.Name = "TxtServerIP";
            this.TxtServerIP.ReadOnly = true;
            this.TxtServerIP.Size = new System.Drawing.Size(212, 22);
            this.TxtServerIP.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BtnSendMessage);
            this.groupBox3.Controls.Add(this.TxtMessage);
            this.groupBox3.Controls.Add(this.TxtSummary);
            this.groupBox3.Location = new System.Drawing.Point(12, 283);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(684, 412);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // BtnSendMessage
            // 
            this.BtnSendMessage.Location = new System.Drawing.Point(599, 362);
            this.BtnSendMessage.Name = "BtnSendMessage";
            this.BtnSendMessage.Size = new System.Drawing.Size(75, 23);
            this.BtnSendMessage.TabIndex = 2;
            this.BtnSendMessage.Text = "Send";
            this.BtnSendMessage.UseVisualStyleBackColor = true;
            // 
            // TxtMessage
            // 
            this.TxtMessage.Location = new System.Drawing.Point(10, 364);
            this.TxtMessage.Name = "TxtMessage";
            this.TxtMessage.Size = new System.Drawing.Size(571, 22);
            this.TxtMessage.TabIndex = 1;
            // 
            // TxtSummary
            // 
            this.TxtSummary.Location = new System.Drawing.Point(10, 22);
            this.TxtSummary.Multiline = true;
            this.TxtSummary.Name = "TxtSummary";
            this.TxtSummary.ReadOnly = true;
            this.TxtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtSummary.Size = new System.Drawing.Size(664, 318);
            this.TxtSummary.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BtnClientConnect);
            this.groupBox2.Controls.Add(this.TxtClientPort);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.TxtClientIP);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 156);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(684, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Client";
            // 
            // BtnClientConnect
            // 
            this.BtnClientConnect.Location = new System.Drawing.Point(599, 39);
            this.BtnClientConnect.Name = "BtnClientConnect";
            this.BtnClientConnect.Size = new System.Drawing.Size(75, 23);
            this.BtnClientConnect.TabIndex = 2;
            this.BtnClientConnect.Text = "Connect";
            this.BtnClientConnect.UseVisualStyleBackColor = true;
            this.BtnClientConnect.Click += new System.EventHandler(this.BtnClientConnect_Click);
            // 
            // TxtClientPort
            // 
            this.TxtClientPort.Location = new System.Drawing.Point(369, 39);
            this.TxtClientPort.Name = "TxtClientPort";
            this.TxtClientPort.Size = new System.Drawing.Size(212, 22);
            this.TxtClientPort.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(316, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Port";
            // 
            // TxtClientIP
            // 
            this.TxtClientIP.Location = new System.Drawing.Point(89, 39);
            this.TxtClientIP.Name = "TxtClientIP";
            this.TxtClientIP.Size = new System.Drawing.Size(212, 22);
            this.TxtClientIP.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "IP Address";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 707);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Tcp Application";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BtnServerStart;
        private System.Windows.Forms.TextBox TxtServerPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtServerIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnSendMessage;
        private System.Windows.Forms.TextBox TxtMessage;
        private System.Windows.Forms.TextBox TxtSummary;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BtnClientConnect;
        private System.Windows.Forms.TextBox TxtClientPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtClientIP;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}

