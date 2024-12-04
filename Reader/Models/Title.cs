using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Reader.Models
{
    
    public class Title: INotifyPropertyChanged
    {
        //Класс для заголовков в оглавлении
        public string Name { get; set; }
        public int PageNumber { get; set; }
        private bool isExpanded;
        public ObservableCollection<Title> SubItems { get; set; } = new ObservableCollection<Title>();

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
