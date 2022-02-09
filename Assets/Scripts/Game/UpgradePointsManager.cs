using Game.Racer;

using UnityEngine;

namespace Game
{
	public class UpgradePointsManager
	{
		private readonly Player.SerializableUpgradePoints _data;
		public DynamicPropertyPoints DynamicPropertyPoints => _data.points;
		public Player.SerializableUpgradePoints UpgradePointsData => _data;
		public DynamicProperties DynamicProperties { get; private set; }
		public int PointsAvailable => _data.pointsAvailable;
		public int TotalPointsSpent => DPPointsHelper.Sum(_data.points);

		public UpgradePointsManager(Player.SerializableUpgradePoints upgradePointsData, Properties prop)
		{
			if (null == upgradePointsData)
			{
				_data = new Player.SerializableUpgradePoints();
			}
			else
			{
				_data = upgradePointsData;
			}
			DynamicProperties = new DynamicProperties(prop, _data.points);
		}

		public void IncrementAvailablePoints(int value)
		{
			_data.pointsAvailable += value;
		}

		public void IncrementDynamicProperty(string propertyId)
		{
			if (_data.pointsAvailable <= 0)
			{
				Debug.Log("no points available");
				return;
			}
			if (DPPointsHelper.Sum(_data.points) >= DynamicPropertyPoints.max)
			{
				Debug.Log("number of points " + DPPointsHelper.Sum(_data.points) + " > " + DynamicPropertyPoints.max);
				return;
			}
			_data.pointsAvailable -= DPPointsHelper.Increment(_data.points, DPPointsHelper.StringToProperty(propertyId));
		}

		public void DecrementDynamicProperty(string propertyId)
		{
			DynamicProperty prop = DPPointsHelper.StringToProperty(propertyId);
			_data.pointsAvailable += DPPointsHelper.Decrement(_data.points, prop);
		}

		public int GetPoint(string propertyId)
		{
			return DPPointsHelper.Value(_data.points, DPPointsHelper.StringToProperty(propertyId));
		}

		public override string ToString()
		{
			return string.Format("distribution:" + DPPointsHelper.ToString(_data.points) + " available:" + _data.pointsAvailable);
		}
	}
}
