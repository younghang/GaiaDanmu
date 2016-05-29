/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/5/28
 * Time: 15:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GaiaDanmu
{
	/// <summary>
	/// Description of FireWorksList.
	/// </summary>
	public partial class FireWorksList : Form
	{
		public FireWorksList()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		List<Thread> listThread=new List<Thread>();
		public void AddRoomid(string id)
		{
			Thread	fireGetThread=new Thread(new ParameterizedThreadStart(Run));
				fireGetThread.Start(id);
				listThread.Add(fireGetThread);
		}
		public void Run(object id)
		{
			String  stid=id as string;
			fireWorks=new FireWorksThread(stid);
			ShowMessage("添加一个"+id);
			fireWorks.StartInitial();
			Thread.Sleep(1000*60*6);
			ShowMessage("完成一个"+id);
			fireWorks.Close();
			
		
		}
 
		int isInShow=0;
		void ShowMessage(string str)
		{
			if (richTextBox1.InvokeRequired) {
				// 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
				Action<string> actionDelegate = (x) => { 
					isInShow++;
					if (isInShow>1) {
						return;
					}
					richTextBox1.Text += x + "\n"; 				 
					richTextBox1.SelectionColor = Color.LimeGreen;
					richTextBox1.SelectionBackColor = Color.White;
					richTextBox1.HideSelection = false;
					isInShow--;
                
				};
				// 或者
				// Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
				this.richTextBox1.Invoke(actionDelegate, str);
			} else {
				this.richTextBox1.Text += str + "\n";
			}

		 
		}
		FireWorksThread fireWorks;
		void RichTextBox1TextChanged(object sender, EventArgs e)
		{
				 richTextBox1.HideSelection=false;
//			
//			 richTextBox1.Select();
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
		}
		void FireWorksListFormClosed(object sender, FormClosedEventArgs e)
		{
			try{
			 foreach (var element in listThread) {
					element.Abort();
					
				}}
				catch
				{
					
				}
		}
	}
}
