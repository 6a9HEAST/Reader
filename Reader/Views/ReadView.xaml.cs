using FFImageLoading.Maui;
using  Reader.ViewModels;
namespace Reader.Views;

public partial class ReadView : ContentPage
{
	public ReadView(ReadViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

    }
}