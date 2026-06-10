using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomcatMono.Graphics {
	public static class PixelLibrary {
		private static GraphicsDevice? _device = null;

		public static Texture2D WhitePixel => CreateWhitePixel();

		public static void Initialize(GraphicsDevice device) {
			_device = device;

			// Eager-load if desired:
			_ = WhitePixel;
		}

		private static Texture2D CreateWhitePixel() {
			if (_device == null) {
				throw new TomcatLibraryException(
						"PixelLibrary must be initialized before first use."
				);
			}

			try {
				var tex = new Texture2D(_device, 1, 1);
				tex.SetData(new[] { Color.White });
				return tex;
			}
			catch (Exception ex) {
				throw new TomcatLibraryException(
						"PixelLibrary failed to create the 1x1 white pixel texture.",
						ex
				);
			}
		}
	}
}
