using UnityInput = UnityEngine.Input;

namespace Game.Racer.Command
{
	public class Keyboard : ICommand
	{
		private float _lastBoostValue = 0.0f;
		public virtual float Boost => UnityInput.GetAxisRaw("Boost");
		public virtual float Acceleration => 1f;

		public virtual float Turn => UnityInput.GetAxis("Steering");

		public Keyboard()
		{
		}

		public virtual void Update()
		{
			if (_lastBoostValue != Boost)
			{
				_lastBoostValue = Boost;
			}
		}
	}
}
