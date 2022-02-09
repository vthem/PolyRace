using UnityEngine;

namespace Game.UI
{
	public class RotationFuncSpeed : MonoBehaviour
	{
		[SerializeField]
		private HUD _hud;

		[SerializeField]
		private float _minY = 0f;

		[SerializeField]
		private float _maxY = 60f;

		private void Update()
		{
			float y = Mathf.Lerp(_minY, _maxY, _hud.GetNormalizedSpeed());
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
		}
	}
}