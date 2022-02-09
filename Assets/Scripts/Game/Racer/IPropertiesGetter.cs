namespace Game.Racer
{
	public interface IPropertiesGetter
	{
		float ShieldRegenRate { get; }
		float ShieldCapacity { get; }
		float ShieldCost { get; }
		float ShieldDelay { get; }
		float Torque { get; }
		float TurnSpeed { get; }
		float TurnEfficiency { get; }
		float Acceleration { get; }
		float BoostAcceleration { get; }
		float Speed { get; }
		float BoostSpeed { get; }
		float Drag { get; }
		float BoostDrag { get; }
		float Grip { get; }
		float AngularDrag { get; }
		float Brake { get; }
	}
}
