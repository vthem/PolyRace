using UnityEngine;

namespace Game.Racer
{
	public interface IRacerHelper
	{
		void Ready(GameObject smokePrefab);
		void Go();
		void PassEndLine();
		void CrashTerrain();
		void CrashGround();
		Audio.MixerOption MixerOption { get; }
	}
}
