using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TomcatMono.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public sealed class Tab : Control {
		private readonly string _label;
		private readonly Action<Tab> _onClick;

		private readonly SpriteFont _font;
		private readonly Texture2D _pixel;

		private bool _isSelected;

		private const int _horizontalPadding = 16;

		Color SelectedColor = Color.SteelBlue;

		public bool IsSelected {
			get { return _isSelected; }
			set { _isSelected = value; }
		}

		public Tab(string label, Action<Tab> onClick)	: base(0, 0, 0, 0) {
			_label = label;
			_onClick = onClick;
			_font = FontLibrary.NormalFont;
			_pixel = PixelLibrary.WhitePixel;
		}

		public int MeasureWidth() {
			Vector2 size = _font.MeasureString(_label);
			return (int)size.X + _horizontalPadding * 2;
		}

		public override bool HandleInput(InputManager input) {
			if (!visible) { return false; }

			Rectangle abs = AbsoluteBounds;
			Point pos = input.Position;

			if (abs.Contains(pos) && input.LeftClicked(AbsoluteBounds)) {
				_onClick?.Invoke(this);
				return true;
			}

			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!visible) { return; }

			Rectangle abs = AbsoluteBounds;

			Color bg = _isSelected ? SelectedColor
					: BackgroundColor;

			spriteBatch.Draw(_pixel, abs, bg);

			Vector2 size = _font.MeasureString(_label);
			Vector2 pos = new Vector2(
					abs.X + (abs.Width - size.X) / 2,
					abs.Y + (abs.Height - size.Y) / 2
			);

			spriteBatch.DrawString(_font, _label, pos, Color.White);

			base.Draw(spriteBatch);
		}
	}
}
