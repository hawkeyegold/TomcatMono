using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls {
	public abstract partial class Control {

		#region Members
		private Control? _parent;
		private readonly List<Control> _children;

		public event Action<Control>? BoundsChangedHandler;

		private Rectangle? _virtualBounds;
		private int _left;
		private int _top;
		private int _width;
		private int _height;

		private AnchorRect _anchorRect;
		private AnchorType _anchorType;

		private bool _visible;
		private Color _backgroundColor = Color.Silver;
		private Color _textColor = Color.Black;
		#endregion

		#region Properties
		public Rectangle VirtualBounds {
			get {
				if (_parent != null)
					throw new TomcatUIVirtualBoundsException("VirtualBounds accessed on a child control.");

				if (_virtualBounds == null)
					throw new TomcatUIVirtualBoundsException("VirtualBounds accessed but not set.");

				return _virtualBounds.Value;
			}
		}
		public Color BackgroundColor {
			get => _backgroundColor;
			set => _backgroundColor = value;
		}
		public Color TextColor {
			get => _textColor;
			set => _textColor = value;
		}
		public bool Visible {
			get => _visible;
			set => _visible = value;
		}
		public int Left {
			get => _left;
			set => SetBounds(value, _top, _width, _height);
		}
		public int Top {
			get => _top;
			set => SetBounds(_left, value, _width, _height);
		}
		public int Width {
			get => _width;
			set => SetBounds(_left, _top, value, _height);
		}
		public int Height {
			get => _height;
			set => SetBounds(_left, _top, _width, value);
		}
		public Rectangle Bounds => new Rectangle(_left, _top, _width, _height);
		public virtual Rectangle AbsoluteBounds {
			get {
				if (_parent != null) {
					Rectangle p = _parent.AbsoluteBounds;
					return new Rectangle(p.X + _left, p.Y + _top, _width, _height);
				}
				if (_virtualBounds == null)
					throw new TomcatUIVirtualBoundsException("Root control missing VirtualBounds.");
				Rectangle v = _virtualBounds.Value;
				return new Rectangle(v.X + _left, v.Y + _top, _width, _height);
			}
		}
		public Control? Parent {
			get => _parent;
			set => SetParent(value);
		}
		public IReadOnlyList<Control> Children => _children;
		public AnchorType AnchorType {
			get => _anchorType;
			set {
				AnchorType old = _anchorType;

				// Normalize Center override
				if ((value & AnchorType.Center) != 0)
					_anchorType = AnchorType.Center;
				else
					_anchorType = value;

				bool oldWasCenter = old == AnchorType.Center;
				bool newIsCenter = _anchorType == AnchorType.Center;

				// Only recapture when switching between Center and non-Center modes
				if (oldWasCenter != newIsCenter) {
					// This will fail quietly if no frame exists yet
					PrivateSetAnchors();
				}
			}
		}

		private void BoundsChanged() {
			BoundsChangedHandler?.Invoke(this);
		}
		protected void SetBoundsChangedHandler(Action<Control> handler) {
			BoundsChangedHandler = handler;
		}
		#endregion

		#region Constructor
		protected Control(int left, int top, int width, int height)
				: this(null, left, top, width, height) { }

		protected Control(Rectangle? virtualBounds, int left, int top, int width, int height) {
			_virtualBounds = virtualBounds;
			_anchorType = AnchorType.None;
			_visible = true;
			_children = new List<Control>();
			SetBounds(left, top, width, height, virtualBounds!=null);
		}
		#endregion

		#region Update / Draw / Input
		public virtual void Update(GameTime gameTime) { }

		public virtual void Draw(SpriteBatch spriteBatch) {
			for (int i = 0; i < _children.Count; i++)
				if (_children[i].Visible) {
					_children[i].Draw(spriteBatch);
				}
		}

		public virtual bool HandleInput(InputManager input) {
			for (int i = _children.Count - 1; i >= 0; i--)
				if (_children[i].HandleInput(input))
					return true;

			return false;
		}
		#endregion
	}
}
