using System.Windows;

namespace Chickweed
{

    public enum SaveOptions
    {
        SAVE,
        OVERWRITE
    }

    public struct StudentCredential
    {

    }

    /// <summary>
    /// SaveWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SaveWindow : Window
    {
        public SaveWindow(string Title, SaveOptions mode)
        {
            InitializeComponent();
            this.Title = Title;
            CurrentSaveMode = mode;
        }

        SaveOptions CurrentSaveMode { set; get; }

        private void foreign_Checked(object sender, RoutedEventArgs e)
        {
            foreigntext.Visibility = Visibility.Visible;
            snum.MaxLength = 2;
        }

        private void foreign_Unchecked(object sender, RoutedEventArgs e)
        {
            foreigntext.Visibility = Visibility.Collapsed;
            snum.MaxLength = 7;
        }

        private void auth_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
