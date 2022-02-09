namespace Game.Camera
{
	public class TopHandle : RacerHandle
	{
		private TSW.Follow _follow;
		private RacerLookAt _lookAt;

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			_follow = GetComponent<TSW.Follow>();
			_follow.Target = Controller.transform;
			_follow.UpdatePosition();

			_lookAt = GetComponent<RacerLookAt>();
			_lookAt.SetRacerController(Controller);
		}
	}
}
