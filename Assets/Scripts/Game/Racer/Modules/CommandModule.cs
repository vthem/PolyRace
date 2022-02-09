using System;

using TSW;

namespace Game.Racer.Modules
{
	public class CommandModule : BaseModule
	{
		public Func<float> TurnInputGetter { get; set; }
		public Func<float> AccelerateInputGetter { get; set; }
		public Func<float> BoostInputGetter { get; set; }
		public Func<float> DodgeInputGetter { get; set; }

		public int BoostActivateCount { get; private set; }
		public int BoostDeactivateCount { get; private set; }
		public int BrakeActivationCount { get; private set; }

		private readonly ButtonTrigger _boostTrigger = new ButtonTrigger(.5f);
		private readonly AxeTrigger _brakeTrigger = new AxeTrigger(0f);

		public float Turn
		{
			get
			{
				if (enabled && TurnInputGetter != null)
				{
					return TurnInputGetter();
				}
				return 0f;
			}
		}

		public float Accelerate
		{
			get
			{
				float accelerate = 0f;
				if (enabled && AccelerateInputGetter != null)
				{
					accelerate = AccelerateInputGetter();
				}
				return accelerate;
			}
		}

		public float Boost
		{
			get
			{
				if (enabled && BoostInputGetter != null)
				{
					return BoostInputGetter();
				}
				return 0f;
			}
		}

		public override void ModuleUpdate()
		{
			// boost activation 
			_boostTrigger.NewValue(Boost);
			_brakeTrigger.NewValue(Accelerate);
			if (_boostTrigger.PassUp)
			{
				BoostActivateCount++;
			}
			else if (_boostTrigger.PassDown)
			{
				BoostDeactivateCount++;
			}
			if (_brakeTrigger.PassNegative)
			{
				BrakeActivationCount++;
			}
		}

		public override void Enable()
		{
			base.Enable();
			BoostActivateCount = 0;
			BoostDeactivateCount = 0;
			BrakeActivationCount = 0;
		}
	}
}
