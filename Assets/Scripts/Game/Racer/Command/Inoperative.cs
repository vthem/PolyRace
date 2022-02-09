namespace Game.Racer.Command
{
	public class Inoperative : ICommand
	{
		public virtual float Boost => 0f;

		public virtual float Acceleration => 0f;

		public virtual float Turn => 0f;

		public Inoperative()
		{
		}

		public virtual void Update()
		{
		}
	}
}
