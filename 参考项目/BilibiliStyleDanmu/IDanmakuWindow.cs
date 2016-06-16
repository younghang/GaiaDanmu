/*
 * Created by SharpDevelop.
 * User: young
 * Date: 06/08/2016
 * Time: 19:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace BilibiliStyleDanmu
{
	/// <summary>
	/// Description of IDanmakuWindow.
	/// </summary>
	public interface IDanmakuWindow : IDisposable
	{
		void Show();

		void Close();

		void ForceTopmost();

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e);

		void AddDanmaku(DanmakuType type, string comment, uint color);
	}
	public enum DanmakuType
	{
		Scrolling = 1,
		Bottom = 4,
		Top,
		Reserve
	}
}
