using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Graphics;

namespace TomcatMono.UI.Controls.Implementations {
	public class DrawPanel : Panel {

		private readonly Texture2D _pixel;
		public DrawPanel() : this(PixelLibrary.WhitePixel) { }
		public DrawPanel(Texture2D pixel) : this(0, 0, 50, 50, pixel) { }
		public DrawPanel(int left, int top, int width, int height, Texture2D pixel)
				: base(left, top, width, height) {
			_pixel = pixel;
		}

		// NOTE:
		// No Draw override.
		// No layout override.
		// DrawPanel is now a semantic Panel that *can* be used
		// by derived classes to draw backgrounds or other visuals.
	}
}
