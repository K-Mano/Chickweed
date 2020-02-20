using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Forms;

namespace ITToolKit_3
{
    /// <summary>
    /// プラグインで実装するインターフェイス
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// プラグインの名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        string Version { get; }

        /// <summary>
        /// プラグインの説明
        /// </summary>
        string Description { get; }

        /// <summary>
        /// プラグインのホスト
        /// </summary>
        IPluginHost Host { get; set; }

        /// <summary>
        /// プラグインを実行する
        /// </summary>
        void Run();
    }

    /// <summary>
    /// プラグインのホストで実装するインターフェイス
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// ホストのメインフォーム
        /// </summary>
        Form MainForm { get; }

        /// <summary>
        /// ホストのRichTextBoxコントロール
        /// </summary>
        System.Windows.Controls.TabControl MainTabControl { get; }
    }

    public partial class MainWindow : Window
    {
        /// <summary>
        /// MainWindow.xaml の相互作用ロジック
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// MainWindowのウィンドウハンドル取得
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                var helper = new System.Windows.Interop.WindowInteropHelper(this);
                return helper.Handle;
            }
        }
        /// <summary>
        /// 各クラスの初期化
        /// </summary>
        SaveWindow saveWindow = new SaveWindow();

        NetworkAdapter network = new NetworkAdapter();
        GetRegistryKeys reg = new GetRegistryKeys();

        int result = 0;

        /// <summary>
        /// データの取得処理
        /// </summary>
        private void Setup()
        {
            int errorcount;
            do
            {
                errorcount = 0;
                try
                {
                    NetworkInterface adapter = network.SearchAdapterTypeFromString(NetworkInterfaceType.Wireless80211, "Wi-Fi");
                    adaptername.Text = network.GetAdapterName(adapter);
                    vendorname.Text = network.GetAdapterVendor(adapter);
                    phynumber.Text = network.GetMacAddressFromAdapter(adapter);

                    string version_id = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
                    string major_version = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();
                    string minor_version = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "").ToString();

                    windows.Text = reg.GetOSFullName();
                    version.Text = "バージョン " + version_id + " (OSビルド " + major_version + "." + minor_version + ")";

                    maker.Text = reg.GetHardwareVendorName();
                    sysname.Text = reg.GetHardwareModelName();
                }
                catch (Exception)
                {
                    errorcount = 1;
                }

                if (errorcount != 0)
                {
                    using (TaskDialog errordialog = new TaskDialog())
                    {
                        result = 0;

                        errordialog.Caption = "ITToolKit";
                        errordialog.InstructionText = "一部の情報を取得できませんでした";
                        errordialog.Text = "評価に必要な情報が不足しています。タスクを選択してください。";
                        errordialog.Icon = TaskDialogStandardIcon.Error;
                        errordialog.OwnerWindowHandle = Handle;

                        var retry = new TaskDialogCommandLink("retry", "再度評価を実施する(&R)\n一時的な問題はこれらによって解決する可能性があります。");
                        retry.Default = true;
                        retry.Click += (sender, e) =>
                        {
                            result = 0;
                            errordialog.Close();
                        };
                        errordialog.Controls.Add(retry);

                        var cancel = new TaskDialogCommandLink("cancel", "評価を終了する(&C)");
                        cancel.Click += (sender, e) =>
                        {
                            result = 1;
                            errordialog.Close();
                        };
                        errordialog.Controls.Add(cancel);

                        errordialog.Show();
                    }
                }
            } while (errorcount != 0);
        }
        private void disp_Loaded(object sender, RoutedEventArgs e)
        {
            Setup();
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            Setup();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            using (TaskDialog savedialog = new TaskDialog())
            {
                result = 0;

                savedialog.Caption = "ITToolKit";
                savedialog.InstructionText = "評価の結果に対するタスクを選択してください。";
                savedialog.Cancelable = true;
                savedialog.OwnerWindowHandle = Handle;

                var savenormal = new TaskDialogCommandLink("savenormal", "既定のファイルに保存(&E)\n既定のファイルに追記します。通常はこれを選択してください。");
                savenormal.Default = true;
                savenormal.Click += (sender, e) =>
                {
                    result = 0;
                    savedialog.Close();
                };
                savedialog.Controls.Add(savenormal);

                var savenew = new TaskDialogCommandLink("savenew", "名前を付けて保存(&A)\n別名で保存します。上書きの場合は元のデータが消去されます。");
                savenew.Click += (sender, e) =>
                {
                    result = 1;
                    savedialog.Close();
                };

                savedialog.Controls.Add(savenew);

                var exit = new TaskDialogCommandLink("exit", "キャンセル(&C)");
                exit.Click += (sender, e) =>
                {
                    result = 2;
                    savedialog.Close();
                };
                savedialog.Controls.Add(exit);

                savedialog.Show();
            }
            switch (result)
            {
                case 0:
                    saveWindow.ShowDialog();
                    break;
                case 1:
                    saveWindow.ShowDialog();
                    break;
                case 2:
                    break;
            }
        }
        private void disp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (TaskDialog savedialog = new TaskDialog())
            {
                result = 0;

                savedialog.Caption = "ITToolKit";
                savedialog.InstructionText = "評価の結果に対するタスクを選択してください。";
                savedialog.Cancelable = true;
                savedialog.OwnerWindowHandle = Handle;

                var savenormal = new TaskDialogCommandLink("savenormal", "既定のファイルに保存(&E)\n既定のファイルに追記します。通常はこれを選択してください。");
                savenormal.Default = true;
                savenormal.Click += (sender, e) =>
                {
                    result = 1;
                    savedialog.Close();
                };
                savedialog.Controls.Add(savenormal);

                var savenew = new TaskDialogCommandLink("savenew", "名前を付けて保存(&A)\n別名で保存します。上書きの場合は元のデータが消去されます。");
                savenew.Click += (sender, e) =>
                {
                    result = 2;
                    savedialog.Close();
                };

                savedialog.Controls.Add(savenew);

                var exit = new TaskDialogCommandLink("exit", "保存せずに終了(&X)");
                exit.Click += (sender, e) =>
                {
                    result = 3;
                    savedialog.Close();
                };
                savedialog.Controls.Add(exit);

                savedialog.Show();
            }
            switch (result)
            {
                case 0:
                    e.Cancel = true;
                    break;
                case 1:
                    saveWindow.ShowDialog();
                    break;
                case 2:
                    saveWindow.ShowDialog();
                    break;
                case 3:
                    break;
            }
        }
        private void GoToProxySetting_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:network-proxy");
        }

        private void GoToUpdateSetting_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:windowsupdate");
        }
    }
}
