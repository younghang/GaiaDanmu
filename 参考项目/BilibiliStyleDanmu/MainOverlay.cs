/*
 * Created by SharpDevelop.
 * User: young
 * Date: 06/08/2016
 * Time: 19:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BilibiliStyleDanmu
{
	/// <summary>
	/// Description of MainOverlay.
	/// </summary>
	public partial class MainOverlay : Window, IComponentConnector
	{

		public MainOverlay()
		{
			this.InitializeComponent();
			Topmost = true;
			this.timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, new EventHandler(this.ShowFront), base.Dispatcher);
			this.timer.Start();
		}
		private void ShowFront(object sender, EventArgs eventArgs)
		{
			
			if (this != null) {
				this.Topmost = false;
				this.Topmost = true;
			}
		}
		private readonly DispatcherTimer timer;
		public void Initail()
		{
			this.OpenOverlay();
			this.Show();
//			new Thread(new ThreadStart(() => {
//			                           	while (true) {
//			                           		this.AddDMText("LoveGaia", "我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥", false, false);
//			                           		Thread.Sleep(1000);
//			                           	}
//			                           })).Start();
		}
		private void sb_Completed(object sender, EventArgs e)
		{
			ClockGroup clockGroup = sender as ClockGroup;
			if (clockGroup == null) {
				return;
			}
			DanmakuTextControl danmakuTextControl = Storyboard.GetTarget(clockGroup.Children[2].Timeline) as DanmakuTextControl;
			if (danmakuTextControl != null) {
				this.LayoutRoot.Children.Remove(danmakuTextControl);
			}
		}
		private void overlay_Deactivated(object sender, EventArgs e)
		{
			if (sender is MainOverlay) {
				(sender as MainOverlay).Topmost = true;
			}
//			overlay.Topmost=true;
		}
		[DllImport("user32")]
		private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

		[DllImport("user32")]
		private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
		public void OpenOverlay()
		{
//			this.overlay = new MainOverlay();
			this.Deactivated += new EventHandler(this.overlay_Deactivated);
			this.SourceInitialized += delegate(object sender, EventArgs e) {
				IntPtr expr_10 = new WindowInteropHelper(this).Handle;
				uint windowLong = GetWindowLong(expr_10, -20);
				SetWindowLong(expr_10, -20, windowLong | 32u);
			};
			this.Background = Brushes.Transparent;
			this.ShowInTaskbar = false;
			this.Topmost = true;
			this.Top = SystemParameters.WorkArea.Top + Store.MainOverlayXoffset;
			this.Left = SystemParameters.WorkArea.Right - Store.MainOverlayWidth + Store.MainOverlayYoffset;
			this.Height = SystemParameters.WorkArea.Height;
			this.Width = Store.MainOverlayWidth;
		}
		public void AddDMText(string user, string text, bool warn = false, bool foreceenablefullscreen = false)
		{
			
			if (Dispatcher.CheckAccess()) {
//				if (this.SideBar.IsChecked == true)
//				{
				DanmakuTextControl danmakuTextControl = new DanmakuTextControl();
				danmakuTextControl.UserName.Text = user;
				if (warn) {
					danmakuTextControl.UserName.Foreground = Brushes.Red;
				}
				danmakuTextControl.Text.Text = text;
				danmakuTextControl.ChangeHeight();
				((Storyboard)danmakuTextControl.Resources["Storyboard1"]).Completed += new EventHandler(this.sb_Completed);
				this.LayoutRoot.Children.Add(danmakuTextControl);
//				}
//				if (this.Full.IsChecked == true && (!warn | foreceenablefullscreen))
//				{
//				this.fulloverlay.AddDanmaku(DanmakuType.Scrolling, text, 4294967295u);
//				return;
//				}
			} else {
				Dispatcher.BeginInvoke(new Action(delegate {
				                                  	this.AddDMText(user, text, false, false);
				                                  }));
			}
		}
		

		
		
	}
}
