using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;
using System;

namespace TomcatMono.UI.Controls.Implementations {
	public class Button : Control {
		private readonly Texture2D _pixel;
		private readonly SpriteFont _font;
		private readonly string _text;

		private bool _hovered;
		private bool _pressed;

		public Color HoverColor = Color.Beige;
		public Color PressedColor = Color.White;
		
		public event Action<Button>? OnClick;

		public Button(
				int left, int top, int width, int height,
				Texture2D pixel, SpriteFont font, string text
		) : base(left, top, width, height) {
			_pixel = pixel;
			_font = font;
			_text = text;
			_hovered= false;
			_pressed= false;
			BackgroundColor = Color.LightSteelBlue;
		}

		public override bool HandleInput(InputManager input) {
			if (!Visible) { return false; }

			Point pos = input.Position;
			_hovered = AbsoluteBounds.Contains(pos);

			// Mouse down inside button
			if (_hovered && input.LeftPressed()) {
				_pressed = true;
			}

			if (_pressed && !AbsoluteBounds.Contains(pos) && !input.LeftMouseDown()) {
				_pressed = false;
			}

			// Mouse released
			if (_pressed && input.LeftClicked(AbsoluteBounds)) {
				_pressed = false;

				if (_hovered) {
					OnClick?.Invoke(this);
					return true;
				}
			}
			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) { return; }

			Color bg = BackgroundColor;
			if (_hovered) { bg = HoverColor; }
			if (_pressed) { bg = PressedColor; }

			// Background
			spriteBatch.Draw(_pixel, AbsoluteBounds, bg);

			// Border
			DrawBorder(spriteBatch);

			// Text centered
			Vector2 size = _font.MeasureString(_text);
			Vector2 pos = new Vector2(
					AbsoluteBounds.X + (AbsoluteBounds.Width - size.X) / 2,
					AbsoluteBounds.Y + (AbsoluteBounds.Height - size.Y) / 2
			);
			spriteBatch.DrawString(_font, _text, pos, TextColor);

			// Draw children (if any)
			base.Draw(spriteBatch);
		}
		private void DrawBorder(SpriteBatch spriteBatch) {
			Rectangle r = AbsoluteBounds;
			int t = 1; // border thickness
			spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y, t, r.Height), Color.DarkGray);
			spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, t), Color.DarkGray);
			spriteBatch.Draw(_pixel, new Rectangle(r.Right - t, r.Y, t, r.Height), TextColor);
			spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Bottom - t, r.Width, t), TextColor);
		}
	}
}
