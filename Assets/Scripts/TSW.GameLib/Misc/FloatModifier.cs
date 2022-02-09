using System;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using UnityEngine;

namespace TSW
{
	public abstract class FloatModifier : MonoBehaviour
	{
		public enum Operand
		{
			Add,
			Multiply
		}

		[SerializeField]
		private float _initialValue = 1f;

		[SerializeField]
		private Modifier[] _modifiers;

		[SerializeField]
		private float _fadeDuration = 1f;

		[Serializable]
		public class Modifier
		{
			public string _commandName;
			public AnimationCurve _curve;
			public bool _enable = true;
			public Operand _operand = Operand.Add;
			public Func<float> GetCommand { get; set; }

			public float GetModifiedValue()
			{
				if (GetCommand != null)
				{
					return _curve.Evaluate(GetCommand());
				}
				return 0f;
			}
		}

		private bool _initialized = false;
		private TweenerCore<float, float, FloatOptions> _startTween = null;

		private void Start()
		{
			OnStart();
			UpdateValue(0f);
			_startTween = DOTween.To(() => GetValue(), (value) => UpdateValue(value), GetTargetValue(), _fadeDuration).OnComplete(() => { _initialized = true; }).SetUpdate(true);
		}

		public void SetGetCommand(Func<string, Func<float>> modifierCommandGetter)
		{
			foreach (Modifier modifier in _modifiers)
			{
				modifier.GetCommand = modifierCommandGetter(modifier._commandName);
			}
		}

		private void Update()
		{
			if (_initialized)
			{
				UpdateValue(GetTargetValue());
			}
		}

		private float GetTargetValue()
		{
			float value = _initialValue;
			foreach (Modifier modifier in _modifiers)
			{
				if (modifier.GetCommand != null && modifier._enable)
				{
					switch (modifier._operand)
					{
						case Operand.Add:
							//                        DebugGraph.Log(GetType().Name + "::" + name + "::add::" + modifier._commandName, modifier.GetModifiedValue());
							value += modifier.GetModifiedValue();
							break;
						case Operand.Multiply:
							//                        DebugGraph.Log(GetType().Name + "::" + name + "::mul::" + modifier._commandName, modifier.GetModifiedValue());
							value *= modifier.GetModifiedValue();
							break;
					}

				}
			}
			//            DebugGraph.Log(GetType().Name + "::" + name + "::result", value);
			return value;
		}

		protected virtual void OnStart() { }
		protected virtual void UpdateValue(float value) { }
		protected virtual float GetValue() { return 0f; }

		private void OnDestroy()
		{
			if (_startTween != null)
			{
				_startTween.Kill();
			}
		}
	}
}
