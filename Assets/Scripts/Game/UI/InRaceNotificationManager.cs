using TSW;

using UnityEngine;

namespace Game.UI
{
	public class InRaceNotificationManager : MonoBehaviour
	{
		private enum NotificationMask
		{
			BonusDistance,
			ChallengeTimerWarning,
			Default
		}

		public delegate float GetValueDelegate();

		[SerializeField]
		private GameObject _inRaceNoticePrefab;

		[SerializeField]
		private float _noticeDisplayDuration = 3f;

		public void NoticeDistanceTimerWarning()
		{
			if (enabled)
			{
				NewNotice(
					() => TSW.Loca.DirtyLoca.GetTextValue("NotificationLowTimer"),
					_noticeDisplayDuration,
					NotificationMask.ChallengeTimerWarning);
			}
		}

		public void NotifyOpponentName(string displayName)
		{
			if (enabled)
			{
				string notify = TSW.Loca.DirtyLoca.GetTextValue("NotificationRacingAgainst") + "\n" + displayName;
				NewNotice(
					() => notify,
					_noticeDisplayDuration,
					NotificationMask.Default);
			}
		}

		public void ActivateAndClear()
		{
			transform.DestroyAllChild();
			enabled = true;
			Display();
		}

		public void DeactivateAndClear()
		{
			transform.DestroyAllChild();
			enabled = false;
		}

		public void Display()
		{
			GetComponent<CanvasGroup>().alpha = 1f;
		}

		public void Hide()
		{
			GetComponent<CanvasGroup>().alpha = 0f;
		}

		private void NewNotice(InRaceNotification.GetTextDelegate getText, float duration, NotificationMask type)
		{
			Audio.SoundFx.Instance.Play("Notify");
			GameObject obj = GameObject.Instantiate(_inRaceNoticePrefab);
			obj.transform.SetParent(transform, false);
			obj.name = type.ToString();
			InRaceNotification inRaceNotice = obj.GetComponent<InRaceNotification>();
			inRaceNotice.GetText = getText;
			GameObject.Destroy(obj, duration);
		}
	}
}
