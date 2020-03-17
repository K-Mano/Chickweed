using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Chickweed
{

    public enum SaveOptions
    {
        SAVE = 0,
        OVERWRITE = 1,
        CANCEL = 2,
        EXIT = 3
    }

    public static class IconHelper
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                   int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
                   IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;

        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |
                  SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

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

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
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
            MainWindow main = new MainWindow();
            main.info.StudentNumber = snum.Text;
            main.info.StudentName = sname.Text;
            main.info.CreatorName = name.Text;

        }
    }
}
