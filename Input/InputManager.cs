using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TomcatMono.Input {

	public class InputManager {

		private MouseState _currentState;
		private MouseState _previousState;

		// --- Dragging state (new system) ---
		private Guid? _dragOwner = null;
		private Point _dragStart;
		private Point _dragCurrent;
		private DraggingState _dragState = DraggingState.None;

		// --- Explicit public properties ---
		public bool IsDragging { get { return _dragState != DraggingState.None; } }
		public Guid? DragOwner { get { return _dragOwner; } }
		public Point DragStart { get { return _dragStart; } }
		public Point DragCurrent { get { return _dragCurrent; } }
		public Point DragDelta { get { return new Point(_dragCurrent.X - _dragStart.X, _dragCurrent.Y - _dragStart.Y); } }
		public DraggingState DragState { get { return _dragState; } }
		public MouseState CurrentState { get { return _currentState; } }
		public MouseState PreviousState { get { return _previousState; } }
		public Point Position { get { return new Point(_currentState.X, _currentState.Y); } }

		public int ScrollDelta { get { return _currentState.ScrollWheelValue - _previousState.ScrollWheelValue; } }

		public InputManager() {
			_currentState = Mouse.GetState();
			_previousState = _currentState;
			_dragCurrent = Position;
		}
		public bool IsDragOwner(Guid id) { return _dragOwner == id; }
		// --- Explicit drag lifecycle ---
		public void BeginDrag(Guid ownerId, DraggingState state) {
			_dragOwner = ownerId;
			_dragState = state;
			_dragStart = Position;
			_dragCurrent = Position;
		}

		public void EndDrag(Guid ownerId) {
			if (_dragOwner == ownerId) {
				_dragOwner = null;
				_dragState = DraggingState.None;
			}
		}

		// --- Update mouse state ---
		public void Update() {
			_previousState = _currentState;
			_currentState = Mouse.GetState();

			// Update drag position if dragging
			if (_dragState != DraggingState.None) {
				_dragCurrent = Position;
			}
		}

		// --- Raw button edges ---
		public bool LeftPressed() {
			return _currentState.LeftButton == ButtonState.Pressed &&
						 _previousState.LeftButton == ButtonState.Released;
		}

		public bool LeftReleased() {
			return _currentState.LeftButton == ButtonState.Released &&
						 _previousState.LeftButton == ButtonState.Pressed;
		}

		public bool RightPressed() {
			return _currentState.RightButton == ButtonState.Pressed &&
						 _previousState.RightButton == ButtonState.Released;
		}

		public bool RightReleased() {
			return _currentState.RightButton == ButtonState.Released &&
						 _previousState.RightButton == ButtonState.Pressed;
		}

		// --- Click gestures ---
		public bool LeftClicked(Rectangle bounds) {
			bool pressedInside =
					bounds.Contains(_previousState.X, _previousState.Y) &&
					_previousState.LeftButton == ButtonState.Pressed;

			bool releasedInside =
					bounds.Contains(_currentState.X, _currentState.Y) &&
					_currentState.LeftButton == ButtonState.Released;

			return pressedInside && releasedInside;
		}

		public bool RightClicked(Rectangle bounds) {
			bool pressedInside =
					bounds.Contains(_previousState.X, _previousState.Y) &&
					_previousState.RightButton == ButtonState.Pressed;

			bool releasedInside =
					bounds.Contains(_currentState.X, _currentState.Y) &&
					_currentState.RightButton == ButtonState.Released;

			return pressedInside && releasedInside;
		}
	}

	public enum DraggingState {
		None,
		LeftMouse,
		RightMouse
	}
}
