namespace Game.Racer.Command
{
	public class Replay : ICommand
	{
		protected float _turn = 0f;
		protected float _acceleration = 1f;

		public virtual float Boost => 0f;

		public virtual float Acceleration => _acceleration;

		public virtual float Turn => _turn;

		public virtual void Update()
		{
		}
	}
}
