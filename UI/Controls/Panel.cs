using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomcatMono.UI.Controls {
	public abstract class Panel : Control {

		protected Panel(int left, int top, int width, int height)
				: base(null, left, top, width, height) { }
		protected Panel(Rectangle? virtualBounds,int left, int top, int width, int height)
				: base(virtualBounds,left, top, width, height) { }

		// Called when Panel becomes the topmost Panel in the PanelStack.
		public virtual void OnActivated() { }

		// Called when Panel is no longer the topmost Panel.
		public virtual void OnDeactivated() { }

		// Called when Panel is added to the PanelStack.
		public virtual void OnAdded() { }

		// Called when Panel is removed from the PanelStack.
		public virtual void OnRemoved() { }

		// Focus/capture hooks for future keyboard or mouse capture systems.
		public virtual void OnFocusGained() { }
		public virtual void OnFocusLost() { }

		// Optional: override if Panel needs layout behavior
		//protected override void LayoutSelf(int screenWidth, int screenHeight) {
		//	// Default: do nothing
		//}
	}
}
