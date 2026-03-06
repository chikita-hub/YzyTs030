using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Propla
{
    /// <summary>
    /// ProPlaMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class ProPlaMenu : Page
    {
        const string PROPRASERCH = "商品検索・データ出力";
        const string SCREENDISPLAY = "表示履歴";
        //const string DATAOUT = "データ出力";
        const string USERMST = "ユーザマスタ";
        const string RELATEDLINKS = "使い方ガイド";
        const string LAWLINKS = "法律関係リンク";
        const string VARIOUSLINKS_KIKAKU = "【商品企画室専用】各種リンク先一覧";//企画室のみ
        const string VARIOUSLINKS_KISEKI = "【キセキ専用】各種リンク先一覧";//キセキのみ
        const string VARIOUSLINKS = "各種リンク先一覧";
        const string PASSMOD = "パスワード変更";
        const string END = "ログアウト";
        const string DEVISIONMST = "区分マスタ";

        public ProPlaMenu()
        {
            InitializeComponent();

            visibleButton(Btn1, PROPRASERCH);   // 商品検索
            visibleButton(Btn12, END);   // 終了

            visibleButton(Btn2, LAWLINKS);      // 法律関係リンク
            visibleButton(Btn3, RELATEDLINKS);      // 使い方ガイド
            visibleButton(Btn4, VARIOUSLINKS);      // 各種リンク先一覧

            //企画室の権限以上は表示
            if (MainWindow.GetAccessPermission(MainWindow.AuthRegular, MainWindow.Organization12))
            {
                visibleButton(Btn5, PASSMOD, true);      // パスワード変更
                visibleButton(Btn7, SCREENDISPLAY, true);      // 表示一覧（ロック解除）
                visibleButton(Btn8, VARIOUSLINKS_KIKAKU, true);      // 各種リンク先一覧 企画室のみ
            }
            //経営戦略室の権限以上は表示　キセキ商品
            if (MainWindow.GetAccessPermission(MainWindow.AuthRegular, MainWindow.Organization33))
            {
                visibleButton(Btn5, PASSMOD, true);      // パスワード変更
                visibleButton(Btn7, SCREENDISPLAY, true);      // 表示一覧（ロック解除）
                visibleButton(Btn6, VARIOUSLINKS_KISEKI, true);      // 各種リンク先一覧 経営戦略室のみ
            }
            //企画・経営戦略室管理者の権限以上は表示
            if (MainWindow.GetAccessPermission(MainWindow.AuthManager, MainWindow.Organization12)
                || MainWindow.GetAccessPermission(MainWindow.AuthManager, MainWindow.Organization33))
            {
                visibleButton(Btn9, USERMST, true);       // ユーザマスタ
            }
            //企画管理者の権限以上は表示
            if (MainWindow.GetAccessPermission(MainWindow.AuthManager, MainWindow.Organization12))
            {
                visibleButton(Btn10, DEVISIONMST, true);      // 区分マスタ 企画室のみ
            }
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Content)
            {
                case PROPRASERCH:  //商品検索";
                    //ProPlaSearch _ProPlaSearch = MainWindow.GetProPlaSearch();
                    //_ProPlaSearch.SetDispMode(ProPlaSearch.DispModeNormal);
                    //NavigationService.Navigate(_ProPlaSearch);

                    MainWindow.GetMainWindow().NavigationProPlaSearch(ProPlaSearch.DispModeNormal);

                    break;
                //case MATERIALMST:  //原材料マスタ";
                //    break;
                case USERMST:  //ユーザマスタ";
                    UserMgr _UserMgr = new(MainWindow.ParaValueGet("PgName"), MainWindow.GetLoginID());
                    _UserMgr.ShowDialog();
                    break;
                case SCREENDISPLAY:  //表示履歴";
                    ScreenDisplay _ScreenDisplay = MainWindow.GetScreenDisplay();
                    _ScreenDisplay.SetData(false);
                    NavigationService.Navigate(_ScreenDisplay);
                    break;
                case PASSMOD:  //パスワード変更";
                    PasswordMod _PasswordMod = new();
                    _PasswordMod.ShowDialog();
                    break;

                //case DATAOUT:  //データ出力";
                //    ProPlaSearchWindow his = new ProPlaSearchWindow("", ProPlaSearch.DispModeOut);
                //    his.ShowDialog();
                //    break;
                case RELATEDLINKS:      // 使い方ガイド
                    {
                        Process.Start("msedge.exe", @"https://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%95%86%E5%93%81%E3%83%9E%E3%82%B9%E3%82%BF/%E4%BD%BF%E3%81%84%E6%96%B9%E3%82%AC%E3%82%A4%E3%83%89.xlsx?d=we61499ebbb784bfc8cab234fea990407&csf=1&web=1&e=NHQqeb");
                    }
                    break;
                case LAWLINKS:          // 法律関係リンク
                    {
                        Process.Start("msedge.exe", @"https://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/Shared%20Documents/General/01.%E3%82%A4%E3%83%99%E3%83%B3%E3%83%88%E6%AF%8E/05.%E6%B3%95%E5%BE%8B%E3%83%81%E3%82%A7%E3%83%83%E3%82%AF%E9%96%A2%E9%80%A3/%E9%96%A2%E9%80%A3%E6%B3%95%E5%BE%8B%E4%B8%80%E8%A6%A7%EF%BC%88%E3%83%AA%E3%83%B3%E3%82%AF%EF%BC%89.xlsx?d=w4be31b361c634526847eb0a6a49a2459&csf=1&web=1&e=ZJoiUd");
                    }
                    break;
                case VARIOUSLINKS_KIKAKU:       // 各種リンク先一覧 企画室のみ
                    {
                        Process.Start("msedge.exe", @"https://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-ki/Shared%20Documents/General/18.%E5%90%84%E7%A8%AE%E3%83%9E%E3%83%8B%E3%83%A5%E3%82%A2%E3%83%AB/00%E3%80%80%E5%90%84%E7%A8%AE%E3%83%AA%E3%83%B3%E3%82%AF%E5%85%88/%E5%90%84%E7%A8%AE%E3%83%AA%E3%83%B3%E3%82%AF%E5%85%88%E4%B8%80%E8%A6%A7.xlsx?d=w7dd1ea9016f648eca05f2dbac34bd976&csf=1&web=1&e=Xo7Rfc");
                    }
                    break;
                case VARIOUSLINKS_KISEKI:       // 各種リンク先一覧 キセキのみ
                    {
                        Process.Start("msedge.exe", @"https://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/_layouts/15/Doc.aspx?sourcedoc=%7B90BA7F9F-09E1-403E-B5C9-21C3142753ED%7D&file=%25u3010%25u30ad%25u30bb%25u30ad%25u5c02%25u7528%25u3011%25u5404%25u7a2e%25u30ea%25u30f3%25u30af%25u5148%25u4e00%25u89a7.xlsx&action=default&mobileredirect=true");
                    }
                    break;
                case VARIOUSLINKS:       // 各種リンク先一覧
                    {
                        Process.Start("msedge.exe", @"https://yazuyaoffice.sharepoint.com/:x:/r/sites/yazuya-all/Shared%20Documents/General/02.%E9%83%A8%E7%BD%B2%E6%AF%8E/%E5%95%86%E5%93%81%E4%BC%81%E7%94%BB/01%E3%80%80%E5%95%86%E5%93%81%E6%83%85%E5%A0%B1/%E5%95%86%E5%93%81%E3%83%9E%E3%82%B9%E3%82%BF/%E5%90%84%E7%A8%AE%E3%83%AA%E3%83%B3%E3%82%AF%E5%85%88%E4%B8%80%E8%A6%A7.xlsx?d=w1afc87d72db8485290e532421148a484&csf=1&web=1&e=t4NDlh");
                    }
                    break;
                case END:  //終了";
                    Application.Current.Shutdown();
                    break;

                case "－":
                    // ウィンドウを最小化
                    MainWindow.WindowStateMinimized();
                    break;

                case DEVISIONMST:  //区分マスタ
                    DivisionsMaster _DivisionsMaster = new();
                    _DivisionsMaster.ShowDialog();
                    break;

                default:
                    break;
            }


        }

        void visibleButton(Button pBtn, string pContent, bool pPropra = false)
        {
            pBtn.Visibility = Visibility.Visible;
            pBtn.Content = pContent;
            pBtn.FontSize = 30;

            if (pPropra)
            {
                //BrushConverter converter = new BrushConverter();
                //Brush brush = (Brush)converter.ConvertFromString("#00339A");
                //pBtn.Background = brush;

                //BrushConverter converterw = new BrushConverter();
                //Brush brushW = (Brush)converter.ConvertFromString("#FFFFFF");
                //pBtn.Foreground = brushW;

                // スタイルを変更する
                ChangeButtonStyle(pBtn);


            }
        }

        private void ChangeButtonStyle(Button button)
        {
            Style newStyle = (Style)FindResource("ButtonStyle3");
            button.Style = newStyle;
        }



    }

}
