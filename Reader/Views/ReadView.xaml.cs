using FFImageLoading.Maui;
using  Reader.ViewModels;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace Reader.Views;

[QueryProperty(nameof(Path), "path")]
[QueryProperty(nameof(Name), "name")]
public partial class ReadView : ContentPage
{  
    public string Path { get; set; }
    public string Name { get; set; }
    public ReadView(ReadViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(Path))
        {
            // Инициализируем ViewModel с переданным путем
            var viewModel = BindingContext as ReadViewModel;

            if (viewModel != null)
            {
              viewModel.InitializeAsync(Path,Name);

            }
        }
    }
    private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        var viewModel = BindingContext as ReadViewModel;
        if (viewModel != null)
        {
            await viewModel.OnWebViewReadyAsync(webView);
        }
    }

}