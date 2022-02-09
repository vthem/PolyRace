
namespace TSW.Gesture
{
	public interface IGestureHandler
	{
		void Reset();

		void OnTap(Gesture gesture);

		void OnSwipe(Gesture gesture);

		void OnPress(Gesture gesture);
	}
}