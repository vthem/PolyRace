using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class Version : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Text>().text = "v" + BuilderConfig.Instance().VersionString;
		}
	}
}
