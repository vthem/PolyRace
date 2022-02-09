using LibNoise.Unity;

using UnityEngine;

using LibNoisePerlin = LibNoise.Unity.Generator.Perlin;

namespace LevelGen
{
	[System.Serializable]
	public class PerlinData
	{
		[SerializeField]
		private float _period = 10.0f;
		public float Period { get => _period; set => _period = value; }

		[SerializeField]
		private float _lacunarity = 2.0f;
		public float Lacunarity { get => _lacunarity; set => _lacunarity = value; }

		[SerializeField]
		private QualityMode _quality = QualityMode.Medium;
		public QualityMode Quality { get => _quality; set => _quality = value; }

		[SerializeField]
		private int _octaveCount = 6;
		public int OctaveCount { get => _octaveCount; set => _octaveCount = value; }

		[SerializeField]
		private float _persistence = 0.5f;
		public float Persistence { get => _persistence; set => _persistence = value; }

		[SerializeField]
		private int _seed = 0;
		public int Seed { get => _seed; set => _seed = value; }

		private readonly LibNoisePerlin _perlin = new LibNoisePerlin();

		public float GetFloatNormalized(Vector3 point)
		{
			return GetFloatNormalized((int)point.x, (int)point.y, (int)point.z);
		}

		/// <summary>
		/// Return a random float between -1.5 => 1.5
		/// </summary>
		/// <returns>The random value</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		private float GetFloatRaw(int x, int y, int z)
		{
			if (_period == 0.0f)
			{
				_period = 10.0f;
			}
			_perlin.Frequency = 1 / _period;
			_perlin.Lacunarity = _lacunarity;
			_perlin.Quality = _quality;
			_perlin.OctaveCount = _octaveCount;
			_perlin.Persistence = _persistence;
			_perlin.Seed = _seed;
			return (float)_perlin.GetValue(x, y, z); // value [-1.5,1.5] 
		}

		private float GetFloatRaw(Vector3 xyz)
		{
			if (_period == 0.0f)
			{
				_period = 10.0f;
			}
			_perlin.Frequency = 1 / _period;
			_perlin.Lacunarity = _lacunarity;
			_perlin.Quality = _quality;
			_perlin.OctaveCount = _octaveCount;
			_perlin.Persistence = _persistence;
			_perlin.Seed = _seed;
			return (float)_perlin.GetValue(xyz.x, xyz.y, xyz.z); // value [-1.5,1.5] 
		}

		/// <summary>
		/// Return a float between -1 and 1
		/// </summary>
		/// <returns>The random value</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		private float GetFloat(int x, int y, int z)
		{
			return Mathf.Clamp(GetFloatRaw(x, y, z) / 1.5f, -1f, 1f);
		}

		private float GetFloat(Vector3 xyz)
		{
			return Mathf.Clamp(GetFloatRaw(xyz) / 1.5f, -1f, 1f);
		}


		/// <summary>
		/// Return a float between 0 and 1
		/// </summary>
		/// <returns>The random value</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public float GetFloatNormalized(int x, int y, int z)
		{
			return (GetFloat(x, y, z) + 1f) / 2f;
		}

		public override string ToString()
		{
			string s = "PerlinData:";
			s += "\n - period: " + _period;
			s += "\n - lacunarity: " + _lacunarity;
			s += "\n - quality: " + _quality;
			s += "\n - octave: " + _octaveCount;
			s += "\n - seed: " + _seed;
			return s;
		}
	}
}
