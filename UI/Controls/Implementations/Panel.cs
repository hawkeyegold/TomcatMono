using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TomcatMono.UI.Controls.Implementations {
	public abstract class Panel : Control {
		protected Panel(int left, int top, int width, int height)
		: base(left, top, width, height) { }

		// Called when Panel becomes the topmost Panel in the PanelStack.
		public virtual void OnActivated() { }

		// Called when Panel is no longer the topmost Panel.
		public virtual void OnDeactivated() { }

		// Called when Panel is added to the PanelStack.
		public virtual void OnAdded() { }

		// Called when Panel is removed from the PanelStack.
		public virtual void OnRemoved() { }

		// Layout hook for dynamic resizing or repositioning.
		public virtual void PerformLayout() { }

		// Focus/capture hooks for future keyboard or mouse capture systems.
		public virtual void OnFocusGained() { }
		public virtual void OnFocusLost() { }
	}
}
