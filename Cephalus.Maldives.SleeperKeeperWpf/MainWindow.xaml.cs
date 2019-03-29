using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using WindowsInput;

namespace Cephalus.Maldives.SleeperKeeperWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//[DllImport("user32.dll")]
		//static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

		//[DllImport("user32.dll")]
		//static extern bool SendInput(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

		//const uint WM_KEYDOWN = 0x0100;
		//const int VK_F15 = 0x7e;

		private readonly DispatcherTimer _timer;
		private int _counter;
		private TimeSpan _activeTime;
		private InputSimulator _simulator;

		public MainWindow()
		{
			InitializeComponent();

			_timer = new DispatcherTimer();
			_timer.Interval = new TimeSpan(0, 0, 0, 1);
			_timer.Tick += _timer_Tick;
			_timer.Start();

			_activeTime = new TimeSpan();
		}

		private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			//var processes = Process.GetProcessesByName("explorer");
			//var success = false;

			//foreach (var process in processes)
			//{
			//	success = PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_F15, 0);
			//}

			_activeTime = _activeTime.Add(TimeSpan.FromSeconds(1));
			lblContent.Text = $"Sent F15 key to prevent sleep {_counter++} times over an interval of {_timer.Interval.Seconds} seconds. System active for {_activeTime.TotalSeconds} seconds with {success}";
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}
	}
}
