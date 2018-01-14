using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Journal
{
    class MainModel : INotifyPropertyChanged
    {
        public FontFamily CurrentFont { get { return _CurrentFont; } set { if (_CurrentFont == value) return; _CurrentFont = value; OnPropertyChanged(nameof(CurrentFont)); } }
        private FontFamily _CurrentFont = new FontFamily("Comic Sans MS");

        public Brush CurrentColor { get { return _CurrentColor; } set { if (_CurrentColor == value) return; _CurrentColor = value; OnPropertyChanged(nameof(CurrentColor)); } }
        private Brush _CurrentColor = Brushes.Black;

        private void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
