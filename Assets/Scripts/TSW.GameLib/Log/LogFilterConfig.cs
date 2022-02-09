using System.Collections.Generic;

using UnityEngine;

using BLogger = TSW.Log.Logger;

public class LogFilterConfig : MonoBehaviour
{
	[System.Serializable]
	private struct Filter
	{
		public string _value;
		public bool _enable;
	}

	private readonly List<Filter> _filters;

	private void UpdateFilter()
	{
		BLogger.ClearFilter();
		if (_filters != null)
		{
			foreach (Filter filter in _filters)
			{
				if (filter._enable)
				{
					BLogger.AddFilter(filter._value);
				}
			}
		}
	}

	private void Start()
	{
		UpdateFilter();
	}
}
