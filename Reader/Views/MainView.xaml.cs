using Reader.ViewModels;

namespace Reader
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(MainViewModel viewModel)
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
