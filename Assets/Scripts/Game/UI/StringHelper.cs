using UnityEngine;

namespace Game.UI
{
	public static class StringHelper
	{
		/// <summary>
		/// Format a string to display a speed in km/h with leading unit
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="speed">Speed in m/s</param>
		public static string SpeedString(float speed, SpeedSystemType type)
		{
			return Mathf.RoundToInt(SpeedUnit.ConvertTo(type, speed)).ToString("D3") + " " + SpeedUnit.UnitString(type);
		}

		/// <summary>
		/// Format a string to display a speed in km/h with leading unit
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="speed">Speed in m/s2</param>
		public static string Acceleration(float acceleration)
		{
			return Mathf.RoundToInt(acceleration).ToString("D3") + " m/s\u00B2";
		}

		/// <summary>
		/// Format a string to display a speed in km/h
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="speed">Speed in m/s</param>
		public static string SpeedNoUnitString(float speed, SpeedSystemType type)
		{
			return Mathf.RoundToInt(SpeedUnit.ConvertTo(type, speed)).ToString("D3");
		}

		/// <summary>
		/// Format a string to display a rotation speed in deg/s
		/// </summary>
		/// <returns>The string</returns>
		/// <param name="speed">Speed in rad/s</param>
		public static string RotationSpeed(float speed)
		{
			return (Mathf.Rad2Deg * speed).ToString("F1") + " deg/s";
		}

		/// <summary>
		/// Format a string to display a duration in second
		/// </summary>
		/// <param name="delay">Delay in second</param>
		public static string Delay(float delay)
		{
			return delay.ToString("F2") + " s";
		}

		/// <summary>
		/// Format a string to display a number on two digits
		/// </summary>
		/// <param name="value">Value.</param>
		public static string Integer2(float value)
		{
			return Mathf.RoundToInt(value).ToString("D2");
		}

		/// <summary>
		/// Format a string to display a number on two digits
		/// </summary>
		/// <param name="value">Value.</param>
		public static string Force(float value)
		{
			return Mathf.RoundToInt(value).ToString("D2") + " N";
		}
	}
}
