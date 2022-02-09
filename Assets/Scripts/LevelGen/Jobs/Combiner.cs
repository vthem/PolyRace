using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class Combiner : Job
	{
		private readonly Transform _staticContainer;

		public Combiner(LevelProfile level, Transform staticContainer) : base(level)
		{
			Weight = 50f;
			_runType = RunType.RunInCoroutine;
			_staticContainer = staticContainer;
		}

		protected override IEnumerator RunByStep()
		{
			_totalStep = 0;
			List<GameObject> objList = new List<GameObject>();
			foreach (Transform t in _staticContainer)
			{
				CombineFlag c = t.GetComponent<CombineFlag>();
				if (c == null || !c.Combined)
				{
					_totalStep++;
					objList.Add(t.gameObject);
				}
			}
			yield return null;
			for (int i = 0; i < objList.Count; ++i)
			{
				StaticBatchingUtility.Combine(objList[i]);
				objList[i].AddComponent<CombineFlag>().Combined = true;
				yield return null;
			}
		}
	}
}
