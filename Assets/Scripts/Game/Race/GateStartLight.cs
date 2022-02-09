using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Race
{
	public class GateStartLight : MonoBehaviour
	{
		[SerializeField]
		private Material _go;

		[SerializeField]
		private Material _hold;

		[SerializeField]
		private Material _ready;

		[SerializeField]
		private Material _off;

		[SerializeField]
		private Material _default;

		[SerializeField]
		private Renderer[] _lightGo;

		[SerializeField]
		private Renderer[] _lightHold;

		[SerializeField]
		private Renderer[] _lightReady;

		[SerializeField]
		private float _waitTime = 1f;

		[SerializeField]
		private Text _bannerText;

		public void TestCountDown()
		{
			StartCoroutine(CountDownRoutine(false));
		}

		public static IEnumerator StartCountDown(bool fastRestart)
		{
			GameObject obj = GameObject.FindWithTag("GateStartLight");
			if (null == obj)
			{
				Debug.Log("Could not find object with tag GateStartLight");
			}
			GateStartLight startLight = obj.GetComponent<GateStartLight>();
			yield return startLight.StartCoroutine(startLight.CountDownRoutine(fastRestart));
		}

		private IEnumerator CountDownRoutine(bool fastRestart)
		{
			_bannerText.text = "";
			if (!fastRestart)
			{
				yield return new WaitForSeconds(_waitTime);
				LightAllColor(_off);
				Audio.SoundFx.Instance.Play("CountdownInter3D", transform);
				_bannerText.text = "3";

				yield return new WaitForSeconds(_waitTime);
				LightHold();
				Audio.SoundFx.Instance.Play("CountdownInter3D", transform);
				_bannerText.text = "2";

				yield return new WaitForSeconds(_waitTime);
				LightReady();
				Audio.SoundFx.Instance.Play("CountdownInter3D", transform);
				_bannerText.text = "1";
			}
			else
			{
				LightAllColor(_off);
				yield return new WaitForSeconds(.25f);
				LightHold();
				yield return new WaitForSeconds(.25f);
				LightReady();
			}

			yield return new WaitForSeconds(_waitTime);
			LightGo();
			Audio.SoundFx.Instance.Play("CountdownLast3D", transform);
			_bannerText.text = "GO!!";
		}

		private void LightAllColor(Material material)
		{
			foreach (Renderer r in _lightGo)
			{
				r.material = material;
			}
			foreach (Renderer r in _lightHold)
			{
				r.material = material;
			}
			foreach (Renderer r in _lightReady)
			{
				r.material = material;
			}
		}

		private void LightReady()
		{
			LightAllColor(_off);
			foreach (Renderer r in _lightReady)
			{
				r.material = _ready;
			}
		}

		private void LightHold()
		{
			LightAllColor(_off);
			foreach (Renderer r in _lightHold)
			{
				r.material = _hold;
			}
		}

		private void LightGo()
		{
			LightAllColor(_off);
			foreach (Renderer r in _lightGo)
			{
				r.material = _go;
			}
		}
	}
}
