using UnityEngine;

namespace Game.Metrics
{
	public interface IMetricFormatter
	{
		string FormatValue(int value);
		string FormatBestValue(int value);
		string FormatDiffValue(int value, Metric.CompareType type);
	}

	public class DefaultMetricFormatter
	{
		public string FormatValue(int value)
		{
			return value.ToString();
		}

		public string FormatBestValue(int value)
		{
			return value.ToString();
		}

		public string FormatDiffValue(int value, Metric.CompareType type)
		{
			return value.ToString();
		}
	}

	public class TimeMetricFormatter : IMetricFormatter
	{
		public string FormatValue(int value)
		{
			return TSW.Chronometer.FormatTime(ConvertToFloat(value));
		}

		public string FormatBestValue(int value)
		{
			return TSW.Chronometer.FormatTime(ConvertToFloat(value));
		}

		public string FormatDiffValue(int value, Metric.CompareType type)
		{
			if (value < 0)
			{
				return "-" + TSW.Chronometer.FormatTime(Mathf.Abs(ConvertToFloat(value)));
			}
			return "+" + TSW.Chronometer.FormatTime(ConvertToFloat(value));
		}

		private float ConvertToFloat(int value)
		{
			return value / 1000f;
		}
	}

	public class IntegerMetricFormatter : IMetricFormatter
	{
		public string FormatValue(int value)
		{
			return value.ToString();
		}

		public string FormatBestValue(int value)
		{
			return value.ToString();
		}

		public string FormatDiffValue(int value, Metric.CompareType type)
		{
			if (value > 0)
			{
				return "+" + value.ToString();
			}
			return value.ToString();
		}
	}

	public class SpeedMetricFormatter : IMetricFormatter
	{
		private readonly SpeedSystemType _speedSystemType;

		public SpeedMetricFormatter(SpeedSystemType speedSystemType)
		{
			_speedSystemType = speedSystemType;
		}

		public string FormatValue(int value)
		{
			return UI.StringHelper.SpeedString(value, _speedSystemType);
		}

		public string FormatBestValue(int value)
		{
			return UI.StringHelper.SpeedString(value, _speedSystemType);
		}

		public string FormatDiffValue(int value, Metric.CompareType type)
		{
			if (value > 0)
			{
				return "+" + Game.UI.StringHelper.SpeedString(value, _speedSystemType);
			}
			return UI.StringHelper.SpeedString(value, _speedSystemType);
		}
	}
}
