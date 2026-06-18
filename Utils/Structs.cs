namespace TomcatMono {
	public struct AnchorRect {
		public AnchorRect() {	}
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
		public bool IsEmpty=true;
	}
	public struct BoundsRect {
		public int Left;
		public int Top;
		public int Width;
		public int Height;
		private int left;
		private int top;
		private int width;
		private int height;

		public BoundsRect(int left, int top, int width, int height) : this() {
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
		}
	}

}
