using Reader.ViewModels;
namespace Reader.Views;

public partial class SearchView : ContentPage
{
	public SearchView(SearchViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}