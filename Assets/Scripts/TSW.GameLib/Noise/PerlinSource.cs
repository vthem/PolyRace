using LibNoise.Unity;

using UnityEngine;

using LibNoisePerlin = LibNoise.Unity.Generator.Perlin;

namespace TSW.Noise
{
	public class PerlinSource : Source
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

		public override float GetFloat(Vector3 xyz)
		{
			return Mathf.Clamp((GetFloatRaw(xyz) + 1.5f) / 3f, 0f, 1f);
		}

		public override void SetSeed(int seed)
		{
			_seed = seed;
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

		public override bool IsValid()
		{
			return true;
		}
	}
}