using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Cephalus.Maldives.SleeperKeeperWpf.KeyHandling
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

		private List<int> hotKeyIdentifiers = Enumerable.Range(9000, 1000).ToList();
		private readonly HwndSource _source;
		private readonly Window _window;
		private Dictionary<int, Action> _handlers = new Dictionary<int, Action>();
		
		public HotkeyHandler(Window window)
		{
			_window = window;

			var helper = new WindowInteropHelper(window);

			_source = HwndSource.FromHwnd(helper.Handle);
			_source.AddHook(HwndHook);
		}

		public void RegisterKeyHandler(Action handler, uint virtualKey, uint modifierKey)
		{
			var helper = new WindowInteropHelper(_window);
			var identifier = GetNewHotkeyId();

			_handlers.Add(identifier, handler);

			if (!RegisterHotKey(helper.Handle, identifier, modifierKey, virtualKey))
			{

			}
		}

		public void UnregisterHotKey()
		{
			UnregisterHotKeyInternal();
		}

		private void UnregisterHotKeyInternal()
		{
			var helper = new WindowInteropHelper(_window);

            foreach (var handler in _handlers)
            {
                UnregisterHotKey(helper.Handle, handler.Key);
            }
		}

		private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			const int WM_HOTKEY = 0x0312;

			switch (msg)
			{
				case WM_HOTKEY:
					var param = wParam.ToInt32();
					var handler = _handlers[param];

					handler?.Invoke();
					break;
			}

			return IntPtr.Zero;
		}

		private int GetNewHotkeyId()
		{
			var identifier = hotKeyIdentifiers.First();

			hotKeyIdentifiers.RemoveAt(0);

			return identifier;
		}
	}
}
