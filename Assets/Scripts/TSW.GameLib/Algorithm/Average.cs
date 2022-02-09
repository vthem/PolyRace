
namespace TSW.Algorithm
{
	public class AverageFloat
	{
		private readonly float[] _data;
		private float _total;
		private int _idx = 0;

		public AverageFloat(int nSample, float initValue)
		{
			_data = new float[nSample];
			for (int i = 0; i < _data.Length; ++i)
			{
				_data[i] = initValue;
				_total += initValue;
			}
		}

		public void Set(float value)
		{
			_total = 0f;
			for (int i = 0; i < _data.Length; ++i)
			{
				_data[i] = value;
				_total += value;
			}
		}

		public float Feed(float sample)
		{
			_idx = ++_idx % _data.Length;
			_total -= _data[_idx];
			_data[_idx] = sample;
			_total += _data[_idx];
			return _total / _data.Length;
		}

		public float Value()
		{
			return _total / _data.Length;
		}
	}
}

