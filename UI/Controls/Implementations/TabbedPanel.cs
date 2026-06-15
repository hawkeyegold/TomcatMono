using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class TabbedPanel : Panel {

		// --- Fields ---
		private readonly List<TabPage> _tabs;
		private DrawPanel _tabsPanel;
		private DrawPanel _contentPanel;
		private DrawPanel? _footerPanel;
		private int _footerHeight;
		private int _selectedIndex;
		private int _tabHeight;
		private int _tabSpacing;
		private int _tabPadding;
		private SpriteFont _font;
		private Texture2D _pixel;

		private Color _selectedTabColor = Color.SteelBlue;
		private Color _unselectedTabColor = Color.DarkSlateGray;

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
		public int TabHeight {
			get => _tabHeight;
			set {
				if (value < 0) value = 0;
				_tabHeight = value;

				// Re-layout internal panels
				_tabsPanel.Height = _tabHeight;
				ApplyContentArea();
			}
		}
		public int TabSpacing { get { return _tabSpacing; } set { _tabSpacing = value; } }
		public TabbedPanel(int left, int top, int width, int height)
				: base(left, top, width, height) {
			_tabs = new List<TabPage>();

			_selectedIndex = -1;

			_tabHeight = 24;
			_tabSpacing = 2;
			_tabPadding = 8;

			_font = FontLibrary.NormalFont;
			_pixel = PixelLibrary.WhitePixel;

			//
			// NEW STRUCTURE — NON-DESTRUCTIVE
			//

			// --- Tabs Panel ---
			_tabsPanel = new DrawPanel(0, 0, width, _tabHeight, _pixel);
			_tabsPanel.Parent = this;
			_tabsPanel.AnchorType = AnchorType.Top | AnchorType.Left | AnchorType.Right;

			// --- Content Panel ---
			_contentPanel = new DrawPanel(0, _tabHeight + _tabSpacing, width, height - (_tabHeight + _tabSpacing), _pixel);
			_contentPanel.Parent = this;
			_contentPanel.AnchorType = AnchorType.Top | AnchorType.Bottom | AnchorType.Left | AnchorType.Right;

			// FooterPanel is created later via SetFooter()
		}
		public TabPage AddTab(string text) {
			// 1. Create the TabPage and get its DrawPanel page
			TabPage tab = new TabPage(text);
			DrawPanel page = tab.Page;

			// Parent the page correctly
			page.Parent = _contentPanel;

			// 2. Compute tab width cleanly
			int width;
			if (tab.AutoSize) {
				int measured = MeasureTabText(text);
				width = measured + (_tabPadding * 2);
			}
			else {
				width = tab.FixedWidth;
			}

			tab.DynamicWidth = width;

			// 3. Compute the new tab's Left by rolling up existing widths
			int newTabLeft = GetTabsTotalWidth();

			// 4. Create the tab header Label
			Label header = new Label(
					newTabLeft,
					_tabSpacing,
					width,
					_tabHeight,
					_font,
					text
			);
			header.Parent = _tabsPanel;
			header.AnchorType = AnchorType.Top | AnchorType.Left;
			header.Alignment = TextAlignment.Center;

			// Store the label on the TabPage
			tab.HeaderLabel = header;

			// 5. Add tab to list
			_tabs.Add(tab);

			// 6. Layout the page inside the content area
			Rectangle content = ComputeContentArea();
			page.SetBounds(content.X, content.Y, content.Width, content.Height);

			// 7. Only select the first tab — never override existing selection
			if (_tabs.Count == 1) {
				SetSelectedIndex(0);
			}

			return tab;
		}
		public void SetFooter(DrawPanel footer) {
			_footerPanel = footer;
			_footerHeight = footer.Height;
			footer.Parent = this;
			ApplyContentArea();
		}
		private int MeasureTabText(string text) {
			Vector2 size = _font.MeasureString(text);
			return (int)size.X;
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
		private Rectangle ComputeContentArea() {
			int left = 0;
			int top = _tabHeight + _tabSpacing;

			int width = Width;
			int height = Height - top;

			if (_footerPanel != null) {
				height -= _footerHeight;
			}

			if (height < 0) { height = 0; }

			return new Rectangle(left, top, width, height);
		}
		private void ApplyContentArea() {
			Rectangle area = ComputeContentArea();

			// Resize all pages
			for (int i = 0; i < _tabs.Count; i++) {
				DrawPanel panel = _tabs[i].Page;
				panel.SetBounds(area.X, area.Y, area.Width, area.Height);
			}

			// Resize footer
			if (_footerPanel != null) {
				_footerPanel.SetBounds(
						0,
						Height - _footerHeight,
						Width,
						_footerHeight
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
			if (!Visible)
				return;

			// 1. Draw the full TabbedPanel background (entire control area-tabsheader)
			Rectangle fullBounds = AbsoluteBounds;
			Rectangle drawBounds= new Rectangle(
				fullBounds.Left, 
				fullBounds.Top+_tabHeight, 
				fullBounds.Width, 
				fullBounds.Height
			);
			spriteBatch.Draw(_pixel, drawBounds, BackgroundColor);

			// 2. Draw tab headers (background + text)
			for (int i = 0; i < _tabs.Count; i++) {
				TabPage tab = _tabs[i];
				Label header = tab.HeaderLabel;

				Rectangle rect = header.AbsoluteBounds;

				Color bg = (i == _selectedIndex)
						? _selectedTabColor
						: _unselectedTabColor;

				spriteBatch.Draw(_pixel, rect, bg);
				header.Draw(spriteBatch);
			}

			// 3. Draw the content area background
			//Rectangle contentArea = ComputeContentArea();
			//spriteBatch.Draw(_pixel, contentArea, BackgroundColor);

			// 4. Draw the selected page (on top of content background)
			if (_selectedIndex >= 0 && _selectedIndex < _tabs.Count) {
				_tabs[_selectedIndex].Page.Draw(spriteBatch);
			}
		}
		public override bool HandleInput(InputManager input) {
			if (!Visible)
				return false;

			// Tab header clicks
			for (int i = 0; i < _tabs.Count; i++) {
				Label header = _tabs[i].HeaderLabel;
				if (input.LeftClicked(header.AbsoluteBounds)) {
					SetSelectedIndex(i);
					return true;
				}
			}

			// Selected page input
			if (_selectedIndex >= 0 && _selectedIndex < _tabs.Count) {
				if (_tabs[_selectedIndex].Page.HandleInput(input))
					return true;
			}

			// Footer input
			if (_footerPanel != null && _footerPanel.HandleInput(input))
				return true;

			return false;
		}
		private int GetTabsTotalWidth() {
			int result = 0;

			for (int i = 0; i < _tabs.Count; i++) {
				result += _tabs[i].DynamicWidth;

				if (i < _tabs.Count - 1) {
					result += _tabSpacing;
				}
			}

			return result;
		}
	}
}
