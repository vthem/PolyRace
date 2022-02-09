using System;
using System.IO;

using TSW.Unity;

using UnityEngine;

namespace Game.Ghost
{

	public class FrameProperties
	{
		private float _positionX;
		private float _positionZ;
		private float _rotationY;
		private float _normalizedAngularSpeedY;

		// this is not serialized; metadata
		public Vector3 Acceleration { get; set; }
		public Vector3 Speed { get; set; }
		public float DeltaTime { get; set; }

		public float PositionX => _positionX;
		public float PositionZ => _positionZ;
		public float RotationY => _rotationY;
		public float NormalizedAngularSpeedY => _normalizedAngularSpeedY;

		public FrameProperties(float positionX,
							   float positionZ,
							   float rotationY,
							   float normalizedAngularSpeedY)
		{
			_positionX = positionX;
			_positionZ = positionZ;
			_rotationY = rotationY;
			_normalizedAngularSpeedY = normalizedAngularSpeedY;
		}

		public FrameProperties()
		{
		}

		public Game.Ghost.FrameProperties Lerp(Game.Ghost.FrameProperties toFrame, float time)
		{
			//return LerpByAcceleration(to, time);
			//			return LerpByAcceleration(to, time);
			//			(LerpByAcceleration(to, time) as FrameProperties).__DebugDrawLine("Acceleration", Color.red);
			FrameProperties to = toFrame;
			FrameProperties fp = new FrameProperties(Mathf.Lerp(_positionX, to.PositionX, time),
									   Mathf.Lerp(_positionZ, to.PositionZ, time),
									   Mathf.LerpAngle(_rotationY, to.RotationY, time),
									   Mathf.Lerp(_normalizedAngularSpeedY, to.NormalizedAngularSpeedY, time));
			//			fp.__DebugDrawLine("Linear", Color.green);
			return fp;
		}

		public Game.Ghost.FrameProperties LerpByAcceleration(Game.Ghost.FrameProperties toFrame, float percent)
		{
			FrameProperties to = toFrame;
			float delta = percent * DeltaTime;
			float x = Mathf.Lerp(Speed.x, to.Speed.x, percent) * delta + PositionX;
			float z = Mathf.Lerp(Speed.z, to.Speed.z, percent) * delta + PositionZ;
			return new FrameProperties(x,
									   z,
									   Mathf.LerpAngle(_rotationY, to.RotationY, percent),
									   Mathf.Lerp(_normalizedAngularSpeedY, to.NormalizedAngularSpeedY, percent));
		}

		public byte[] Serialize()
		{
			MemoryStream mem = new MemoryStream();
			byte[] data = BitConverter.GetBytes(_positionX);
			mem.Write(data, 0, data.Length);
			data = BitConverter.GetBytes(_positionZ);
			mem.Write(data, 0, data.Length);
			data = BitConverter.GetBytes(_rotationY);
			mem.Write(data, 0, data.Length);
			data = BitConverter.GetBytes(_normalizedAngularSpeedY);
			mem.Write(data, 0, data.Length);
			return mem.ToArray();
		}

		public void Deserialize(byte[] data, int pos)
		{
			_positionX = BitConverter.ToSingle(data, pos); pos += sizeof(float);
			_positionZ = BitConverter.ToSingle(data, pos); pos += sizeof(float);
			_rotationY = BitConverter.ToSingle(data, pos); pos += sizeof(float);
			_normalizedAngularSpeedY = BitConverter.ToSingle(data, pos);
		}

		public void PreviousFrameProperties(Game.Ghost.FrameProperties prevFrame, float deltaTime)
		{
			FrameProperties prev = prevFrame;
			Vector3 currentPos = new Vector3(_positionX, 0, _positionZ);
			Vector3 prevPos = new Vector3(prev._positionX, 0, prev._positionZ);
			Speed = (currentPos - prevPos) / deltaTime;
			Acceleration = (Speed - prev.Speed) / deltaTime;
			DeltaTime = deltaTime;
		}

		public override string ToString()
		{
			return string.Format("[FrameProperties: Acceleration={0}, Speed={1}, PositionX={2}, PositionZ={3}, RotationY={4}, NormalizedAngularSpeedY={5}]", Acceleration, Speed, PositionX, PositionZ, RotationY, NormalizedAngularSpeedY);
		}

		public void __DebugDrawLine(string tag, Color color)
		{
			Vector3 position = new Vector3(_positionX, 0, _positionZ);
			DrawLine.Add(position, position + Vector3.up * 100f, tag, color);
		}
	}
}
