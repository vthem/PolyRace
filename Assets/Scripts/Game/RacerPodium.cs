using TSW;
using TSW.Messaging;

using UnityEngine;

namespace Game
{
	public class RacerPodium : MonoBehaviour
	{
		public float _racerRotateSpeed = 18f;
		public Transform _podiumCamera;

		public Racer.Controller Racer { get; private set; }

		// make it time.scale independent
		private float _lastUpdate;

		private void Start()
		{
			_lastUpdate = Time.realtimeSinceStartup;
		}

		private void Awake()
		{
			Dispatcher.AddHandler<Race.Setup.UpdatedEvent>(OnRacerSelectionUpdate);
		}

		private void Update()
		{
			float delta = Time.realtimeSinceStartup - _lastUpdate;
			_lastUpdate = Time.realtimeSinceStartup;
			if (Racer != null)
			{
				Racer.transform.Rotate(Vector3.up, _racerRotateSpeed * delta);
			}
		}

		public void OnRacerSelectionUpdate(Race.Setup.UpdatedEvent evt)
		{
			Quaternion rotation = Quaternion.identity;
			if (Racer != null)
			{
				if (Racer.Properties.Id == evt.Value1.RacerDynamicProperties.Properties.Id)
				{
					if (evt.Value1.RacerColor != null)
					{
						Racer.ColorModule.SetColor(evt.Value1.RacerColor);
					}
					return;
				}
				if (Racer.Properties.Id != evt.Value1.RacerDynamicProperties.Properties.Id)
				{
					rotation = Racer.transform.rotation;
					Racer.gameObject.Destroy();
				}
			}
			Racer = evt.Value1.RacerDynamicProperties.Instantiate();
			Racer.name = "RacerOnPodium";
			Racer.transform.position = Vector3.zero;
			Racer.transform.rotation = rotation;
			Racer.ColorModule.SetColor(evt.Value1.RacerColor);

			TSW.Camera.MouseOrbit mouseOrbit = _podiumCamera.GetComponent<TSW.Camera.MouseOrbit>();
			mouseOrbit.transform.position = _podiumCamera.position;
			mouseOrbit.transform.rotation = _podiumCamera.rotation;
			mouseOrbit.UpdateTarget(Racer.transform);
		}
	}
}