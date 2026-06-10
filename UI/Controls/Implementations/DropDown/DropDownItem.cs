using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class DropDownItem : Control {
		private readonly SpriteFont _font;
		private readonly Texture2D _pixel;

		private string _text;
		private bool _hovered;
		private Color _textColor=Color.Black;
		private Color _hoverTextColor = Color.DarkSlateBlue;
		private Color _hoverBackgroundColor = Color.SkyBlue;

		public string Text { get { return _text; } set { _text = value; } }
		public bool Hovered { get { return _hovered; } set { _hovered = value; } }
		public Color TextColor { get => _textColor; set => _textColor = value; }
		public Color HoverTextColor { get => _hoverTextColor; set => _hoverTextColor = value; }
		public Color HoverBackgroundColor { get => _hoverBackgroundColor; set => _hoverBackgroundColor = value; }

		public DropDownItem(string text)
				: base(0, 0, 0, 0) {
			_pixel = PixelLibrary.WhitePixel;
			_font = FontLibrary.ListFont;
			_text = text;
			base.BackgroundColor = Color.White;
		}
		public DropDownItem Clone() {
			DropDownItem clone = new DropDownItem(_text);
			clone.BackgroundColor = BackgroundColor;
			clone.HoverBackgroundColor = HoverBackgroundColor;
			clone.TextColor = TextColor;
			clone.HoverTextColor = HoverTextColor;
			clone.Width = Width;
			clone.Height = Height;
			return clone;
		}
		public override bool HandleInput(InputManager input) {
			if (!visible) { return false; }

			Point pos = input.Position;

			_hovered = AbsoluteBounds.Contains(pos);

			if (_hovered && input.LeftClicked(AbsoluteBounds)) {
				return true; // DropDownList will interpret this as "item clicked"
			}

			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!visible) { return; }

			Rectangle abs = AbsoluteBounds;

			// background and text color with hover state adjustments
			Color bgColor = _hovered ? _hoverBackgroundColor : base.BackgroundColor;
			Color txtColor = _hovered ? _hoverTextColor : _textColor;

			spriteBatch.Draw(_pixel, abs, bgColor);

			// text (centered vertically, clipped by width naturally)
			int nudgeUp = 2;
			int padLeft = 10;
			float textY = (abs.Y + (abs.Height - _font.LineSpacing) / 2f)- nudgeUp;
			Vector2 pos = new Vector2(abs.X + padLeft, textY);

			spriteBatch.DrawString(_font, _text, pos, txtColor);
		}

	}
}
