using Reader.ViewModels;
using System.Collections.ObjectModel;
using System.Text.Json;
using Xceed.Document.NET;
namespace Reader.Views;
using Reader.ViewModels;
using Reader.Models;


public partial class SearchView : ContentPage
{

    public List<string> Pages { get; set; } 

    public SearchView(SearchViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var json = Preferences.Get("SearchData", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            Pages = JsonSerializer.Deserialize<List<string>>(json);
        }

        // Инициализируем ViewModel с переданным путем
        var viewModel = BindingContext as SearchViewModel;

        if (viewModel != null)
        {
            viewModel.InitializeAsync(Pages);

        }

    }
}