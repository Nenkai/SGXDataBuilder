//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/SGXDataBuilder
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System.Windows;

namespace SGXDBuilder.GUI
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
