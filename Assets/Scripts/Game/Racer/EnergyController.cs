using TSW;
using TSW.Messaging;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;

namespace Game.Racer
{
	public class EnergyController
	{
		public class BoostActivatedEvent : BDEvent { }
		public class BoostDeactivatedEvent : BDEvent { }
		public class BatteryEmptyEvent : BDEvent { }
		public class LowEnergyInEvent : BDEvent { }
		public class LowEnergyOutEvent : BDEvent { }
		public class ShieldActivatedEvent : BDEvent { }

		private readonly DynamicProperties _dynProperties;
		private float _currentCapacity;
		public float CurrentCapacity => _currentCapacity;
		public float CurrentCapacityNormalized => _currentCapacity / _dynProperties.ShieldCapacity;
		public bool BoostState { get; set; }

		private readonly ButtonTrigger _boostTrigger = new ButtonTrigger(.5f);
		private bool _lowCapacity = false;

		public EnergyController(DynamicProperties properties)
		{
			_dynProperties = properties;
			_currentCapacity = _dynProperties.ShieldCapacity;
			BoostState = false;
		}

		public void UpdateCapacity(float steering, float boost)
		{
			// produce
			_currentCapacity += _dynProperties.ShieldRegenRate * Time.deltaTime;
			_currentCapacity = Mathf.Min(_currentCapacity, _dynProperties.ShieldCapacity);

			// boost activation 
			_boostTrigger.NewValue(boost);
			if (_boostTrigger.PassUp)
			{
				BoostState = true;
				Dispatcher.FireEvent(new BoostActivatedEvent());
			}
			else if (_boostTrigger.PassDown)
			{
				BoostState = false;
				Dispatcher.FireEvent(new BoostDeactivatedEvent());
			}

			// capacity monitoring
			if (_currentCapacity < LowCapacityThreshold && !_lowCapacity)
			{
				_lowCapacity = true;
				Dispatcher.FireEvent(new LowEnergyInEvent());
			}
			if (_currentCapacity > HighCapacityThreshold && _lowCapacity)
			{
				_lowCapacity = false;
				Dispatcher.FireEvent(new LowEnergyOutEvent());
			}
		}

		private float LowCapacityThreshold => .35f * _dynProperties.ShieldCapacity;

		private float HighCapacityThreshold => .6f * _dynProperties.ShieldCapacity;

		public void TriggerShield()
		{
			float multiplier = BoostState ? 2f : 1f;
			if (!ConsumeIsEmpty(_dynProperties.ShieldCost * multiplier))
			{
				Dispatcher.FireEvent(new ShieldActivatedEvent());
			}
		}

		private bool ConsumeIsEmpty(float value)
		{
			_currentCapacity -= value;
			if (_currentCapacity < 0f)
			{
				_currentCapacity = 0f;
				Dispatcher.FireEvent(new BatteryEmptyEvent());
				return true;
			}
			return false;
		}
	}
}
