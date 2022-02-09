using System;

using Game.Camera;

using GameInputManager = Game.Input.InputManager;

namespace Game.Input
{
	public static class InputHelper
	{
		private static readonly Action NextCameraType = () => CameraManager.Instance.CameraCycle(1);
		private static readonly Action PreviousCameraType = () => CameraManager.Instance.CameraCycle(-1);
		private static readonly Action NextCameraView = () => CameraManager.Instance.NextView(1);
		private static readonly Action PreviousCameraView = () => CameraManager.Instance.NextView(-1);

		public static void EnableCameraInput()
		{
			GameInputManager.RegisterInputPressed(InputAction.NextCameraType, NextCameraType);
			GameInputManager.RegisterInputPressed(InputAction.PreviousCameraType, PreviousCameraType);
			GameInputManager.RegisterInputPressed(InputAction.NextCameraView, NextCameraView);
			GameInputManager.RegisterInputPressed(InputAction.PreviousCameraView, PreviousCameraView);
		}

		public static void DisableCameraInput()
		{
			GameInputManager.UnregisterInputPressed(InputAction.NextCameraType, NextCameraType);
			GameInputManager.UnregisterInputPressed(InputAction.PreviousCameraType, PreviousCameraType);
			GameInputManager.UnregisterInputPressed(InputAction.NextCameraView, NextCameraView);
			GameInputManager.UnregisterInputPressed(InputAction.PreviousCameraView, PreviousCameraView);
		}
	}
}
