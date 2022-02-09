using System;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class GhostModule : BaseModule
	{
		public Renderer[] _renderers;
		public float _fadeDuration = 2f;
		private readonly Coroutine _fadeRoutine;

		public Ghost.Player GhostPlayer { get; set; }
		public Ghost.GhostType GhostType { get; set; }
		public string GhostName { get; set; }
		public Racer.ColorId GhostColor { get; set; }

		public override void Enable()
		{
			if (GhostColor == ColorId.Default && GhostType == Ghost.GhostType.Remote || GhostType == Ghost.GhostType.Self || GhostType == Ghost.GhostType.Dev)
			{
				Material ghostMat = new Material(CommonProperties.GetGhostMaterial(GhostType));
				MapRenderers((r) =>
				{
					Material[] mats = r.sharedMaterials;
					for (int i = 0; i < mats.Length; ++i)
					{
						mats[i] = ghostMat;
					}
					r.sharedMaterials = mats;

				});
			}
			MakeInvisible();
		}

		private void MapRenderers(Action<Renderer> func)
		{
			for (int i = 0; i < _renderers.Length; ++i)
			{
				func(_renderers[i]);
			}
		}

		public void MakeVisible()
		{
			MapRenderers((r) =>
			{
				r.enabled = true;
			});
		}

		public void MakeInvisible()
		{
			MapRenderers((r) =>
			{
				r.enabled = false;
			});
		}
	}
}
