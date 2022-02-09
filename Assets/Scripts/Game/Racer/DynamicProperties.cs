using UnityEngine;

namespace Game.Racer
{
	public enum DynamicProperty
	{
		Steering,
		Boost,
		Engine,
		Battery,
		ShieldBattery,
		BoostBattery,
		Generator,
		EnumMax
	}

	[System.Serializable]
	public class DynamicPropertyPoints
	{
		public int[] values = new int[(int)DynamicProperty.EnumMax];
		public const int max = 50;
	}

	public static class DPPointsHelper
	{
		public static int Sum(DynamicPropertyPoints p)
		{
			int sum = 0;
			for (int i = 0; i < p.values.Length; i++)
			{
				sum += p.values[i];
			}
			return sum;
		}

		public static void Reset(DynamicPropertyPoints p)
		{
			for (int i = 0; i < p.values.Length; i++)
			{
				p.values[i] = 0;
			}
		}

		public static int Value(DynamicPropertyPoints p, DynamicProperty prop)
		{
			return p.values[(int)prop];
		}

		public static int Increment(DynamicPropertyPoints p, DynamicProperty prop)
		{
			p.values[(int)prop]++;
			return 1;
		}

		public static int Decrement(DynamicPropertyPoints p, DynamicProperty prop)
		{
			if (p.values[(int)prop] > 0)
			{
				p.values[(int)prop]--;
				return 1;
			}
			return 0;
		}

		public static DynamicProperty StringToProperty(string name)
		{
			return (Game.Racer.DynamicProperty)System.Enum.Parse(typeof(Game.Racer.DynamicProperty), name);
		}

		public static string ToString(DynamicPropertyPoints p)
		{
			string s = "[";
			for (int i = 0; i < p.values.Length; ++i)
			{
				s += ((DynamicProperty)(i)).ToString() + "=" + p.values[i] + " ";
			}
			s += "]";
			return s;
		}
	}

	public class DynamicProperties : IPropertiesGetter
	{
		public Properties Properties { get; private set; }

		private readonly DynamicPropertyPoints _points;

		private float MaxPoints => DynamicPropertyPoints.max;

		public DynamicProperties(Properties prop, DynamicPropertyPoints points)
		{
			Properties = prop;
			_points = points;
		}

		public static DynamicProperties NewFromRacerId(int racerId)
		{
			Racer.Properties props = Racer.PropertiesCollection.GetAsset().GetProperties(racerId);
			return new Racer.DynamicProperties(props, new Racer.DynamicPropertyPoints());
		}

		public float ShieldRegenRate => Mathf.Lerp(Properties.ShieldRegenRate, Properties.ShieldRegenRate, Value(DynamicProperty.Generator) / MaxPoints);
		public float ShieldCapacity => Mathf.Lerp(Properties.ShieldCapacity, Properties.ShieldCapacity, Value(DynamicProperty.Battery) / MaxPoints);
		public float ShieldCost => Properties.ShieldCost;
		public float ShieldDelay => Properties.ShieldDelay;
		public float ShieldCount => Properties.ShieldCount;
		public float Torque => Mathf.Lerp(Properties.Torque, Properties.Torque, Value(DynamicProperty.Steering) / MaxPoints);
		public float TurnSpeed => Properties.TurnSpeed;
		public float TurnEfficiency => Properties.TurnEfficiency;
		public float Acceleration => Mathf.Lerp(Properties.Acceleration, Properties.Acceleration, Value(DynamicProperty.Engine) / MaxPoints);
		public float BoostAcceleration => Mathf.Lerp(Properties.BoostAcceleration, Properties.BoostAcceleration, Value(DynamicProperty.Engine) / MaxPoints);
		public float Speed => Properties.Speed;
		public float BoostSpeed => Properties.BoostSpeed;
		public float Drag => Properties.Drag;
		public float BoostDrag => Properties.BoostDrag;
		public float Grip => Mathf.Lerp(Properties.Grip, Properties.Grip, Value(DynamicProperty.Engine) / MaxPoints);
		public float AngularDrag => Properties.AngularDrag;
		public float Brake => Properties.Brake;

		private int Value(DynamicProperty prop)
		{
			return DPPointsHelper.Value(_points, prop);
		}

		public Controller Instantiate()
		{
			GameObject obj = GameObject.Instantiate(Properties.Prefab);
			Controller controller = obj.GetComponent<Controller>();
			if (null == controller)
			{
				throw new System.Exception("There is no controller script on the racer");
			}
			return controller;
		}
	}
}
