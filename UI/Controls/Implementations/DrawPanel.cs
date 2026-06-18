using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Graphics;

namespace TomcatMono.UI.Controls {
	public class DrawPanel : Panel {

		private readonly Texture2D _pixel;
		private bool _drawBackground;
		public DrawPanel() : this(PixelLibrary.WhitePixel) { }
		public DrawPanel(Texture2D pixel) : this(0, 0, 50, 50, pixel) { }
		public DrawPanel(int left, int top, int width, int height, Texture2D pixel)
				: base(left, top, width, height) {
			_pixel = pixel;
			_drawBackground = true;
		}
		public DrawPanel(Rectangle? virtualBounds, int left, int top, int width, int height, Texture2D pixel)
				: base(virtualBounds, left, top, width, height) {
			_pixel = pixel;
			_drawBackground = true;
		}

		public bool DrawBackground { get => _drawBackground; set => _drawBackground = value; }

		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) { return; }
			System.Diagnostics.Debug.WriteLine(
				$"PANEL: abs=({AbsoluteBounds.X},{AbsoluteBounds.Y}) size=({AbsoluteBounds.Width},{AbsoluteBounds.Height})"
			);

			if (_drawBackground) { spriteBatch.Draw(_pixel, AbsoluteBounds, BackgroundColor); }
			base.Draw(spriteBatch);
		}
	}
}
