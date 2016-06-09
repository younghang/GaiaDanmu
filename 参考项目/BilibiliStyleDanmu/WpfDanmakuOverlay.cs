///*
// * Created by SharpDevelop.
// * User: young
// * Date: 06/08/2016
// * Time: 20:24
// * 
// * To change this template use Tools | Options | Coding | Edit Standard Headers.
// */
//using System;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Interop;
//using System.Windows.Markup;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Threading;
//
//namespace BilibiliStyleDanmu
//{
//	/// <summary>
//	/// Description of WpfDanmakuOverlay.
//	/// </summary>
//	public class WpfDanmakuOverlay : Window, IDanmakuWindow, IDisposable, IComponentConnector
//	{
//		[CompilerGenerated]
//		[Serializable]
//		private sealed class Ceal
//		{
//			public static readonly WpfDanmakuOverlay wpfd = new WpfDanmakuOverlay();
//
//			public static Func<KeyValuePair<double, bool>, bool> fpair1;
//
//			public static Func<KeyValuePair<double, bool>, double> fpair2;
//
//			public static Func<KeyValuePair<double, bool>, bool> fpair3;
//
//			public static Func<KeyValuePair<double, bool>, double> fpair4;
//
//			internal bool AddDanmaku1(KeyValuePair<double, bool> p)
//			{
//				return !p.Value;
//			}
//
//			internal double AddDanmaku2(KeyValuePair<double, bool> p)
//			{
//				return p.Key;
//			}
//
//			internal bool AddDanmaku3(KeyValuePair<double, bool> p)
//			{
//				return p.Value;
//			}
//
//			internal double AddDanmaku4(KeyValuePair<double, bool> p)
//			{
//				return p.Key;
//			}
//		}
//
//		private const int WS_EX_TRANSPARENT = 32;
//
//		private const int GWL_EXSTYLE = -20;
//
//		internal WpfDanmakuOverlay Window;
//
//		internal Grid LayoutRoot;
//
//		private bool _contentLoaded;
//
//		[DllImport("user32")]
//		private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
//
//		[DllImport("user32")]
//		private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
//
//		public WpfDanmakuOverlay()
//		{
//			this.InitializeComponent();
//			base.Deactivated += new EventHandler(this.Overlay_Deactivated);
//			base.Background = Brushes.Transparent;
//			base.SourceInitialized += delegate(object sender, EventArgs e)
//			{
//				IntPtr expr_0B = new WindowInteropHelper(this).Handle;
//				uint windowLong = WpfDanmakuOverlay.GetWindowLong(expr_0B, -20);
//				WpfDanmakuOverlay.SetWindowLong(expr_0B, -20, windowLong | 32u);
//			};
//			base.ShowInTaskbar = false;
//			base.Topmost = true;
//			base.Top = SystemParameters.WorkArea.Top;
//			base.Left = SystemParameters.WorkArea.Left;
//			base.Width = SystemParameters.WorkArea.Width;
//			base.Height = 550.0;
//		}
//
//		void IDisposable.Dispose()
//		{
//		}
//
//		void IDanmakuWindow.Show()
//		{
//			base.Show();
//		}
//
//		void IDanmakuWindow.Close()
//		{
//			base.Close();
//		}
//
//		void IDanmakuWindow.ForceTopmost()
//		{
//			base.Topmost = false;
//			base.Topmost = true;
//		}
//
//		void IDanmakuWindow.AddDanmaku(DanmakuType type, string comment, uint color)
//		{
//			if (base.CheckAccess())
//			{
//				UIElementCollection children = this.LayoutRoot.Children;
//				lock (children)
//				{
//					FullScreenDanmaku fullScreenDanmaku = new FullScreenDanmaku();
//					fullScreenDanmaku.Text.Text = comment;
//					fullScreenDanmaku.ChangeHeight();
//					double width = fullScreenDanmaku.Text.DesiredSize.Width;
//					Dictionary<double, bool> dictionary = new Dictionary<double, bool>();
//					dictionary.Add(0.0, true);
//					foreach (object current in this.LayoutRoot.Children)
//					{
//						if (current is FullScreenDanmaku)
//						{
//							FullScreenDanmaku fullScreenDanmaku2 = current as FullScreenDanmaku;
//							if (!dictionary.ContainsKey((double)Convert.ToInt32(fullScreenDanmaku2.Margin.Top)))
//							{
//								dictionary.Add((double)Convert.ToInt32(fullScreenDanmaku2.Margin.Top), true);
//							}
//							if (fullScreenDanmaku2.Margin.Left > SystemParameters.PrimaryScreenWidth - width - 50.0)
//							{
//								dictionary[(double)Convert.ToInt32(fullScreenDanmaku2.Margin.Top)] = false;
//							}
//						}
//					}
//					IEnumerable<KeyValuePair<double, bool>> arg_186_0 = dictionary;
//					Func<KeyValuePair<double, bool>, bool> arg_186_1;
//					if ((arg_186_1 = WpfDanmakuOverlay.<>c.<>9__9_0) == null)
//					{
//						arg_186_1 = (WpfDanmakuOverlay.<>c.<>9__9_0 = new Func<KeyValuePair<double, bool>, bool>(WpfDanmakuOverlay.<>c.<>9.<Bililive_dm.IDanmakuWindow.AddDanmaku1));
//					}
//					double top;
//					if (arg_186_0.All(arg_186_1))
//					{
//						IEnumerable<KeyValuePair<double, bool>> arg_1AE_0 = dictionary;
//						Func<KeyValuePair<double, bool>, double> arg_1AE_1;
//						if ((arg_1AE_1 = WpfDanmakuOverlay.<>c.<>9__9_1) == null)
//						{
//							arg_1AE_1 = (WpfDanmakuOverlay.<>c.<>9__9_1 = new Func<KeyValuePair<double, bool>, double>(WpfDanmakuOverlay.<>c.<>9.<Bililive_dm.IDanmakuWindow.AddDanmaku2));
//						}
//						top = arg_1AE_0.Max(arg_1AE_1) + fullScreenDanmaku.Text.DesiredSize.Height;
//					}
//					else
//					{
//						IEnumerable<KeyValuePair<double, bool>> arg_1ED_0 = dictionary;
//						Func<KeyValuePair<double, bool>, bool> arg_1ED_1;
//						if ((arg_1ED_1 = WpfDanmakuOverlay.<>c.<>9__9_2) == null)
//						{
//							arg_1ED_1 = (WpfDanmakuOverlay.<>c.<>9__9_2 = new Func<KeyValuePair<double, bool>, bool>(WpfDanmakuOverlay.<>c.<>9.<Bililive_dm.IDanmakuWindow.AddDanmaku3));
//						}
//						IEnumerable<KeyValuePair<double, bool>> arg_211_0 = arg_1ED_0.Where(arg_1ED_1);
//						Func<KeyValuePair<double, bool>, double> arg_211_1;
//						if ((arg_211_1 = WpfDanmakuOverlay.<>c.<>9__9_3) == null)
//						{
//							arg_211_1 = (WpfDanmakuOverlay.<>c.<>9__9_3 = new Func<KeyValuePair<double, bool>, double>(WpfDanmakuOverlay.<>c.<>9.<Bililive_dm.IDanmakuWindow.AddDanmaku4));
//						}
//						top = arg_211_0.Min(arg_211_1);
//					}
//					Storyboard storyboard = new Storyboard();
//					Duration duration = new Duration(TimeSpan.FromTicks(Convert.ToInt64((SystemParameters.PrimaryScreenWidth + width) / Store.FullOverlayEffect1 * 10000000.0)));
//					ThicknessAnimation thicknessAnimation = new ThicknessAnimation(new Thickness(SystemParameters.PrimaryScreenWidth, top, 0.0, 0.0), new Thickness(-width, top, 0.0, 0.0), duration);
//					storyboard.Children.Add(thicknessAnimation);
//					storyboard.Duration = duration;
//					Storyboard.SetTarget(thicknessAnimation, fullScreenDanmaku);
//					Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("(FrameworkElement.Margin)", new object[0]));
//					this.LayoutRoot.Children.Add(fullScreenDanmaku);
//					storyboard.Completed += new EventHandler(this.s_Completed);
//					storyboard.Begin();
//					return;
//				}
//			}
//			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
//			{
//				((IDanmakuWindow)this).AddDanmaku(type, comment, color);
//			}));
//		}
//
//		private void s_Completed(object sender, EventArgs e)
//		{
//			ClockGroup clockGroup = sender as ClockGroup;
//			if (clockGroup == null)
//			{
//				return;
//			}
//			FullScreenDanmaku fullScreenDanmaku = Storyboard.GetTarget(clockGroup.Children[0].Timeline) as FullScreenDanmaku;
//			if (fullScreenDanmaku != null)
//			{
//				this.LayoutRoot.Children.Remove(fullScreenDanmaku);
//			}
//		}
//
//		private void Overlay_Deactivated(object sender, EventArgs e)
//		{
//			if (sender is WpfDanmakuOverlay)
//			{
//				(sender as WpfDanmakuOverlay).Topmost = true;
//			}
//		}
//
//		void IDanmakuWindow.OnPropertyChanged(object sender, PropertyChangedEventArgs e)
//		{
//		}
//
//		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
//		public void InitializeComponent()
//		{
//			if (this._contentLoaded)
//			{
//				return;
//			}
//			this._contentLoaded = true;
//			Uri resourceLocator = new Uri("/Bililive_dm;component/wpfdanmakuoverlay.xaml", UriKind.Relative);
//			Application.LoadComponent(this, resourceLocator);
//		}
//
//		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
//		void IComponentConnector.Connect(int connectionId, object target)
//		{
//			if (connectionId == 1)
//			{
//				this.Window = (WpfDanmakuOverlay)target;
//				return;
//			}
//			if (connectionId != 2)
//			{
//				this._contentLoaded = true;
//				return;
//			}
//			this.LayoutRoot = (Grid)target;
//		}
//	}
//}
