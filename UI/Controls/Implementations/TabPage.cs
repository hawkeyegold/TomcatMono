using TomcatMono.Graphics;
using TomcatMono.UI.Controls;
using TomcatMono.UI.Controls.Implementations;

public sealed class TabPage {
	private string _text;
	private bool _autoSize;
	private int _dynamicWidth;
	private int _fixedWidth;
	private DrawPanel _page;
	private DrawPanel _tabHandle;
	private bool _selected;
	//private Label _headerLabel;

	public Label HeaderLabel { get; set; } = null!;
	public string Text { get { return _text; } set { _text = value; } }
	public bool AutoSize { get { return _autoSize; } set { _autoSize = value; } }
	public int FixedWidth { get { return _fixedWidth; } set { _fixedWidth = value; } }
	public int DynamicWidth { get { return _dynamicWidth; } internal set { _dynamicWidth = value; } }

	public DrawPanel Page { get { return _page; } set { _page = value; } }
	public DrawPanel TabHandle { get { return _tabHandle; } set { _tabHandle = value; } }
	public bool Selected { get { return _selected; } internal set { _selected = value; } }
	public TabPage(string text) : this(text, null, null) { }
	public TabPage(string text, DrawPanel? pagePanel, DrawPanel? handlePanel) {
		_text = text;
		_autoSize = true;
		_dynamicWidth = 0;
		_fixedWidth = 50;
		_selected = false;
		_page = (pagePanel != null) ? pagePanel : new DrawPanel();
		_tabHandle = (handlePanel != null) ? handlePanel : new DrawPanel();
	}
}