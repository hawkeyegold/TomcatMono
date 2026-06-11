using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls {
	public abstract class Control {

		#region Members

		// --- Core geometry (LOCAL to parent) ---
		private int _left;
		private int _top;
		private int _width;
		private int _height;

		private bool _visible;
		private Control? _parent;
		private readonly List<Control> _children;

		private Color _backgroundColor = Color.SlateGray;

		#endregion

		#region Properties

		public Color BackgroundColor {
			get { return _backgroundColor; }
			set { _backgroundColor = value; }
		}

		public bool Visible {
			get { return _visible; }
			set { _visible = value; }
		}

		public int Left {
			get { return _left; }
			set { _left = value; }
		}

		public int Top {
			get { return _top; }
			set { _top = value; }
		}

		public int Width {
			get { return _width; }
			set { _width = value; }
		}

		public int Height {
			get { return _height; }
			set { _height = value; }
		}

		public Rectangle Bounds {
			get { return new Rectangle(_left, _top, _width, _height); }
		}

		public virtual Rectangle AbsoluteBounds {
			get {
				if (_parent == null) {
					return new Rectangle(_left, _top, _width, _height);
				}

				Rectangle p = _parent.AbsoluteBounds;
				return new Rectangle(
						p.X + _left,
						p.Y + _top,
						_width,
						_height
				);
			}
		}
		public Control? Parent {
			get { return _parent; }
			set { SetParent(value); }
		}

		public IReadOnlyList<Control> Children {
			get { return _children; }
		}

		#endregion

		#region Constructor
		protected Control(int left, int top, int width, int height) {
			_left = left;
			_top = top;
			_width = width;
			_height = height;

			_visible = true;
			_children = new List<Control>();
		}
		#endregion

		#region Update / Draw / Input
		public virtual void Update(GameTime gameTime) { }
		public virtual void Draw(SpriteBatch spriteBatch) {
			for (int i = 0; i < _children.Count; i++) {
				_children[i].Draw(spriteBatch);
			}
		}
		public virtual bool HandleInput(InputManager input) {
			for (int i = _children.Count - 1; i >= 0; i--) {
				if (_children[i].HandleInput(input)) {
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Bounds
		public virtual void SetBounds(int left, int top, int width, int height) {
			_left = left;
			_top = top;
			_width = width;
			_height = height;
		}
		#endregion

		private void SetParent(Control? value) {
			if (_parent == value) { return; }
			if (value == this) { throw new ControlHierarchyException("Control cannot be its own parent."); }
			Control? parent = value;
			while (parent != null) {
				if (parent == this) {
					throw new ControlHierarchyException("Recursive parent chain detected.");
				}
				parent = parent.Parent;
			}
			if (_parent != null) { _parent.RemoveChild(this); }
			_parent = value;
			if (_parent != null) { _parent.AddChild(this); }
		}
		private void AddChild(Control? candidate) {
			if (candidate == null) { return; }
			// Prevent duplicates
			if (_children.Contains(candidate)) { return; }
			// Ensure the child is not already parented elsewhere
			if (candidate._parent != null && candidate._parent != this) {
				candidate._parent.RemoveChild(candidate);
			}
			_children.Add(candidate);
		}
		private void RemoveChild(Control? child) {
			if (child == null) { return; }
			if (_children.Contains(child)) { _children.Remove(child);	}
		}
		
		#region Layout
		public Action<Control, int, int>? OnBeforeLayout;
		public Action<Control, int, int>? OnLayout;
		public Action<Control, int, int>? OnAfterLayout;

		public void PerformLayout(int screenWidth, int screenHeight) {
			// --- 1. Before layout hook ---
			if (OnBeforeLayout != null) {	OnBeforeLayout(this, screenWidth, screenHeight); }

			// --- 2. Layout logic for this control ---
			LayoutSelf(screenWidth, screenHeight);

			// --- 3. Layout hook (during layout) ---
			if (OnLayout != null) { OnLayout(this, screenWidth, screenHeight); }

			// --- 4. Recurse into children ---
			for (int i = 0; i < _children.Count; i++) {	_children[i].PerformLayout(screenWidth, screenHeight); }

			// --- 5. After layout hook ---
			if (OnAfterLayout != null) { OnAfterLayout(this, screenWidth, screenHeight); }
		}
		protected virtual void LayoutSelf(int screenWidth, int screenHeight) {
			// Default: do nothing
			// Derived controls override this to implement their own layout logic
		}
		#endregion
	}
	public sealed class ControlHierarchyException : Exception {
		public ControlHierarchyException(string message) : base(message) { }
	}
}
