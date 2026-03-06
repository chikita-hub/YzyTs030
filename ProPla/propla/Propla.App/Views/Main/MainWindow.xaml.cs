using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;


namespace Propla
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        static public string Server = "";

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static Cursor _originalCursor;

        string _ParameterFile = System.Environment.CurrentDirectory + @"\Parameter.json";

        //パラメータ
        public class Parameter
        {
            public string Key { get; set; } = "";
            public string Value { get; set; } = "";
            public string Extend { get; set; } = "";
        }
        public static List<Parameter> _Parameters = new();

        //一般社員
        public const string AuthRegular = "50";
        //管理者
        public const string AuthManager = "80";

        //部署：企画
        public const string Organization12 = "12";
        //部署：経戦
        public const string Organization33 = "33";

        static private ProPlaMain _ProPlaMain = new();
        static private ProPlaSearch _ProPlaSearch;
        static private StdLogin _login = new();
        static private ProPlaMenu _ProPlaMenu;
        static private ScreenDisplay _ScreenDisplay;
        static private MainWindow _this;

        static private ProductInfo _ProInfo;
        static public ProPlaSearchWindow ProPlaSearchWindow;

        //DataTable
        static DataTable _DtInformation = new();

        /// <summary>
        /// カーソルを待機カーソルに変更
        /// _originalCursor = Mouse.OverrideCursor;
        /// Mouse.OverrideCursor = Cursors.Wait;
        /// </summary>
        static public void SetCursorsWait()
        {
            // カーソルを待機カーソルに変更
            _originalCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            return;
        }

        /// <summary>
        /// 待機が完了したら元のカーソルに戻す
        /// Mouse.OverrideCursor = _originalCursor;
        /// </summary>
        static public void SetCursorsNormal()
        {
            // 待機が完了したら元のカーソルに戻す
            Mouse.OverrideCursor = _originalCursor;
            return;
        }

        /// <summary>
        /// ウィンドウを最小化
        /// </summary>
        static public void WindowStateMinimized()
        {
            if (ProPlaSearchWindow != null)
            {
                ProPlaSearchWindow.WindowState = WindowState.Minimized;
            }
            _this.WindowState = WindowState.Minimized;
            return;
        }

        /// <summary>
        /// ウィンドウ最小化を戻す
        /// </summary>
        static public void WindowStateNormal()
        {
            _this.WindowState = WindowState.Normal;
            return;
        }

        /// <summary>
        /// ログイン処理を行う
        /// </summary>
        public void WindowLogin()
        {
            _login.SetProgramId(ParaValueGet("PgName"));
            _login.SetServer(Server);
            _login.ShowDialog();
            if (_login.GetLoginOk() == "")
            {
                System.Windows.Application.Current.Shutdown();
                return;
            }
            // ウィンドウを戻す
            WindowStateNormal();

            _ProPlaMenu = new();
            NavigationService.Navigate(_ProPlaMenu);
            return;
        }
        /// <summary>
        // ロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //閉じるボタンを消す
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        /// <summary>
        /// MainWindow
        /// </summary>
        public MainWindow()
        {
            //カーソルをWAITに変更
            SetCursorsWait();


            InitializeComponent();

            parameterRead();
            Server = ParaValueGet("Server");
            if (Server != "192.168.14.243") { Title = "★★★★★★★　テスト環境　★★★★★★★"; }

            Title += " Version " + ParaValueGet("Version");

            _this = this;

            _ProPlaSearch = new("");
            _ScreenDisplay = new();

            // ウィンドウを最小化
            WindowStateMinimized();

            Loaded += (o, e) =>
            {
                //#if DEBUG
                //#else
                //                //画面初期化処理
                //                ProPlaSetup _ProPlaSetup = new();
                //                _ProPlaSetup.ShowDialog();
                //#endif

                //カーソルをNormalに変更
                SetCursorsNormal();
                WindowLogin();

            };

            try
            {
                string folderPath = @"Z:\"; // チェックするフォルダのパス
                bool folderExists = Directory.Exists(folderPath);
                if (folderExists == false)
                {
                    string currentDirectory = System.IO.Directory.GetCurrentDirectory();
                    // Excelを起動してファイルを開く
                    Process.Start(currentDirectory + @"\nw.bat", "");
                }
                Directory.CreateDirectory(@"C:\YzyTs\temp");
                Directory.CreateDirectory(@"C:\YzyTs\temp\YzyTs002");
                Directory.CreateDirectory(@"C:\YzyTs\temp\YzyTs002\OUT");
            }
            catch (Exception)
            {
            }

            //項目情報
            _ProInfo = new();

        }

        static public MainWindow GetMainWindow() { return _this; }


        //static public ProPlaMain GetProPlaMain() { return _ProPlaMain; }
        //static public ProPlaSearch GetProPlaSearch() { return _ProPlaSearch; }
        //static public ProPlaMenu GetProPlaMenu() { return _ProPlaMenu; }
        public void NavigationProPlaSearch(string pMode)
        {
            _ProPlaMain = null;

            _ProPlaSearch.SetDispMode(pMode);
            NavigationService.Navigate(_ProPlaSearch);
        }


        public void NavigationProPlaMenu()
        {
            NavigationService.Navigate(_ProPlaMenu);
        }
        public void NavigationProPlaMain(string pID)
        {
            _ProPlaMain = new();

            _ProPlaMain.SetProductPlanningID(pID);
            NavigationService.Navigate(_ProPlaMain);
        }


        static public ScreenDisplay GetScreenDisplay() { return _ScreenDisplay; }
        static public string GetLoginName() { return _login.GetLoginName(); }

        static public string GetOrganizationId() { return _login.GetOrganizationId(); }

        /// <summary>
        /// ログインID取得を行う。
        /// </summary>
        /// <returns></returns>
        static public string GetLoginID()
        {
            if (_login == null)
            {
                return "";
            }
            return _login.GetLoginID();
        }

        /// <summary>
        /// ログイン終了時間セット処理を行う。
        /// </summary>
        static public void SetLoginEndingTime()
        {
            _login.SetLoginEndingTime();
            return;
        }

        /// <summary>
        /// アクセス権限の確認
        /// </summary>
        /// <param name="pAuthority"></param>
        /// <returns></returns>
        static public bool GetAccessPermission(string pAuthority, string pOrganization) { return _login.GetAccessPermission(pAuthority, pOrganization); }

        /// <summary>
        /// パラメータ読込
        /// </summary>
        /// <returns></returns>
        bool parameterRead()
        {
            try
            {
                // 保村されたJSONを読み取り
                string resumeJson = File.ReadAllText(_ParameterFile);

                // JSONデータからオブジェクトを復元
                _Parameters = JsonSerializer.Deserialize<List<Parameter>>(resumeJson);

            }
            catch (Exception ex)
            {
                string s = "";
                s += "エラーが発生しました" + "\r\n";
                s += ex.Message;

                System.Windows.MessageBox.Show(s);
                return false;
            }
            return true;
        }

        /// <summary>
        /// パラメータ取得
        /// </summary>
        /// <param name="pKey"></param>
        /// <returns></returns>
        public static string ParaValueGet(string pKey)
        {
            var query2 = _Parameters
                            .FirstOrDefault(x => x.Key == pKey);
            if (query2 is null)
            {
                MessageBox.Show("パラメータが見つかりません：" + pKey);
                return "";
            }
            return query2.Value;
        }

        /// <summary>
        /// 項目を表示できるかの判断を行う。
        /// </summary>
        /// <param name="pItemName">項目名</param>
        /// <param name="pOrganizationId">商品部署</param>
        /// <param name="pProductDispType">商品表示種別</param>
        /// <returns>true:表示OK</returns>
        static public bool IsDispVisible(string pItemName, string pOrganizationId, string pProductDispType)
        {
            return _ProInfo.IsDispVisible(pItemName, pOrganizationId, pProductDispType);
        }

        /// <summary>
        /// 項目をEXCELに出力できるかの判断を行う。
        /// </summary>
        /// <param name="pItemName">項目名</param>
        /// <returns>true:表示OK</returns>
        static public bool IsOutExcelColum(string pItemName)
        {
            return _ProInfo.IsOutExcelColum(pItemName);
        }

        /// <summary>
        /// 項目を表示できるかの判断を行う。
        /// </summary>
        /// <param name="pItemName">項目名</param>
        /// <param name="pOrganizationId">商品部署</param>
        /// <returns>true:表示OK</returns>
        static public bool IsOutExcelData(string pItemName, string pOrganizationId)
        {
            return _ProInfo.IsOutExcelData(pItemName, pOrganizationId);
        }
        static public string SelectWhere()
        {
            return _ProInfo.SelectWhere();
        }
    }
}