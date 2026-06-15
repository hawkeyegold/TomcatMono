
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
			bool changed = _left != left || _top != top || _width != width || _height != height;

			_left = left;
			_top = top;
			_width = width;
			_height = height;
			InternalBoundsChanged(changed);
		}

		private void InternalBoundsChanged(bool changed) {
			if (changed) {
				BoundsChanged(); // this is probably not going to be needed most of the time in future implementations, we'll leave it in case we do
				PrivateSetAnchors();
				for (int i = 0; i < Children.Count; i++) {
					_children[i].ApplyAnchoring();
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
			AnchorRect anchorRect = new AnchorRect {
				Left = _left,
				Top = _top,
				Right = _left + _width,
				Bottom = _top + _height
			};
			_anchorRect = anchorRect;	
		}
		private Rectangle ComputeBounds(Rectangle container) {
			// TODO
			return Rectangle.Empty;
		}
		private void SetAnchorRect(Rectangle parentBounds) { } //dont think we need this, but we'll leave the stub for now
		private void ApplyAnchoring() {
			Rectangle parentBounds = _parent!=null?_parent.Bounds:VirtualBounds;
			BoundsRect bounds = new BoundsRect(_left, _top, _width, _height);

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
