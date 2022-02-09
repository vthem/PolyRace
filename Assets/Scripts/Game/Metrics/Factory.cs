namespace Game.Metrics
{
	public static class Factory
	{
		public static Metric Create(MetricType id)
		{
			Metric m = null;
			switch (id)
			{
				case MetricType.ElapsedTime:
					m = new Metric(MetricType.ElapsedTime, Metric.CompareType.LowerBetter, new BestTimeProgress());
					break;
				case MetricType.HitNumber:
					m = new Metric(MetricType.HitNumber, Metric.CompareType.LowerBetter, new DefaulLowerBetterProgress());
					break;
				case MetricType.AvgSpeed:
					m = new Metric(MetricType.AvgSpeed, Metric.CompareType.HigherBetter, new DefaultHigherBetterProgress());
					break;
				case MetricType.MaxSpeed:
					m = new Metric(MetricType.MaxSpeed, Metric.CompareType.HigherBetter, new DefaultHigherBetterProgress());
					break;
				case MetricType.Distance:
					m = new Metric(MetricType.Distance, Metric.CompareType.HigherBetter, new DefaultHigherBetterProgress());
					break;
			}
			if (null == m)
			{
				throw new System.Exception("The metric [" + id + "] does not exist");
			}
			return m;
		}

		public static IMetricFormatter CreateFormatter(MetricType id, SpeedSystemType speedSystemType)
		{
			IMetricFormatter formatter = null;
			switch (id)
			{
				case MetricType.ElapsedTime:
					formatter = new TimeMetricFormatter();
					break;
				case MetricType.HitNumber:
					formatter = new IntegerMetricFormatter();
					break;
				case MetricType.AvgSpeed:
					formatter = new SpeedMetricFormatter(speedSystemType);
					break;
				case MetricType.MaxSpeed:
					formatter = new SpeedMetricFormatter(speedSystemType);
					break;
				case MetricType.Distance:
					formatter = new IntegerMetricFormatter();
					break;
			}
			return formatter;
		}
	}
}
