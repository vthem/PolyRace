namespace Game.Racer
{
	public interface ICommand
	{
		float Acceleration { get; }
		float Turn { get; }
		float Boost { get; }
		void Update();
	}
}

