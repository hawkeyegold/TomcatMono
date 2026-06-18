
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace TomcatMono.UI.Controls {
	public abstract partial class Control {

		#region Bounds
		public void SetVirtualBounds(Rectangle? bounds) {
			bool changed = _virtualBounds != bounds;
			_virtualBounds = bounds;
			InternalBoundsChanged(changed);
		}

		public void SetBounds(int left, int top, int width, int height) {
			SetBounds(left, top, width, height, true);
		}
		public void SetBounds(int left, int top, int width, int height, bool setAnchors) {
			bool changed = _left != left || _top != top || _width != width || _height != height;

			_left = left;
			_top = top;
			_width = width;
			_height = height;
			InternalBoundsChanged(changed, setAnchors);
		}
		private void InternalBoundsChanged(bool changed) {
			InternalBoundsChanged(changed, true);
		}
		private void InternalBoundsChanged(bool changed, bool setAnchors) {
			if (changed) {
				BoundsChanged(); // this is probably not going to be needed most of the time in future implementations, we'll leave it in case we do
				if (setAnchors && !_anchorRect.IsEmpty) {
					PrivateSetAnchors();
					for (int i = 0; i < Children.Count; i++) {
						_children[i].ApplyAnchoring();
					}
				}
			}
		}
		#endregion
		// -----------------------------
		// PRIVATE GEOMETRY CORE
		// -----------------------------
		private void PrivateSetBounds(int left, int top, int width, int height) {
			_left= left;
			_top= top;
			_width= width;
			_height= height;
		}
		private void PrivateSetAnchors() {
			if (_parent == null && _virtualBounds == null)
				return; // fail quietly, leave IsEmpty as-is

			Rectangle parentBounds = _parent != null ? _parent.Bounds : VirtualBounds;

			// --- Center anchoring capture ---
			if (_anchorType == AnchorType.Center) {
				int parentCenterX = parentBounds.Width / 2;
				int parentCenterY = parentBounds.Height / 2;

				_anchorRect = new AnchorRect {
					Left = _left - parentCenterX,   // X offset from center
					Top = _top - parentCenterY,   // Y offset from center
					Right = _width,                  // store width
					Bottom = _height,                  // store height
					IsEmpty = false
				};
				return;
			}

			// --- Default edge-based anchoring capture ---
			_anchorRect = new AnchorRect {
				Left = _left,
				Top = _top,
				Right = _left + _width,
				Bottom = _top + _height,
				IsEmpty=false
			};
		}

		private void ApplyAnchoring() {
			if (_anchorRect.IsEmpty || _anchorType == AnchorType.None) { return; }

			Rectangle parentBounds = _parent!=null?_parent.Bounds:VirtualBounds;
			BoundsRect bounds = new BoundsRect(_left, _top, _width, _height);

			if (_anchorType == AnchorType.Center) {
				int parentCenterX = parentBounds.Width / 2;
				int parentCenterY = parentBounds.Height / 2;

				bounds.Left = parentCenterX + _anchorRect.Left;
				bounds.Top = parentCenterY + _anchorRect.Top;

				// Width/Height come from the current bounds (not mutated)
				// because Center anchoring does not stretch.
				PrivateSetBounds(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
				return;
			}

			// --- Horizontal mutation ---
			if (_anchorType.HasFlag(AnchorType.Left) && !_anchorType.HasFlag(AnchorType.Right)) {
				// Left anchored only → no mutation
				bounds.Left = _anchorRect.Left;
			}
			else if (_anchorType.HasFlag(AnchorType.Right) && !_anchorType.HasFlag(AnchorType.Left)) {
				// Right anchored only → shift left so right stays fixed
				bounds.Left = parentBounds.Width - (_anchorRect.Right - _width);
			}
			else if (_anchorType.HasFlag(AnchorType.Left) && _anchorType.HasFlag(AnchorType.Right)) {
				// Stretch width between anchors
				bounds.Left = _anchorRect.Left;
				bounds.Width = parentBounds.Width - (_anchorRect.Left + (parentBounds.Width - _anchorRect.Right));
			}
			// --- Vertical mutation ---
			if (_anchorType.HasFlag(AnchorType.Top) && !_anchorType.HasFlag(AnchorType.Bottom)) {
				// Top anchored only → no mutation
				bounds.Top = _anchorRect.Top;
			}
			else if (_anchorType.HasFlag(AnchorType.Bottom) && !_anchorType.HasFlag(AnchorType.Top)) {
				// Bottom anchored only → shift top so bottom stays fixed
				bounds.Top = parentBounds.Height - (_anchorRect.Bottom - _height);
			}
			else if (_anchorType.HasFlag(AnchorType.Top) && _anchorType.HasFlag(AnchorType.Bottom)) {
				// Stretch height between anchors
				bounds.Top = _anchorRect.Top;
				bounds.Height = parentBounds.Height - (_anchorRect.Top + (parentBounds.Height - _anchorRect.Bottom));
			}

			// --- Commit mutation ---
			PrivateSetBounds(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
		}
	}
}
