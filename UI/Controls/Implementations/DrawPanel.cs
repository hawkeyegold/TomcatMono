using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomcatMono.UI.Controls.Implementations {
	public class DrawPanel : Panel {
		private readonly Texture2D _pixel;

		public DrawPanel(int left, int top, int width, int height, Texture2D pixel)
				: base(left, top, width, height) {
			_pixel = pixel;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) return;

			// Use the inherited BackgroundColor from Control
			spriteBatch.Draw(_pixel, AbsoluteBounds, BackgroundColor);

			base.Draw(spriteBatch);
		}
	}
}
