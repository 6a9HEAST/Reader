using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Services
{
    public class SharedDataService : INotifyPropertyChanged
    {
        private int currentPageIndex;

        public int CurrentPageIndex
        {
            get => currentPageIndex;
            set
            {
                if (currentPageIndex != value)
                {
                    currentPageIndex = value;
                    OnPropertyChanged(nameof(CurrentPageIndex));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
