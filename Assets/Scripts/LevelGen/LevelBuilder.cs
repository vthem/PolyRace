using System.Collections;

using TSW;

using UnityEngine;

using BLogger = TSW.Log.Logger;

namespace LevelGen
{
	public class LevelBuilder : TSW.Design.USingleton<LevelBuilder>
	{
		public enum UseType
		{
			Seed,
			Profile,
			Meta
		}

		[SerializeField]
		private UseType _useType;

		[SerializeField]
		private bool _static = false;
		public bool IsStatic => _static;

		[SerializeField]
		private int _buildSegmentLength;

		[SerializeField]
		private int _preBuildLength = 16;

		[SerializeField]
		private int _length;
		public int Length => _length;

		[SerializeField]
		private LevelRegion _region;

		[SerializeField]
		private LevelDifficulty _difficulty;

		[SerializeField]
		private string _seedString;

		[SerializeField]
		private LevelProfile _levelProfile;

		[SerializeField]
		private bool _forceNoCloud = false;

		[SerializeField]
		private bool _forceNoMountain = false;

		[SerializeField]
		private bool _forceSkybox = false;

		[SerializeField]
		private Material _skybox = null;

		[SerializeField]
		private bool _forceLight = false;

		[SerializeField]
		private GameObject _lightPrefab = null;

		[SerializeField]
		private float _envLightIntensity;

		[SerializeField]
		private Color _envLightColor;
		private Level _level;

		public string SeedString { get => _seedString; set => _seedString = value; }
		public Level Level { get => _level; set => _level = value; }

		public static GameObject LevelObject => GameObject.Find("Level");

		#region Unity Methods
		private void OnDestroy()
		{
			Clear();
		}
		#endregion

		public bool CreateStatic()
		{
			_static = true;
			if (_level == null)
			{
				try
				{
					switch (_useType)
					{
						case UseType.Profile:
							InitializeLevelProfile(new LevelIdentifier(_seedString), _length);
							break;
						case UseType.Seed:
							LoadLevelProfile(new LevelIdentifier(_seedString), _length);
							break;
						case UseType.Meta:
							LoadLevelProfile(new LevelIdentifier(_region, _difficulty), _length);
							break;
					}
					_level = new Level(_levelProfile);
				}
				catch (System.Exception ex)
				{
					Debug.LogWarning("fail to initialize level:" + ex.Message);
					Debug.LogException(ex);
				}
			}
			if (_level.Chunks.Count < _length)
			{
				try
				{
					_level.Create(_buildSegmentLength);
				}
				catch (System.Exception ex)
				{
					Debug.LogWarning("fail to create level:" + ex.Message);
					Debug.LogException(ex);
				}
			}
			else
			{
				return false;
			}
			return _level.Chunks.Count < _length;
		}

		public GameObject ArchiveLevel(string name)
		{
			if (_level != null)
			{
				GameObject level = _level.RootContainer.gameObject;
				level.name = name;
				_level = null;
				Clear();
				return level;
			}
			return null;
		}

		public void Clear()
		{
			_static = false;
			if (_level != null)
			{
				_level.Clear();
				_level = null;
			}
			GameObject level = LevelObject;
			if (level != null)
			{
				level.Destroy();
			}
		}

		public LevelIdentifier GetLevelIdentifier()
		{
			return new LevelIdentifier(_seedString);
		}

		public IEnumerator LoadLevel(string seedString, bool endless)
		{
			BLogger.Add("LoadLevel seed:" + seedString + " endless:" + endless);
			if (_level != null && (_level.Profile.SeedString != seedString || _level.Profile.IsEndless() != endless))
			{
				_level.Clear();
				_level = null;
			}

			if (_level == null)
			{
				LoadLevelProfile(new LevelIdentifier(seedString), endless ? -1 : _length);
				_level = new Level(_levelProfile);
				if (!_static)
				{
					_level.Create(_buildSegmentLength, true);
					while (Progress() < 1f)
					{
						yield return null;
					}
				}
			}
			else
			{
				if (!_static)
				{
					while (Progress() < 1f)
					{
						yield return null;
					}
					_level.Reload();
				}
			}
		}

		public void LoadNextSegmentIfRequired(int playerIndex)
		{
			if (_level == null)
			{
				return;
			}

			if (_level.LoadedCount == _levelProfile.ShapeLength)
			{
				BLogger.Add("Builder/ check loadable chunk. All level loaded");
				return;
			}

			if (_level.LoadedCount - playerIndex > _preBuildLength)
			{
				BLogger.Add("Builder/ check loadable chunk. PreChunk:" + (_level.LoadedCount - playerIndex) + " / " + _preBuildLength);
				return;
			}
			if (_level.JobQueue != null && _level.JobQueue.Progress < 1)
			{
				BLogger.Add("Builder/ check loadable chunk. Job in progress:" + _level.JobQueue.Progress);
				return;
			}
			_level.Create(_buildSegmentLength, true);
		}

		public float Progress()
		{
			if (_static)
			{
				return 1f;
			}
			if (_level != null && _level.JobQueue != null)
			{
				return _level.JobQueue.Progress;
			}
			return 0f;
		}

		public bool JobInProgress()
		{
			return _level != null && _level.JobQueue != null && _level.JobQueue.Progress < 1;
		}

		public string ProgressStep()
		{
			if (_static)
			{
				return "Level is static";
			}
			if (_level != null && _level.JobQueue != null)
			{
				return _level.JobQueue.WorkStep;
			}
			else
			{
				return "Looking under the table";
			}
		}

		private void LoadLevelProfile(LevelIdentifier levelIdentifier, int length)
		{
			string name = "Level/" + levelIdentifier.LevelRegion + "/" + levelIdentifier.LevelDifficulty + "/" + levelIdentifier.LevelRegion + levelIdentifier.LevelDifficulty + "_Level";
			_levelProfile = Resources.Load(name) as LevelProfile;
			if (_levelProfile == null)
			{
				Debug.LogWarning("Fail to load levelProfile:" + name);
			}
			else
			{
				InitializeLevelProfile(levelIdentifier, length);
			}
		}

		private void InitializeLevelProfile(LevelIdentifier levelIdentifier, int length)
		{
			if (_forceNoCloud)
			{
				_levelProfile._cloud._enable = false;
			}
			if (_forceNoMountain)
			{
				_levelProfile._mountain._enable = false;
			}
			if (_forceSkybox)
			{
				_levelProfile._skybox = _skybox;
			}
			if (_forceLight)
			{
				_levelProfile._lightPrefab = _lightPrefab;
				_levelProfile._envLightColor = _envLightColor;
				_levelProfile._envLightIntensity = _envLightIntensity;
			}
			_levelProfile.Initialize(levelIdentifier.SeedString, length);
		}
	}
}

