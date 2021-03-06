﻿using ClosedXML.Excel;
using CredentialManagement;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chickweed
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

    public interface IPluginHost
    {
        /// <summary>
        /// ホストのメインウィンドウ
        /// </summary>
        Window MainWindow { get; }

        /// <summary>
        /// ホストのTabControl
        /// </summary>
        TabControl MainTabControl { get; }
    }
    public struct DeviceInfo
    {
        /// <summary>
        /// 物理アドレス
        /// </summary>
        public int[] PhysicalAddress;

        /// <summary>
        /// 学籍番号
        /// </summary>
        public string StudentNumber;

        /// <summary>
        /// 製造元
        /// </summary>
        public string Vendor;

        /// <summary>
        /// OS名
        /// </summary>
        public string OSName;

        /// <summary>
        /// OSのリリースID(Windows10のみ)
        /// </summary>
        public string ReleaseID;

        /// <summary>
        /// OSのメジャーバージョン
        /// </summary>
        public string MajorVersion;

        /// <summary>
        /// OSのマイナーバージョン
        /// </summary>
        public string MinorVersion;

        /// <summary>
        /// 
        /// </summary>
        public string DeviceName;

        /// <summary>
        /// 学生の名前
        /// </summary>
        public string StudentName;

        /// <summary>
        /// 入力者名
        /// </summary>
        public string CreatorName;
    }

    public enum TaskDialogResult
    {
        CANCEL = 0,
        RETRY  = 1
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

        // MainWindowのウィンドウハンドル取得
        public IntPtr Handle
        {
            get
            {
                var helper = new System.Windows.Interop.WindowInteropHelper(this);
                return helper.Handle;
            }
        }

        public TabControl GetTabControl
        {
            get
            {
                return MainTabControl;
            }
        }

        public struct SupportData
        {
            public bool IsSupportAvailable;
            public string EvaluationText;
            public DateTime WExpireDate;
            public DateTime DExpireDate;
        }

        // 各クラスの初期化

        NetworkAdapter network = new NetworkAdapter();
        HardwareInfo reg = new HardwareInfo();
        Utilities util = new Utilities();

        public DeviceInfo info = new DeviceInfo();

        // データの取得処理

        private void Setup()
        {
            int errorcount;
            DateTime dateTime = DateTime.Now;

            do
            {
                errorcount = 0;
                try
                {
                    appversion.Text = "バージョン " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    info.ReleaseID    = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
                    info.MajorVersion = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();
                    info.MinorVersion = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "").ToString();

                    windows.Text = reg.GetOSFullName();
                    version.Text = "バージョン " + info.ReleaseID + " (OSビルド " + info.MajorVersion + "." + info.MinorVersion + ")";

                    releaseid.Text = info.ReleaseID;

                    SupportData data = JudgeVersion(info.ReleaseID, dateTime);

                    support.Text    = data.WExpireDate.ToString("yyyy年MM月dd日まで");
                    active.Text     = data.DExpireDate.ToString("yyyy年MM月31日まで");
                    evaluation.Text = data.EvaluationText;
                    maker.Text      = reg.GetHardwareVendorName();
                    sysname.Text    = reg.GetHardwareModelName();

                    NetworkInterface adapter = network.SearchAdapterTypeFromString(NetworkInterfaceType.Wireless80211, "Wi-Fi");

                    adaptername.Text = network.GetAdapterName(adapter);
                    vendorname.Text  = network.GetAdapterVendor(adapter);
                    phynumber.Text   = network.GetMacAddressFromAdapter(adapter);
                }
                catch (Exception)
                {
                    errorcount += 1;
                    continue;
                }
                if (errorcount != 0)
                {
                    TaskDialogResult errorDialogResult = new TaskDialogResult();
                    using (TaskDialog errordialog = new TaskDialog())
                    {

                        errordialog.Caption = "Chickweed 評価システム";
                        errordialog.InstructionText = "一部の情報を取得できませんでした";
                        errordialog.Text = "評価に必要な情報が不足しています("+errorcount+"項目)。タスクを選択してください。";
                        errordialog.Icon = TaskDialogStandardIcon.Error;
                        errordialog.OwnerWindowHandle = Handle;
                        errordialog.HyperlinksEnabled = true;
                        errordialog.FooterText = "再度実行しても評価ができない場合は<a href=\"./Chickweedマニュアル.pdf\">マニュアル</a>に沿って解決してください。";

                        var retry = new TaskDialogCommandLink("retry", "再度評価を実施する(&R)\n一時的な問題はこれらによって解決する可能性があります。");
                        retry.Default = true;
                        errordialog.HyperlinkClick += (sender, e) =>
                        {
                            Process.Start(e.LinkText);
                        };

                        retry.Click += (sender, e) =>
                        {
                            errorDialogResult = TaskDialogResult.RETRY;
                            errordialog.Close();
                        };
                        errordialog.Controls.Add(retry);

                        var cancel = new TaskDialogCommandLink("cancel", "評価を終了する(&X)");
                        cancel.Click += (sender, e) =>
                        {
                            errorDialogResult = TaskDialogResult.CANCEL;
                            errordialog.Close();
                        };
                        errordialog.Controls.Add(cancel);

                        errordialog.Show();
                    }
                    switch (errorDialogResult)
                    {
                        case TaskDialogResult.RETRY:
                            continue;
                        case TaskDialogResult.CANCEL:
                            return;
                    }
                }
            } while (errorcount != 0);
            TaskDialog success = new TaskDialog
            {
                Caption = "Chickweed™",
                InstructionText = "評価終了",
                Text = "評価を正常に完了しました",
                OwnerWindowHandle = Handle,
                Icon = TaskDialogStandardIcon.Information
            };
            success.Show();

        }
        private void InitPlugins()
        {
            //インストールされているプラグインを調べる
            PluginInfo[] pis = PluginInfo.FindPlugins();

            //すべてのプラグインクラスのインスタンスを作成する
            IPlugin[] plugins = new IPlugin[pis.Length];
            for (int i = 0; i < plugins.Length; i++) plugins[i] = pis[i].CreateInstance(this);

            for (int number = 0; number < plugins.Length; number++)
            {
                //プラグインのRunメソッドを呼び出し
                plugins[number].Run();
            }
        }

        private void FileOK(string filename, SaveOptions options)
        {
            try
            {
                switch (options)
                {
                    case SaveOptions.OVERWRITE:
                        break;
                    case SaveOptions.SAVE:
                        XLWorkbook objWBook = new XLWorkbook();
                        IXLWorksheet objSheet = objWBook.Worksheets.Add("Sheet1");

                        if (!File.Exists(filename))
                        {
                            //n列目
                            objSheet.Range("A2").Value = string.Empty;
                            objSheet.Range("B2").Value = 1;
                            objSheet.Range("C2").Value = info.StudentNumber;
                            objSheet.Range("D2").Value = info.StudentName;
                            objSheet.Range("E2").Value = info.Vendor;
                            objSheet.Range("F2").Value = info.DeviceName;
                            objSheet.Range("G2").Value = info.OSName;
                            ///MACアドレス
                            objSheet.Range("H2").Value = info.PhysicalAddress[0];
                            objSheet.Range("I2").Value = info.PhysicalAddress[1];
                            objSheet.Range("J2").Value = info.PhysicalAddress[2];
                            objSheet.Range("K2").Value = info.PhysicalAddress[3];
                            objSheet.Range("L2").Value = info.PhysicalAddress[4];
                            objSheet.Range("M2").Value = info.PhysicalAddress[5];
                            ///
                            objSheet.Range("N2").Value = info.CreatorName;
                            //保存(新規)
                            objWBook.SaveAs(filename);
                        }
                        else
                        {
                            objSheet.Range("G1:L1").Merge();
                            //1列目(ヘッダー)
                            objSheet.Range("A1").Value = "確認欄";
                            objSheet.Range("B1").Value = "NO.";
                            objSheet.Range("C1").Value = "学籍番号";
                            objSheet.Range("D1").Value = "名前";
                            objSheet.Range("E1").Value = "メーカー";
                            objSheet.Range("F1").Value = "機種名";
                            objSheet.Range("G1").Value = "OS";
                            objSheet.Range("H1").Value = "MACアドレス";
                            objSheet.Range("I1").Value = "入力者名";
                            //2列目
                            objSheet.Range("A2").Value = string.Empty;
                            objSheet.Range("B2").Value = 1;
                            objSheet.Range("C2").Value = info.StudentNumber;
                            objSheet.Range("D2").Value = info.StudentName;
                            objSheet.Range("E2").Value = info.Vendor;
                            objSheet.Range("F2").Value = info.DeviceName;
                            objSheet.Range("G2").Value = info.OSName;
                            ///MACアドレス
                            objSheet.Range("H2").Value = info.PhysicalAddress[0];
                            objSheet.Range("I2").Value = info.PhysicalAddress[1];
                            objSheet.Range("J2").Value = info.PhysicalAddress[2];
                            objSheet.Range("K2").Value = info.PhysicalAddress[3];
                            objSheet.Range("L2").Value = info.PhysicalAddress[4];
                            objSheet.Range("M2").Value = info.PhysicalAddress[5];
                            ///
                            objSheet.Range("N2").Value = info.CreatorName;
                            //保存(新規)
                            objWBook.SaveAs(filename);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                TaskDialog error = new TaskDialog
                {
                    Caption = "Chickweed™",
                    InstructionText = "ファイルの保存に失敗しました",
                    Text = "エラーの詳細は以下を確認してください。",
                    DetailsExpandedLabel = "詳細を非表示",
                    DetailsCollapsedLabel = "詳細を表示",
                    DetailsExpandedText = e.Message,
                    FooterText = "「<a>Chickweed™ マニュアル</a>」を開いて改善策を模索します",
                    FooterIcon = TaskDialogStandardIcon.Information,
                    HyperlinksEnabled = true,
                    OwnerWindowHandle = Handle,
                    DetailsExpanded = true,
                    Icon = TaskDialogStandardIcon.Error
                };
                error.Show();
            }
        }
        private void Disp_Loaded(object sender, RoutedEventArgs e)
        {
            InitPlugins();
            Setup();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Setup();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if ((Check1.IsChecked & Check2.IsChecked & Check3.IsChecked) == false)
            {
                int result = 0;
                using (TaskDialog checkdialog = new TaskDialog())
                {
                    checkdialog.Caption = "Chickweed™";
                    checkdialog.InstructionText = "チェックリストは確認しましたか?";
                    checkdialog.Text = "[続行]をクリックするとこの操作を完了します。";
                    checkdialog.Cancelable = false;
                    checkdialog.FooterIcon = TaskDialogStandardIcon.Information;
                    checkdialog.FooterText = "チェックリストについては「<a>詳細</a>」タブを確認してください";
                    checkdialog.HyperlinksEnabled = true;

                    TaskDialogButton cancel = new TaskDialogButton("cancel", "キャンセル");
                    TaskDialogButton allow = new TaskDialogButton("allow", "続行(&C)");

                    checkdialog.HyperlinkClick += (sender, e) =>
                    {
                        checkdialog.Close();
                        MainTabControl.SelectedIndex = 1;
                        result = 1;
                    };

                    allow.Click += (sender, e) =>
                    {
                        checkdialog.Close();
                        result = 0;
                        Check1.IsChecked = true;
                        Check2.IsChecked = true;
                        Check3.IsChecked = true;
                    };

                    cancel.Click += (sender, e) =>
                    {
                        checkdialog.Close();
                        result = 1;
                    };

                    cancel.Default = true;

                    checkdialog.Controls.Add(allow);
                    checkdialog.Controls.Add(cancel);

                    checkdialog.OwnerWindowHandle = Handle;
                    checkdialog.Show();
                }
                if (result != 0)
                {
                    return;
                }
            }
            SaveOptions saveDialogResult = new SaveOptions();
            using (TaskDialog savedialog = new TaskDialog())
            {
                savedialog.Caption = "Chickweed™";
                savedialog.InstructionText = "評価の結果に対するタスクを選択してください。";
                savedialog.Cancelable = true;
                savedialog.OwnerWindowHandle = Handle;

                var savenormal = new TaskDialogCommandLink("savenormal", "既定のファイルに保存(&E)\n既定のファイルに追記します。通常はこれを選択してください。");
                savenormal.Default = true;
                savenormal.Click += (sender, e) =>
                {
                    saveDialogResult = SaveOptions.SAVE;
                    savedialog.Close();
                };
                savedialog.Controls.Add(savenormal);

                var savenew = new TaskDialogCommandLink("savenew", "名前を付けて保存(&A)\n別名で保存します。上書きの場合は元のデータが消去されます。");
                savenew.Click += (sender, e) =>
                {
                    saveDialogResult = SaveOptions.OVERWRITE;
                    savedialog.Close();
                };

                savedialog.Controls.Add(savenew);

                var exit = new TaskDialogCommandLink("exit", "キャンセル(&C)");
                exit.Click += (sender, e) =>
                {
                    saveDialogResult = SaveOptions.CANCEL;
                    savedialog.Close();
                };
                savedialog.Controls.Add(exit);

                savedialog.Show();
            }
            switch (saveDialogResult)
            {
                case SaveOptions.CANCEL:
                    break;
                case SaveOptions.SAVE:
                    SaveWindow saveNormal = new SaveWindow("評価を保存", SaveOptions.SAVE);
                    saveNormal.ShowDialog();
                    break;
                case SaveOptions.OVERWRITE:
                    SaveWindow saveAnother = new SaveWindow("名前を付けて評価を保存", SaveOptions.OVERWRITE);
                    saveAnother.ShowDialog();
                    break;
            }
        }
        
        /*private void Disp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveOptions saveDialogResult = new SaveOptions();
            using (TaskDialog savedialog = new TaskDialog())
            {
                savedialog.Caption = "Chickweed™";
                savedialog.InstructionText = "評価の結果に対するタスクを選択してください。";
                savedialog.Cancelable = true;
                savedialog.OwnerWindowHandle = Handle;

                var savenormal = new TaskDialogCommandLink("savenormal", "既定のファイルに保存(&E)\n既定のファイルに追記します。通常はこれを選択してください。");
                savenormal.Default = true;
                savenormal.Click += (sender, e) =>
                {
                    saveDialogResult = SaveOptions.SAVE;
                    savedialog.Close();
                };
                savedialog.Controls.Add(savenormal);

                var savenew = new TaskDialogCommandLink("savenew", "名前を付けて保存(&A)\n別名で保存します。上書きの場合は元のデータが消去されます。");
                savenew.Click += (sender, e) =>
                {
                    saveDialogResult = SaveOptions.OVERWRITE;
                    savedialog.Close();
                };

                savedialog.Controls.Add(savenew);

                var exit = new TaskDialogCommandLink("exit", "保存せずに終了(&X)");
                exit.Click += (sender, e) =>
                {
                    saveDialogResult = SaveOptions.EXIT;
                    savedialog.Close();
                };
                savedialog.Controls.Add(exit);

                savedialog.Show();
            }
            switch (saveDialogResult)
            {
                case SaveOptions.CANCEL:
                    e.Cancel = true;
                    break;
                case SaveOptions.SAVE:
                    break;
                case SaveOptions.OVERWRITE:
                    break;
                case SaveOptions.EXIT:
                    Application.Current.Shutdown();
                    break;
            }
        }*/

        private void GoToProxySetting_Click(object sender, RoutedEventArgs e)
        {
            string winver = JudgeWindowsVersion(reg.GetOSFullName());
            if (winver == "10")
            {
                Process.Start("ms-settings:network-proxy");
            }
            /* Windows 8.1 以下だった場合の処理
            else
            {
                Process.Start("control.exe", "inetcpl.cpl,,4");
            }
            */
        }

        private void GoToUpdateSetting_Click(object sender, RoutedEventArgs e)
        {
            string winver = JudgeWindowsVersion(reg.GetOSFullName());
            if (winver == "10")
            {
                Process.Start("ms-settings:windowsupdate-action");
            }
            /*
            else
            {
                Process.Start("control.exe", "wuaucpl.cpl");
            }
            */
        }

        private void CreateProxyShortcutToDesktop_Click(object sender, RoutedEventArgs e)
        {
            string winver = JudgeWindowsVersion(reg.GetOSFullName());
            if (winver == "10")
            {
                CreateShortcut("プロキシ設定", "ms-settings:network-proxy", "0", "C:\\Windows\\System32\\Shell32.dll" + ",316");
            }
            /* 謎のダブルクォーテーションが入るlinkPath
            else
            {
                CreateShortcut("プロキシ設定", @"C:\\Windows\\System32\\rundll32.exe shell32.dll,Control_RunDLL inetcpl.cpl,,4", "C:\\WINDOWS\\system32", "C:\\Windows\\System32\\Shell32.dll" + ",316");
            }
            */
        }

        private void CreateUpdateShortcutToDesktop_Click(object sender, RoutedEventArgs e)
        {
            string winver = JudgeWindowsVersion(reg.GetOSFullName());
            if (winver == "10")
            {
                CreateShortcut("Windows Update", "ms-settings:windowsupdate-action", "0", "C:\\Windows\\System32\\Shell32.dll" + ",316");
            }
            /* 
            else
            {
                CreateShortcut("Windows Update", @"C:\\Windows\\System32\\rundll32.exe shell32.dll,Control_RunDLL /name Microsoft.WindowsUpdate", "C:\\WINDOWS\\system32", "C:\\Windows\\System32\\Shell32.dll" + ",316");
            }
            */
        }

        public void CreateShortcut(string Name, string linkPath, string workingPath, string iconPath)
        {
            // 作成するショートカットのパス
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), @Name + ".lnk");

            // WshShellを作成
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            dynamic shell = Activator.CreateInstance(t);

            // WshShortcutを作成
            var shortcut = shell.CreateShortcut(shortcutPath);

            // リンク先
            shortcut.TargetPath = linkPath;

            // ワーキングディレクトリ
            if (workingPath != "0")
            {
                shortcut.WorkingDirectory = workingPath;
            }

            // アイコンのパス
            shortcut.IconLocation = iconPath;

            // ショートカットを作成
            shortcut.Save();

            // 後始末
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
        }

        public SupportData JudgeVersion(string id, DateTime date)
        {
            const string allowed = "申請許可";
            const string denied  = "申請不可(アップデートが必要です)";
            SupportData support = new SupportData();
            //サポート期間の定数
            const int supportPeriod = 18;
            //チェックするタイミング
            DateTime checkSpan = new DateTime(date.Year, 4, 1, 0, 0, 0);

            //VersionIDから取得したリリース年月
            int idYear = int.Parse(util.SubstringAtCount(id, 2)[0]);
            int idMonth = int.Parse(util.SubstringAtCount(id, 2)[1]);

            //そのバージョンのサポート期間の計算
            int yearExpire = util.DateCount(idYear, idMonth, 0, supportPeriod)[0];
            int monthExpire = util.DateCount(idYear, idMonth, 0, supportPeriod)[1];

            //DateTime型に変換したサポート期間
            DateTime expire = DateTime.Parse(string.Format("20{0:00}/{1}/31", yearExpire, monthExpire));

            //年度換算
            DateTime checkpoint = new DateTime(date.Year, 3, 31, 0, 0, 0);
            switch (date.CompareTo(checkSpan))
            {
                case 1:
                    checkpoint.AddYears(1);
                    break;
            }

            //年度末までサポートがあるか確認
            switch (checkpoint.CompareTo(expire))
            {
                case -1:
                    evaluation.Foreground = new SolidColorBrush(Colors.Green);
                    support.IsSupportAvailable = true;
                    support.EvaluationText = allowed;
                    break;
                case 0:
                    evaluation.Foreground = new SolidColorBrush(Colors.Red);
                    support.IsSupportAvailable = false;
                    support.EvaluationText = denied;
                    break;
                case 1:
                    evaluation.Foreground = new SolidColorBrush(Colors.Red);
                    support.IsSupportAvailable = false;
                    support.EvaluationText = denied;
                    break;
            }

            support.DExpireDate = checkpoint;
            support.WExpireDate = expire;

            return support;

            /*
            int Base_year, Year_end, Version_support, Version_year, Version_month;
            // 期限用数値の作成
            DateTime dateTime = DateTime.Now;
            Base_year = int.Parse(dateTime.ToString("yy")) + 1;
            Year_end = int.Parse(Convert.ToString(Base_year) + "03");

            // バージョンの計算
            Version_year = int.Parse(Version_id.Substring(0, 2));
            Version_month = int.Parse(Version_id.Substring(3));
            for (int i = 0; i < 18; i++) {
                Version_month++;
                if (Version_month == 12) {
                    Version_month = 1;
                    Version_year++;
                }
            }
            Version_support = int.Parse(Convert.ToString(Version_year) + string.Format("{0:00}", Version_month));
            
            // 申請判定
            if (Year_end < Version_support)
            {
                evaluation.Foreground = new SolidColorBrush(Colors.Green);
                return "申請許可";
            }
            else
            {
                evaluation.Foreground = new SolidColorBrush(Colors.Red);
                return "申請不可(Windows Updateが必要です)";
            }*/
        }

        public string JudgeWindowsVersion(string winver)
        {
            if (winver.Contains("10"))
            {
                return "10";
            }
            else if (winver.Contains("8"))
            {
                return "8";
            }
            else
            {
                return null;
            }
            //string[] winver_splitted = winver.Split(' ');
            //return "8"; //winver_splitted[2];
        }

        private void RunCMD(string strcmd)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = "/c " + strcmd;
                proc.Start();
                proc.WaitForExit();
                proc.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("プロキシの設定に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void setProxy_Click(object sender, RoutedEventArgs e)
        {
            if (noproxy.IsChecked == true)
            {
                //通常のプロキシ
                RunCMD(@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"" /f /v ProxyEnable /t reg_dword /d 0");
                //WinHTTPプロキシ(Windows Update等に使用)
                //RunCMD("netsh winhttp reset proxy");
            }
            else if (kosen.IsChecked == true)
            {
                //通常のプロキシ
                RunCMD(@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"" /f /v ProxyEnable /t reg_dword /d 1");
                RunCMD(@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"" /f /v ProxyServer /t reg_sz /d proxy.anan-nct.ac.jp:8080");
                RunCMD(@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"" /f /v ProxyOverride /t reg_sz /d ""<local>""");
                //RunCMD(@"reg add ""HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"" /f /v ProxyOverride /t reg_sz /d ""192.168.0.50;<local>""");
                //WinHTTPプロキシ(Windows Update等に使用)
                //RunCMD("netsh winhttp set proxy proxy-server=\"proxy.anan-nct.ac.jp:8080\"");
                //RunCMD("netsh winhttp set proxy proxy-server=\"proxy.anan-nct.ac.jp:8080\" bypass-list=\"192.168.0.50\"");
            }
        }
    }
}
