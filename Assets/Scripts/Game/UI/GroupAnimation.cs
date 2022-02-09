using System.Collections;

using DG.Tweening;

using UnityEngine;

namespace Game.UI
{
	public class GroupAnimation
	{
		public static IEnumerator AnimateObjectStateChange(GameObject obj, bool state)
		{
			if (obj.activeSelf != state)
			{
				bool complete = false;
				obj.transform.DOKill();
				if (state)
				{
					obj.SetActive(true);
					obj.transform.eulerAngles = new Vector3(0, 90, 0);
					obj.transform.DOLocalRotate(new Vector3(0, 0, 0), .5f).OnComplete(() =>
					{
						complete = true;
					});
				}
				else
				{
					obj.transform.eulerAngles = Vector3.zero;
					obj.transform.DOLocalRotate(new Vector3(0, 90, 0), .5f).OnComplete(() =>
					{
						complete = true;
						obj.SetActive(false);
					});
				}
				while (!complete)
				{
					yield return null;
				}
			}
		}

	}
}
