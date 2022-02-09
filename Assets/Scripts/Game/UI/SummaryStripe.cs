using System.Collections.Generic;

using UnityEngine;

namespace Game.UI
{
	public class SummaryStripe : MonoBehaviour
	{
		[SerializeField]
		private GameObject _stripeBackground;

		public virtual bool ShouldBeDisplayed(SummaryDisplayData data)
		{
			return false;
		}

		public virtual IEnumerable<SummaryStripe> GetDisplayInstance(SummaryDisplayData data)
		{
			SummaryStripe stripe = GameObject.Instantiate(gameObject).GetComponent<SummaryStripe>();
			stripe.ConfigureInstance(data);
			yield return stripe;
		}

		protected virtual void ConfigureInstance(SummaryDisplayData data)
		{
		}

		public virtual void Display(int index)
		{
			gameObject.SetActive(true);
			_stripeBackground.SetActive(index % 2 == 0);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}
