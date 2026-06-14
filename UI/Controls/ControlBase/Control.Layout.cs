namespace TomcatMono.UI.Controls {
	public abstract partial class Control {

		public Action<Control, int, int>? OnBeforeLayout;
		public Action<Control, int, int>? OnLayout;
		public Action<Control, int, int>? OnAfterLayout;

		public void PerformLayout(int screenWidth, int screenHeight) {

			OnBeforeLayout?.Invoke(this, screenWidth, screenHeight);

			// LayoutSelf(screenWidth, screenHeight);
			// OnLayout?.Invoke(this, screenWidth, screenHeight);

			for (int i = 0; i < _children.Count; i++)
				_children[i].PerformLayout(screenWidth, screenHeight);

			OnAfterLayout?.Invoke(this, screenWidth, screenHeight);
		}
	}
}
