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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using Microsoft.Win32;

using SGXDataBuilder;
using System.ComponentModel;

namespace SGXDataBuilderGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Sgxd _sgxd { get; set; } = new Sgxd();
        public bool SplitBody { get => _sgxd.SplitBody; set { _sgxd.SplitBody = value; OnPropertyChanged(nameof(SplitBody)); } }
        public string Label { get => _sgxd.Label; set { _sgxd.Label = value; OnPropertyChanged(nameof(Label)); } }

        public ObservableCollection<SoundEntry> Entries { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    // Quickly check the file, not the most efficient
                    try
                    {
                        HandleNewFile(file);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Not a project or audio source file", "Could not load XML file",
                           MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

            }
        }
            
        private void MenuItem_ExportSGD_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string outputName = !string.IsNullOrEmpty(Label) ? Label : "myfile";
            if (_sgxd.SplitBody)
            {
                saveFileDialog.FileName = $"{outputName}.sgh";
                saveFileDialog.Filter = "Sony SGX Synthetizer Sound Header + Bank (*.sgh/sgb)|*.sgh;";
            }
            else
            {
                saveFileDialog.FileName = $"{outputName}.sgd";
                saveFileDialog.Filter = "Sony SGX Synthetizer Sound File (*.sgd)|*.sgd;";
            }

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    this._sgxd.Build(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export to SGD: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"SGD saved as: {saveFileDialog.FileName}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void mi_RemoveSound_Click(object sender, RoutedEventArgs e)
        {
            for (int i1 = lv_Sounds.SelectedItems.Count - 1; i1 >= 0; i1--)
            {
                object? i = lv_Sounds.SelectedItems[i1];
                SoundEntry item = i as SoundEntry;
                if (item is null)
                    return;

                _sgxd.RemoveWave(item.SGXDWave);
                Entries.Remove(item);
            }

        }

        private void MenuItem_ImportSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.CheckFileExists = true;
            openFile.Filter = "Waveform Files (*.wav)|*.wav;|Dolby Digital AC3 Files (*.ac3)|*.ac3;|Sony Adaptive Transform Acoustic Coding Files (*.at3)|*.at3;";

            if (openFile.ShowDialog() == true)
            {
                HandleNewFile(openFile.FileName);
            }
        }

        private void MenuItem_SaveProject_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON SGXD Builder Project (*.sgxdproj)|*.sgxdproj;";

            if (!string.IsNullOrEmpty(_sgxd.Label))
                saveFileDialog.FileName = $"{_sgxd.Label}.sgxdproj";
            else
                saveFileDialog.FileName = $"my_project.sgxdproj";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _sgxd.ExportAsProject(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save the project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Project saved as: {saveFileDialog.FileName}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItem_LoadProject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.CheckFileExists = true;
            openFile.Filter = "JSON SGXD Builder Project (*.sgxdproj)|*.sgxdproj;";

            if (openFile.ShowDialog() == true)
            {
                try
                {
                    HandleImportProject(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the project: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void HandleNewFile(string file)
        {
            if (file.EndsWith(".sgxdproj"))
            {
                HandleImportProject(file);
            }
            else
            {
                SgxdWave wave = _sgxd.AddNewFile(file, System.IO.Path.GetFileNameWithoutExtension(file));
                var soundEntry = new SoundEntry()
                {
                    Path = System.IO.Path.GetFullPath(file),
                    Name = wave.Name.Name,
                    Length = wave.GetLength(),
                    SGXDWave = wave
                };

                Entries.Add(soundEntry);
            }

            MenuItem_SaveProject.IsEnabled = Entries.Count > 0;
        }

        private void HandleImportProject(string fileName)
        {
            _sgxd.ImportFromProject(fileName);

            Entries.Clear();

            foreach (var entry in _sgxd.WaveHeader.Waves)
            {
                var soundEntry = new SoundEntry()
                {
                    Path = entry.FullPath,
                    Name = entry.Name.Name,
                    SGXDWave = entry
                };

                Entries.Add(soundEntry);
            }

            SplitBody = _sgxd.SplitBody;
            Label = _sgxd.Label;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void lv_Sounds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_Sounds.SelectedItem is null)
                return;

            var window = new SoundEntryEditWindow(lv_Sounds.SelectedItem as SoundEntry);
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
