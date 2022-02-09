using TSW;
using TSW.Messaging;

using Game.Ghost;

using UnityEngine;

namespace Game.Racer
{

	/// <summary>
	/// Mono.
	/// </summary>
	public class Controller : MonoBehaviour
	{
		[SerializeField]
		private CommonProperties _commonProperties;

		[SerializeField]
		private Properties _properties;
		private Modules.BaseModule[] _allModules;
		private Modules.BaseModule[] _fixedUpdateModules;
		private Modules.BaseModule[] _updateModules;
		public IRacerHelper Helper { get; set; }

		public Properties Properties => _properties;
		public CommonProperties CommonProperties => _commonProperties;
		public DynamicProperties DynProperties { get; private set; }

		public Modules.AudioModule AudioModule { get; private set; }
		public Modules.BonusModule BonusModule { get; private set; }
		public Modules.BurnSmokeModule BurnSmokeModule { get; private set; }
		public Modules.CollisionModule CollisionModule { get; private set; }
		public Modules.CommandModule CommandModule { get; private set; }
		public Modules.DataModule DataModule { get; private set; }
		public Modules.DetachModule DetachModule { get; private set; }
		public Modules.EnergyModule EnergyModule { get; private set; }
		public Modules.ExplosionModule ExplosionModule { get; private set; }
		public Modules.GateDetection GateDetection { get; private set; }
		public Modules.GhostModule GhostModule { get; private set; }
		public Modules.ColorModule ColorModule { get; private set; }
		public Modules.SlideModule SlideModule { get; private set; }
		public Modules.GroundSmokeModule GroundSmokeModule { get; private set; }
		public Modules.HoveringModule HoveringModule { get; private set; }
		public Modules.HullModule HullModule { get; private set; }
		public Modules.PhysicDriverModule PhysicDriverModule { get; private set; }
		public VelocityMeter VelocityMeter { get; private set; }

		public static Controller GetActive(string tag)
		{
			GameObject obj = GameObject.FindGameObjectWithTag(tag);
			if (obj == null)
			{
				throw new System.Exception("Could not find object with tag:" + tag);
			}
			Controller controller = obj.GetComponent<Game.Racer.Controller>();
			if (controller == null)
			{
				throw new System.Exception("Object " + obj.name + " does not have Controller");
			}
			return controller;
		}

		public static Controller Instantiate(int racerId)
		{
			return (new DynamicProperties(PropertiesCollection.GetAsset().GetProperties(racerId), new DynamicPropertyPoints())).Instantiate();
		}

		public void RegisterForEvent()
		{
			Dispatcher.AddHandler(this);
			foreach (Modules.BaseModule module in _allModules)
			{
				Dispatcher.AddHandler(module);
			}
		}

		private void Awake()
		{
			AudioModule = GetComponent<Modules.AudioModule>();
			BurnSmokeModule = GetComponent<Modules.BurnSmokeModule>();
			BonusModule = transform.GetComponentInChildren<Modules.BonusModule>();
			CollisionModule = GetComponent<Modules.CollisionModule>();
			CommandModule = GetComponent<Modules.CommandModule>();
			DataModule = GetComponent<Modules.DataModule>();
			DetachModule = GetComponent<Modules.DetachModule>();
			EnergyModule = GetComponent<Modules.EnergyModule>();
			ExplosionModule = GetComponent<Modules.ExplosionModule>();
			GateDetection = GetComponent<Modules.GateDetection>();
			GhostModule = GetComponent<Modules.GhostModule>();
			ColorModule = GetComponent<Modules.ColorModule>();
			SlideModule = GetComponent<Modules.SlideModule>();
			GroundSmokeModule = GetComponent<Modules.GroundSmokeModule>();
			HoveringModule = GetComponent<Modules.HoveringModule>();
			HullModule = GetComponent<Modules.HullModule>();
			PhysicDriverModule = GetComponent<Modules.PhysicDriverModule>();
			VelocityMeter = GetComponent<VelocityMeter>();

			_allModules = new Modules.BaseModule[] { AudioModule, BurnSmokeModule, BonusModule, CollisionModule, CommandModule, DataModule, DetachModule, GateDetection,
				EnergyModule, ExplosionModule, GhostModule, ColorModule, SlideModule, GroundSmokeModule, HoveringModule, HullModule, PhysicDriverModule };
			// order of module updates matters. DataModule compute the velocity, it must be first
			_fixedUpdateModules = new Modules.BaseModule[] { DataModule, AudioModule, CollisionModule, GateDetection,
				EnergyModule, HoveringModule, HullModule, PhysicDriverModule, SlideModule };
			_updateModules = new Modules.BaseModule[] { BonusModule, CommandModule };

			int index = 0;
			foreach (Modules.BaseModule module in _allModules)
			{
				if (null == module)
				{
					Debug.LogWarning("Module index:" + index + " not set");
				}
				else
				{
					module.DefaultDisable();
				}
				index++;
			}
		}

		private void Start()
		{
			// initialize module in start to take the transform position into account; if set after instantiate
			DynProperties = new DynamicProperties(_properties, new Racer.DynamicPropertyPoints());
			foreach (Modules.BaseModule module in _allModules)
			{
				module.Initialize(this);
			}
		}

		private void OnDestroy()
		{
			Dispatcher.RemoveHandler(this);
			foreach (Modules.BaseModule module in _allModules)
			{
				Dispatcher.RemoveHandler(module);
			}
		}

		private void Update()
		{
			foreach (Modules.BaseModule module in _updateModules)
			{
				if (module.enabled)
				{
					module.ModuleUpdate();
				}
			}
		}

		private void FixedUpdate()
		{
			foreach (Modules.BaseModule module in _fixedUpdateModules)
			{
				if (module.enabled)
				{
					module.ModuleUpdate();
				}
			}
		}

		public FrameProperties GetFrameProperties()
		{
			return new Game.Ghost.FrameProperties(
				transform.position.x,
				transform.position.z,
				transform.eulerAngles.y,
				DataModule.NormalizedAngularSpeed
			);
		}

		private bool _first = true;
		public void FramePropertiesUpdate(FrameProperties frameProperties)
		{
			if (_first)
			{
				_first = false;
			}
			Ghost.FrameProperties frame = frameProperties;
			transform.position = new Vector3(frame.PositionX, transform.position.y, frame.PositionZ);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, frame.RotationY, transform.eulerAngles.z);
			DataModule.SetValue(frame.NormalizedAngularSpeedY);
		}

	}
}
