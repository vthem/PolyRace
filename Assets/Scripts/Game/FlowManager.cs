using System;
using System.Collections;
using System.Collections.Generic;

using TSW;
using TSW.Messaging;

using Game.Ghost;
using Game.Race;

using LevelGen;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;
using GameInputManager = Game.Input.InputManager;
using InputAction = Game.Input.InputAction;


namespace Game
{
	[RequireComponent(typeof(ScanForHandler))]
	public class FlowManager : MonoBehaviour
	{
		public class LevelLoadedEvent : BDEvent { }
		public class PauseToggleEvent : BDEvent { }
		public class EndReplayEvent : Event<EndRaceType> { }

		[SerializeField]
		private LevelBuilder _builder;

		[SerializeField]
		private UI.HUD _HUDUI;

		[SerializeField]
		private UI.LoadLevel _loadLevelUI;

		[SerializeField]
		private UI.Summary _summaryUI;

		[SerializeField]
		private UI.InRaceNotificationManager _noticeManager;

		[SerializeField]
		private UI.Intro _intro;

		[SerializeField]
		private UI.ActiveSwitcher _mainUISwitcher;

		[SerializeField]
		private UI.UIElement _curtainUI;

		[SerializeField]
		private GameObject _mainMenuRabitPrefab;

		public Racer.Controller PlayerRacer { get; private set; }
		public Chronometer RaceChrono { get; private set; }
		public DistanceScore DistanceScore { get; private set; }

		private Ghost.Synchronizer _synchronizer;
		private PauseState _pauseState;
		private Metrics.Collection _currentMetrics;
		private GameState _gameState = GameState.None;
		private EndRaceType _endRaceType = EndRaceType.None;
		private MainMenuRabbit _existingRabbit;
		private readonly float _animateEndRaceTime = 1.5f;

		private List<Racer.Controller> GhostRacers { get; set; }

		private enum LoadSceneOption
		{
			UseExisting,
			ClearExisting
		};

		private void Start()
		{
			TSW.Loca.DirtyLoca.Load("LanguageDef");

			GhostRacers = new List<Racer.Controller>();

			if (Config.GetAsset().IsDebug)
			{
				TSW.Log.Logger.Enable();
				TSW.Log.Logger.Add("Starting PolyRace " + BuilderConfig.Instance().VersionString);
			}
			Debug.Log("Starting PolyRace " + BuilderConfig.Instance().VersionString);

			RaceChrono = new Chronometer(() => { return Time.fixedTime; });
			DistanceScore = DistanceScore.Instantiate();
			Camera.CameraManager.Instance.SetSceneMode(Camera.CameraSceneMode.Default);

			// load language
			string lang = Player.PlayerManager.Instance.GetLanguage();
			try
			{
				TSW.Loca.DirtyLoca.UseLanguage(lang);
			}
			catch (System.Exception ex)
			{
				Debug.Log("Language " + lang + " not loaded, fallback on english");
				Debug.LogException(ex);
				TSW.Loca.DirtyLoca.UseLanguage("en");
			}

			// bootstrap
			switch (Config.GetAsset().GameBootType)
			{
				case Config.BootType.Intro:
					_intro.AnimateDisplayState(true, null);
					StartCoroutine(LoadMainMenu(false));
					break;
				case Config.BootType.Menu:
					_mainUISwitcher.Switch("MainMenu_UIElement");
					break;
				case Config.BootType.Sandbox:
					Setup.New();
					Setup.Next.UpdateMode(GameMode.Training);
					Setup.Next.UpdateType(RaceType.TimeAttack);
					Setup.Next.UpdateRacerDynamicProperties(Racer.DynamicProperties.NewFromRacerId(0));
					Setup.Swap();
					OnLoadNewScene(null);
					break;
			}
		}

		[EventHandler(typeof(UI.RaceButton.StartRaceEvent))]
		public void OnLoadNewScene(BDEvent evt)
		{
			StopAllCoroutines();
			StartCoroutine(LoadScene(LoadSceneOption.ClearExisting, false));
		}

		[EventHandler(typeof(UI.Navigation.RestartRaceEvent))]
		public void OnButtonRestart(BDEvent evt)
		{
			if (_gameState == GameState.Race || (_endRaceType == EndRaceType.FrontalImpact || _endRaceType == EndRaceType.OutOfEnergy))
			{
				RestartRace(true);
			}
			else
			{
				RestartRace(false);
			}
		}

		private void RestartRace(bool fastRestart)
		{
			StopAllCoroutines();
			DistanceScore.StopScoring();
			if (Race.Setup.Current.RaceType == Game.Race.RaceType.Distance)
			{
				Timer.Instance.Cancel("distance");
			}
			StartCoroutine(LoadScene(LoadSceneOption.UseExisting, fastRestart));
		}

		[EventHandler(typeof(Racer.Modules.GateDetection.StartLinePassedEvent))]
		public void OnRacerPassesStartLine(Racer.Modules.GateDetection.StartLinePassedEvent evt)
		{
			if (_gameState == GameState.LoadLevel)
			{

				SetQuickRestartState(true);

				SetPlayerRacerRaceCommand();
				_gameState = GameState.Race;
				DistanceScore.StartScoring(PlayerRacer, _builder);
				RaceChrono.Start();
				if (Race.Setup.Current.RaceType == Race.RaceType.Distance)
				{
					Timer.Instance.Arm("distance", false, _builder.Level.Profile._raceTrack._startBonusTimer, () =>
					{
						Bonus.Controller.MoveToSkyAllBonuses();
						StartCoroutine(EndRace(EndRaceType.RaceFinished));
					});
					ArmDistanceEndTimerNotice();
				}
				_HUDUI.AnimateDisplayState(true, null);

				StartCoroutine(WaitAndDisplayOpponentName());
			}
		}

		[EventHandler(typeof(Racer.Modules.GateDetection.EndLinePassedEvent))]
		public void OnRacerPassesEndLine(Racer.Modules.GateDetection.EndLinePassedEvent evt)
		{
			if (_gameState == GameState.Race)
			{
				StartCoroutine(EndRace(EndRaceType.RaceFinished));
			}
		}

		[EventHandler(typeof(MainMenuRabbit.RabbitReachedEnd))]
		public void OnRabbitReachEnd(MainMenuRabbit.RabbitReachedEnd evt)
		{
			_gameState = GameState.None;
			StartCoroutine(LoadMainMenu(false));
		}

		[EventHandler(typeof(UI.Navigation.DisplayMainMenuEvent))]
		public void OnDisplayMainMenu(BDEvent evt)
		{
			Timer.Instance.Cancel("distance");
			StopAllCoroutines();
			StartCoroutine(LoadMainMenu(true));
		}

		[EventHandler(typeof(PauseToggleEvent), typeof(UI.Navigation.ResumeRaceEvent))]
		public void OnPauseToggle(BDEvent evt)
		{
			if (_pauseState != null)
			{
				_pauseState.Resume();
				_pauseState = null;
				SetQuickRestartState(true);
				SetPauseTriggerState(true);
			}
			else
			{
				SetPauseTriggerState(false);
				SetQuickRestartState(false);
				_pauseState = new PauseState(_HUDUI, _mainUISwitcher, _noticeManager, DistanceScore, RaceChrono);
			}
		}

		[EventHandler(typeof(Racer.Modules.CollisionModule.TerrainHardCrashEvent))]
		public void OnHardCrash(BDEvent evt)
		{
			if (_gameState == GameState.Race)
			{
				StartCoroutine(EndRace(EndRaceType.FrontalImpact));
			}
		}

		[EventHandler(typeof(Racer.EnergyController.BatteryEmptyEvent))]
		public void OnBatteryEmtpy(BDEvent evt)
		{
			if (_gameState == GameState.Race)
			{
				StartCoroutine(EndRace(EndRaceType.OutOfEnergy));
			}
		}

		[EventHandler(typeof(EndReplayEvent))]
		public void OnEndReplay(EndReplayEvent evt)
		{
			if (_gameState == GameState.Replay)
			{
				StartCoroutine(EndReplay(evt.Value1));
			}
		}

		[EventHandler(typeof(Bonus.Controller.BonusEvent))]
		public void OnDistanceBonusEvent(Bonus.Controller.BonusEvent evt)
		{
			if (_gameState != GameState.Race)
			{
				return;
			}
			Audio.SoundFx.Instance.Play("Bonus");
			if (evt.Value1 == Bonus.BonusType.InverseTimer)
			{
				Timer.Instance.Add("distance", _builder.Level.Profile._raceTrack._bonusTimer);
			}
		}

		private IEnumerator WaitAndDisplayOpponentName()
		{
			yield return new WaitForSeconds(1f);

			for (int i = 0; i < GhostRacers.Count; ++i)
			{
				_noticeManager.NotifyOpponentName(GhostRacers[i].GhostModule.GhostName);
				GhostRacers[i].GhostModule.MakeVisible();
				yield return new WaitForSeconds(.8f);
			}
		}

		private IEnumerator EndReplay(EndRaceType endRaceType)
		{
			Input.InputHelper.EnableCameraInput();
			AnimateEndRace(endRaceType);
			yield return new WaitForSeconds(_animateEndRaceTime);
			StartCoroutine(LoadReplay());
		}

		private IEnumerator LoadMainMenu(bool displayUI)
		{
			// HIDE NAV UI, DISPLAY LOAD UI
			if (displayUI)
			{
				_mainUISwitcher.Switch("MainMenu_UIElement");
			}

			if (_gameState != GameState.MainMenu)
			{
				_gameState = GameState.LoadLevel;

				// PLAY MENU MUSIC
				Audio.Music.Instance.PlayMenu();

				_loadLevelUI.SetBuilder(_builder);
				yield return StartCoroutine(_curtainUI.AnimateDisplayState(true));
				yield return StartCoroutine(_loadLevelUI.AnimateDisplayState(true));

				// STOP EMPHASIS / LEVEL ACTIVATOR
				EmphasisPath.Instance.StopEmphasis();
				LevelActivator.Instance.Stop();

				// DESTROY THE RABBIT
				if (_existingRabbit != null)
				{
					GameObject.Destroy(_existingRabbit.gameObject);
					yield return null;
				}

				// ENABLE THE DEFAULT CAMERA
				Camera.CameraManager.Instance.SetSceneMode(Camera.CameraSceneMode.Default);

				// DISABLE CHALLENGE TIMER IF NOT IN CHALLENGE


				// CLEAR PAUSE STATE
				if (_pauseState != null)
				{
					_pauseState.Clear();
					_pauseState = null;
				}

				// DISABLE ANY REPLAY IN PROGRESS
				if (null != _synchronizer && _synchronizer.enabled)
				{
					_synchronizer.enabled = false;
				}

				// DISABLE INPUTS
				SetPauseTriggerState(false);
				SetQuickRestartState(false);
				Input.InputHelper.DisableCameraInput();

				// DEFAULT CAMERA
				Camera.CameraManager.Instance.SetSceneMode(Camera.CameraSceneMode.Default);

				// Clear existing RACER
				ClearRacers();

				// LOAD THE LEVEL
				yield return StartCoroutine(LoadLevel(LevelIdentifier.Randomize().SeedString, false));

				// PREPARE RABIT
				GameObject obj = GameObject.Instantiate(_mainMenuRabitPrefab);
				_existingRabbit = obj.GetComponent<MainMenuRabbit>();
				_existingRabbit.Run(_builder.Level);

				// ENABLE DYNAMIC LEVEL
				LevelActivator.Instance.Initialize(
					LevelBuilder.LevelObject,
					obj.transform,
					_builder
				);

				// READY TO SHOW THE SCENE
				_loadLevelUI.AnimateDisplayState(false, null);
				_curtainUI.AnimateDisplayState(false, null);

				// RESET AUDIO FX
				Audio.SoundFx.Instance.ApplyDefault();

				// ENABLE THE CHASE CAMERA
				Camera.CameraManager.Instance.SetSceneMode(Camera.CameraSceneMode.MainMenu);
			}
			_gameState = GameState.MainMenu;

		}

		private IEnumerator LoadScene(LoadSceneOption option, bool fastStart)
		{
			_gameState = GameState.LoadLevel;

			// RESET CURRENT METRICS
			if (option == LoadSceneOption.ClearExisting)
			{
				_currentMetrics = new Metrics.Collection();
			}

			// CLEAR PAUSE STATE
			if (_pauseState != null)
			{
				_pauseState.Clear();
				_pauseState = null;
			}

			// DISABLE ANY REPLAY IN PROGRESS
			if (null != _synchronizer && _synchronizer.enabled)
			{
				_synchronizer.enabled = false;
			}

			// DISABLE INPUTS
			SetPauseTriggerState(false);
			SetQuickRestartState(false);
			Input.InputHelper.DisableCameraInput();

			// HIDE NAV UI, DISPLAY LOAD UI
			if (_mainUISwitcher.Active != null)
			{
				yield return StartCoroutine(_mainUISwitcher.HideActive());
			}
			if (_HUDUI.DisplayState)
			{
				_HUDUI.AnimateDisplayState(false, null);
			}
			if (_summaryUI.DisplayState)
			{
				_summaryUI.AnimateDisplayState(false, null);
			}

			_loadLevelUI.SetBuilder(_builder);
			yield return StartCoroutine(_curtainUI.AnimateDisplayState(true));
			yield return StartCoroutine(_loadLevelUI.AnimateDisplayState(true));

			// DEFAULT CAMERA
			Camera.CameraManager.Instance.SetSceneMode(Camera.CameraSceneMode.Default);

			// DESTROY ANY EXISTING RABBIT
			if (_existingRabbit != null)
			{
				GameObject.Destroy(_existingRabbit.gameObject);
				yield return null;
			}

			// STOP EMPHASIS / LEVEL ACTIVATOR
			EmphasisPath.Instance.StopEmphasis();
			LevelActivator.Instance.Stop();

			// LOAD THE LEVEL
			yield return StartCoroutine(LoadLevel(Race.Setup.Current.LevelIdentifier.SeedString, Race.Setup.Current.RaceType.IsEndless()));

			// PREPARE RACERS
			yield return StartCoroutine(PrepareRacersForRace());

			// ENABLE DYNAMIC LEVEL
			LevelActivator.Instance.Initialize(
				LevelGen.LevelBuilder.LevelObject,
				PlayerRacer.transform,
				_builder
			);
			EmphasisPath.Instance.StartEmphasis(
				PlayerRacer.transform,
				_builder
			);

			// PREPARE HUD DATA
			if (Race.Setup.Current.RaceType == Race.RaceType.Distance)
			{
				_HUDUI.SetDataMethod(
					() => DistanceScore.ScoreString,
					() => Chronometer.FormatTime(Timer.Instance.CurrentTime("distance")),
					() => UI.StringHelper.SpeedNoUnitString(PlayerRacer.DataModule.Speed, Player.PlayerManager.Instance.GetSpeedUnit()),
					() => PlayerRacer.DataModule.NormalizedSpeed,
					() => PlayerRacer.EnergyModule.EnergyController.CurrentCapacityNormalized);
			}
			else if (Race.Setup.Current.RaceType == Race.RaceType.TimeAttack)
			{
				_HUDUI.SetDataMethod(
					() => DistanceScore.ScoreString,
					() => TSW.Chronometer.FormatTime(RaceChrono.ElapsedTime),
					() => UI.StringHelper.SpeedNoUnitString(PlayerRacer.DataModule.Speed, Player.PlayerManager.Instance.GetSpeedUnit()),
					() => PlayerRacer.DataModule.NormalizedSpeed,
					() => PlayerRacer.EnergyModule.EnergyController.CurrentCapacityNormalized);
			}
			else
			{
				_HUDUI.SetDataMethod(
					() => DistanceScore.ScoreString,
					() => Chronometer.FormatTime(RaceChrono.ElapsedTime),
					() => UI.StringHelper.SpeedNoUnitString(PlayerRacer.DataModule.Speed, Player.PlayerManager.Instance.GetSpeedUnit()),
					() => PlayerRacer.DataModule.NormalizedSpeed,
					() => PlayerRacer.EnergyModule.EnergyController.CurrentCapacityNormalized);
			}

			// ENABLE THE CHASE CAMERA
			Camera.CameraManager.Instance.SetSceneMode(Game.Camera.CameraSceneMode.Race);

			// MAKE RACER IN READY STATE
			// should be after camera initialization to avoid 'wurp' noise in sound effect
			// because camera move ...
			yield return new WaitForSeconds(.1f);
			PlayerRacer.Helper.Ready(_builder.Level.Profile._racerGroundParticlePrefab);
			MapGhostRacers((gr) =>
			{
				gr.Helper.Ready(_builder.Level.Profile._racerGroundParticlePrefab);
			});

			// READY TO SHOW THE SCENE
			_loadLevelUI.AnimateDisplayState(false, null);
			_curtainUI.AnimateDisplayState(false, null);
			_noticeManager.ActivateAndClear();

			// RESET AUDIO FX
			Audio.SoundFx.Instance.ApplyDefault();

			// ENABLE COUNTDOWN OR DISPLAY CHALLENGE INFO
			yield return StartCoroutine(CountdownStage(option == LoadSceneOption.UseExisting));
		}

		private class CoroutineFunc
		{
			public bool Result { get; protected set; }
		}

		private class SaveRaceData : CoroutineFunc
		{
			public bool Improved { get; private set; }
			public float ChallengeProgress { get; private set; }
			public float ProgressDelta { get; private set; }
			public List<UnlockReward.Reward> Rewards { get; private set; }

			public IEnumerator Execute(Metrics.Collection currentMetrics, float elapsedTime, int hitNumber, float maxSpeed, float score, float distance, Ghost.Synchronizer ghostSynchro, int nBoostActivate, int nBoostDeactivation, int nBrakeActivation)
			{
				// update metric collection into local player info
				if (Race.Setup.Current.RaceType == Race.RaceType.TimeAttack)
				{
					currentMetrics.UpdateMetric(Metrics.MetricType.ElapsedTime, Mathf.RoundToInt(elapsedTime * 1000f));
					currentMetrics.UpdateMetric(Metrics.MetricType.HitNumber, hitNumber);
					currentMetrics.UpdateMetric(Metrics.MetricType.MaxSpeed, Mathf.RoundToInt(maxSpeed));
					currentMetrics.UpdateMetric(Metrics.MetricType.AvgSpeed, Mathf.RoundToInt(distance / elapsedTime));
					currentMetrics.EndUpdate();
					Improved = currentMetrics.GetMetric(Metrics.MetricType.ElapsedTime).Improved;
				}
				else if (Race.Setup.Current.RaceType == Race.RaceType.Distance)
				{
					currentMetrics.UpdateMetric(Metrics.MetricType.Distance, Mathf.RoundToInt(score));
					currentMetrics.UpdateMetric(Metrics.MetricType.AvgSpeed, Mathf.RoundToInt(distance / elapsedTime));
					currentMetrics.UpdateMetric(Metrics.MetricType.MaxSpeed, Mathf.RoundToInt(maxSpeed));
					currentMetrics.EndUpdate();
					Improved = currentMetrics.GetMetric(Metrics.MetricType.Distance).Improved;
				}
				else if (Race.Setup.Current.RaceType == Race.RaceType.Endless)
				{
					currentMetrics.UpdateMetric(Metrics.MetricType.ElapsedTime, Mathf.RoundToInt(elapsedTime * 1000f));
					currentMetrics.UpdateMetric(Metrics.MetricType.Distance, Mathf.RoundToInt(score));
					currentMetrics.UpdateMetric(Metrics.MetricType.HitNumber, hitNumber);
					currentMetrics.UpdateMetric(Metrics.MetricType.AvgSpeed, Mathf.RoundToInt(distance / elapsedTime));
					currentMetrics.UpdateMetric(Metrics.MetricType.MaxSpeed, Mathf.RoundToInt(maxSpeed));
					currentMetrics.EndUpdate();
					Improved = false;
				}
				Debug.Log("New race metrics:" + currentMetrics);

				// SAVE GHOST DATA
				if (Improved)
				{
					try
					{
						Ghost.Record ghost = ghostSynchro.GetGhost();
						Race.Setup.Current.UpdateGhost(ghost, Ghost.GhostType.Self);
					}
					catch (System.Exception ex)
					{
						TSW.Log.Logger.Add("Fail to create challenge data: " + ex.Message);
					}
				}

				yield return null;
			}
		}

		private IEnumerator CountdownStage(bool fastRestart)
		{
			// ENABLE INPUT LISTENER
			SetPauseTriggerState(true);
			Input.InputHelper.EnableCameraInput();

			// START THE MUSIC & SFX
			Audio.Music.Instance.PlayRandom(Race.Setup.Current.LevelIdentifier.SeedString.GetHashCode());
			Audio.SoundFx.Instance.ApplyDefault();

			// START THE COUNTDOWN

			yield return StartCoroutine(Race.GateStartLight.StartCountDown(fastRestart));

			// START THE PRE RACE
			PlayerRacer.Helper.Go();
			MapGhostRacers((gr) =>
			{
				gr.Helper.Go();
			});
			_synchronizer.enabled = true;
			if (Race.Setup.Current.RaceType == Race.RaceType.Distance)
			{
				Bonus.Spawner.Instance.StartSpawning(PlayerRacer.transform);
			}
		}

		private void AnimateEndRace(EndRaceType endRaceType)
		{
			switch (endRaceType)
			{
				case EndRaceType.FrontalImpact:
					PlayerRacer.Helper.CrashTerrain();
					break;
				case EndRaceType.OutOfEnergy:
					PlayerRacer.Helper.CrashGround();
					break;
				case EndRaceType.RaceFinished:
					PlayerRacer.Helper.PassEndLine();
					break;
			}
		}

		private IEnumerator EndRace(EndRaceType endRaceType)
		{
			_gameState = GameState.EndRace;
			_endRaceType = endRaceType;

			// DISABLE INPUTS
			Input.InputHelper.DisableCameraInput();
			SetPauseTriggerState(false);

			// RESET RACE STATE
			RaceChrono.Stop();
			DistanceScore.StopScoring();
			Timer.Instance.Cancel("distance");
			_synchronizer.SetEndRaceType(endRaceType);
			_synchronizer.enabled = false;
			_noticeManager.DeactivateAndClear();

			// DISPLAY / HIDE UI
			_HUDUI.AnimateDisplayState(false, null);

			// STOP EMPHASIS
			EmphasisPath.Instance.StopEmphasis();

			// ANIMATE RACERS
			AnimateEndRace(endRaceType);

			// SAVE RACE DATA
			Coroutine saveRaceDataRoutine = null;
			SaveRaceData saveRaceDataFunc = new SaveRaceData();
			Coroutine uploadROTDRoutine = null;
			Coroutine uploadMissionRoutine = null;
			if (Race.Setup.Current.RaceType == Race.RaceType.Distance
				|| Race.Setup.Current.RaceType == Race.RaceType.TimeAttack && endRaceType == EndRaceType.RaceFinished)
			{
				saveRaceDataRoutine = StartCoroutine(saveRaceDataFunc.Execute(
					_currentMetrics,
					RaceChrono.ElapsedTime,
					PlayerRacer.CollisionModule.HitNumber,
					PlayerRacer.DataModule.MaxSpeed,
					DistanceScore.Score,
					DistanceScore.Distance,
					_synchronizer,
					PlayerRacer.CommandModule.BoostActivateCount,
					PlayerRacer.CommandModule.BoostDeactivateCount,
					PlayerRacer.CommandModule.BrakeActivationCount));
			}
			yield return new WaitForSeconds(_animateEndRaceTime);

			// RESET AUDIO FX
			Audio.SoundFx.Instance.ApplyDefault();

			// LOAD THE REPLAY
			StartCoroutine(LoadReplay());

			yield return saveRaceDataRoutine;
			yield return uploadROTDRoutine;
			yield return uploadMissionRoutine;

			// DISPLAY UI
			Debug.Log("Race metrics:" + _currentMetrics);

			UI.SummaryDisplayData data = null;

			if (data == null)
			{
				data = new UI.SummaryDisplayData();
			}
			data.SetData(Race.Setup.Current, _currentMetrics, endRaceType);
			_summaryUI.UpdateSummaryDisplayData(data);

			yield return StartCoroutine(_mainUISwitcher.SwitchRoutine(_summaryUI));
		}

		private IEnumerator LoadReplay()
		{
			_gameState = GameState.LoadReplay;

			// DISPLAY LOAD UI
			_loadLevelUI.SetBuilder(_builder);
			_loadLevelUI.AnimateDisplayState(true, null);
			yield return StartCoroutine(_curtainUI.AnimateDisplayState(true));
			Camera.CameraManager.Instance.SetSceneMode(Game.Camera.CameraSceneMode.Default);

			// RELOAD THE LEVEL
			yield return StartCoroutine(LoadLevel(Race.Setup.Current.LevelIdentifier.SeedString, Race.Setup.Current.RaceType.IsEndless()));

			PrepareRacersForReplay();
			yield return null;

			// PREPARE THE RACERS
			PlayerRacer.Helper.Ready(_builder.Level.Profile._racerGroundParticlePrefab);
			PlayerRacer.Helper.Go();
			MapGhostRacers((gr) =>
			{
				gr.Helper.Ready(_builder.Level.Profile._racerGroundParticlePrefab);
				gr.Helper.Go();
			});

			// ENABLE REAL TIME LEVEL ACTIVATION
			LevelActivator.Instance.Initialize(
				LevelGen.LevelBuilder.LevelObject,
				PlayerRacer.transform,
				_builder
			);

			// ENABLE CAMERA FOR REPLAY
			Camera.CameraManager.Instance.SetSceneMode(Game.Camera.CameraSceneMode.Replay);

			// LAUNCH REPLAY
			_synchronizer.enabled = true;
			_gameState = GameState.Replay;
			if (Race.Setup.Current.RaceType == Race.RaceType.Distance)
			{
				Bonus.Spawner.Instance.StartSpawning(PlayerRacer.transform);
			}

			// HIDE LOAD UI
			_loadLevelUI.AnimateDisplayState(false, null);
			_curtainUI.AnimateDisplayState(false, null);

			// ENABLE CYCLE CAMERA MODE / VIEW
			Input.InputHelper.EnableCameraInput();

			// DISPLAY GHOST
			MapGhostRacers((gr) =>
			{
				gr.GhostModule.MakeVisible();
			});
		}

		private IEnumerator LoadLevel(string seedString, bool endless)
		{
			Bonus.Spawner.Instance.ClearBonuses();
			yield return StartCoroutine(_builder.LoadLevel(seedString, endless));
		}

		private void ClearRacers()
		{
			if (PlayerRacer != null)
			{
				GameObject.DestroyImmediate(PlayerRacer.gameObject);
				PlayerRacer = null;
			}
			MapGhostRacers((gr) =>
			{
				GameObject.DestroyImmediate(gr.gameObject);
			});
			GhostRacers.Clear();
		}

		private void CreatePlayerRacer()
		{
			PlayerRacer = Race.Setup.Current.RacerDynamicProperties.Instantiate();
			PlayerRacer.gameObject.name = "PlayerRacer";
			PlayerRacer.gameObject.tag = "PlayerRacer";
			SetPlayerRacerPreRaceCommand();
			PlayerRacer.transform.position = Config.GetAsset().StartPosition;
			PlayerRacer.ColorModule.SetColor(Race.Setup.Current.RacerColor);
		}

		private void SetPlayerRacerRaceCommand()
		{
			System.Func<float> Boost = () => GameInputManager.GetBool(InputAction.Boost) ? 1f : 0f;
			PlayerRacer.CommandModule.TurnInputGetter = () => GameInputManager.GetFloat(InputAction.Steer);
			PlayerRacer.CommandModule.BoostInputGetter = Boost;
			PlayerRacer.CommandModule.AccelerateInputGetter = () =>
			{
				if (GameInputManager.GetBool(InputAction.Brake) && Boost() <= 0)
				{
					return -1f;
				}
				else
				{
					if (Config.GetAsset().AutoAcceleration)
					{
						return 1f;
					}
					else
					{
						return GameInputManager.GetBool(InputAction.Accelerate) ? 1f : 0f;
					}
				}
			};
		}

		private void SetPlayerRacerPreRaceCommand()
		{
			PlayerRacer.CommandModule.AccelerateInputGetter = () =>
			{
				if (Config.GetAsset().AutoAcceleration)
				{
					return 1f;
				}
				else
				{
					return GameInputManager.GetBool(InputAction.Accelerate) ? 1f : 0f;
				}
			};
		}

		private void CreateGhostRacer(Ghost.Player ghostPlayer, Ghost.GhostType ghostType, Racer.ColorId ghostColor)
		{
			Ghost.RecordProperties prop = ghostPlayer.GetRecordProperties();
			Racer.Controller racer = Racer.Controller.Instantiate(Ghost.RacerIdColor.GetRacerIndex(prop.RacerId));
			racer.gameObject.name = "GhostRacer(" + ghostType.ToString() + ")";
			racer.gameObject.tag = "GhostRacer";
			racer.gameObject.AddComponent<Game.Racer.GhostRacerHelper>();
			racer.DataModule.GhostDriven = true;
			racer.GhostModule.GhostPlayer = ghostPlayer;
			racer.GhostModule.GhostType = ghostType;
			racer.GhostModule.GhostName = ghostPlayer.GetRecordProperties().PlayerName;
			racer.GhostModule.GhostColor = ghostColor;
			racer.ColorModule.SetColor(Racer.ColorSet.GetAsset().GetMaterial(ghostColor));
			switch (ghostPlayer.GetEndRaceType())
			{
				case EndRaceType.FrontalImpact:
					ghostPlayer.EndAction = () => { racer.Helper.CrashTerrain(); };
					break;
				case EndRaceType.OutOfEnergy:
					ghostPlayer.EndAction = () => { racer.Helper.CrashGround(); };
					break;
				case EndRaceType.RaceFinished:
					ghostPlayer.EndAction = () => { racer.Helper.PassEndLine(); };
					break;
			}
			racer.transform.position = Config.GetAsset().StartPosition;
			GhostRacers.Add(racer);
		}

		private class GhostEqualityComparer : IEqualityComparer<Ghost.Record>
		{
			public bool Equals(Record x, Record y)
			{
				if (x == null && y == null)
				{
					return true;
				}
				if (x == null || y == null)
				{
					return false;
				}
				// This is not really true; Ghost are equal if all frames are equals.
				return x.Properties.Name == y.Properties.Name && x.Count == y.Count;
			}

			public int GetHashCode(Record obj)
			{
				int hash = 0;
				if (obj == null)
				{
					return hash;
				}
				return obj.Properties.PlayerName.GetHashCode() + obj.Count;
			}
		}

		private string CreateGhostRacers(bool addself)
		{
			string oppName = string.Empty;
			HashSet<Ghost.Record> createdGhost = new HashSet<Record>(new GhostEqualityComparer());
			GhostType[] types = null;
			if (addself)
			{
				types = new GhostType[] { GhostType.Self, GhostType.Remote, GhostType.Dev };
			}
			else
			{
				types = new GhostType[] { GhostType.Remote, GhostType.Dev };
			}

			// GHOST RACER
			foreach (Ghost.GhostType type in types)
			{
				Record ghost = Setup.Current.GetGhost(type);
				Debug.Log("prepare ghost -- " + type + " ghost:" + ghost);
				if (ghost != null)
				{
					if (createdGhost.Contains(ghost))
					{
						Debug.Log("A ghost has already been added");
						continue;
					}
					CreateGhostRacer(Ghost.Synchronizer.CreatePlayer(ghost), type, (Racer.ColorId)Ghost.RacerIdColor.GetColorIndex(ghost.Properties.RacerId));
					createdGhost.Add(ghost);
				}
			}
			return oppName;
		}

		private IEnumerator PrepareRacersForRace()
		{
			ClearRacers();

			// PLAYER RACER
			CreatePlayerRacer();
			PlayerRacer.gameObject.AddComponent<Game.Racer.PlayerRacerHelper>();

			// GHOST RACER
			CreateGhostRacers(true);

			// SYNCHRONIZER TO RECORD PLAYER AND PLAY GHOST
			if (_synchronizer != null)
			{
				Game.Ghost.Synchronizer.DestroySynchronizer(_synchronizer);
			}
			_synchronizer = Game.Ghost.Synchronizer.CreateSynchronizer(
				Config.GetAsset().MaxGhostInterval,
				CreateRecordPropertiesHelper(_currentMetrics.UpdateId + 1)
			);
			TSW.Log.Logger.Add("Recording ghost as " + "Race#" + (_currentMetrics.UpdateId + 1).ToString());
			_synchronizer.SetProvider(PlayerRacer.GetFrameProperties);
			MapGhostRacers((gr) =>
			{
				_synchronizer.AddModifier("ghost-" + gr.GhostModule.GhostType.ToString(), gr.GhostModule.GhostPlayer, gr.FramePropertiesUpdate);
			});
			yield break;
		}

		private Ghost.RecordProperties CreateRecordPropertiesHelper(int raceCounterId)
		{
			return new Game.Ghost.RecordProperties(
				Ghost.RacerIdColor.GetIntValue(Race.Setup.Current.RacerDynamicProperties.Properties.Id, (int)Race.Setup.Current.ColorIndex),
				Race.Setup.Current.LevelIdentifier.SeedString,
				"Race#" + raceCounterId.ToString(),
 				Player.PlayerManager.Instance.DisplayName,
				(int)Race.Setup.Current.RaceType
			);
		}

		private void PrepareRacersForReplay()
		{
			ClearRacers();

			// PLAYER RACER
			// get replay from the record
			Game.Ghost.Player playerPlayer = _synchronizer.GetReplay();
			// or get replay from previous replay
			if (playerPlayer == null)
			{
				playerPlayer = _synchronizer.GetPlayer("player");
			}

			CreatePlayerRacer();
			PlayerRacer.gameObject.AddComponent<Game.Racer.ReplayRacerHelper>();
			PlayerRacer.DataModule.GhostDriven = true;

			// GHOST RACER
			CreateGhostRacers(false);

			// UPDATE SYNCHRONIZER
			Ghost.Synchronizer.DestroySynchronizer(_synchronizer);
			_synchronizer = Game.Ghost.Synchronizer.CreateSynchronizer();
			if (playerPlayer != null)
			{
				_synchronizer.AddModifier("player", playerPlayer, PlayerRacer.FramePropertiesUpdate);
				playerPlayer.EndAction = () => { Dispatcher.FireEvent(new EndReplayEvent().SetValue1(playerPlayer.GetEndRaceType())); };
			}
			else
			{
				Debug.Log("playerPlayer is null!!");
			}
			MapGhostRacers((gr) =>
			{
				_synchronizer.AddModifier("ghost-" + gr.GhostModule.GhostType.ToString(), gr.GhostModule.GhostPlayer, gr.FramePropertiesUpdate);
			});
		}

		private void ArmDistanceEndTimerNotice()
		{
			if (Timer.Instance.CurrentTime("distance") > 3f)
			{
				Timer.Instance.AddCallback("distance", 3f, () =>
				{
					_noticeManager.NoticeDistanceTimerWarning();
				});
			}
		}

		private void SetPauseTriggerState(bool state)
		{
			if (state)
			{
				GameInputManager.RegisterInputPressed(InputAction.PauseToggle, PauseTriggerInputHandler);
			}
			else
			{
				GameInputManager.UnregisterInputPressed(InputAction.PauseToggle, PauseTriggerInputHandler);
			}
		}

		private void SetQuickRestartState(bool state)
		{
			if (state)
			{
				GameInputManager.RegisterInputPressed(InputAction.QuickRestart, QuickRestartInputHandler);
			}
			else
			{
				GameInputManager.UnregisterInputPressed(InputAction.QuickRestart, QuickRestartInputHandler);
			}
		}

		private void QuickRestartInputHandler()
		{
			RestartRace(true);
		}

		private void PauseTriggerInputHandler()
		{
			Dispatcher.FireEvent(new PauseToggleEvent());
		}

		private void MapGhostRacers(System.Action<Racer.Controller> func)
		{
			for (int i = 0; i < GhostRacers.Count; ++i)
			{
				func(GhostRacers[i]);
			}
		}
	}
}