using System.Collections;

using TSW.Messaging;

using LevelGen;

namespace Game.Race
{
	public class Setup
	{
		public enum RacerUpdate
		{
			Default,
			FromLevelIdentifier,
			Direct
		}

		public class UpdatedEvent : Event<Setup> { }

		public static Setup Current { get; private set; }
		public static Setup Next { get; private set; }
		private static Setup BackupCopy { get; set; }

		public RaceType RaceType { get; private set; }

		public GameMode GameMode { get; private set; }

		public LevelIdentifier LevelIdentifier { get; private set; }

		public Ghost.GhostType GhostType { get; private set; }

		private readonly Ghost.Record[] _ghosts = new Ghost.Record[System.Enum.GetValues(typeof(Ghost.GhostType)).Length];

		public Ghost.Record GetGhost(Ghost.GhostType type)
		{
			return _ghosts[(int)type];
		}

		public Racer.DynamicProperties RacerDynamicProperties { get; private set; }

		public UnityEngine.Material RacerColor { get; private set; }
		public Racer.ColorId ColorIndex { get; private set; }

		public string RemoteDataId { get; private set; }

		public string RaceIdentifier => Identifier.BuildString(LevelIdentifier, RaceType);

		public RacerUpdate RacerUpdateType { get; private set; }

		public Setup(Racer.DynamicProperties racerDynProp, UnityEngine.Material defaultColor)
		{
			RaceType = RaceType.TimeAttack;
			GameMode = GameMode.None;
			LevelIdentifier = LevelIdentifier.Randomize();
			GhostType = Ghost.GhostType.Remote;
			RacerDynamicProperties = racerDynProp;
			RacerUpdateType = RacerUpdate.Default;
			RacerColor = defaultColor;
			ColorIndex = Racer.ColorId.Default;
		}

		public Setup(Setup other)
		{
			RaceType = other.RaceType;
			GameMode = other.GameMode;
			LevelIdentifier = other.LevelIdentifier;
			GhostType = other.GhostType;
			for (int i = 0; i < _ghosts.Length; ++i)
			{
				_ghosts[i] = other._ghosts[i];
			}
			RacerDynamicProperties = other.RacerDynamicProperties;
			RacerUpdateType = other.RacerUpdateType;
			RemoteDataId = other.RemoteDataId;
			RacerColor = other.RacerColor;
			ColorIndex = other.ColorIndex;
		}

		public void UpdateType(RaceType value)
		{
			RaceType = value;
			NotifyUpdate();
		}

		public void UpdateMode(GameMode value)
		{
			GameMode = value;
			NotifyUpdate();
		}

		public void UpdateLevelIdentifier(LevelIdentifier value)
		{
			Log("New LevelIdentifier:" + value + " old:" + LevelIdentifier);
			LevelIdentifier = value;
			for (int i = 0; i < _ghosts.Length; ++i)
			{
				_ghosts[i] = null;
			}
			NotifyUpdate();
		}

		public void UpdateRacerColor(UnityEngine.Material color, Racer.ColorId index)
		{
			RacerColor = color;
			ColorIndex = index;
			NotifyUpdate();
		}

		public void UpdateGhost(Ghost.Record value, Ghost.GhostType type)
		{
			_ghosts[(int)type] = value;
			NotifyUpdate();
		}

		public void UpdateRacerDynamicProperties(Racer.DynamicProperties value)
		{
			RacerDynamicProperties = value;
			RacerUpdateType = RacerUpdate.Direct;
			NotifyUpdate();
		}

		public void UpdateRacerDynamicPropertiesFromLevelIdentifier()
		{
			System.Random rand = new System.Random(LevelIdentifier.SeedString.GetHashCode());
			RacerDynamicProperties = Racer.DynamicProperties.NewFromRacerId(rand.Next(0, 4));
			RacerUpdateType = RacerUpdate.FromLevelIdentifier;
			NotifyUpdate();
		}

		public IEnumerator RandomizeIfInvalid(System.Func<LevelIdentifier, bool> validate)
		{
			LevelIdentifier levelId = LevelIdentifier;
			bool update = false;
			while (validate(levelId))
			{
				levelId = LevelIdentifier.Randomize();
				update = true;
				yield return null;
			}

			if (update)
			{
				UpdateLevelIdentifier(levelId);
			}
		}

		public static void New()
		{
			Next = new Setup(Racer.DynamicProperties.NewFromRacerId(0), Racer.PropertiesCollection.GetAsset().Common._defaultColor);
			Log("New " + Next);
			Next.NotifyUpdate();
		}

		public static void Swap()
		{
			Current = new Setup(Next)
			{
				_enableNotify = false
			};
			Log("Using current " + Current);
		}

		public static void Backup()
		{
			BackupCopy = new Setup(Next);
		}

		public static void Restore()
		{
			Next = new Setup(BackupCopy);
		}

		private bool _enableNotify = true;
		public void NotifyUpdate()
		{
			if (_enableNotify)
			{
				Dispatcher.FireEvent(new UpdatedEvent().SetValue1(this));
			}
		}

		public override string ToString()
		{
			return string.Format($"[Setup: RaceType={RaceType}, GameMode={GameMode}, LevelIdentifier={LevelIdentifier}, GhostType={GhostType}, RacerDynamicProperties={RacerDynamicProperties}, RemoteDataId={RemoteDataId}, RaceIdentifier={RaceIdentifier}, RacerColor={RacerColor}]",
				RaceType, GameMode, LevelIdentifier, GhostType, RacerDynamicProperties.Properties.HovercraftName, RemoteDataId, RaceIdentifier, RacerColor);
		}

		private static void Log(string text)
		{
			UnityEngine.Debug.Log("RaceSetup/ " + text);
		}
	}
}
