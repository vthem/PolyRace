namespace Game.UI
{
	public static class RacerPropertyHelper
	{
		public static string GetPropertyDisplayValue(string identifier, Game.Racer.DynamicProperties prop)
		{
			switch (identifier)
			{
				case "Speed": return StringHelper.SpeedString(prop.BoostSpeed, Player.PlayerManager.Instance.GetSpeedUnit());
				case "Acceleration": return StringHelper.Acceleration(prop.BoostAcceleration + prop.Acceleration);
				case "TurnSpeed": return StringHelper.RotationSpeed(prop.TurnSpeed);
				case "Trajectory": return StringHelper.Force(prop.Grip * 23); // 23 => make the value 'realistic' against brake and acceleration
				case "ShieldDelay": return StringHelper.Delay(prop.ShieldDelay);
				case "Brake": return StringHelper.Force(prop.Brake);
			}
			return "unknown-GetPropertyDisplayValue";
		}
	}
}
