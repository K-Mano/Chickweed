using System.Windows;

namespace Chickweed
{

    public enum SaveMode
    {
        SAVE,
        OVERWRITE
    }

    /// <summary>
    /// SaveWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SaveWindow : Window
    {
        public SaveWindow(string Title, SaveMode mode)
        {
            InitializeComponent();
            this.Title = Title;
            CurrentSaveMode = mode;
        }

        SaveMode CurrentSaveMode { set; get; }
        private void SaveDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void foreign_Checked(object sender, RoutedEventArgs e)
        {
            foreigntext.Visibility = Visibility.Visible;
        }

        private void foreign_Unchecked(object sender, RoutedEventArgs e)
        {
            foreigntext.Visibility = Visibility.Collapsed;
        }
    }
}
