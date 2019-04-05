using Cephalus.Maldives.SleeperKeeperWpf.KeyHandling;
using System;
using System.ComponentModel;
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
		private const int DefaultIntervalInMilliseconds = 1000;

		private int _counter;
		private DispatcherTimer _timer;
		private TimeSpan _activeTime;
		private InputSimulator _simulator;
		private static HotkeyHandler _hotkeyHandler;
		private NotifyIcon _notifyIcon;
		private readonly FormKeyHandler _keyHandler;

		public MainWindow()
		{
			InitializeComponent();

			_simulator = new InputSimulator();
			_keyHandler = new FormKeyHandler();
			_keyHandler.AddHandler(System.Windows.Input.Key.Escape, () => {
				if (WindowState == WindowState.Maximized)
				{
					WindowStyle = WindowStyle.ThreeDBorderWindow;
					WindowState = WindowState.Normal;
				}
                else if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Minimized;
                    Hide();
                }
			});

			SetupNotifyIcon();
			SetupTimer();

			_activeTime = new TimeSpan();
		}

		private void SetupNotifyIcon()
		{
            var path = AppDomain.CurrentDomain.BaseDirectory;

            _notifyIcon = new NotifyIcon
			{
				Visible = true,
				//Icon = new System.Drawing.Icon(@"C:\Users\jgribic\source\repos\Cephalus.Maldives.SleeperKeeper\Cephalus.Maldives.SleeperKeeperWpf\Resources\icon-active.ico"),
				Text = Title
			};
			_notifyIcon.DoubleClick += (s, e) => VisibilityHandler();
			_notifyIcon.ShowBalloonTip(2000, "Sleeper Keeper", "Hi I am actively keeping your maching awake", ToolTipIcon.Info);
		}

		private void SetupTimer()
		{
			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 1)
			};
			_timer.Tick += _timer_Tick;
			_timer.Start();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (_hotkeyHandler == null)
			{
				_hotkeyHandler = new HotkeyHandler(this);
				_hotkeyHandler.RegisterKeyHandler(() => VisibilityHandler(), 0x79, 0x0002);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			_hotkeyHandler.UnregisterHotKey();
			_notifyIcon.Dispose();
		}

		private void VisibilityHandler()
		{
			if (WindowState == WindowState.Normal)
			{
				Show();
				WindowState = WindowState.Maximized;
				WindowStyle = WindowStyle.None;
				Topmost = true;

				Activate();
			}
			else
			{
				WindowState = WindowState.Normal;

				Topmost = false;
				Hide();
			}
		}

		private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			try
			{
				_simulator.Mouse.MoveMouseBy(1, 1).MoveMouseBy(-1, -1);
				_activeTime = _activeTime.Add(TimeSpan.FromMilliseconds(_timer.Interval.TotalMilliseconds));
				lblContent.Text = $"Preventing sleep {_counter++} times over an interval of {_timer.Interval.Seconds} seconds.\n\rSystem is active for {_activeTime.TotalSeconds} seconds.";
			}
			catch
			{

			}
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void TxtInterval_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				if (!int.TryParse(txtInterval.Text, out int interval))
				{
					interval = DefaultIntervalInMilliseconds;
					txtInterval.Text = interval.ToString();
				}

				e.Handled = true;
				_timer.Interval = new TimeSpan(0, 0, 0, 0, interval);
			}
		}

		private void TxtInterval_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				e.Handled = true;
			}
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			_keyHandler.Handle(e);
		}
	}
}
