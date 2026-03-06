using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace Propla
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProPlaSearchWindow : NavigationWindow
    {


        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static string ProPlaAuth { get; set; } = "50";
        public static string ProPlaMgrAuth { get; set; } = "80";


        //static private ProPlaMain _ProPlaMain = new();
        //static private StdLogin _login = new();
        //static private ProPlaMenu _ProPlaMenu ;

        static private ProPlaSearchWindow _this;


        private ObservableCollection<ProductPlanning> _Items = new();
        private ProductPlanning _Item = new();



        // ロードイベント
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //閉じるボタンを消す
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }



        public ProPlaSearchWindow(string pID, string pMode)
        {
            InitializeComponent();

            ProPlaSearch _ProPlaSearch = new ProPlaSearch(pID);

            _ProPlaSearch.SetDispMode(pMode);
            _this = this;
            MainWindow.ProPlaSearchWindow = this;

            NavigationService.Navigate(_ProPlaSearch);

        }

        static public void SetDialogResult(bool pBoolean)
        {
            _this.DialogResult = pBoolean;
        }


    }



}