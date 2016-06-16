/*
 * Created by SharpDevelop.
 * User: young
 * Date: 06/08/2016
 * Time: 19:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Animation;


namespace BilibiliStyleDanmu
{
	 public partial class DanmakuTextControl : UserControl, IComponentConnector
	{
 

		public DanmakuTextControl()
		{
			this.InitializeComponent();
			base.Loaded += new RoutedEventHandler(this.DanmakuTextControl_Loaded);
			Storyboard expr_33 = (Storyboard)base.Resources["Storyboard1"];
			Storyboard.SetTarget(expr_33.Children[2], this);
//			(expr_33.Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = KeyTime.FromTimeSpan(new TimeSpan(Convert.ToInt64(Store.MainOverlayEffect1 * 10000000.0)));
//			(expr_33.Children[1] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = KeyTime.FromTimeSpan(new TimeSpan(Convert.ToInt64(Store.MainOverlayEffect1 * 10000000.0)));
//			(expr_33.Children[1] as DoubleAnimationUsingKeyFrames).KeyFrames[2].KeyTime = KeyTime.FromTimeSpan(new TimeSpan(Convert.ToInt64((Store.MainOverlayEffect2 + Store.MainOverlayEffect1) * 10000000.0)));
//			(expr_33.Children[2] as DoubleAnimationUsingKeyFrames).KeyFrames[0].KeyTime = KeyTime.FromTimeSpan(new TimeSpan(Convert.ToInt64((Store.MainOverlayEffect3 + Store.MainOverlayEffect2 + Store.MainOverlayEffect1) * 10000000.0)));
//			(expr_33.Children[2] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = KeyTime.FromTimeSpan(new TimeSpan(Convert.ToInt64((Store.MainOverlayEffect4 + Store.MainOverlayEffect3 + Store.MainOverlayEffect2 + Store.MainOverlayEffect1) * 10000000.0)));
		}

		public void ChangeHeight()
		{
			this.TextBox.FontSize = Store.MainOverlayFontsize;
			this.TextBox.Measure(new Size(Store.MainOverlayWidth, 2147483647.0));
			(((Storyboard)base.Resources["Storyboard1"]).Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = this.TextBox.DesiredSize.Height;
		}

		private void DanmakuTextControl_Loaded(object sender, RoutedEventArgs e)
		{
			base.Loaded -= new RoutedEventHandler(this.DanmakuTextControl_Loaded);
		}

	 

	 
	}
}
