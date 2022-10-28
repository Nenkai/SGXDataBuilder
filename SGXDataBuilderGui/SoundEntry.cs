using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using SGXDataBuilder;

namespace SGXDataBuilderGui
{
    public class SoundEntry : INotifyPropertyChanged
    {
        public string Path { get; set; }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        public TimeSpan Length { get; set; }
        public SgxdWave SGXDWave { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
