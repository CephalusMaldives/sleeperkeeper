using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Cephalus.Maldives.SleeperKeeperWpf.Hotkeys
{
	public class HotkeyHandler
	{
		[DllImport("User32.dll")]
		private static extern bool RegisterHotKey(
			[In] IntPtr hWnd,
			[In] int id,
			[In] uint fsModifiers,
			[In] uint vk);

		[DllImport("User32.dll")]
		private static extern bool UnregisterHotKey(
			[In] IntPtr hWnd,
			[In] int id);

		private const int HOTKEY_ID = 9000;

		private readonly HwndSource _source;
		private readonly Window _window;
		private Action _handler;

		public HotkeyHandler(Window window)
		{
			_window = window;

			var helper = new WindowInteropHelper(window);

			_source = HwndSource.FromHwnd(helper.Handle);
			_source.AddHook(HwndHook);

			RegisterHotKeyInternal();
		}

		public void RegisterKeyHandler(Action handler)
		{
			_handler = handler;
		}

		public void UnregisterHotKey()
		{
			UnregisterHotKeyInternal();
		}

		private void UnregisterHotKeyInternal()
		{
			var helper = new WindowInteropHelper(_window);

			UnregisterHotKey(helper.Handle, HOTKEY_ID);
		}

		private void RegisterHotKeyInternal()
		{
			var helper = new WindowInteropHelper(_window);

			const uint VK_F10 = 0x79;
			const uint MOD_CTRL = 0x0002;

			if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_F10))
			{
				// handle error
			}
		}

		private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			const int WM_HOTKEY = 0x0312;

			switch (msg)
			{
				case WM_HOTKEY:
					switch (wParam.ToInt32())
					{
						case HOTKEY_ID:
							if (_handler != null)
							{
								_handler();
							}

							handled = true;
							break;
					}
					break;
			}

			return IntPtr.Zero;
		}
	}
}
