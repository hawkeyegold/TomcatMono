using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TomcatMono.Graphics {
	public static class FontLibrary {
		private static ContentManager? _content=null;
		public static SpriteFont MenuFont => LoadFont("MenuFont");
		public static SpriteFont ListFont => LoadFont("ListFont");
		public static SpriteFont NormalFont => LoadFont("NormalFont");
		public static SpriteFont MediumFont => LoadFont("MediumFont");
		public static SpriteFont DebugFont => LoadFont("NormalFont");
		public static SpriteFont Symbols => LoadFont("Unicode");

		public static void Initialize(ContentManager content) {
			_content = content;
			_ = MenuFont;
			_ = ListFont;
			_ = NormalFont;
			_ = MediumFont;
			_ = DebugFont;
			_ = Symbols;
		}

		private static SpriteFont LoadFont(string assetName) {
			if (_content == null) {
				throw new TomcatLibraryException($"FontLibrary must be initialized before first use.");
			}
			else {
				try {
					return _content.Load<SpriteFont>(assetName);
				}
				catch (ContentLoadException ex) {
					throw new TomcatContentException(
							$"FontLibrary could not load required font asset '{assetName}'. " +
							$"Ensure the .spritefont file exists in the game's Content project.",
							ex
					);
				}
			}
		}
	}
	public class TomcatLibraryException : Exception {
		public TomcatLibraryException(string message)
		: base(message) {
		}
		public TomcatLibraryException(string message, Exception innerException)
		: base(message, innerException) {
		}
	}
	public class TomcatContentException : Exception {
		public TomcatContentException(string message, Exception innerException)
				: base(message, innerException) {
		}
	}
}