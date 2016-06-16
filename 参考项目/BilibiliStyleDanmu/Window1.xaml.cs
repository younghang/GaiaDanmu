/*
 * Created by SharpDevelop.
 * User: young
 * Date: 06/08/2016
 * Time: 19:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BilibiliStyleDanmu
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class BiliDanmujiStyle : Window
	{
		public BiliDanmujiStyle()
		{
			InitializeComponent();
			this.timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, new EventHandler(this.ShowFront), base.Dispatcher);
			this.timer.Start();
		

		}
		private void ShowFront(object sender, EventArgs eventArgs)
		{
		 
			if (this.overlay != null) {
				this.overlay.Topmost = false;
				this.overlay.Topmost = true;
			}
		}
		private readonly DispatcherTimer timer;
		public void window1_Closed(object sender, EventArgs e)
		{
			this.overlay.Close();
			Application.Current.Shutdown();
		}
		public void Initail()
		{
			this.OpenOverlay();
			this.overlay.Show();
		}
		void button1_Click(object sender, RoutedEventArgs e)
		{
			
//			new Thread(new ThreadStart(() => {
//				while (true) {
//					this.AddDMText("LoveGaia", "我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥", false, false);
//					Thread.Sleep(1000);
//				}
//			})).Start();
			
			this.AddDMText("LoveGaia", "我爱大哥我爱大哥我爱大哥我爱大哥我爱大哥尽管大哥不爱我，我依然爱着大哥", false, false);
//			this.OpenFullOverlay();
//			this.fulloverlay.Show();
//			if (new Random().Next(100) > 50) {
//				this.AddDMText("彈幕姬報告", "這不是個測試", true, false);
//			} else {
//				this.AddDMText("彈幕姬報告", "這是一個測試", false, false);
//			}
			
		}
		private void sb_Completed(object sender, EventArgs e)
		{
			ClockGroup clockGroup = sender as ClockGroup;
			if (clockGroup == null) {
				return;
			}
			DanmakuTextControl danmakuTextControl = Storyboard.GetTarget(clockGroup.Children[2].Timeline) as DanmakuTextControl;
			if (danmakuTextControl != null) {
				this.overlay.LayoutRoot.Children.Remove(danmakuTextControl);
			}
		}
		private IDanmakuWindow fulloverlay;
		private bool showerror_enabled = true;
		internal ItemsControl Log;
		public MainOverlay overlay;
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
		private void OpenOverlay()
		{
			this.overlay = new MainOverlay();
			this.overlay.Deactivated += new EventHandler(this.overlay_Deactivated);
			this.overlay.SourceInitialized += delegate(object sender, EventArgs e) {
				IntPtr expr_10 = new WindowInteropHelper(this.overlay).Handle;
				uint windowLong = BiliDanmujiStyle.GetWindowLong(expr_10, -20);
				BiliDanmujiStyle.SetWindowLong(expr_10, -20, windowLong | 32u);
			};
			this.overlay.Background = Brushes.Transparent;
			this.overlay.ShowInTaskbar = false;
			this.overlay.Topmost = true;
			this.overlay.Top = SystemParameters.WorkArea.Top + Store.MainOverlayXoffset;
			this.overlay.Left = SystemParameters.WorkArea.Right - Store.MainOverlayWidth + Store.MainOverlayYoffset;
			this.overlay.Height = SystemParameters.WorkArea.Height;
			this.overlay.Width = Store.MainOverlayWidth;
		}
		private void OpenFullOverlay()
		{
//			Version v = new Version(6, 2, 9200);
//			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= v && Store.WtfEngineEnabled)
//			{
//				this.fulloverlay = new WtfDanmakuWindow();
//			}
//			else
//			{
//				this.fulloverlay = new WpfDanmakuOverlay();
//			}
//			this.settings.PropertyChanged += new PropertyChangedEventHandler(this.fulloverlay.OnPropertyChanged);
//			this.fulloverlay.Show();
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
				this.overlay.LayoutRoot.Children.Add(danmakuTextControl);
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
		void window1_Loaded(object sender, RoutedEventArgs e)
		{
			Initail();
		}

	}
}