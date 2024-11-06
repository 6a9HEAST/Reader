using Reader.ViewModels;

namespace Reader.Views
{
    public partial class MainView : ContentPage
    {
        int count = 0;

        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

        }
    }

}
