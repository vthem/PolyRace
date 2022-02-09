using TSW;

using UnityEngine;

public class GateBannerStabilizer : MonoBehaviour
{
	[SerializeField]
	private Transform _leftAnchorPoint;

	[SerializeField]
	private Transform _rightAnchorPoint;
	private Vector3 _offset;

	private void Start()
	{
		RectTransform rectTransform = transform as RectTransform;
		_offset = rectTransform.anchoredPosition3D - (_leftAnchorPoint.position + _rightAnchorPoint.position) / 2f;
	}

	// Update is called once per frame
	private void Update()
	{
		RectTransform rectTransform = transform as RectTransform;
		rectTransform.anchoredPosition3D = (_leftAnchorPoint.position + _rightAnchorPoint.position) / 2f + _offset;
		transform.rotation = Quaternion.LookRotation(transform.forward, transform.forward.Up((_rightAnchorPoint.position - _leftAnchorPoint.position)));
	}
}
