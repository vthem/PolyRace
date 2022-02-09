using System;
using System.Collections;

using TSW;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


namespace Game.UI
{
	public class Summary : UIElement, INavigationOption
	{
		[SerializeField]
		private Transform _editorStripes;

		[SerializeField]
		private Transform _runTimeStripes;

		[SerializeField]
		private float _interStripeDelay = .4f;
		private bool _animated = false;
		private bool _challengeExpired = false;
		private bool _raceFinished = false;
		private float _height;

		public void UpdateSummaryDisplayData(SummaryDisplayData data)
		{
			_animated = false;
			_runTimeStripes.DestroyAllChild();
			_height = 120f;
			foreach (Transform child in _editorStripes)
			{
				SummaryStripe stripe = child.GetComponent<SummaryStripe>();
				if (stripe == null)
				{
					continue;
				}
				if (stripe.ShouldBeDisplayed(data))
				{
					foreach (SummaryStripe instance in stripe.GetDisplayInstance(data))
					{
						instance.transform.SetParent(_runTimeStripes);
						instance.transform.localScale = Vector3.one;
						instance.Hide();
						_height += stripe.GetComponent<LayoutElement>().minHeight;
					}
				}
			}
			if (_height > 120f)
			{
				_height += 50f;
			}
			ChallengeDisplayData challengeData = data as ChallengeDisplayData;
			if (challengeData != null)
			{
				_challengeExpired = challengeData.ChallengeExpired;
			}
			else
			{
				_challengeExpired = false;
			}

			ROTDDisplayData rotdData = data as ROTDDisplayData;
			if (rotdData != null)
			{
				_raceFinished = rotdData.RaceFinished;
			}
			else
			{
				_raceFinished = false;
			}
		}

		protected override IEnumerator OnDisplay()
		{
			_editorStripes.gameObject.SetActive(false);
			_runTimeStripes.gameObject.SetActive(true);
			RectTransform r = transform as RectTransform;
			r.sizeDelta = new Vector2(r.sizeDelta.x, _height);
			yield return null;
		}

		protected override IEnumerator OnDisplayComplete()
		{
			if (!_animated)
			{
				_animated = true;
				StartCoroutine("AnimateStripes");
			}
			yield return null;
		}

		protected override IEnumerator OnHide()
		{
			StopCoroutine("AnimateStripes");
			yield return null;
		}

		private IEnumerator AnimateStripes()
		{
			int index = 0;
			foreach (Transform child in _runTimeStripes)
			{
				SummaryStripe stripe = child.GetComponent<SummaryStripe>();
				if (stripe == null)
				{
					continue;
				}
				Audio.SoundFx.Instance.Play("UIEndRaceStripe", transform);
				stripe.Display(index++);
				yield return new WaitForSeconds(_interStripeDelay);
			}
		}

		public static void EmphasisByScale(Transform t, Action onComplete = null)
		{
			t.DOScale(1.3f, .1f).OnComplete(() =>
			{
				t.DOScale(1f, .4f).OnComplete(() =>
				{
					if (null != onComplete)
					{
						onComplete();
					}
				});
			});
		}

		public static void EmphasisByShake(Transform t, Action onComplete = null)
		{
			t.DOShakeScale(1f, new Vector2(1f, 1f)).OnComplete(() =>
			{
				if (null != onComplete)
				{
					onComplete();
				}
			});
		}

		public string[] GetActiveButton()
		{
			if (_challengeExpired || _raceFinished)
			{
				return new string[] { "Options_Button", "MainMenu_Button", "Replay_Button" };
			}
			if (Config.GetAsset().QuickSaveMission)
			{
				return new string[] { "Restart_Button", "Options_Button", "MainMenu_Button", "Replay_Button", "SaveMission_Button" };
			}
			else
			{
				return new string[] { "Restart_Button", "Options_Button", "MainMenu_Button", "Replay_Button" };
			}
		}

		public string GetFocusButton()
		{
			return "Restart_Button";
		}
	}
}
