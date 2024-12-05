using Reader.ViewModels;
using System.Collections.ObjectModel;
using Reader.Models;
using System.Text.Json;
using static Java.Util.Jar.Attributes;

namespace Reader.Views;

[QueryProperty("SerializedData", "data")]
public partial class ContentView : ContentPage
{
    public ObservableCollection<Title> TableOfContents { get; set; } = new ObservableCollection<Title>();

    private string serializedData;
    public string SerializedData
    {
        get => serializedData;
        set
        {
            serializedData = value;
            if (!string.IsNullOrEmpty(serializedData))
            {
                TableOfContents = JsonSerializer.Deserialize<ObservableCollection<Title>>(serializedData);
                OnPropertyChanged(nameof(TableOfContents));
            }
        }
    }

    public ContentView(ContentViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
            // Инициализируем ViewModel с переданным путем
            var viewModel = BindingContext as ContentViewModel;

            if (viewModel != null)
            {
                viewModel.InitializeAsync(TableOfContents);

            }
        
    }

    
}