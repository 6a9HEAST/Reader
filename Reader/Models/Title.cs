using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Reader.Models
{
    
    public class Title: INotifyPropertyChanged
    {
        //Класс для заголовков в оглавлении
        public string Name { get; set; }
        public int PageNumber { get; set; }
        private bool _isExpanded;
        public ObservableCollection<Title> SubItems { get; set; } = new ObservableCollection<Title>();

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }
        public bool HasSubItems => SubItems != null && SubItems.Count > 0;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
