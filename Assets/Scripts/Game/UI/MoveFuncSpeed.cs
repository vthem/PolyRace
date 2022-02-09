using UnityEngine;

namespace Game.UI
{
	public class MoveFuncSpeed : MonoBehaviour
	{
		[SerializeField]
		private HUD _hud;

		[SerializeField]
		private float _minZ = 0f;

		[SerializeField]
		private float _maxZ = 60f;

		private void Update()
		{
			if (_hud.GetNormalizedSpeed != null)
			{
				float z = Mathf.Lerp(_minZ, _maxZ, _hud.GetNormalizedSpeed());
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
			}
		}
	}
}