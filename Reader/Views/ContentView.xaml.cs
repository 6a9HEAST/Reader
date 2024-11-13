using Reader.ViewModels;

namespace Reader.Views;

public partial class ContentView : ContentPage
{
	public ContentView(ContentViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

    }
}