using Game.Metrics;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SummaryMetricStripe : SummaryStripe
	{
		[SerializeField]
		private Text _id;

		[SerializeField]
		private Text _value;

		[SerializeField]
		private Text _bestValue;

		[SerializeField]
		private Text _diffValue;

		[SerializeField]
		private Text _record;

		[SerializeField]
		private Image _upDown;

		[SerializeField]
		private Metrics.MetricType _metricType;

		private enum DisplayOption
		{
			TimeAttack,
			Distance,
			Both
		}

		[SerializeField]
		private DisplayOption _displayOption;
		private bool _newRecordSet = false;

		public override bool ShouldBeDisplayed(SummaryDisplayData data)
		{
			bool shouldBeDisplayed = false;
			shouldBeDisplayed = !(data.RaceSetup.RaceType == Race.RaceType.TimeAttack && data.EndType != EndRaceType.RaceFinished);
			if (shouldBeDisplayed)
			{
				if (_displayOption == DisplayOption.TimeAttack)
				{
					shouldBeDisplayed = data.RaceSetup.RaceType == Race.RaceType.TimeAttack;
				}
				else if (_displayOption == DisplayOption.Distance)
				{
					shouldBeDisplayed = data.RaceSetup.RaceType == Race.RaceType.Distance;
				}
			}
			Metric metric = data.RaceMetrics.GetMetric(_metricType);
			if (null == metric)
			{
				shouldBeDisplayed = false;
			}
			return shouldBeDisplayed;
		}

		protected override void ConfigureInstance(SummaryDisplayData data)
		{
			Metric metric = data.RaceMetrics.GetMetric(_metricType);
			IMetricFormatter formatter = Factory.CreateFormatter(_metricType, Player.PlayerManager.Instance.GetSpeedUnit());
			_newRecordSet = false;

			foreach (Transform child in transform.Find("LayoutGroup"))
			{
				if (child.name != "PlaceHolder" && child.name != "Icon_Layout")
				{
					child.gameObject.SetActive(false);
				}
			}
			SetTextAndEnable(_id, GetLocalizedString("Metric" + metric.Id.ToString()) + ":");
			SetTextAndEnable(_value, metric.ValueToString(formatter));
			if (data.RaceMetrics.UpdateId > 1 && metric.Result != Metric.ResultType.Equal)
			{
				Vector3 scale = Vector3.zero;
				switch (metric.Result)
				{
					case Metric.ResultType.Improve:
						scale = new Vector3(1, 1, 1);
						SetTextAndEnable(_record, GetLocalizedString("NewRecord"));
						_newRecordSet = true;
						break;
					case Metric.ResultType.Worse:
						scale = new Vector3(-1, 1, 1);
						SetTextAndEnable(_record, GetLocalizedString("Record") + ":");
						SetTextAndEnable(_bestValue, metric.BestValueToString(formatter));
						break;
				}
				_upDown.transform.localScale = scale;
				_upDown.transform.parent.gameObject.SetActive(true);
				SetTextAndEnable(_diffValue, metric.DiffValueToString(formatter));
			}
		}

		public override void Display(int index)
		{
			base.Display(index);
			Summary.EmphasisByShake(_value.transform);
			if (_newRecordSet)
			{
				Summary.EmphasisByShake(_record.transform);
			}
		}

		private void SetTextAndEnable(Text text, string value)
		{
			text.text = value;
			text.gameObject.SetActive(true);
		}

		private string GetLocalizedString(string locKey)
		{
			string result = TSW.Loca.DirtyLoca.GetTextValue(locKey);
			if (string.IsNullOrEmpty(result))
			{
				Debug.LogWarning("Could not find localization for key:" + locKey + " object:" + name);
				return string.Empty;
			}
			return result;
		}
	}
}
