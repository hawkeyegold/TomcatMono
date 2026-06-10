using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TomcatMono.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class DropDownList : Control {
		private readonly Texture2D _pixel;
		private readonly SpriteFont _font;

		private List<DropDownItem> _items;
		private int _selectedIndex;
		private bool _expanded;

		private Color _selectedColor = Color.LightGray;
		private Color _borderColor = Color.Black;
		private int _borderThickness = 1;

		// scrollbar support using New drag model for scrollbar
		private Rectangle _scrollBarRect;
		private readonly Guid _scrollbarDragId = Guid.NewGuid();
		private int _dragStartOffset = 0;

		public delegate ref string RefStringAccessor();
		private RefStringAccessor? _boundValue;
		private bool _isBound;

		private Rectangle _popupBounds;

		private int _scrollOffset = 0;
		private int _maxVisibleItems = 10;
		private Point _debugMousePos;
		public Color SelectedColor { get { return _selectedColor; } set { _selectedColor = value; } }
		public int SelectedIndex { get { return _selectedIndex; } }
		public string SelectedText { get { return _items.Count > 0 ? _items[_selectedIndex].Text : string.Empty; } }

		public DropDownList(int x, int y, int width, int height, Texture2D pixel, SpriteFont font)
						: base(x, y, width, height) {
			_pixel = pixel;
			_font = font;

			_items = new List<DropDownItem>();
			_selectedIndex = 0;
			_expanded = false;

			BackgroundColor = Color.White;
		}

		// bind a caller-owned string to this DropDownList
		public void Bind(RefStringAccessor accessor) {
			_boundValue = accessor;
			_isBound = true;
			Select(_boundValue()); // sync UI to data
		}

		public void ClearItems() {
			this.ClearItems(false);
		}

		// hard = true will *destroy* the existing list and invalidate all prior relationships.
		// This includes selection index, popup geometry, and any external references to items.
		// Use hard clear only when you intend to rebuild the list from scratch.
		public void ClearItems(bool hard) {
			switch (hard) {
				case true: { this._items = new List<DropDownItem>(); break; }
				case false: { this._items.Clear(); break; }
			}
			_selectedIndex = 0;
			_expanded = false;
			_scrollOffset = 0;
			RecalculatePopupBounds();
		}

		// add a new item to the dropdown
		public void AddItem(string text) {
			DropDownItem item = new DropDownItem(text);
			item.Width = Width;
			item.Height = Height;
			item.BackgroundColor = Color.White;

			_items.Add(item);

			if (_items.Count == 1) {
				_selectedIndex = 0;
			}

			RecalculatePopupBounds();
		}

		// programmatically select an item by text
		public void Select(string text) {
			for (int i = 0; i < _items.Count; i++) {
				if (_items[i].Text == text) {
					SetSelectedIndex(i);
					return;
				}
			}
		}

		// internal selection logic
		private void SetSelectedIndex(int index) {
			if (_items.Count == 0) { return; }

			// restore old background
			_items[_selectedIndex].BackgroundColor = Color.White;

			_selectedIndex = index;

			// apply selected color if expanded
			if (_expanded) {
				_items[_selectedIndex].BackgroundColor = _selectedColor;
			}

			// update bound value
			if (_isBound) {
				ref string target = ref _boundValue!();
				target = _items[_selectedIndex].Text;
			}
		}

		// expand the dropdown
		private void Expand() {
			_expanded = true;
			if (_items.Count == 0 || _selectedIndex >= _items.Count || _selectedIndex < 0) { return; }
			_items[_selectedIndex].BackgroundColor = _selectedColor;
		}

		// collapse the dropdown
		private void Collapse() {
			_expanded = false;
			if (_items.Count == 0 || _selectedIndex >= _items.Count || _selectedIndex < 0) { return; }
			_items[_selectedIndex].BackgroundColor = Color.White;
		}

		// recalc popup bounds based on item count (absolute position, capped height)
		private void RecalculatePopupBounds() {
			Rectangle rec = AbsoluteBounds;
			int itemHeight = Height;
			int visibleCount = _items.Count < _maxVisibleItems ? _items.Count : _maxVisibleItems;
			int popupHeight = visibleCount * itemHeight;
			_popupBounds = new Rectangle(rec.X, rec.Y + rec.Height, rec.Width, popupHeight);
			RecalculateScrollBarRect();
		}
		private void RecalculateScrollBarRect() {
			if (_items.Count > _maxVisibleItems) {
				int barWidth = 10;
				int barX = _popupBounds.Right - barWidth;
				int barY = _popupBounds.Y;
				int barHeight = _popupBounds.Height;
				_scrollBarRect = new Rectangle(barX, barY, barWidth, barHeight);
			}
			else {
				_scrollBarRect = Rectangle.Empty;
			}
		}

		public override bool HandleInput(InputManager input) {
			if (!visible) { return false; }

			_debugMousePos = input.Position;
			Point pos = input.Position;

			// collapsed click toggles expansion
			if (!_expanded) {
				if (AbsoluteBounds.Contains(pos) && input.LeftPressed() && (!input.IsDragging)) {
					RecalculatePopupBounds();
					_scrollOffset = 0;
					Expand();
					return true;
				}
				return false;
			}

			// expanded: click outside collapses
			if (!_popupBounds.Contains(pos)) {
				if (input.LeftPressed() && !(input.IsDragging)) {
					Collapse();
					return true;
				}
			}

			// scroll wheel
			int wheel = input.ScrollDelta;
			if (wheel != 0 && _items.Count > _maxVisibleItems) {
				if (wheel > 0 && _scrollOffset > 0) { _scrollOffset--; }
				if (wheel < 0 && _scrollOffset < _items.Count - _maxVisibleItems) { _scrollOffset++; }
				return true;
			}

			// scrollbar dragging
			if (_items.Count > _maxVisibleItems) {

				Rectangle barRect = _scrollBarRect;

				int barX = barRect.X;
				int barY = barRect.Y;
				int barWidth = barRect.Width;
				int barHeight = barRect.Height;

				float ratio = (float)_maxVisibleItems / (float)_items.Count;
				int thumbHeight = (int)(barHeight * ratio);
				if (thumbHeight < 8) { thumbHeight = 8; }

				float offsetRatio = (float)_scrollOffset / (float)(_items.Count - _maxVisibleItems);
				int thumbY = barY + (int)((barHeight - thumbHeight) * offsetRatio);

				Rectangle thumbRect = new Rectangle(barX, thumbY, barWidth, thumbHeight);

				// --- STOP DRAGGING FIRST ---
				if (input.LeftReleased() && input.DragOwner == _scrollbarDragId) {
					input.EndDrag(_scrollbarDragId);
					return true;
				}

				// --- START DRAGGING ---
				if (!input.IsDragging && input.LeftPressed() && thumbRect.Contains(pos)) {
					input.BeginDrag(_scrollbarDragId, DraggingState.LeftMouse);
					_dragStartOffset = _scrollOffset;
					return true;
				}

				// --- DRAG MOVEMENT ---
				if (input.IsDragging &&
						input.IsDragOwner(_scrollbarDragId) &&
						input.DragState == DraggingState.LeftMouse) {
					int dy = input.DragDelta.Y;

					float scrollRange = (float)(barHeight - thumbHeight);
					float scrollRatio = dy / scrollRange;

					int newOffset = _dragStartOffset +
													(int)(scrollRatio * (_items.Count - _maxVisibleItems));

					if (newOffset < 0) { newOffset = 0; }
					if (newOffset > _items.Count - _maxVisibleItems) {
						newOffset = _items.Count - _maxVisibleItems;
					}

					_scrollOffset = newOffset;
					return true;
				}
			}

			// absorb clicks on scrollbar track
			if (_items.Count > _maxVisibleItems) {
				if (_scrollBarRect.Contains(pos)) {
					return true;
				}
			}

			// expanded: forward input to visible items
			int itemHeight = Height;
			int start = _scrollOffset;
			int end = _items.Count < _scrollOffset + _maxVisibleItems ? _items.Count : _scrollOffset + _maxVisibleItems;

			for (int i = start; i < end; i++) {
				DropDownItem item = _items[i];
				item.Left = _popupBounds.X;
				item.Top = _popupBounds.Y + ((i - start) * itemHeight);

				if (item.HandleInput(input)) {
					SetSelectedIndex(i);
					Collapse();
					return true;
				}
			}

			return false;
		}


		public override void Draw(SpriteBatch spriteBatch) {
			if (!visible) { return; }

			// collapsed
			DropDownItem drawItem = _items.Count > 0 ? _items[_selectedIndex].Clone() : new DropDownItem("");
			Rectangle rec = AbsoluteBounds;
			drawItem.BackgroundColor = Color.White;
			drawItem.SetBounds(rec.X, rec.Y, rec.Width, rec.Height);
			drawItem.Draw(spriteBatch);

			// border
			DrawBorder(spriteBatch, rec, _borderThickness, _borderColor);

			// arrow
			int arrowAreaWidth = 24;
			int arrowX = rec.Right - arrowAreaWidth + 4;
			int arrowY = rec.Y + ((rec.Height - FontLibrary.Symbols.LineSpacing) / 2) + 2;
			string arrowGlyph = _expanded ? ((char)9650).ToString() : ((char)9660).ToString();
			spriteBatch.DrawString(FontLibrary.Symbols, arrowGlyph, new Vector2(arrowX, arrowY), _borderColor);

			// expanded popup
			if (_expanded) {
				Rectangle absPopup = _popupBounds;
				Color scrollBarColor = new Color(0, 0, 0, 80);
				Color scrollThumbColor = new Color(0, 0, 0, 160);

				// clipping
				Rectangle oldScissor = spriteBatch.GraphicsDevice.ScissorRectangle;
				spriteBatch.End();
				RasterizerState rs = new RasterizerState() { ScissorTestEnable = true };
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, rs);
				spriteBatch.GraphicsDevice.ScissorRectangle = absPopup;

				// background + border
				spriteBatch.Draw(_pixel, absPopup, Color.White);
				DrawBorder(spriteBatch, absPopup, _borderThickness, _borderColor);

				// visible items
				int itemHeight = Height;
				int start = _scrollOffset;
				int end = _items.Count < _scrollOffset + _maxVisibleItems ? _items.Count : _scrollOffset + _maxVisibleItems;

				for (int i = start; i < end; i++) {
					DropDownItem item = _items[i];
					item.SetBounds(absPopup.X, absPopup.Y + ((i - start) * itemHeight), absPopup.Width, itemHeight);
					item.Draw(spriteBatch);
				}

				// scrollbar (pseudo)
				if (_items.Count > _maxVisibleItems) {
					int barX = _scrollBarRect.X;
					int barY = _scrollBarRect.Y;
					int barWidth = _scrollBarRect.Width;
					int barHeight = _scrollBarRect.Height; 
					_scrollBarRect = new Rectangle(barX, barY, barWidth, barHeight);

					// draw track
					spriteBatch.Draw(_pixel, new Rectangle(barX, barY, barWidth, barHeight), scrollBarColor);

					// thumb height proportional to visible range
					float ratio = (float)_maxVisibleItems / (float)_items.Count;
					int thumbHeight = (int)(barHeight * ratio);
					if (thumbHeight < 8) { thumbHeight = 8; }

					// thumb position proportional to scroll offset
					float offsetRatio = (float)_scrollOffset / (float)(_items.Count - _maxVisibleItems);
					int thumbY = barY + (int)((barHeight - thumbHeight) * offsetRatio);

					// draw thumb
					spriteBatch.Draw(_pixel, new Rectangle(barX, thumbY, barWidth, thumbHeight), scrollThumbColor);
				}

				spriteBatch.Draw(_pixel, new Rectangle(_debugMousePos.X, _debugMousePos.Y, 2, 2), Color.Magenta);

				// restore
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.GraphicsDevice.ScissorRectangle = oldScissor;
			}
		}

		private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color) {
			// top
			spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
			// bottom
			spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
			// left
			spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
			// right
			spriteBatch.Draw(_pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
		}
	}
}
