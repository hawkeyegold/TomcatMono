using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class TabbedPanel : Panel {

		// --- Fields ---
		private readonly List<TabPage> _tabs;
		private readonly List<Rectangle> _tabRects;
		private DrawPanel? _footerPanel;
		private int _footerHeight;
		private int _selectedIndex;
		private int _tabHeight;
		private int _tabSpacing;
		private int _tabPadding;
		private SpriteFont _font;
		private Texture2D _pixel;

		// --- Properties ---
		public TabPage? SelectedTab {
			get {
				if (_selectedIndex < 0 || _selectedIndex >= _tabs.Count) { return null; }
				return _tabs[_selectedIndex];
			}
		}
		public DrawPanel? SelectedPage {
			get {
				if (_selectedIndex < 0 || _selectedIndex >= _tabs.Count) { return null; }
				return _tabs[_selectedIndex].Page;
			}
		}
		public DrawPanel? FooterPanel { get { return _footerPanel; } }
		public TabbedPanel(int left, int top, int width, int height)
				: base(left, top, width, height) {

			_tabs = new List<TabPage>();
			_tabRects = new List<Rectangle>();

			_selectedIndex = -1;

			_tabHeight = 24;
			_tabSpacing = 2;
			_tabPadding = 8;

			_font = FontLibrary.NormalFont;
			_pixel = PixelLibrary.WhitePixel;
		}
		public TabPage AddTab(string text) {
			TabPage tab = new TabPage(text);
			DrawPanel page = tab.Page;

			// Parent the page correctly
			page.Parent = this;

			// Compute tab width
			if (tab.AutoSize) {
				int measured = MeasureTabText(text);
				tab.DynamicWidth = measured + (_tabPadding * 2);
			}
			else {
				tab.DynamicWidth = tab.FixedWidth;
			}

			_tabs.Add(tab);
			RecomputeTabRects();

			// Layout the page inside the content area
			Rectangle content = ComputeContentArea();
			page.SetBounds(content.X, content.Y, content.Width, content.Height);

			// First tab is selected, others hidden
			if (_tabs.Count == 1) {
				SetSelectedIndex(0);
			}
			else {
				page.Visible = false;
			}

			return tab;
		}
		public void SetFooter(DrawPanel footer) {
			_footerPanel = footer;
			_footerHeight = footer.Height;
			footer.Parent = this;
			PerformLayout(Width, Height);
		}
		private int MeasureTabText(string text) {
			Vector2 size = _font.MeasureString(text);
			return (int)size.X;
		}
		private void RecomputeTabRects() {
			_tabRects.Clear();
			int x = 0;
			int y = 0;

			for (int i = 0; i < _tabs.Count; i++) {
				TabPage tab = _tabs[i];
				int width = tab.DynamicWidth;

				Rectangle rect = new Rectangle(x, y, width, _tabHeight);
				_tabRects.Add(rect);

				x += width + _tabSpacing;
			}
		}
		private Rectangle ComputeContentArea() {
			int left = 0;
			int top = _tabHeight + _tabSpacing;
			int width = Width;
			int height = Height - top;

			if (height < 0) { height = 0; }

			return new Rectangle(left, top, width, height);
		}
		private void SetSelectedIndex(int index) {
			if (_tabs.Count == 0) {
				_selectedIndex = -1;
				return;
			}

			if (index < 0) { index = 0; }
			if (index >= _tabs.Count) { index = _tabs.Count - 1; }

			_selectedIndex = index;

			for (int i = 0; i < _tabs.Count; i++) {
				TabPage tab = _tabs[i];
				bool isSelected = (i == _selectedIndex);

				tab.Selected = isSelected;
				tab.Page.Visible = isSelected;
			}
		}
		protected override void LayoutSelf(int screenWidth, int screenHeight) {
			RecomputeTabRects();

			int footerH = (_footerPanel != null) ? _footerHeight : 0;

			Rectangle content = new Rectangle(
					0,
					_tabHeight + _tabSpacing,
					Width,
					Height - (_tabHeight + _tabSpacing + footerH)
			);

			for (int i = 0; i < _tabs.Count; i++) {
				_tabs[i].Page.SetBounds(content.X, content.Y, content.Width, content.Height);
			}

			if (_footerPanel != null) {
				_footerPanel.SetBounds(
						0,
						Height - footerH,
						Width,
						footerH
				);
			}
		}
		public override void Update(GameTime gameTime) {
			if (!Visible) { return; }

			if (_selectedIndex >= 0 && _selectedIndex < _tabs.Count) {
				_tabs[_selectedIndex].Page.Update(gameTime);
			}
		}
		public override void Draw(SpriteBatch spriteBatch) {
			if (!Visible) { return; }

			for (int i = 0; i < _tabs.Count; i++) {
				TabPage tab = _tabs[i];
				Rectangle rect = _tabRects[i];

				Color back = tab.Selected ? Color.DimGray : Color.Gray;
				Color textColor = Color.White;

				spriteBatch.Draw(_pixel, rect, back);

				Vector2 size = _font.MeasureString(tab.Text);
				float tx = rect.X + (rect.Width - size.X) * 0.5f;
				float ty = rect.Y + (rect.Height - size.Y) * 0.5f;

				spriteBatch.DrawString(_font, tab.Text, new Vector2(tx, ty), textColor);
			}

			if (_selectedIndex >= 0 && _selectedIndex < _tabs.Count) {
				_tabs[_selectedIndex].Page.Draw(spriteBatch);
			}
			if (_footerPanel != null) {
				_footerPanel.Draw(spriteBatch);
			}
		}
		public override bool HandleInput(InputManager input) {
			if (!Visible) { return false; }

			for (int i = 0; i < _tabs.Count; i++) {
				if (input.LeftClicked(_tabRects[i])) {
					SetSelectedIndex(i);
					return true;
				}
			}

			if (_selectedIndex >= 0 && _selectedIndex < _tabs.Count) {
				return _tabs[_selectedIndex].Page.HandleInput(input);
			}
			
			if (_footerPanel != null && _footerPanel.HandleInput(input)) {
				return true;
			}

			return false;
		}


	}
}
