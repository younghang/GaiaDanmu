/*
 * Created by SharpDevelop.
 * User: young
 * Date: 2016/6/8
 * Time: 13:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace OtherDanmuTest
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		DanmakuCurtain dmkCurt;
		public Window1()
		{
			InitializeComponent();
			dmkCurt=new DanmakuCurtain(false);
			dmkCurt.Show();
			 
		}
		void button1_Click(object sender, RoutedEventArgs e)
		{
		  if (dmkCurt != null) {
                Random ran = new Random();
                var text = "我爱大哥";
                for (var i = 0; i < ran.Next(1, 20); i += 1) {
                    text += "0.0";
                }
                ShootDanmaku(text);
//                dmkCurt.Shoot(text);
            } else {
                MessageBox.Show("Cannot find any curtains.");
            }
		}
		 public  void ShootDanmaku(string text) {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                dmkCurt.Shoot(text);
                
            }));
//dmkCurt.Shoot(text);
        }
		void window1_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}