using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SGXDataBuilderGui
{
    /// <summary>
    /// Interaction logic for SoundEntryEditWindow.xaml
    /// </summary>
    public partial class SoundEntryEditWindow : Window
    {
        public SoundEntry _entry { get; set; }
        public SoundEntryEditWindow(SoundEntry entry)
        {
            _entry = entry;
            InitializeComponent();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            _entry.Name = tb_Name.Text;
            _entry.SGXDWave.Name.Name = _entry.Name;

            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
