using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls {
	public abstract class Control {
		// --- Core geometry (LOCAL to parent) ---
		protected int _left;
		protected int _top;
		protected int _width;
		protected int _height;

		protected bool visible;
		private Control? parent;
		protected List<Control> children;
		
		private Color _backgroundColor = Color.SlateGray;
		public Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

		protected Control(int left, int top, int width, int height) {
			_left = left;
			_top = top;
			_width = width;
			_height = height;

			visible = true;
			children = new List<Control>();
		}

		// --- Local geometry ---
		public int Left { get => _left; set => _left = value; }
		public int Top { get => _top; set => _top = value; }
		public int Width { get => _width; set => _width = value; }
		public int Height { get => _height; set => _height = value; }

		public Rectangle Bounds {
			get => new Rectangle(_left, _top, _width, _height);
		}

		// --- Absolute geometry (LOCAL + parent chain) ---
		public virtual Rectangle AbsoluteBounds {
			get {
				if (parent == null) {
					return new Rectangle(_left, _top, _width, _height);
				}

				Rectangle p = parent.AbsoluteBounds;
				return new Rectangle(
						p.X + _left,
						p.Y + _top,
						_width,
						_height
				);
			}
		}

		// --- Parent property with recursion protection ---
		public Control? Parent {
			get => parent;
			set {
				if (parent == value) { return; }
				if (value == this) {
					throw new ControlHierarchyException("Control cannot be its own parent.");
				}

				Control? cursor = value;
				while (cursor != null) {
					if (cursor == this) {
						throw new ControlHierarchyException("Recursive parent chain detected.");
					}
					cursor = cursor.parent;
				}

				if (parent != null) {
					parent.children.Remove(this);
				}

				parent = value;

				if (parent != null) {
					parent.children.Add(this);
				}
			}
		}

		public IReadOnlyList<Control> Children => children;
		public bool IsVisible { get => visible; set => visible = value; }

		// --- Update / Draw / Input ---
		public virtual void Update(GameTime gameTime) { }

		public virtual void Draw(SpriteBatch spriteBatch) {
			//if (GameMain.DebugDrawBounds) {
			//	spriteBatch.Draw(
			//			PixelLibrary.WhitePixel,
			//			AbsoluteBounds,
			//			Color.Red * 0.35f
			//	);
			//}

			for (int i = 0; i < children.Count; i++) {
				children[i].Draw(spriteBatch);
			}
		}

		public virtual bool HandleInput(InputManager input) {
			for (int i = children.Count - 1; i >= 0; i--) {
				if (children[i].HandleInput(input)) {
					return true;
				}
			}
			return false;
		}

		// --- Convenience setter ---
		public virtual void SetBounds(int left, int top, int width, int height) {
			_left = left;
			_top = top;
			_width = width;
			_height = height;
		}
	}

	public sealed class ControlHierarchyException : Exception {
		public ControlHierarchyException(string message) : base(message) { }
	}
}
