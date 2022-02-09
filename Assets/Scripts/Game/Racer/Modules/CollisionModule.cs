using System;

using TSW.Messaging;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;

namespace Game.Racer.Modules
{
	public class CollisionModule : BaseModule
	{
		public class CollisionEvent : Event<bool> { }
		public class TerrainHardCrashEvent : BDEvent { }

		private int _hitNumber;
		public int HitNumber => _hitNumber;
		public bool GhostMode { get; private set; }

		private Action _onCollision;

		private enum CollisionType
		{
			Obstacle,
			Terrain,
			None
		}

		private CollisionType _collision = CollisionType.None;

		public override void Disable()
		{
			base.Disable();
			_collider.enabled = false;
		}

		public override void Enable()
		{
			base.Enable();
			_collider.enabled = true;
		}

		public void EnableGhostMode()
		{
			_collider.isTrigger = true;
			GhostMode = true;
		}

		public override void ModuleUpdate()
		{
			if (_collision != CollisionType.None)
			{
				_hitNumber++;

				if (_onCollision != null)
				{
					_onCollision();
					_onCollision = null;
				}

				if (_collision == CollisionType.Terrain && IsHardCollision())
				{
					Dispatcher.FireEvent(new TerrainHardCrashEvent());
				}
				else
				{
					Dispatcher.FireEvent(new CollisionEvent());
				}

				_collision = CollisionType.None;
			}
		}

		public void RegisterOnCollision(Action action)
		{
			_onCollision = action;
		}

		private void OnCollisionEnter(Collision collision)
		{
			_collision = CollisionType.Terrain;
		}

		public Vector3 GetCollisionPoint()
		{
			RaycastHit hit;
			if (!Physics.Raycast(transform.position, transform.forward, out hit, 100f, CommonProperties.TerrainHeightLayer))
			{
				Debug.LogWarning("No collision point found with terrain");
			}
			return hit.point;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (enabled && other.tag == "Obstacle")
			{
				other.GetComponent<ObstacleExplosion>().Explode(Controller.HullModule._terrainRollPitch.forward, Controller.HullModule._terrainRollPitch.up, Controller.VelocityMeter.Velocity.z, !GhostMode);
				Audio.SoundFx.Instance.Play("ObstacleHit3D", transform, Controller.Helper.MixerOption);
				if (!GhostMode)
				{
					_collision = CollisionType.Obstacle;
				}
			}
		}

		private bool IsHardCollision()
		{
			RaycastHit hit;
			Vector3 pLeft, pRight;
			float distance = 8f;
			int points = 0;
			float maxAngle = 50f;
			if (Physics.Raycast(transform.position, transform.forward, out hit, distance, CommonProperties.TerrainHeightLayer))
			{
				pLeft = pRight = hit.point;
				points++;
				if (Physics.Raycast(transform.position + transform.right, transform.forward, out hit, distance, CommonProperties.TerrainHeightLayer))
				{
					pLeft = hit.point;
					points++;
				}
				if (Physics.Raycast(transform.position - transform.right, transform.forward, out hit, distance, CommonProperties.TerrainHeightLayer))
				{
					pRight = hit.point;
					points++;
				}
				float angle = Vector3.Angle(transform.forward, (pRight - pLeft));
				if (points > 1 && angle > maxAngle && angle < 180f - maxAngle)
				{
					return true;
				}
			}
			return false;
		}
	}
}
