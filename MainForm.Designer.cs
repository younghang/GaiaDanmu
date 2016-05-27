/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/20
 * Time: 22:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ZQDanmuTest
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button btn_Send;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btn_SendMessage;
		private System.Windows.Forms.CheckBox IPCheck;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.btn_Send = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btn_SendMessage = new System.Windows.Forms.Button();
			this.IPCheck = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox1.Location = new System.Drawing.Point(12, 1);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(503, 487);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.richTextBox1.TextChanged += new System.EventHandler(this.RichTextBox1TextChanged);
			this.richTextBox1.MouseEnter += new System.EventHandler(this.RichTextBox1MouseEnter);
			this.richTextBox1.MouseLeave += new System.EventHandler(this.RichTextBox1MouseLeave);
			// 
			// btn_Send
			// 
			this.btn_Send.Location = new System.Drawing.Point(366, 485);
			this.btn_Send.Name = "btn_Send";
			this.btn_Send.Size = new System.Drawing.Size(72, 25);
			this.btn_Send.TabIndex = 1;
			this.btn_Send.Text = "停止";
			this.btn_Send.UseVisualStyleBackColor = true;
			this.btn_Send.Click += new System.EventHandler(this.Btn_SendClick);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(13, 485);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(212, 21);
			this.textBox1.TabIndex = 2;
			// 
			// btn_SendMessage
			// 
			this.btn_SendMessage.Location = new System.Drawing.Point(258, 485);
			this.btn_SendMessage.Name = "btn_SendMessage";
			this.btn_SendMessage.Size = new System.Drawing.Size(72, 25);
			this.btn_SendMessage.TabIndex = 1;
			this.btn_SendMessage.Text = "发送";
			this.btn_SendMessage.UseVisualStyleBackColor = true;
			this.btn_SendMessage.Click += new System.EventHandler(this.Btn_SendMessageClick);
			// 
			// IPCheck
			// 
			this.IPCheck.Location = new System.Drawing.Point(444, 489);
			this.IPCheck.Name = "IPCheck";
			this.IPCheck.Size = new System.Drawing.Size(72, 19);
			this.IPCheck.TabIndex = 3;
			this.IPCheck.Text = "IP";
			this.IPCheck.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 511);
			this.Controls.Add(this.IPCheck);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btn_SendMessage);
			this.Controls.Add(this.btn_Send);
			this.Controls.Add(this.richTextBox1);
			this.Name = "MainForm";
			this.Text = "GaiaDanmu";
			this.TopMost = true;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
