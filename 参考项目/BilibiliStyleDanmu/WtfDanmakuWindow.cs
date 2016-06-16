/*
 * Created by SharpDevelop.
 * User: young
 * Date: 06/08/2016
 * Time: 20:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace BilibiliStyleDanmu
{
	/// <summary>
	/// Description of WtfDanmakuWindow.
	/// </summary>
	public class WtfDanmakuWindow : Form, IDanmakuWindow, IDisposable
	{
		private const int WS_EX_LAYERED = 524288;

		private const int WS_EX_TRANSPARENT = 32;

		private const int WS_EX_NOREDIRECTIONBITMAP = 2097152;

		private const int GWL_EXSTYLE = -20;

		private IntPtr _wtf;

		private IContainer components;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams expr_06 = base.CreateParams;
				expr_06.ExStyle |= 2097152;
				return expr_06;
			}
		}

		[DllImport("user32")]
		private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

		[DllImport("user32")]
		private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

		[DllImport("libwtfdanmaku")]
		private static extern IntPtr WTF_CreateInstance();

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_ReleaseInstance(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern int WTF_InitializeWithHwnd(IntPtr instance, IntPtr hwnd);

		[DllImport("libwtfdanmaku")]
		private static extern int WTF_InitializeOffscreen(IntPtr instance, uint initialWidth, uint initialHeight);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_Terminate(IntPtr instance);

		[DllImport("libwtfdanmaku", CharSet = CharSet.Unicode)]
		private static extern void WTF_AddLiveDanmaku(IntPtr instance, int type, long time, string comment, int fontSize, int fontColor, long timestamp, int danmakuId);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_Start(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_Pause(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_Resume(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_Stop(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_Resize(IntPtr instance, uint width, uint height);

		[DllImport("libwtfdanmaku")]
		private static extern int WTF_IsRunning(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern float WTF_GetFontScaleFactor(IntPtr instance);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_SetFontScaleFactor(IntPtr instance, float factor);

		[DllImport("libwtfdanmaku", CharSet = CharSet.Unicode)]
		private static extern void WTF_SetFontName(IntPtr instance, string fontName);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_SetDanmakuStyle(IntPtr instance, int style);

		[DllImport("libwtfdanmaku")]
		private static extern void WTF_SetCompositionOpacity(IntPtr instance, float opacity);

		public WtfDanmakuWindow()
		{
			this.InitializeComponent();
			base.Resize += new EventHandler(this.WtfDanmakuWindow_Resize);
			base.FormClosing += new FormClosingEventHandler(this.WtfDanmakuWindow_FormClosing);
		}

		~WtfDanmakuWindow()
		{
			((IDisposable)this).Dispose();
		}

		private void WtfDanmakuWindow_Load(object sender, EventArgs e)
		{
			base.ShowInTaskbar = false;
			base.TopMost = true;
			base.FormBorderStyle = FormBorderStyle.None;
			base.WindowState = FormWindowState.Maximized;
			uint windowLong = WtfDanmakuWindow.GetWindowLong(base.Handle, -20);
			WtfDanmakuWindow.SetWindowLong(base.Handle, -20, windowLong | 524288u | 32u);
			this.CreateWTF();
		}

		private void WtfDanmakuWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.DestroyWTF();
		}

		private void WtfDanmakuWindow_Resize(object sender, EventArgs e)
		{
			if (this._wtf != IntPtr.Zero)
			{
				WtfDanmakuWindow.WTF_Resize(this._wtf, (uint)base.ClientSize.Width, (uint)base.ClientSize.Height);
			}
		}

		private void CreateWTF()
		{
			this._wtf = WtfDanmakuWindow.WTF_CreateInstance();
			WtfDanmakuWindow.WTF_InitializeWithHwnd(this._wtf, base.Handle);
			WtfDanmakuWindow.WTF_SetFontName(this._wtf, "SimHei");
			WtfDanmakuWindow.WTF_SetFontScaleFactor(this._wtf, (float)(Store.FullOverlayFontsize / 25.0));
			WtfDanmakuWindow.WTF_SetCompositionOpacity(this._wtf, 0.85f);
		}

		private void DestroyWTF()
		{
			if (this._wtf != IntPtr.Zero)
			{
				if (WtfDanmakuWindow.WTF_IsRunning(this._wtf) != 0)
				{
					WtfDanmakuWindow.WTF_Stop(this._wtf);
				}
				WtfDanmakuWindow.WTF_Terminate(this._wtf);
				WtfDanmakuWindow.WTF_ReleaseInstance(this._wtf);
				this._wtf = IntPtr.Zero;
			}
		}

		void IDisposable.Dispose()
		{
			if (this._wtf != IntPtr.Zero)
			{
				this.DestroyWTF();
			}
		}

		void IDanmakuWindow.Show()
		{
			base.Show();
			WtfDanmakuWindow.WTF_Start(this._wtf);
		}

		void IDanmakuWindow.Close()
		{
			base.Close();
		}

		void IDanmakuWindow.ForceTopmost()
		{
		}

		void IDanmakuWindow.AddDanmaku(DanmakuType type, string comment, uint color)
		{
			WtfDanmakuWindow.WTF_AddLiveDanmaku(this._wtf, (int)type, 0L, comment, 25, (int)color, 0L, 0);
		}

		void IDanmakuWindow.OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this._wtf != IntPtr.Zero)
			{
				WtfDanmakuWindow.WTF_SetFontScaleFactor(this._wtf, (float)(Store.FullOverlayFontsize / 25.0));
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(8f, 15f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(915, 534);
			base.Name = "WtfDanmakuWindow";
			this.Text = "WtfDanmakuWindow";
			base.Load += new EventHandler(this.WtfDanmakuWindow_Load);
			base.ResumeLayout(false);
		}
	}
}
