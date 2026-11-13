namespace DemoApp;

public partial class App : Application
{
	private readonly MainPage _mainPage;
	private readonly HybridPage _hybridPage;

	public App(MainPage mainPage, HybridPage hybridPage)
	{
		_mainPage = mainPage;
		_hybridPage = hybridPage;
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var tabbedPage = new TabbedPage { Children = { _mainPage, _hybridPage } };
		return new Window(tabbedPage) { Title = "Maui.Hybrid.YouTube.Embed Demo" };
	}
}
