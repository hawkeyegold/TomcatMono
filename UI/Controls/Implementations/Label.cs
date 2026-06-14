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
				// NO geometry mutation here
			}
		}

		public bool AutoSize { get; set; } = false;

		public Label(int left, int top, int width, int height, SpriteFont font, string text)
				: base(left, top, width, height) {
			_font = font;
			_text = text;
		}

		public void ResizeToFitText() {
			int textWidth = (int)_font.MeasureString(_text).X;
			int textHeight = _font.LineSpacing;
			SetBounds(Left, Top, textWidth, textHeight);
		}

		public override bool HandleInput(InputManager input) {
			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) return;

			Rectangle abs = AbsoluteBounds;
			Vector2 pos = new Vector2(abs.X, abs.Y);

			spriteBatch.DrawString(_font, _text, pos, Color);
		}
	}

}
