using System.Collections;

using UnityEngine;

namespace TSW
{
	public class CoroutineCentral : TSW.Design.USingleton<CoroutineCentral>
	{
		public delegate IEnumerator CoroutineMethod();

		public Coroutine StartACoroutine(CoroutineMethod coroutineMethod)
		{
			return StartCoroutine(coroutineMethod());
		}
	}
}
