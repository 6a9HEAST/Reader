using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reader.Models;
using Reader.Services;

namespace Reader.ViewModels
{
    public class ReadViewModel : BaseViewModel
    {
        private bool _isOverlayVisible;

        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set => SetProperty(ref _isOverlayVisible, value);
        }

        public ICommand ShowOverlayCommand { get; }
        public ICommand HideOverlayCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand OpenSearchCommand { get; }
        public ICommand OpenContentCommand { get; }

        protected readonly IDataStore<Book> DataStore;

        public ReadViewModel(IDataStore<Book> dataStore) : base(dataStore)
        {
            IsOverlayVisible = false;
            DataStore = dataStore;
            ShowOverlayCommand = new Command(ShowOverlay);
            HideOverlayCommand = new Command(HideOverlay);
            
            GoBackCommand = new Command(GoBack);
            OpenSearchCommand = new Command(OpenSearch);
            OpenContentCommand= new Command(OpenContent);
        }

        private void ShowOverlay()
        {
            IsOverlayVisible = true;
        }

        private void HideOverlay()
        {
            IsOverlayVisible = false;
        }

        private async void GoBack()
        {
            await Shell.Current.GoToAsync("..");
            IsOverlayVisible = false;
        }

        private async void OpenSearch()
        {
            await Shell.Current.GoToAsync("SearchView");
        }
        private async void OpenContent()
        {
            await Shell.Current.GoToAsync("ContentView");
        }

    }
}
