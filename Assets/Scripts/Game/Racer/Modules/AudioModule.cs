using TSW.Messaging;

namespace Game.Racer.Modules
{
	public class AudioModule : BaseModule
	{
		private Audio.Loop _engineLoop;
		private Audio.Loop _windLoop;
		private Audio.Loop _lowCapacityWarningAudioEffect;
		private Audio.Loop _boostLoop;

		[EventHandler(typeof(EnergyController.LowEnergyInEvent))]
		public void OnLowEnergyIn(EnergyController.LowEnergyInEvent evt)
		{
			_lowCapacityWarningAudioEffect = new Audio.Loop("LowEnergyLoop", transform, Controller.Helper.MixerOption);
		}

		[EventHandler(typeof(EnergyController.LowEnergyOutEvent))]
		public void OnLowEnergyOut(EnergyController.LowEnergyOutEvent evt)
		{
			EndLoop(_lowCapacityWarningAudioEffect);
		}

		[EventHandler(typeof(EnergyController.BoostActivatedEvent))]
		public void OnBoostActivated(EnergyController.BoostActivatedEvent evt)
		{
			Audio.SoundFx.Instance.Play("BoostActivated3D", transform, Controller.Helper.MixerOption);
			_boostLoop = new Audio.Loop("BoostLoop3D", transform, Controller.Helper.MixerOption, Controller.DataModule.GetAudioSourceParameterValue);
		}

		[EventHandler(typeof(EnergyController.BoostDeactivatedEvent))]
		public void OnBoostDeactivated(EnergyController.BoostDeactivatedEvent evt)
		{
			Audio.SoundFx.Instance.Play("BoostDeactivated3D", transform, Controller.Helper.MixerOption);
			EndLoop(_boostLoop);
		}

		[EventHandler(typeof(EnergyController.ShieldActivatedEvent))]
		public void OnBoostDeactivated(EnergyController.ShieldActivatedEvent evt)
		{
			Audio.SoundFx.Instance.Play("ShieldActivated3D", transform, Controller.Helper.MixerOption);
		}

		protected override void ModuleInit()
		{
		}

		public override void Enable()
		{
			base.Enable();
			Audio.SoundFx.Instance.Play("EngineIgnition3D", Controller.transform, Controller.Helper.MixerOption);
			_engineLoop = new Audio.Loop("EngineLoop3D", transform, Controller.Helper.MixerOption, Controller.DataModule.GetAudioSourceParameterValue);
			_windLoop = new Audio.Loop("WindLoop3D", transform, Controller.Helper.MixerOption, Controller.DataModule.GetAudioSourceParameterValue);
		}

		public override void Disable()
		{
			base.Disable();
			EndLoop(_engineLoop);
			EndLoop(_windLoop);
			EndLoop(_boostLoop);
			EndLoop(_lowCapacityWarningAudioEffect);
		}

		private static void EndLoop(Audio.Loop loop)
		{
			if (loop != null)
			{
				loop.End();
			}
		}
	}
}
