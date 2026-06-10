using TomcatMono.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class Label : Control {
		private readonly SpriteFont _font;
		private string _text;

		public Color Color = Color.White;

		public string Text {
			get { return _text; }
			set {
				_text = value;
				SetControlWidth();
			}
		}

		public Label(int left, int top, int height, SpriteFont font, string text) : base(left, top, 0, height) {
			_font = font;
			_text = text;
			SetControlWidth();
		}

		public override void SetBounds(int left, int top, int width, int height) {
			_left = left;
			_top = top;
			_height = height;
			SetControlWidth();
		}

		private void SetControlWidth() {
			int textWidth = (int)_font.MeasureString(_text).X;
			_width = textWidth;
		}

		public override bool HandleInput(InputManager input) {
			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!visible) { return; }

			Rectangle abs = AbsoluteBounds;
			Vector2 pos = new Vector2(abs.X, abs.Y);

			spriteBatch.DrawString(_font, _text, pos, Color);
		}
	}
}
