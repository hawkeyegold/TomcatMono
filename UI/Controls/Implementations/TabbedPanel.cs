using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomcatMono.Input;

namespace TomcatMono.UI.Controls.Implementations {
	public class TabbedPanel : Panel {
		private readonly List<Tab> tabs = new List<Tab>();
		private readonly List<Panel> pages = new List<Panel>();
		private int selectedIndex;

		private int tabHeight = 24;
		private int tabSpacing = 2;

		public Tab? SelectedTab {
			get {
				if (selectedIndex < 0 || selectedIndex >= tabs.Count)
					return null;
				return tabs[selectedIndex];
			}
		}
		public Panel? SelectedPage {
			get {
				if (selectedIndex < 0 || selectedIndex >= pages.Count)
					return null;
				return pages[selectedIndex];
			}
		}
		public TabbedPanel(int left, int top, int width, int height)
		: base(left, top, width, height) { }

		public void AddTab(string label, Panel page) {
			Tab tab = new Tab(label, OnTabClicked);

			tab.Parent = this;
			page.Parent = this;

			int newIndex = tabs.Count;
			
			tabs.Add(tab);
			pages.Add(page);

			pages[newIndex].IsVisible = newIndex == selectedIndex ? true : false;
			tabs[newIndex].IsSelected = newIndex == selectedIndex ? true : false;
		}

		private void OnTabClicked(Tab tab) {
			for (int i = 0; i < tabs.Count; i++) {
				if (tabs[i] == tab) {
					selectedIndex = i;
					break;
				}
			}
			UpdateSelection();
		}

		private void UpdateSelection() {
			for (int i = 0; i < tabs.Count; i++) {
				tabs[i].IsSelected = (i == selectedIndex);
				pages[i].IsVisible = (i == selectedIndex);
			}
		}

		public override void PerformLayout() {
			int x = 0;
			int y = 0;

			// Layout tabs
			for (int i = 0; i < tabs.Count; i++) {
				Tab tab = tabs[i];
				int width = tab.MeasureWidth();
				tab.SetBounds(x, y, width, tabHeight);
				x += width + tabSpacing;
			}

			// Layout pages
			int pageLeft = 0;
			int pageTop = tabHeight + tabSpacing;
			int pageWidth = Width;
			int pageHeight = Height - (tabHeight + tabSpacing);

			for (int i = 0; i < pages.Count; i++) {
				pages[i].SetBounds(pageLeft, pageTop, pageWidth, pageHeight);
			}
		}

		public override void Update(GameTime gameTime) {
			if (!visible) { return; }
			for (int i = 0; i < tabs.Count; i++) {
				tabs[i].Update(gameTime);
			}
			pages[selectedIndex].Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (!visible) { return; }
			for (int i = 0; i < tabs.Count; i++) {
				tabs[i].Draw(spriteBatch);
			}
			pages[selectedIndex].Draw(spriteBatch);
		}

		public override bool HandleInput(InputManager input) {
			if (!visible) { return false; }

			// Tabs get first chance
			for (int i = tabs.Count - 1; i >= 0; i--) {
				if (tabs[i].HandleInput(input)) {
					return true;
				}
			}

			// Then active page
			return pages[selectedIndex].HandleInput(input);
		}
	}

	public class TabPage : Panel {
		public TabPage(int left, int top, int width, int height)
		: base(left, top, width, height) { }
	}
}
