//using System;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using System.Collections.Generic;
//using Rewired;
//using Rewired.Integration;

//namespace Game {

//	[AddComponentMenu("Event/Rewired Standalone Input Module")]
//	public class GameInputModule : PointerInputModule {

//		#region Rewired Variables and Properties

//		/// <summary>
//		/// Allow all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Use all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.")]
//		private bool useAllRewiredGamePlayers = false;

//		/// <summary>
//		/// Allow the Rewired System Player to control the UI.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Allow the Rewired System Player to control the UI.")]
//		private bool useRewiredSystemPlayer = false;

//		/// <summary>
//		/// A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.")]
//		private int[] rewiredPlayerIds = new int[1] { 0 };

//		/// <summary>
//		/// Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.")]
//		private bool moveOneElementPerAxisPress;

//		private int[] playerIds;

//		private bool recompiling;

//		/// <summary>
//		/// Allow all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.
//		/// </summary>
//		public bool UseAllRewiredGamePlayers {
//			get { return useAllRewiredGamePlayers; }
//			set {
//				bool changed = value != useAllRewiredGamePlayers;
//				useAllRewiredGamePlayers = value;
//				if(changed) SetupRewiredVars();
//			}
//		}

//		/// <summary>
//		/// Allow the Rewired System Player to control the UI.
//		/// </summary>
//		public bool UseRewiredSystemPlayer {
//			get { return useRewiredSystemPlayer; }
//			set {
//				bool changed = value != useRewiredSystemPlayer;
//				useRewiredSystemPlayer = value;
//				if(changed) SetupRewiredVars();
//			}
//		}
//		/// <summary>
//		/// A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.
//		/// Returns a clone of the array.
//		/// </summary>
//		public int[] RewiredPlayerIds {
//			get { return (int[])rewiredPlayerIds.Clone(); }
//			set {
//				rewiredPlayerIds = (value != null ? (int[])value.Clone() : new int[0]);
//				SetupRewiredVars();
//			}
//		}

//		/// <summary>
//		/// Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.
//		/// </summary>
//		public bool MoveOneElementPerAxisPress {
//			get { return moveOneElementPerAxisPress; }
//			set { moveOneElementPerAxisPress = value; }
//		}


//		#endregion

//		private float m_PrevActionTime;
//		Vector2 m_LastMoveVector;
//		int m_ConsecutiveMoveCount = 0;

//		private Vector2 m_LastMousePosition;
//		private Vector2 m_MousePosition;

//		private bool isTouchSupported;

//		[SerializeField]
//		private string m_HorizontalAxis = "Horizontal";

//		/// <summary>
//		/// Name of the vertical axis for movement (if axis events are used).
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Name of the vertical axis for movement (if axis events are used).")]
//		private string m_VerticalAxis = "Vertical";

//		/// <summary>
//		/// Name of the action used to submit.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Name of the action used to submit.")]
//		private string m_SubmitButton = "Submit";

//        [SerializeField]
//        private string m_SubmitSecondaryButton = "SubmitSecondary";

//		/// <summary>
//		/// Name of the action used to cancel.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Name of the action used to cancel.")]
//		private string m_CancelButton = "Cancel";

//		/// <summary>
//		/// Number of selection changes allowed per second when a movement button/axis is held in a direction.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Number of selection changes allowed per second when a movement button/axis is held in a direction.")]
//		private float m_InputActionsPerSecond = 10;

//		/// <summary>
//		/// Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.")]
//		private float m_RepeatDelay = 0.0f;

//		/// <summary>
//		/// Allows the module to control UI input on mobile devices.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Allows the module to control UI input on mobile devices.")]
//		private bool m_AllowActivationOnMobileDevice;

//		/// <summary>
//		/// Allows the mouse to be used to select elements.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Allows the mouse to be used to select elements.")]
//		private bool m_allowMouseInput = true;

//		/// <summary>
//		/// Allows the mouse to be used to select elements if the device also supports touch control.
//		/// </summary>
//		[SerializeField]
//		[Tooltip("Allows the mouse to be used to select elements if the device also supports touch control.")]
//		private bool m_allowMouseInputIfTouchSupported = false;

//		/// <summary>
//		/// Allows the module to control UI input on mobile devices..
//		/// </summary>
//		public bool allowActivationOnMobileDevice {
//			get { return m_AllowActivationOnMobileDevice; }
//			set { m_AllowActivationOnMobileDevice = value; }
//		}

//		// <summary>
//		/// Number of selection changes allowed per second when a movement button/axis is held in a direction.
//		/// </summary>
//		public float inputActionsPerSecond {
//			get { return m_InputActionsPerSecond; }
//			set { m_InputActionsPerSecond = value; }
//		}

//		/// <summary>
//		/// Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.
//		/// </summary>
//		public float repeatDelay {
//			get { return m_RepeatDelay; }
//			set { m_RepeatDelay = value; }
//		}

//		/// <summary>
//		/// Name of the horizontal axis for movement (if axis events are used).
//		/// </summary>
//		public string horizontalAxis {
//			get { return m_HorizontalAxis; }
//			set { m_HorizontalAxis = value; }
//		}

//		/// <summary>
//		/// Name of the vertical axis for movement (if axis events are used).
//		/// </summary>
//		public string verticalAxis {
//			get { return m_VerticalAxis; }
//			set { m_VerticalAxis = value; }
//		}

//		/// <summary>
//		/// Name of the action used to submit.
//		/// </summary>
//		public string submitButton {
//			get { return m_SubmitButton; }
//			set { m_SubmitButton = value; }
//		}

//		/// <summary>
//		/// Name of the action used to cancel.
//		/// </summary>
//		public string cancelButton {
//			get { return m_CancelButton; }
//			set { m_CancelButton = value; }
//		}

//		/// <summary>
//		/// Allows the mouse to be used to select elements.
//		/// </summary>
//		public bool allowMouseInput {
//			get { return m_allowMouseInput; }
//			set { m_allowMouseInput = value; }
//		}

//		/// <summary>
//		/// Allows the mouse to be used to select elements if the device also supports touch control.
//		/// </summary>
//		public bool allowMouseInputIfTouchSupported {
//			get { return m_allowMouseInputIfTouchSupported; }
//			set { m_allowMouseInputIfTouchSupported = value; }
//		}

//		private bool isMouseSupported {
//			get {
//				if(!m_allowMouseInput) return false;
//				return isTouchSupported ? m_allowMouseInputIfTouchSupported : true;
//			}
//		}

//		// Constructor

//		protected GameInputModule() { }

//		// Methods

//		protected override void Awake() {
//			base.Awake();

//			// Determine if touch is supported
//			isTouchSupported = UnityEngine.Input.touchSupported;

//			// Initialize Rewired
//			InitializeRewired();
//		}

//		public override void UpdateModule() {
//			CheckEditorRecompile();
//			if(recompiling) return;
//			if(!ReInput.isReady) return;

//			if(isMouseSupported) {
//				m_LastMousePosition = m_MousePosition;
//				m_MousePosition = ReInput.controllers.Mouse.screenPosition;
//			}
//		}

//		public override bool IsModuleSupported() {
//			// Check for mouse presence instead of whether touch is supported,
//			// as you can connect mouse to a tablet and in that case we'd want
//			// to use StandaloneInputModule for non-touch input events.
//			if(Application.isMobilePlatform) return m_AllowActivationOnMobileDevice || UnityEngine.Input.mousePresent;
//			return true;
//		}

//		public override bool ShouldActivateModule() {
//			if(!base.ShouldActivateModule())
//				return false;
//			if(recompiling) return false;
//			if(!ReInput.isReady) return false;

//			bool shouldActivate = false;

//			// Combine input for all players
//			for(int i = 0; i < playerIds.Length; i++) {
//				Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
//				if(player == null) continue;

//				shouldActivate |= player.GetButtonDown(m_SubmitButton);
//                shouldActivate |= player.GetButtonDown(m_SubmitSecondaryButton);
//                shouldActivate |= player.GetButtonDown(m_CancelButton);
//				if(moveOneElementPerAxisPress) { // axis press moves only to the next UI element with each press
//					shouldActivate |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
//					shouldActivate |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
//				} else { // default behavior - axis press scrolls quickly through UI elements
//					shouldActivate |= !Mathf.Approximately(player.GetAxisRaw(m_HorizontalAxis), 0.0f);
//					shouldActivate |= !Mathf.Approximately(player.GetAxisRaw(m_VerticalAxis), 0.0f);
//				}
//			}

//			if(isMouseSupported) {
//				shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
//				shouldActivate |= ReInput.controllers.Mouse.GetButtonDown(0);
//			}

//			return shouldActivate;
//		}

//		public override void ActivateModule() {
//			base.ActivateModule();

//			if(isMouseSupported) {
//				m_MousePosition = UnityEngine.Input.mousePosition;
//				m_LastMousePosition = UnityEngine.Input.mousePosition;
//			}

//			var toSelect = eventSystem.currentSelectedGameObject;
//			if(toSelect == null)
//				toSelect = eventSystem.firstSelectedGameObject;

//			eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
//		}

//		public override void DeactivateModule() {
//			base.DeactivateModule();
//			ClearSelection();
//		}

//		public override void Process() {
//			if(!ReInput.isReady) return;

//			bool usedEvent = SendUpdateEventToSelectedObject();

//			if(eventSystem.sendNavigationEvents) {
//				if(!usedEvent)
//					usedEvent |= SendMoveEventToSelectedObject();

//				if(!usedEvent)
//					SendSubmitEventToSelectedObject();
//			}

//			if(isMouseSupported) {
//				ProcessMouseEvent();
//			}
//		}

//		/// <summary>
//		/// Process submit keys.
//		/// </summary>
//		private bool SendSubmitEventToSelectedObject() {
//			if(eventSystem.currentSelectedGameObject == null)
//				return false;
//			if(recompiling) return false;

//			var data = GetBaseEventData();
//			for(int i = 0; i < playerIds.Length; i++) {
//				Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
//				if(player == null) continue;

//				if(player.GetButtonDown(m_SubmitButton)) {
//					ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
//					break;
//				}

//                if (player.GetButtonDown(m_SubmitSecondaryButton)) {
//                    ExecuteEvents.Execute<UI.ISubmitSecondaryHandler>(eventSystem.currentSelectedGameObject, data, UI.SubmitSecondaryEvent.Execute);
//                    break;
//                }
//			}
//			return data.used;
//		}

//		private Vector2 GetRawMoveVector() {
//			if(recompiling) return Vector2.zero;

//			Vector2 move = Vector2.zero;
//			bool horizButton = false;
//			bool vertButton = false;

//			// Combine inputs of all Players
//			for(int i = 0; i < playerIds.Length; i++) {
//				Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
//				if(player == null) continue;

//				if(moveOneElementPerAxisPress) { // axis press moves only to the next UI element with each press
//					float x = 0.0f;
//					if(player.GetButtonDown(m_HorizontalAxis)) x = 1.0f;
//					else if(player.GetNegativeButtonDown(m_HorizontalAxis)) x = -1.0f;

//					float y = 0.0f;
//					if(player.GetButtonDown(m_VerticalAxis)) y = 1.0f;
//					else if(player.GetNegativeButtonDown(m_VerticalAxis)) y = -1.0f;

//					move.x += x;
//					move.y += y;

//				} else { // default behavior - axis press scrolls quickly through UI elements
//					move.x += player.GetAxisRaw(m_HorizontalAxis);
//					move.y += player.GetAxisRaw(m_VerticalAxis);
//				}


//				horizButton |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
//				vertButton |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
//			}

//			if(horizButton) {
//				if(move.x < 0)
//					move.x = -1f;
//				if(move.x > 0)
//					move.x = 1f;
//			}
//			if(vertButton) {
//				if(move.y < 0)
//					move.y = -1f;
//				if(move.y > 0)
//					move.y = 1f;
//			}
//			return move;
//		}

//		/// <summary>
//		/// Process keyboard events.
//		/// </summary>
//		private bool SendMoveEventToSelectedObject() {
//			if(recompiling) return false; // never allow movement while recompiling

//			float time = Time.unscaledTime; // get the current time

//			// Check for zero movement and clear
//			Vector2 movement = GetRawMoveVector();
//			if(Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f)) {
//				m_ConsecutiveMoveCount = 0;
//				return false;
//			}

//			// Check if movement is in the same direction as previously
//			bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);

//			// Check if a button/key/axis was just pressed this frame
//			bool buttonJustPressed = CheckButtonOrKeyMovement(time);

//			// If user just pressed button/key/axis, always allow movement
//			bool allow = buttonJustPressed;
//			if(!allow) {

//				// Apply repeat delay and input actions per second limits

//				if(m_RepeatDelay > 0.0f) { // apply repeat delay
//					// Otherwise, user held down key or axis.
//					// If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
//					if(similarDir && m_ConsecutiveMoveCount == 1) { // this is the 2nd tick after the initial that activated the movement in this direction
//						allow = (time > m_PrevActionTime + m_RepeatDelay);
//						// If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
//					} else {
//						allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // apply input actions per second limit
//					}

//				} else { // not using a repeat delay
//					allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // apply input actions per second limit
//				}
//			}
//			if(!allow) return false; // movement not allowed, done

//			// Get the axis move event
//			var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
//			if(axisEventData.moveDir == MoveDirection.None) return false; // input vector was not enough to move this cycle, done

//			// Execute the move
//			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);

//			// Update records and counters
//			if(!similarDir) m_ConsecutiveMoveCount = 0;
//			m_ConsecutiveMoveCount++;
//			m_PrevActionTime = time;
//			m_LastMoveVector = movement;

//			return axisEventData.used;
//		}

//		private bool CheckButtonOrKeyMovement(float time) {
//			bool allow = false;

//			for(int i = 0; i < playerIds.Length; i++) {
//				Rewired.Player player = ReInput.players.GetPlayer(playerIds[i]);
//				if(player == null) continue;

//				allow |= player.GetButtonDown(m_HorizontalAxis) || player.GetNegativeButtonDown(m_HorizontalAxis);
//				allow |= player.GetButtonDown(m_VerticalAxis) || player.GetNegativeButtonDown(m_VerticalAxis);
//			}

//			return allow;
//		}

//		/// <summary>
//		/// Process all mouse events.
//		/// </summary>
//		private void ProcessMouseEvent() {
//            if (!UnityEngine.Cursor.visible)
//                return;

//			// Breaking change to UnityEngine.EventSystems.PointerInputModule.GetMousePointerEventData() in Unity 5.1.2p1. This code cannot compile in these patch releases because no defines exist for patch releases
//			#if !UNITY_5 || (UNITY_5 && (UNITY_5_0 || UNITY_5_1_0 || UNITY_5_1_1 || UNITY_5_1_2))
//			var mouseData = GetMousePointerEventData();
//			#else
//			var mouseData = GetMousePointerEventData(kMouseLeftId);
//			#endif

//			var pressed = mouseData.AnyPressesThisFrame();
//			var released = mouseData.AnyReleasesThisFrame();

//			var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

//			if(!UseMouse(pressed, released, leftButtonData.buttonData))
//				return;

//			// Process the first mouse button fully
//			ProcessMousePress(leftButtonData);
//			ProcessMove(leftButtonData.buttonData);
//			ProcessDrag(leftButtonData.buttonData);

//			// Now process right / middle clicks
//			ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
//			ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
//			ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
//			ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

//			if(!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f)) {
//				var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
//				ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
//			}
//		}

//		private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData) {
//			if(pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling())
//				return true;

//			return false;
//		}

//		private bool SendUpdateEventToSelectedObject() {
//			if(eventSystem.currentSelectedGameObject == null)
//				return false;

//			var data = GetBaseEventData();
//			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
//			return data.used;
//		}

//		/// <summary>
//		/// Process the current mouse press.
//		/// </summary>
//		private void ProcessMousePress(MouseButtonEventData data) {
//			var pointerEvent = data.buttonData;
//			var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

//			// PointerDown notification
//			if(data.PressedThisFrame()) {
//				pointerEvent.eligibleForClick = true;
//				pointerEvent.delta = Vector2.zero;
//				pointerEvent.dragging = false;
//				pointerEvent.useDragThreshold = true;
//				pointerEvent.pressPosition = pointerEvent.position;
//				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

//				DeselectIfSelectionChanged(currentOverGo, pointerEvent);

//				// search for the control that will receive the press
//				// if we can't find a press handler set the press
//				// handler to be what would receive a click.
//				var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

//				// didnt find a press handler... search for a click handler
//				if(newPressed == null)
//					newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

//				// Debug.Log("Pressed: " + newPressed);

//				float time = Time.unscaledTime;

//				if(newPressed == pointerEvent.lastPress) {
//					var diffTime = time - pointerEvent.clickTime;
//					if(diffTime < 0.3f)
//						++pointerEvent.clickCount;
//					else
//						pointerEvent.clickCount = 1;

//					pointerEvent.clickTime = time;
//				} else {
//					pointerEvent.clickCount = 1;
//				}

//				pointerEvent.pointerPress = newPressed;
//				pointerEvent.rawPointerPress = currentOverGo;

//				pointerEvent.clickTime = time;

//				// Save the drag handler as well
//				pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

//				if(pointerEvent.pointerDrag != null)
//					ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
//			}

//			// PointerUp notification
//			if(data.ReleasedThisFrame()) {
//				// Debug.Log("Executing pressup on: " + pointer.pointerPress);
//				ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

//				// Debug.Log("KeyCode: " + pointer.eventData.keyCode);

//				// see if we mouse up on the same element that we clicked on...
//				var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

//				// PointerClick and Drop events
//				if(pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
//					ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
//				} else if(pointerEvent.pointerDrag != null) {
//					ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
//				}

//				pointerEvent.eligibleForClick = false;
//				pointerEvent.pointerPress = null;
//				pointerEvent.rawPointerPress = null;

//				if(pointerEvent.pointerDrag != null && pointerEvent.dragging)
//					ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

//				pointerEvent.dragging = false;
//				pointerEvent.pointerDrag = null;

//				// redo pointer enter / exit to refresh state
//				// so that if we moused over somethign that ignored it before
//				// due to having pressed on something else
//				// it now gets it.
//				if(currentOverGo != pointerEvent.pointerEnter) {
//					HandlePointerExitAndEnter(pointerEvent, null);
//					HandlePointerExitAndEnter(pointerEvent, currentOverGo);
//				}
//			}
//		}

//		#region Rewired Methods

//		private void InitializeRewired() {
//			if(!Rewired.ReInput.isReady) {
//				Debug.LogError("Rewired is not initialized! Are you missing a Rewired Input Manager in your scene?");
//				return;
//			}
//			Rewired.ReInput.EditorRecompileEvent += OnEditorRecompile;
//			SetupRewiredVars();
//		}

//		private void SetupRewiredVars() {
//			// Set up Rewired vars

//			// Set up Rewired Players
//			if(useAllRewiredGamePlayers) {
//				IList<Rewired.Player> rwPlayers = useRewiredSystemPlayer ? Rewired.ReInput.players.AllPlayers : Rewired.ReInput.players.Players;
//				playerIds = new int[rwPlayers.Count];
//				for(int i = 0; i < rwPlayers.Count; i++) {
//					playerIds[i] = rwPlayers[i].id;
//				}
//			} else {
//				int rewiredPlayerCount = rewiredPlayerIds.Length + (useRewiredSystemPlayer ? 1 : 0);
//				playerIds = new int[rewiredPlayerCount];
//				for(int i = 0; i < rewiredPlayerIds.Length; i++) {
//					playerIds[i] = Rewired.ReInput.players.GetPlayer(rewiredPlayerIds[i]).id;
//				}
//				if(useRewiredSystemPlayer) playerIds[rewiredPlayerCount - 1] = Rewired.ReInput.players.GetSystemPlayer().id;
//			}
//		}

//		private void CheckEditorRecompile() {
//			if(!recompiling) return;
//			if(!Rewired.ReInput.isReady) return;
//			recompiling = false;
//			InitializeRewired();
//		}

//		private void OnEditorRecompile() {
//			recompiling = true;
//			ClearRewiredVars();
//		}

//		private void ClearRewiredVars() {
//			Array.Clear(playerIds, 0, playerIds.Length);
//		}

//		#endregion
//	}
//}