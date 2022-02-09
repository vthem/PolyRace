using System;
using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class UIElement : MonoBehaviour
	{
		[SerializeField]
		private float _animationTime = .5f;

		[SerializeField]
		private Vector3 _animationOffset;

		[SerializeField]
		private Selectable _targetFocus;

		public Selectable BackTargetFocus { get; set; }

		[SerializeField]
		private AnimationType _animationType = AnimationType.Alpha;

		[SerializeField]
		private AnimationType _reloadAnimationType = AnimationType.Alpha;

		[SerializeField]
		private bool _displayAwake = false;

		[SerializeField]
		private bool _controlFocus = true;

		public static UIElement Active { get; set; }
		public bool DisplayAwake => _displayAwake;

		private RectTransform _rectTransform;
		private CanvasGroup _canvasGroup;

		private enum AnimationType
		{
			Alpha,
			Move,
			Rotate,
			Scale
		}

		private Tweener _animationTweener;

		public bool DisplayState { get; private set; }

		public Vector3 Origin { get; private set; }

		public virtual void Initialize()
		{
			_rectTransform = GetComponent<RectTransform>();
			_canvasGroup = GetComponent<CanvasGroup>();
			// must initialize it if sub class check its value in SetDisplayState
			DisplayState = true;
			Origin = GetComponent<RectTransform>().anchoredPosition3D;
			if (_animationOffset.sqrMagnitude > 0f)
			{
				_animationType = AnimationType.Move;
			}
			else
			{
				_animationType = AnimationType.Alpha;
			}
		}

		public void ResetAnimation()
		{
			_rectTransform.anchoredPosition3D = Origin;
			_canvasGroup.alpha = 1f;
		}

		public void UpdateDisplayState(bool state, bool force = false)
		{
			Log("=> [" + name + "] " + DisplayState + " => " + state);
			if (force || state != DisplayState)
			{
				SetChildState(state);
				DisplayState = state;
			}
		}

		protected virtual IEnumerator OnDisplay()
		{
			yield return null;
		}

		protected virtual IEnumerator OnHide()
		{
			yield return null;
		}

		protected virtual IEnumerator OnDisplayComplete()
		{
			yield return null;
		}

		protected virtual IEnumerator OnHideComplete()
		{
			yield return null;
		}

		public void AnimateDisplayState(bool state, Action onComplete)
		{
			Log("[AD] [" + name + "] " + DisplayState + " => " + state);
			if (state)
			{
				CheckObjectState();
			}
			StartCoroutine(InnerAnimateDisplayState(state, onComplete));
		}

		private IEnumerator InnerAnimateDisplayState(bool state, Action onComplete)
		{
			if (state != DisplayState)
			{
				_canvasGroup.interactable = _canvasGroup.blocksRaycasts = false;
				ClearFocus();
				if (state)
				{
					yield return StartCoroutine(OnBeforeDisplay());
					PrepareAnimationIn(_animationType);
					UpdateDisplayState(true);
					yield return StartCoroutine(OnDisplay());
					yield return StartCoroutine(AnimateIn(_animationType));
					yield return StartCoroutine(OnDisplayComplete());
				}
				else
				{
					yield return StartCoroutine(OnHide());
					yield return StartCoroutine(AnimateOut(_animationType));
					yield return StartCoroutine(OnHideComplete());
					UpdateDisplayState(false);
				}
				_canvasGroup.interactable = _canvasGroup.blocksRaycasts = true;
			}
			if (DisplayState)
			{
				SetFocus();
			}
			if (null != onComplete)
			{
				onComplete();
			}
		}

		public virtual void SetFocus()
		{
			if (BackTargetFocus != null)
			{
				SetFocus(BackTargetFocus);
			}
			else
			{
				SetFocus(_targetFocus);
			}
		}

		protected bool SetFocus(Selectable target)
		{
			if (!_controlFocus)
			{
				Log("[FOCUS] [" + name + "] Does not control focus");
				return false;
			}
			if (target == null)
			{
				Log("[FOCUS] [" + name + "] No focus. Target is null");
				return false;
			}
			if (EventSystem.current.alreadySelecting)
			{
				Log("[FOCUS] [" + name + "] No focus. Focus already set");
				return false;
			}
			if (Input.InputManager.IsMouseActive)
			{
				Log("[FOCUS] [" + name + "] No focus. Mouse is last activity");
				return false;
			}
			if (!target.gameObject.activeInHierarchy)
			{
				Log("[FOCUS] [" + name + "] No focus. Target not active");
				return false;
			}
			Log("[FOCUS] [" + name + "] Focus to " + target.name + " active:" + target.gameObject.activeInHierarchy + " interactable:" + target.gameObject.GetComponent<Selectable>().interactable + " frame:" + Time.frameCount);
			EventSystem.current.SetSelectedGameObject(target.gameObject);
			if (BackTargetFocus == target)
			{
				Log("[FOCUS] [" + name + "] Clear BackTarget");
				BackTargetFocus = null;
			}
			return true;
		}

		private void ClearFocus()
		{
			if (_controlFocus)
			{
				Log("[FOCUS] [" + name + "] Clearing");
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		protected bool IsFocusSet()
		{
			return EventSystem.current.alreadySelecting;
		}

		public IEnumerator AnimateDisplayState(bool state)
		{
			Log("[AD] [" + name + "] " + DisplayState + " => " + state);
			if (state)
			{
				CheckObjectState();
			}
			yield return StartCoroutine(InnerAnimateDisplayState(state, null));
		}

		protected virtual void SetChildState(bool state, bool recursive = false)
		{
			foreach (Transform child in transform)
			{
				UIElement ui = child.gameObject.GetComponent<UIElement>();
				if (ui != null && recursive)
				{
					ui.SetChildState(state);
				}
				else if (ui == null)
				{
					child.gameObject.SetActive(state);
				}
			}
		}

		protected void AnimateReload(Action between, Action onComplete)
		{
			StartCoroutine(InnerAnimateReload(between, onComplete));
		}

		protected IEnumerator InnerAnimateReload(Action between, Action onComplete)
		{
			Log("[AR] [" + name + "] =>");
			ClearFocus();
			_canvasGroup.interactable = _canvasGroup.blocksRaycasts = false;
			yield return StartCoroutine(AnimateOut(_reloadAnimationType));
			between();
			Log("[AR] [" + name + "] <=");
			yield return StartCoroutine(AnimateIn(_reloadAnimationType));
			_canvasGroup.interactable = _canvasGroup.blocksRaycasts = true;
			if (onComplete != null)
			{
				onComplete();
			}
		}

		protected IEnumerator AnimateHide()
		{
			yield return StartCoroutine(AnimateOut(_reloadAnimationType));
		}

		protected IEnumerator AnimateDisplay()
		{
			yield return StartCoroutine(AnimateIn(_reloadAnimationType));
		}

		private void CheckObjectState()
		{
			if (!gameObject.activeInHierarchy)
			{
				DisplayParentUIElement(transform.parent);
			}
		}

		private void DisplayParentUIElement(Transform parent)
		{
			if (null != parent)
			{
				UIElement ui = parent.GetComponent<UIElement>();
				if (null != ui)
				{
					ui.UpdateDisplayState(true);
				}
				DisplayParentUIElement(parent.parent);
			}
		}

		protected virtual IEnumerator OnBeforeDisplay()
		{
			yield return null;
		}

		private void PrepareAnimationIn(AnimationType type)
		{
			switch (type)
			{
				case AnimationType.Alpha:
					_canvasGroup.alpha = 0f;
					break;
				case AnimationType.Move:
					_rectTransform.anchoredPosition3D = Origin + _animationOffset;
					break;
				case AnimationType.Rotate:
					break;
				case AnimationType.Scale:
					break;
			}
		}

		private IEnumerator AnimateIn(AnimationType type)
		{
			switch (type)
			{
				case AnimationType.Alpha:
					yield return StartCoroutine(AnimateInAlpha());
					break;
				case AnimationType.Move:
					yield return StartCoroutine(AnimateInMove());
					break;
				case AnimationType.Rotate:
					break;
				case AnimationType.Scale:
					break;
			}
		}

		private IEnumerator AnimateOut(AnimationType type)
		{
			switch (type)
			{
				case AnimationType.Alpha:
					yield return StartCoroutine(AnimateOutAlpha());
					break;
				case AnimationType.Move:
					yield return StartCoroutine(AnimateOutMove());
					break;
				case AnimationType.Rotate:
					break;
				case AnimationType.Scale:
					break;
			}
		}

		public static IEnumerator WaitForRealSeconds(float delay)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + delay)
			{
				yield return null;
			}
		}

		private IEnumerator AnimateInRotate()
		{
			KillTweener();
			_animationTweener = transform.DOLocalRotate(
				new Vector3(0, 90, 0),
				.2f)
				.SetUpdate(true);
			yield return StartCoroutine(WaitForRealSeconds(_animationTime));
		}

		private IEnumerator AnimateOutRotate()
		{
			KillTweener();
			_animationTweener = transform.DOLocalRotate(new Vector3(0, 0, 0), .2f)
			.SetUpdate(true);
			yield return StartCoroutine(WaitForRealSeconds(_animationTime));
		}

		private IEnumerator AnimateInMove()
		{
			KillTweener();
			_animationTweener = DOTween.To(
				() => _rectTransform.anchoredPosition3D,
				(v) => _rectTransform.anchoredPosition3D = v,
				Origin,
				_animationTime)
				.SetUpdate(true).SetEase(Ease.InOutExpo);
			yield return StartCoroutine(WaitForRealSeconds(_animationTime));
		}

		private IEnumerator AnimateOutMove()
		{
			KillTweener();
			_animationTweener = DOTween.To(
				() => _rectTransform.anchoredPosition3D,
				(v) => _rectTransform.anchoredPosition3D = v,
				_rectTransform.anchoredPosition3D + _animationOffset,
				_animationTime)
				.SetUpdate(true)
				.SetEase(Ease.InOutExpo);
			yield return StartCoroutine(WaitForRealSeconds(_animationTime));
		}

		private IEnumerator AnimateInAlpha()
		{
			KillTweener();
			_canvasGroup.alpha = 0f;
			_animationTweener = DOTween.To(
				() => _canvasGroup.alpha,
				(v) => _canvasGroup.alpha = v,
				1f,
				_animationTime)
				.SetUpdate(true)
				.SetEase(Ease.InOutExpo);
			yield return StartCoroutine(WaitForRealSeconds(_animationTime));
		}

		private IEnumerator AnimateOutAlpha()
		{
			KillTweener();
			_animationTweener = DOTween.To(
				() => _canvasGroup.alpha,
				(v) => _canvasGroup.alpha = v,
				0f,
				_animationTime)
				.SetUpdate(true)
				.SetEase(Ease.InOutExpo);
			yield return StartCoroutine(WaitForRealSeconds(_animationTime));
		}

		private void KillTweener()
		{
			if (_animationTweener != null)
			{
				_animationTweener.Kill();
			}
		}

		public void Hide()
		{
			SetChildState(false);
		}

		public void HideAll()
		{
			SetChildState(false, true);
		}

		public void Display()
		{
			SetChildState(true);
		}

		public void DisplayAll()
		{
			SetChildState(true, true);
		}

		private static void Log(string text)
		{
			//            Debug.Log(text);
		}
	}
}
