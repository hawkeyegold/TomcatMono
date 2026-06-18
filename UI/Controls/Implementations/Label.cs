using System;
using TomcatMono.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class Label : Control {
		private readonly SpriteFont _font;
		private float _scale = 1f;
		private const int MaxLabelLength = 64;
		private readonly char[] _textBuffer = new char[MaxLabelLength];
		private string _cachedText="";
		private bool _autoSize;
		private Vector2 _measuredSize;

		public float Scale {
			get => _scale;
			set => _scale = value;
		}
		public string Text {
			get => _cachedText;
			set {
				if (value == _cachedText) { return; }
				if (value == null)
					value = string.Empty;

				// normalize to single line
				value = value.Replace("\r", "").Replace("\n", "");

				// clamp, copy into buffer
				int len = Math.Min(value.Length, MaxLabelLength);
				for (int i = 0; i < len; i++)
					_textBuffer[i] = value[i];

				// zero out the rest
				for (int i = len; i < MaxLabelLength; i++)
					_textBuffer[i] = '\0';

				// update cached string
				_cachedText = new string(_textBuffer, 0, len);

				// update measured size
				_measuredSize = _font.MeasureString(_cachedText);

				// autosize if needed
				if (AutoSize)
					ResizeToFitText();
			}
		}


		public bool AutoSize {
			get => _autoSize;
			set {
				_autoSize = value;
				if (_autoSize)
					ResizeToFitText();
			}
		}
		public TextAlignment Alignment { get; set; } = TextAlignment.Center;

		public Label(int left, int top, SpriteFont font, string text)
				: base(left, top, 0, 0) {
			if (font == null) { throw new ArgumentNullException(nameof(font)); }
			_font = font;
			Text = text;
			AutoSize = true;
			ResizeToFitText();
		}
		public Label(int left, int top, int width, int height, SpriteFont font, string text)
				: base(left, top, width, height) {
			if (font == null) { throw new ArgumentNullException(nameof(font)); }
			_font = font;
			Text = text;
		}
		public void ResizeToFitText() {
			if (!AutoSize) { return; }// Do nothing if autosize is off

			int w = (int)System.Math.Max(0, _measuredSize.X);
			int h = (int)System.Math.Max(0, _measuredSize.Y);
			SetBounds(Left, Top, w, h);
		}
		public override void Update(GameTime gameTime) {
			if (AutoSize) {
				int w = (int)_measuredSize.X;
				int h = (int)_measuredSize.Y;

				// If external code changed the size, user is taking control
				if (Width != w || Height != h) {
					AutoSize = false;
				}
			}
			base.Update(gameTime);
		}

		public override bool HandleInput(InputManager input) {
			return false;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) return;

			string text = _cachedText;
			Rectangle abs = AbsoluteBounds;

			// Measure at scale 1.0
			Vector2 baseSize = AutoSize ? _measuredSize : _font.MeasureString(text);
			Vector2 scaledSize = baseSize * _scale;

			float x, y;

			switch (Alignment) {
				case TextAlignment.TopLeft:
					x = abs.X;
					y = abs.Y;
					break;

				case TextAlignment.Center:
				default:
					x = abs.X + (abs.Width - scaledSize.X) / 2f;
					y = abs.Y + (abs.Height - scaledSize.Y) / 2f;
					break;
			}

			x = (float)Math.Round(x);
			y = (float)Math.Round(y);

			Vector2 pos = new Vector2(x, y);

			spriteBatch.DrawString(_font,text,pos,TextColor,0f,Vector2.Zero,_scale,SpriteEffects.None,0f);
		}
	}
}
