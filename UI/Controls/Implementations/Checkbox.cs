using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class Checkbox : Control {
		public delegate ref bool RefBoolAccessor();
		private RefBoolAccessor? _boundValue;

		private readonly Texture2D _pixel;
		private readonly SpriteFont _font;
		private readonly string _labelText;

		private CheckOreintation _orientation = CheckOreintation.LeftCheck;
		private bool _checked;
		private int _boxSize;
		private int _spacing;

		private bool _isBound;

		public Color CheckedColor = Color.SteelBlue;
		public Color UncheckedColor = Color.White;

		public CheckOreintation Orientation {
			get => _orientation;
			set => _orientation = value;
		}

		public bool Checked {
			get => _checked;
			set => _checked = value;
		}

		public int BoxSize {
			get => _boxSize;
			set {
				_boxSize = value;
				SetControlSize();
			}
		}

		public int Spacing {
			get => _spacing;
			set {
				_spacing = value;
				SetControlSize();
			}
		}

		public Checkbox(int left, int top, int boxSize, Texture2D pixel, SpriteFont font, string label)
				: base(left, top, boxSize, boxSize) {
			_spacing = 4;
			_pixel = pixel;
			_font = font;
			_labelText = label;
			_boxSize = boxSize;
			SetControlSize();
		}

		public void Bind(RefBoolAccessor accessor) {
			_boundValue = accessor;
			_isBound = true;
			_checked = _boundValue(); // sync UI to data
		}

		private void SetControlSize() {
			int labelWidth = (int)_font.MeasureString(_labelText).X;
			Width = _boxSize + _spacing + labelWidth; // calls Control.SetBounds
			Height = _boxSize;                         // calls Control.SetBounds
		}

		public override bool HandleInput(InputManager input) {
			if (!Visible) return false;

			Point pos = input.Position;

			if (AbsoluteBounds.Contains(pos) && input.LeftClicked(AbsoluteBounds)) {
				_checked = !_checked;

				if (_isBound) {
					ref bool target = ref _boundValue!();
					target = _checked;
				}

				return true;
			}
			return false;
		}

		public override void Update(GameTime gameTime) {
			if (_isBound) {
				bool bound = _boundValue!();
				if (bound != _checked) {
					_checked = bound;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) return;

			Rectangle abs = AbsoluteBounds;

			Rectangle box;
			Vector2 labelPos;

			switch (_orientation) {
				default:
					labelPos = new Vector2(abs.X + _boxSize + _spacing, abs.Y);
					box = new Rectangle(abs.X, abs.Y, _boxSize, _boxSize);
					break;

				case CheckOreintation.RightCheck:
					labelPos = new Vector2(abs.X, abs.Y);
					box = new Rectangle(abs.Right - _boxSize, abs.Y, _boxSize, _boxSize);
					break;
			}

			spriteBatch.Draw(_pixel, box, UncheckedColor);

			if (_checked) {
				int innerCheckMargin = 3;
				Rectangle inner = new Rectangle(
						box.X + innerCheckMargin,
						box.Y + innerCheckMargin,
						box.Width - innerCheckMargin * 2,
						box.Height - innerCheckMargin * 2
				);
				spriteBatch.Draw(_pixel, inner, CheckedColor);
			}

			spriteBatch.DrawString(_font, _labelText, labelPos, TextColor);
		}
	}

	public enum CheckOreintation {
		LeftCheck,
		RightCheck
	}
}
