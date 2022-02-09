using TSW;

using UnityEngine;

namespace Game.Racer.Command
{
	public class KeyboardFull : Keyboard
	{
		private readonly KeyboardInputListener _accInputListener;

		public override float Acceleration => _accInputListener.GetValue();

		public KeyboardFull()
		{
			_accInputListener = new KeyboardInputListener(0f);
			_accInputListener.AddEntry(KeyCode.UpArrow, 1f);
			_accInputListener.AddEntry(KeyCode.DownArrow, -1f);
		}

		public override void Update()
		{
			base.Update();
			_accInputListener.Update();
		}
	}
}
