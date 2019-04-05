using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Cephalus.Maldives.SleeperKeeperWpf.KeyHandling
{
	public class FormKeyHandler
	{
		private readonly Dictionary<Key, Action> _keyHandlers = new Dictionary<Key, Action>();

		public FormKeyHandler AddHandler(Key key, Action action)
		{
			if (!_keyHandlers.ContainsKey(key))
			{
				_keyHandlers.Add(key, action);
			}

            return this;
		}

		public void HandleDown(KeyEventArgs e)
		{
			_keyHandlers.TryGetValue(e.Key, out var handler);

			handler?.Invoke();
		}

        public void HandleUp(KeyEventArgs e)
        {
            _keyHandlers.TryGetValue(e.Key, out var handler);

            handler?.Invoke();
        }
	}
}
