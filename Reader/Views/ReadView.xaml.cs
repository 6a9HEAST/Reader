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
              await viewModel.InitializeAsync(Path,Name);

            }
        }
    }
    

}