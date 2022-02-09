using UnityEngine;

namespace Game.Racer.Modules
{
	public class BaseModule : MonoBehaviour
	{
		private bool IsEnabled => enabled;

		private Controller _controller;
		protected Collider _collider;
		protected Rigidbody _rigidbody;

		protected Controller Controller => _controller;
		protected DynamicProperties DynProperties => _controller.DynProperties;
		protected CommonProperties CommonProperties => _controller.CommonProperties;

		public void Initialize(Controller controller)
		{
			_controller = controller;
			_collider = controller.GetComponent<Collider>();
			_rigidbody = controller.GetComponent<Rigidbody>();
			ModuleInit();
			Disable();
		}

		public virtual void ModuleUpdate()
		{
		}

		protected virtual void ModuleInit()
		{
		}

		public virtual void Enable()
		{
			enabled = true;
		}

		public virtual void Disable()
		{
			enabled = false;
		}

		public void DefaultDisable()
		{
			enabled = false;
		}
	}
}