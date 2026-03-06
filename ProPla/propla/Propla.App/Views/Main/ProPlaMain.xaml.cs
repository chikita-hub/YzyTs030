using Microsoft.Web.WebView2.Wpf;
using ProPla.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

//using System.Windows.Forms;
//using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace Propla
{
    public class ColList
    {
        public string ColId;
        public string ColNm;
    }

    /// <summary>
    /// Interaction logic for ProPlaMain.xaml
    /// </summary>
    public partial class ProPlaMain : Page
    {
        class ControlList
        {
            public Object Obj { get; set; }
            public string Name { get; set; } = "";
            public string Type { get; set; } = "";
            public Double ScrollY { get; set; } = 0;
            public bool UpdateFlg { get; set; } = false;
            public bool DispFlg { get; set; } = true;
        }


        private List<ControlList> _ControlList;
        private MenuItem _UpdateMenuItem;

        private ViewModel _vm;

        List<string> _ContextMainDisp = new List<string> {
            "商品名",
            "商品情報",
            "製造",
            "品質・製品規格書",
            "特色のある表示",
            "アレルゲン",
            "広告表示",
            "価格",
            "原価・粗利率",
            "成分・食品換算",
            "パッケージ情報",
            "一括表示",
            "栄養成分表示",
            "備考"
        };

        //private bool _InputFlg = false;  step2

        //TextBOxに変更があったら背景を変えるフラグ(初期処理に伴う変更を除外するため)
        private bool _UpdateBackground = false;

        //WebView2の初期処理が完了しているかを判断するフラグ
        private bool _WebView2InitializationCompleted = false;

        Brush _BrushUpdateBackGround = Brushes.Orange;
        Brush _BrushDefultBackGround = Brushes.White;
        Brush _BrushDefultBtnBackGround = Brushes.White;

        StdListBox _list = new StdListBox();

        //　商品分類
        int _ProductCategorySelectedIndex = 0;
        //　部署ID
        string _OrganizationId = "";
        //　商品種別
        string _ProductDispType = "";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProPlaMain()
        {
            InitializeComponent();

            /// 初期処理
            InitializeMain();

            Loaded += (o, e) =>
            {
                /// コントロールのロード完了処理
                LoadMain();
            };

        }
        /// <summary>
        /// 初期処理
        /// </summary>
        private void InitializeMain()
        {
            _UpdateBackground = false;
        }
        /// <summary>
        /// コントロールのロード完了処理
        /// </summary>
        private void LoadMain()
        {
            // ロードが完了した場合に処理を行う　　※呼び出しのたびに実行される。
            // ロードが完了したら、コントロールリストを作成する

            // グローバルなKeyDownイベントハンドラーを追加する
            //System.Windows.Input.Keyboard.AddKeyDownHandler(this, GlobalKeyDownHandler);

            _ControlList = new();
            this.WalkInChildren(child =>
            {
                if (child is Control)
                {
                    ControlList _cl = new();
                    _cl.Obj = (Object)child;
                    _cl.Name = ((Control)child).Name;
                    _cl.Type = child.GetType().ToString().Replace("System.Windows.Controls.", "");
                    _ControlList.Add(_cl);
                }
                if (child is Decorator)
                {
                    ControlList _cl = new();
                    _cl.Obj = (Object)child;
                    _cl.Name = ((Decorator)child).Name;
                    _cl.Type = child.GetType().ToString().Replace("System.Windows.Decorators.", "");
                    _ControlList.Add(_cl);
                }
                if (child is Panel)
                {
                    ControlList _cl = new();
                    _cl.Obj = (Object)child;
                    _cl.Name = ((Panel)child).Name;
                    _cl.Type = child.GetType().ToString().Replace("System.Windows.Controls.", "");
                    _ControlList.Add(_cl);
                }
                if (child is WebView2)
                {
                    ControlList _cl = new();
                    _cl.Obj = (Object)child;
                    _cl.Name = ((WebView2)child).Name;
                    _cl.Type = "WebView2";
                    _ControlList.Add(_cl);
                }
            });

            /// WebView2初期化
            WebView2Initialize();

            // 新規の場合コンポーネントを活性化
            if (ProductPlanningID.Text == "")
            {
                SetIsEnabled(true);
            }
            else
            {
                // 既存・コンポーネントの非活性化
                SetIsEnabled(false);

                // 項目の非表示化　商品分類により項目の表示を切り替える
                SetDisp();

            }

            /// コンテキストメニューのとび先の設定
            setScrollY();

            /// 背景クリア
            resetBackGround();

            _UpdateBackground = true;//TextBOxに変更があったら背景を変えるフラグ

            /// メインコンテキストメニュー設定
            setContextMainMenu(true);

            ////ポップアップ表示 STEP2削除
            //if (_vm.Item.PopUpLink != "")
            //{
            //    StdWebBrowserDisp _StdWebBrowserDisp = new();
            //    _StdWebBrowserDisp.SetText(_vm.Item.PopUpLink);
            //    _StdWebBrowserDisp.ShowDialog();
            //}

            //スクロールを一番上へ
            MainScrollViewer.ScrollToVerticalOffset(0);

            //更新権限があるかのチェック処理
            {
                BtnUpdPer.Visibility = Visibility.Hidden;//表示しないがスペースを使用する。

                //商品区分の部署を取得
                var query1 = _vm.ItemMst.MstProductCategoryDt.AsEnumerable()
                 .FirstOrDefault(x => x["value"].ToString() == _vm.Item.ProductCategory)
                 ;
                if (query1 is null)
                {
                    MessageBox.Show("ProductCategoryが見つかりません：" + _vm.Item.ProductCategory);
                    return;
                }
                string organizationId = query1["extend_field_1"].ToString();

                //企画室の権限以上あれば更新可
                if (organizationId == MainWindow.Organization12
                    && MainWindow.GetAccessPermission(MainWindow.AuthRegular, MainWindow.Organization12))
                {
                    BtnUpdPer.Visibility = Visibility.Visible;
                    BtnUpdPer.IsEnabled = true;
                }
                //キセキの権限以上あれば更新可
                if (organizationId == MainWindow.Organization33
                    && MainWindow.GetAccessPermission(MainWindow.AuthRegular, MainWindow.Organization33))
                {
                    BtnUpdPer.Visibility = Visibility.Visible;
                    BtnUpdPer.IsEnabled = true;
                }
            }

            //新規の場合IDを自動採番する。
            if (ProductPlanningID.Text == "")
            {
                string s = "";

                switch (MainWindow.GetOrganizationId())
                {
                    case MainWindow.Organization12:
                        s = _vm.GetAutoProductPlanningid(MainWindow.Organization12, "");
                        break;
                    case MainWindow.Organization33:
                        s = _vm.GetAutoProductPlanningid(MainWindow.Organization33, "K");
                        break;
                    default:
                        break;
                }

                _vm.Item.ProductPlanningID = s;

                ProductPlanningID.Text = s;
                BtnUpdPer.Visibility = Visibility.Hidden;//表示しないがスペースを使用する。
            }

            //LabelProductNameセット
            LabelProductName.Text = ProductNameOfficial.Text;

            //本発売日から周年を求めて表示する
            setProductAnniversary();

            //ラベルの内容を変更する。
            var target = _ControlList.Where(x => x.Type == "Label");
            foreach (var item in target)
            {
                string s = ((Label)(item.Obj)).Content.ToString();
                if (s == "原価・粗利率")
                {
                    s = "原価・粗利率                              ※※ 商品原価(税抜き)に数字入力 → 「Tab」キーで他の項目は自動計算 ※※";
                    ((Label)(item.Obj)).Content = s;
                }
            }

            ////入力初期値を半角にする。
            //// 一般価格(税抜き)1個
            //GeneralPriceWithoutTax.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 一般価格(税抜き)2個
            //GeneralPriceWithoutTax2.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 一般価格(税抜き)3個
            //GeneralPriceWithoutTax3.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 一般価格(税込)1個
            //GeneralPrice.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 一般価格(税込)2個
            //GeneralPrice2.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 一般価格(税込)3個
            //GeneralPrice3.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 定期コース価格(毎月1個お届け)
            //RegularCoursePrice.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 定期コース価格(毎月2個お届け)
            //RegularCoursePrice2.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 定期コース価格(毎月3個お届け)
            //RegularCoursePrice3.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 商品原価(税抜き)
            //ProductCost.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };
            //// 商品原価(税込)
            //ProductCost2.InputScope = new InputScope() { Names = { new InputScopeName() { NameValue = InputScopeNameValue.Number } } };

            // 待機が完了したら元のカーソルに戻す
            MainWindow.SetCursorsNormal();

        }
        // グローバルキーイベント処理  step2
        //private void Page_KeyDown(object sender, KeyEventArgs e)
        //{
        //    // 押されたキーがBackspaceであるかを確認
        //    if (e.Key == Key.Back)
        //    {
        //        // Backspaceキーのデフォルトの動作を無効にする
        //        e.Handled = true;
        //    }
        //}

        //本発売日から周年を求めて表示する
        void setProductAnniversary()
        {
            int elapsedYears = 0;

            try
            {
                string date = ProductReleaseDate.Text; //本発売日
                date = StdCalendar.DateAnz(date);
                date = date.Replace("//", "/1/");
                if (date.Substring(date.Length - 1, 1) == "/")
                {
                    date += "1";
                }

                DateTime date1 = DateTime.Parse(date);
                DateTime currentDate = DateTime.Now;

                elapsedYears = currentDate.Year - date1.Year;

                if (currentDate.Month < date1.Month || (currentDate.Month == date1.Month && currentDate.Day < date1.Day))
                {
                    elapsedYears--;
                }
            }
            catch (System.Exception)
            {
                elapsedYears = 0;
            }

            ProductAnniversary.Text = elapsedYears.ToString() + "周年";
        }

        /// <summary>
        /// IMEモードを日本語にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxGotFocusNative(object sender, RoutedEventArgs e)
        {
            InputMethod.Current.ImeState = InputMethodState.On;
            InputMethod.Current.ImeConversionMode = ImeConversionModeValues.FullShape | ImeConversionModeValues.Native;
        }
        /// <summary>
        /// IMEモードを英数字にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxGotFocusAlphanumeric(object sender, RoutedEventArgs e)
        {
            InputMethod.Current.ImeState = InputMethodState.On;
            InputMethod.Current.ImeConversionMode = ImeConversionModeValues.Alphanumeric;
        }

        /// <summary>
        /// 背景のクリア
        /// </summary>
        void resetBackGround()
        {
            var query1 = _ControlList.Where(x => x.Name.IndexOf("BorderComboBox") >= 0);
            foreach (var item in query1) ((Border)(item.Obj)).Background = _BrushDefultBackGround;

            query1 = _ControlList.Where(x => x.Type == "TextBox");
            foreach (var item in query1) ((TextBox)(item.Obj)).Background = _BrushDefultBackGround;

            query1 = _ControlList.Where(x => x.Name.IndexOf("BorderButton") >= 0);
            foreach (var item in query1) ((Border)(item.Obj)).Background = _BrushDefultBtnBackGround;

            query1 = _ControlList.Where(x => x.Type == "ComboBox");
            foreach (var item in query1) ((ComboBox)(item.Obj)).SelectedItem = "0";
        }
        /// <summary>
        /// 表示データのクリア
        /// </summary>
        void resetData()
        {
            var query1 = _ControlList.Where(x => x.Type == "TextBox");
            foreach (var item in query1) ((TextBox)(item.Obj)).Text = "";

            query1 = _ControlList.Where(x => x.Type == "ComboBox");
            foreach (var item in query1) ((ComboBox)(item.Obj)).SelectedIndex = 0;

            query1 = _ControlList.Where(x => x.Type == "WebView2");
            foreach (var item in query1) ((WebView2)(item.Obj)).NavigateToString("");
            foreach (var item in query1) _vm.Item[item.Name.Substring(3)] = "";

        }

        /// <summary>
        /// スクロールさせるための位置のセット(Labelの位置)
        /// </summary>
        void setScrollY()
        {
            var queryx = _ControlList
                 .OrderBy(x => x.Name)
                 .Where(x => x.Type == "Label" && x.Name.IndexOf("Label") >= 0 && x.DispFlg == true);
            double _ScrollY = 0;
            foreach (var item1 in queryx)
            {
                for (int i = 0; i < _ControlList.Count; i++)
                {
                    if (_ControlList[i].Name == item1.Name)
                    {
                        _ControlList[i].ScrollY = _ScrollY;
                    }
                }
                _ScrollY += ((Label)(item1.Obj)).ActualHeight;
                //MainWindow.WriteDebugLog(((Label)(item1.Obj)).ActualHeight.ToString());
                var query1 = _ControlList
                    .FirstOrDefault(x => x.Name == "LSP_" + item1.Name)
                    ;
                if (!(query1 is null))
                {
                    _ScrollY += ((Panel)(query1.Obj)).ActualHeight;
                    //MainWindow.WriteDebugLog(((Panel)(query1.Obj)).ActualHeight.ToString());
                }
            }
        }


        //private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    // スクロール位置が変更されたときに実行する処理をここに記述する
        //    MainGrid.InvalidateVisual();
        //}

        /// <summary>
        ///  コンテキストメニュー初期化
        /// </summary>
        void setContextMainMenu(bool pInit)
        {
            if (MainScrollViewer.ContextMenu is null)
            {
                MainScrollViewer.ContextMenu = new();
            }
            else
            {
                MainScrollViewer.ContextMenu.Items.Clear();
            }

            var query1 = _ControlList
                .Where(x => x.Type == "Label" && x.Name != "" && x.DispFlg == true)
                .Select(x => x)
                ;
            foreach (var item in query1)
            {
                string content = (string)((Label)item.Obj).Content;

                var query = _ContextMainDisp.Where(x => x == content)
                                            .FirstOrDefault();
                if (query != null)
                {
                    // メインコンテキストメニューに追加
                    MenuItem _MenuItem = new MenuItem();
                    _MenuItem.Name = ((Label)item.Obj).Name;
                    _MenuItem.Header = content;
                    _MenuItem.Click += new RoutedEventHandler(OnButtonCreatedByCodeClick);
                    MainScrollViewer.ContextMenu.Items.Add(_MenuItem);
                }
            }
            Separator menuFileSeparator = new Separator();  // ←セパレータ
            MainScrollViewer.ContextMenu.Items.Add(menuFileSeparator);

            if (pInit)
            {
                _UpdateMenuItem = new();
                //// 作成したコンテキストメニューを設定
                _UpdateMenuItem.Name = "ContextMenuUpdate";
                _UpdateMenuItem.Header = "更新一覧(なし)";
                //項目更新用サブコンテキストメニューをコンテキストメニューに設定
            }
            MainScrollViewer.ContextMenu.Items.Add(_UpdateMenuItem);
        }
        /// <summary>
        ///  コンテキストメニュー追加
        /// </summary>
        void AddContextMenu(ControlList _c)
        {
            MenuItem _MenuItem = new MenuItem();
            _MenuItem.Name = ((Control)_c.Obj).Name;
            switch (_c.Type)
            {
                case "TextBox":
                    _MenuItem.Header = ((TextBox)_c.Obj).ToolTip;
                    break;
                case "ComboBox":
                    _MenuItem.Header = ((ComboBox)_c.Obj).ToolTip;
                    break;
                default:
                    break;
            }
            // イベントの追加
            _MenuItem.Click += new RoutedEventHandler(OnButtonCreatedByCodeClick);
            // 更新コンテキストメニューに追加
            _UpdateMenuItem.Items.Add(_MenuItem);
            _UpdateMenuItem.Header = "更新一覧(あり)";
        }

        /// <summary>
        /// コンポーネントの非活性・活性
        /// </summary>
        /// <param name="pIsEnabled"></param>
        private void SetIsEnabled(Boolean pIsEnabled)
        {
            var query1 = _ControlList
                 .Where(x => (x.Type == "Button" || x.Type == "TextBox" || x.Type == "ComboBox")
                          && (x.Name != "BtnEnd" && x.Name != "BtnUpdPer" && x.Name != "BtnHis" && x.Name != "BtnWindowStateMinimized"))
                 .Select(x => x)
                 ;
            foreach (var item in query1)
            {
                switch (item.Type)
                {
                    case "Button":

                        if (item.Name == "BtnUpdPer")
                        {
                            MessageBox.Show(item.Name);
                        }

                        if (item.Name.IndexOf("BtnPop_") >= 0)
                        {
                            break;
                        }
                        ((Button)item.Obj).IsEnabled = pIsEnabled;
                        break;
                    case "TextBox":
                        ((TextBox)item.Obj).IsReadOnly = !(pIsEnabled);
                        break;
                    case "ComboBox":
                        ((ComboBox)item.Obj).IsEnabled = pIsEnabled;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 商品種別により項目の表示・非表示を切り替える。
        /// </summary>
        private void SetDisp()
        {
            //(1)Label001:Label
            //(2)LSP_Label001:StackPanel
            //(3)RSP_Label001_001:StackPanel
            //(4)CSP_Label001_001_001_ProductPlanningID:StackPanel

            if (_vm.Item.ProductCategory == "0" || _vm.Item.ProductCategory == null) { return; }
            var query1 = _vm.ItemMst.MstProductCategoryDt.AsEnumerable()
                .FirstOrDefault(x => x["value"].ToString() == _vm.Item.ProductCategory)
                ;
            if (query1 is null)
            {
                MessageBox.Show("ProductCategoryが見つかりません：" + _vm.Item.ProductCategory);
                return;
            }

            _ProductCategorySelectedIndex = ProductCategory.SelectedIndex;
            _OrganizationId = query1["extend_field_1"].ToString();
            _ProductDispType = query1["extend_field_2"].ToString();

            //商品分類ごと(2)
            var queryx = _ControlList
                     .Where(x => x.Type == "StackPanel" && x.Name.IndexOf("LSP_") >= 0);
            foreach (var item1 in queryx)
            {
                //分類内の行ごと(3)
                bool listdisp = false;
                var query2 = _ControlList
                     .Where(x => x.Type == "StackPanel" && x.Name.IndexOf(((StackPanel)(item1.Obj)).Name.Replace("LSP_", "RSP_")) >= 0);
                foreach (var item2 in query2)
                {
                    //分類内の行ごとの項目ごと(4)
                    bool rowdisp = false;
                    var query3 = _ControlList
                       .Where(x => x.Type == "StackPanel" && x.Name.IndexOf(((StackPanel)(item2.Obj)).Name.Replace("RSP_", "CSP_")) >= 0);
                    foreach (var item3 in query3)
                    {
                        string nm = ((StackPanel)(item3.Obj)).Name;
                        string[] arr = nm.Split('_');

                        //分類内の行ごとの項目ごと(4)表示・非表示切り替え
                        if (MainWindow.IsDispVisible(arr[4], _OrganizationId, _ProductDispType))
                        {
                            ((StackPanel)(item3.Obj)).Visibility = Visibility.Visible;//表示する。
                            rowdisp = true;
                        }
                        else
                        {
                            ((StackPanel)(item3.Obj)).Visibility = Visibility.Hidden;//表示しないがスペースを使用する。
                        }

                    }
                    //分類内の行ごと(3)表示・非表示切り替え
                    if (rowdisp)
                    {
                        ((StackPanel)(item2.Obj)).Visibility = Visibility.Visible;//表示する。
                        listdisp = true;
                    }
                    else
                    {
                        ((StackPanel)(item2.Obj)).Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                    }
                }
                //商品分類ごと(2)表示・非表示切り替え
                var query11 = _ControlList
                        .FirstOrDefault(x => x.Name == ((StackPanel)(item1.Obj)).Name.Replace("LSP_", ""))
                        ;
                if (query11 is null)
                {
                    MessageBox.Show("Listが見つかりません：" + ((StackPanel)(item1.Obj)).Name.Replace("LSP_", ""));
                    return;
                }

                //Border
                var query12 = _ControlList
                    .FirstOrDefault(x => x.Name == "Border_" + item1.Name)
                    ;
                if (query12 is null)
                {
                    MessageBox.Show("Listが見つかりません：" + "Border_" + item1.Name);
                    return;
                }

                if (listdisp)
                {
                    ((StackPanel)(item1.Obj)).Visibility = Visibility.Visible;//表示する。
                    ((Label)(query11.Obj)).Visibility = Visibility.Visible;//表示する。
                    ((Border)(query12.Obj)).Visibility = Visibility.Visible;//表示する。
                    query11.DispFlg = true;
                }
                else
                {
                    ((StackPanel)(item1.Obj)).Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                    ((Label)(query11.Obj)).Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                    ((Border)(query12.Obj)).Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                    query11.DispFlg = false;
                }
            }
        }


        /// <summary>
        /// コンテキストメニューから指定ラベルへ飛ぶ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonCreatedByCodeClick(object sender, RoutedEventArgs e)
        {
            string sourceName = ((FrameworkElement)e.Source).Name;
            string senderName = ((FrameworkElement)sender).Name;

            var query1 = _ControlList
                .FirstOrDefault(x => x.Name == sourceName)
                ;
            if (query1 is null)
            {
                MessageBox.Show("コンテキストメニューが見つかりません：" + sourceName);
                return;
            }

            if (query1.Type == "Label")
            {
                MainScrollViewer.ScrollToVerticalOffset(query1.ScrollY - 100);
                return;
            }

            Control _c = (Control)(query1.Obj);
            StackPanel _s;
            if (query1.Type == "ComboBox")
            {
                Border _b = (Border)(_c.Parent);
                _s = (StackPanel)(_b.Parent);
            }
            else
            {
                _s = (StackPanel)(_c.Parent);
            }


            string[] arr = _s.Name.Split('_');
            string _LabelName = arr[1];
            query1 = _ControlList
                .FirstOrDefault(x => x.Name == _LabelName)
                ;
            if (!(query1 is null))
            {
                MainScrollViewer.ScrollToVerticalOffset(query1.ScrollY - 100);
            }
        }

        /// <summary>
        /// WebView2　初期処理
        /// </summary>
        async void WebView2Initialize()
        {
            try
            {
                var query1 = _ControlList
                     //                    .Where(x => x.Name == "WebTrademarkCertificate")
                     .Where(x => x.Type == "WebView2")
                     .Select(x => (x.Obj, x.Name))
                     ;
                foreach (var item in query1)
                {
                    if (_WebView2InitializationCompleted)
                    {
                        SetWebView2Text((WebView2)item.Obj);
                    }
                    else
                    {
                        await ((WebView2)item.Obj).EnsureCoreWebView2Async(null);
                    }
                }

                _WebView2InitializationCompleted = true;
            }
            catch (System.Exception ex)
            {
                string s = ex.Message + "\r\n" + "WebView2ランタイムがインストールされていない可能性があります。";
                TxtMsgBox _txt = new(s);
                _txt.ShowDialog();
            }
        }
        private void WebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {

            WebView2 webView2 = (WebView2)sender;

            string _Name = webView2.Name.Substring(3);
            var query1 = _ControlList
                        .FirstOrDefault(x => x.Name == _Name)
                        ;
            if (query1 is null) { MessageBox.Show("ControlListにありません：" + _Name); return; }

            TextBox _TextBox = (TextBox)query1.Obj;
            string s = _TextBox.Text;
            s = StdWebBrowserEdit.HtmlHheader + _TextBox.Text + StdWebBrowserEdit.HtmlFooter;
            //_WebView2.NavigateToString(@_s);

            Task<int> a = XRunTaskAAsync(webView2, s);

        }
        private void SetWebView2Text(WebView2 pWebView2)
        {
            string _Name = pWebView2.Name.Substring(3);
            var query1 = _ControlList
                        .FirstOrDefault(x => x.Name == _Name)
                        ;
            if (query1 is null) { MessageBox.Show("ControlListにありません：" + _Name); return; }

            string _s = ((TextBox)query1.Obj).Text;
            _s = StdWebBrowserEdit.HtmlHheader + _s + StdWebBrowserEdit.HtmlFooter;
            pWebView2.NavigateToString(@_s);

        }

        public async Task<int> XRunTaskAAsync(WebView2 WebView2, string _s)
        {
            await Task.Delay(100); // 0.1秒待機
            WebView2.NavigateToString(@_s);

            //MainWindow.WriteDebugLog("A");
            return 0;
        }

        public void SetProductPlanningID(string pID)
        {

            _vm = new();
            //　【重要】xamlとのリンクセット
            this.DataContext = _vm;

            if (_vm.CheckColumnComment() == 0)
            {
                MessageBox.Show("システムエラー（コメントが設定されていません）テクニカルサポート室に連絡してください。");
                return;
            }

            _UpdateBackground = false;//TextBOxに変更があったら背景を変えるフラグ
            ProductPlanningID.Text = pID;            // 画面にセット

            if (ProductPlanningID.Text == "")//新規登録
            {
                _vm.IdDataNew();
                BtnUpdPer.Visibility = Visibility.Hidden; //"更新許可"  
                BtnReg.Visibility = Visibility.Hidden;    //"引用"      
                //BtnHis.Visibility = Visibility.Hidden;   //"更新履歴"  
                BtnUpdate.Content = "登録";//"更新"      

                return;
            }
            else
            {
                BtnUpdPer.Visibility = Visibility.Visible; //"更新許可"  
                BtnReg.Visibility = Visibility.Visible;    //"引用"      
                //BtnHis.Visibility = Visibility.Visible;   //"更新履歴"  
                BtnUpdate.Content = "登録";//"更新"      
            }

            int _i = _vm.IdDataSelect(ProductPlanningID.Text);            // データセット
            switch (_i)
            {
                case 0:
                    MessageBox.Show("データが不正ですテクニカルサポート室に連絡してください。");
                    return;
                case 1:
                    //表示履歴START
                    _vm.SetScreenDisplayStartTime(ProductPlanningID.Text);
                    return;
                default:
                    MessageBox.Show("データが不正ですテクニカルサポート室に連絡してください。");
                    return;
            }
        }
        /// <summary>
        /// TextBoxが変更されたので背景色を変える
        /// ContextMenuも追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

            if (((TextBox)sender).Name == "ProductReleaseDate")
            {
                //本発売日からの周年
                setProductAnniversary();
            }
            if (_UpdateBackground)
            {
                ((TextBox)(sender)).Background = _BrushUpdateBackGround;
                var query1 = _ControlList
                    .FirstOrDefault(x => x.Name == "BorderButton" + ((TextBox)(sender)).Name)
                    ;
                if (!(query1 is null))
                {
                    ((Border)(query1.Obj)).Background = _BrushUpdateBackGround;
                }


                query1 = _ControlList
                    .FirstOrDefault(x => x.Name == ((TextBox)(sender)).Name && x.UpdateFlg == false)
                    ;
                if (query1 is null)
                {
                    return;
                }
                query1.UpdateFlg = true;
                AddContextMenu(query1);

            }
        }
        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            //小数点なし通常の販売価格などを整形する
            bool priceCnv(Key pKey, string pTextBox)
            {
                TextBox targetTextBox = null;
                var query1 = _ControlList.FirstOrDefault(x => x.Name == pTextBox);
                if (query1 == null)
                {
                    MessageBox.Show("一般価格とコース価格との差額計算エラー　:" + pTextBox);
                }
                else
                {
                    targetTextBox = (TextBox)(query1.Obj);
                }
                switch (pKey)
                {
                    case Key.Enter:
                        break;
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                        return false;
                }

                string txtOut = "";
                string num = "";
                string txtData = targetTextBox.Text;
                try
                {
                    txtData = txtData.Replace(",", "");
                    txtData = txtData.Replace("円", "");
                    for (int i = 0; i < txtData.Length; i++)
                    {
                        string s = txtData.Substring(i, 1);
                        string nums = "0123456789";
                        if (nums.IndexOf(s) >= 0)
                        {
                            num += s;
                        }
                        else
                        {
                            int i1 = int.Parse(num);
                            txtOut += i1.ToString("N0");
                            txtOut += s;
                            num = "";
                        }
                    }
                }
                catch (System.Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
                if (num != "")
                {
                    int i1 = int.Parse(num);
                    txtOut += i1.ToString("N0") + "円";
                }

                if (targetTextBox.Text != txtOut)
                {
                    ((TextBox)(query1.Obj)).Text = txtOut;
                    targetTextBox.CaretIndex = targetTextBox.Text.Length;
                    _vm.Item[pTextBox] = txtOut;
                    e.Handled = true;
                }
                return true;
            }
            //小数点なし　一般価格とコース価格との差額
            void priceCnv2(string pTextBox, string p1, string p2)
            {
                TextBox targetTextBox = null;
                var query1 = _ControlList.FirstOrDefault(x => x.Name == pTextBox);
                if (query1 == null)
                {
                    MessageBox.Show("一般価格とコース価格との差額計算エラー　:" + pTextBox);
                }
                else
                {
                    targetTextBox = (TextBox)(query1.Obj);
                }

                string s1 = _vm.Item[p1].ToString();
                string s2 = _vm.Item[p2].ToString();
                if (s1 == "" || s2 == "") { return; }

                string txtOut = "";
                string txtData = targetTextBox.Text;
                try
                {
                    s1 = s1.Replace(",", "");
                    s1 = s1.Replace("円", "");

                    s2 = s2.Replace(",", "");
                    s2 = s2.Replace("円", "");

                    if (s1 != "" && s2 != "")
                    {
                        int i1 = int.Parse(s1) - int.Parse(s2);
                        txtOut = i1.ToString("N0") + "円";
                    }
                    else
                    {
                        txtOut = "";
                    }
                }
                catch (System.Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }

                if (targetTextBox.Text != txtOut)
                {
                    targetTextBox.Text = txtOut;
                    targetTextBox.CaretIndex = targetTextBox.Text.Length;
                    _vm.Item[pTextBox] = txtOut;
                    e.Handled = true;
                }
            }
            // 小数点以下2桁の四捨五入　
            void priceCnvCost(Key pKey, string pTextBox)
            {
                TextBox targetTextBox = null;
                var query1 = _ControlList.FirstOrDefault(x => x.Name == pTextBox);
                if (query1 == null)
                {
                    MessageBox.Show("一般価格とコース価格との差額計算エラー　:" + pTextBox);
                }
                else
                {
                    targetTextBox = (TextBox)(query1.Obj);
                }
                switch (pKey)
                {
                    case Key.Enter:
                        break;
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                    case Key.OemPeriod:   // . ピリオド
                    case Key.Decimal:     // . ピリオド
                        return;
                }

                string s = targetTextBox.Text;
                try
                {
                    s = s.Replace(",", "");
                    s = s.Replace("円", "");

                    if (s != "")
                    {
                        double d = double.Parse(s);
                        double roundedValue = Math.Round(d, 2); // 2桁で四捨五入
                        s = roundedValue.ToString("N"); // ３桁をカンマで区切るフォーマットで文字列に変換
                        if (s.Substring(s.Length - 3) == ".00") { s = s.Substring(0, s.Length - 3); }
                        s += "円";
                    }
                }
                catch (System.Exception exp)
                {
                    MessageBox.Show("商品原価エラー　金額が不正です。:" + exp.Message);
                }

                if (targetTextBox.Text != s)
                {
                    targetTextBox.Text = s;
                    _vm.Item[pTextBox] = s;
                }
            }

            // 小数点以下2桁の四捨五入　粗利率
            void grossProfitMarginCnv(Key pKey, string pTextBox, string pPrice, string pProductCost2)
            {
                TextBox targetTextBox = null;
                var query1 = _ControlList.FirstOrDefault(x => x.Name == pTextBox);
                if (query1 == null)
                {
                    MessageBox.Show("一般価格とコース価格との差額計算エラー　:" + pTextBox);
                }
                else
                {
                    targetTextBox = (TextBox)(query1.Obj);
                }
                switch (pKey)
                {
                    case Key.Enter:
                        break;
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                        return;
                }
                try
                {
                    string s;
                    s = _vm.Item[pPrice].ToString();
                    if (s == "") { return; }

                    double price = int.Parse(s.Replace(",", "").Replace("円", ""));
                    s = _vm.Item[pProductCost2].ToString();
                    s = s.Replace(",", "").Replace("円", "");
                    if (s == "") { return; }

                    double cost = double.Parse(s);

                    //(価格－商品原価(税込))÷価格×100（%）
                    double d = (price - cost) / price; // 浮動小数点数型;

                    d *= 100;
                    double roundedValue = Math.Round(d, 2); // 2桁で四捨五入
                    s = roundedValue.ToString("F2"); // 小数点以下2桁のフォーマットで文字列に変換
                    if (s.Substring(s.Length - 3) == ".00") { s = s.Substring(0, s.Length - 3); }
                    s += "%";

                    //targetTextBox.Text = s;
                    ((TextBox)(query1.Obj)).Text = s;
                    _vm.Item[pTextBox] = s;
                }
                catch (System.Exception exp)
                {
                    MessageBox.Show("粗利率計算エラー　金額が不正です。:" + pPrice + ":" + pProductCost2 + ":" + exp.Message);
                }
                return;
            }

            // 
            void priceCnvCostZK(Key pKey)
            {
                switch (pKey)
                {
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                        return;
                }

                string costZN = (string)((_vm.Item["ProductCost"]));
                costZN = costZN.Replace(",", "").Replace("円", "");
                if (costZN == "") { return; }

                string taxRate = _vm.GetMstConv("TaxRate", (string)((_vm.Item["TaxRate"])));
                taxRate = taxRate.Replace("%", "");
                if (taxRate == "") { return; }

                try
                {
                    double c = double.Parse(costZN);
                    double r = double.Parse(taxRate);
                    c = c + (c * (r / 100));
                    double roundedValue = Math.Round(c, 2); // 2桁で四捨五入
                    string s = roundedValue.ToString("N"); // ３桁をカンマで区切るフォーマットで文字列に変換
                    if (s.Substring(s.Length - 3) == ".00") { s = s.Substring(0, s.Length - 3); }
                    s += "円";

                    var query1 = _ControlList.FirstOrDefault(x => x.Name == "ProductCost2");
                    if (query1 == null)
                    {
                        MessageBox.Show("ProductCost2がありません");
                    }
                    else
                    {
                        if (((TextBox)(query1.Obj)).Text != s)
                        {
                            ((TextBox)(query1.Obj)).Text = s;
                            _vm.Item["ProductCost2"] = s;
                        }
                    }

                }
                catch (System.Exception exp)
                {
                    MessageBox.Show("商品原価税込み計算エラー　金額が不正です。:" + exp.Message);
                }
            }

            //_InputFlg = true; step2
            // キーが押された時の処理
            switch (((TextBox)(sender)).Name)
            {
                //本発売日からの周年
                case "ProductAnniversary":
                    //本発売日	ProductReleaseDate
                    break;

                // 一般価格(税抜き)1個
                case "GeneralPriceWithoutTax":
                // 一般価格(税抜き)2個
                case "GeneralPriceWithoutTax2":
                // 一般価格(税抜き)3個
                case "GeneralPriceWithoutTax3":
                // 一般価格(税込)1個
                case "GeneralPrice":
                // 一般価格(税込)2個
                case "GeneralPrice2":
                // 一般価格(税込)3個
                case "GeneralPrice3":
                // 定期コース価格(毎月1個お届け)
                case "RegularCoursePrice":
                // 定期コース価格(毎月2個お届け)
                case "RegularCoursePrice2":
                // 定期コース価格(毎月3個お届け)
                case "RegularCoursePrice3":
                    if (!priceCnv(e.Key, ((TextBox)sender).Name))
                    {
                        return;
                    }
                    break;

                // 商品原価(税抜き)
                case "ProductCost":
                    priceCnvCost(e.Key, "ProductCost");
                    break;

                default:
                    break;
            }

            // キーが押された時の処理
            switch (((TextBox)(sender)).Name)
            {
                // 一般価格との差額 1個
                case "PriceDifference":
                case "GeneralPrice":
                case "RegularCoursePrice":
                    priceCnv2(
                      "PriceDifference"      // 一般価格との差額
                    , "GeneralPrice"         // 一般価格(税込)1個
                    , "RegularCoursePrice"); // 定期コース価格(毎月1個お届け)
                    break;
                // 一般価格との差額 2個
                case "PriceDifference2":
                case "GeneralPrice2":
                case "RegularCoursePrice2":
                    priceCnv2(
                      "PriceDifference2"      // 一般価格との差額
                    , "GeneralPrice2"         // 一般価格(税込)2個
                    , "RegularCoursePrice2"); // 定期コース価格(毎月2個お届け)
                    break;
                // 一般価格との差額 3個
                case "PriceDifference3":
                case "GeneralPrice3":
                case "RegularCoursePrice3":
                    priceCnv2(
                      "PriceDifference3"      // 一般価格との差額
                    , "GeneralPrice3"         // 一般価格(税込)3個
                    , "RegularCoursePrice3"); // 定期コース価格(毎月3個お届け)
                    break;
                default:
                    break;
            }

            switch (((TextBox)(sender)).Name)
            {
                // 商品原価(税込)
                case "ProductCost":
                case "ProductCost2":
                    priceCnvCostZK(e.Key);
                    break;
                default:
                    break;
            }

            // キーが押された時の処理
            switch (((TextBox)(sender)).Name)
            {
                //定期コース粗利率
                case "GrossProfitMargin":
                case "RegularCoursePrice": // 定期コース価格(毎月1個お届け)
                case "ProductCost":
                case "ProductCost2":          //商品原価(税込)
                    grossProfitMarginCnv(e.Key
                    , "GrossProfitMargin"       //定期コース粗利率
                    , "RegularCoursePrice"      //定期コース価格(毎月1個お届け)
                    , "ProductCost2");          //商品原価(税込)

                    break;
                default:
                    break;
            }
            // キーが押された時の処理
            switch (((TextBox)(sender)).Name)
            {
                //一般価格粗利率
                case "GrossProfitMargin2":
                case "GeneralPrice":         // 一般価格(税込)1個
                case "ProductCost":
                case "ProductCost2":          //商品原価(税込)
                    grossProfitMarginCnv(e.Key
                    , "GrossProfitMargin2"      //一般価格粗利率
                    , "GeneralPrice"            //一般価格(税込)　1個
                    , "ProductCost2");          //商品原価(税込)

                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// ComboBoxが変更されたので背景色を変える
        /// ContextMenuも追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_UpdateBackground)
            {
                //商品区分が変更されたら表示項目を切り替える
                if (((ComboBox)sender).Name == "ProductCategory")
                {


                    var query2 = _vm.ItemMst.MstProductCategoryDt.AsEnumerable()
                                .FirstOrDefault(x => x["value"].ToString() == _vm.Item.ProductCategory)
                    ;
                    if (query2 is null)
                    {
                        return;
                    }

                    // 商品が対象部署でないと設定不可
                    if (query2["extend_field_1"].ToString() != "" && MainWindow.GetOrganizationId() != query2["extend_field_1"].ToString())
                    {
                        MessageBox.Show("管理部署が異なりますので変更できません");
                        // 商品種別を元の値に戻す
                        ProductCategory.SelectedIndex = _ProductCategorySelectedIndex;
                        return;
                    }





                    /// 表示項目の設定
                    SetDisp();
                    setScrollY();

                    setContextMainMenu(false);

                }

                //ComboBoxは直接背景色を変更できないためBorderの背景色を変更する
                var query1 = _ControlList
                    .FirstOrDefault(x => x.Name == ((ComboBox)(sender)).Name && x.UpdateFlg == false)
                    ;
                if (!(query1 is null))
                {
                    query1.UpdateFlg = true;
                    AddContextMenu(query1);

                    query1 = _ControlList
                        .FirstOrDefault(x => x.Name == "BorderComboBox" + ((ComboBox)(sender)).Name)
                        ;
                    if (!(query1 is null))
                    {
                        ((Border)(query1.Obj)).Background = _BrushUpdateBackGround;
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button _btn = (Button)sender;

            switch (_btn.Name)
            {
                /// 引用
                case "BtnReg":
                    {
                        ProductPlanningID.IsReadOnly = false;//引用時のみ変更可能
                        //ProductPlanningID.Text = "";
                        //新規の場合IDを自動採番する。
                        string s = "";
                        switch (MainWindow.GetOrganizationId())
                        {
                            case MainWindow.Organization12:
                                s = _vm.GetAutoProductPlanningid(MainWindow.Organization12, "");
                                break;
                            case MainWindow.Organization33:
                                s = _vm.GetAutoProductPlanningid(MainWindow.Organization33, "K");
                                break;
                            default:
                                break;
                        }

                        _vm.Item.ProductPlanningID = s;
                        _vm.Item.version = 0;
                        ProductPlanningID.Text = s;

                        break;
                    }
                /// 更新
                case "BtnUpdate":
                    {
                        if (ProductPlanningID.Text == "")
                        {
                            MessageBox.Show("IDが入力されていません。");
                            return;
                        }


                        // メッセージボックスを表示
                        if (MessageBox.Show("更新して良いでしょうか？", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information)
                                    == MessageBoxResult.No)
                        { return; }

                        //データ更新
                        _vm.Item.version += 1;
                        _vm.Item.updateuser = MainWindow.GetLoginName();
                        _vm.Item.updatedate = DateTime.Now.ToString("yy/MM/dd HH:mm:ss");

                        if (ProductPlanningID.IsReadOnly == false)   //新規登録・引用の場合「IsReadOnly == false」
                        {
                            //新規登録
                            _vm.IdDataInsert();
                        }

                        DataTable updatedataTable = _vm.GetUpdatedatatable();//空のDataTable
                        var query2 = _ControlList
                                .Where(x => x.UpdateFlg == true);
                        foreach (var item in query2)
                        {
                            updatedataTable.Rows.Add();
                            updatedataTable.Rows[updatedataTable.Rows.Count - 1]["productplanningid"] = _vm.Item.ProductPlanningID;
                            updatedataTable.Rows[updatedataTable.Rows.Count - 1]["version"] = _vm.Item.version;
                            updatedataTable.Rows[updatedataTable.Rows.Count - 1]["columnname"] = item.Name;
                        }

                        _vm.IdDataUpdate(updatedataTable);

                        MessageBox.Show("更新が完了しました。");
                        await CreateMail2Async(); // メール送信完了を待機

                        _UpdateBackground = false;//TextBOxに変更があったら背景を変えるフラグ

                        //表示履歴終了セット

                        _vm.SetScreenDisplayEndingTime();

                        //resetData();
                        //NavigationService.Navigate(MainWindow.GetProPlaSearch());
                        MainWindow.GetMainWindow().NavigationProPlaSearch(ProPlaSearch.DispModeNormal);

                        break;
                    }
                // 終了
                case "BtnEnd":
                    //MainScrollViewer.ContextMenu = null;

                    var query1 = _ControlList
                            .FirstOrDefault(x => x.UpdateFlg == true)
                            ;
                    if (!(query1 is null))
                    {
                        // メッセージボックスを表示
                        if (MessageBox.Show("データが更新されています。終了してよいでしょうか？", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information)
                                    == MessageBoxResult.No)
                        { return; }
                    }

                    // カーソルを待機カーソルに変更
                    MainWindow.SetCursorsWait();

                    //表示履歴終了セット
                    _vm.SetScreenDisplayEndingTime();

                    //resetData();
                    //NavigationService.Navigate(MainWindow.GetProPlaSearch());
                    MainWindow.GetMainWindow().NavigationProPlaSearch(ProPlaSearch.DispModeNormal);

                    break;
                // 更新可能に変更
                case "BtnUpdPer":

                    //表示履歴更新セット
                    string msg = _vm.SetScreenDisplayUpdateTime();
                    if (msg != "")
                    {
                        //ほかの人が更新中
                        MessageBox.Show(msg);
                        return;
                    }

                    // コンポーネントの活性化
                    SetIsEnabled(true);
                    ProductPlanningID.IsReadOnly = true;//IDは変更不可
                    BtnUpdPer.IsEnabled = false;//更新可能ボタンを２重で押せないように非活性化
                    ProductAnniversary.IsReadOnly = true;//本発売日からの**周年は表示のみ
                    break;
                case "BtnHis":
                    ProPlaSearchWindow his = new ProPlaSearchWindow(_vm.Item.ProductPlanningID, ProPlaSearch.DispModeHistory);
                    his.ShowDialog();
                    break;

                case "BtnWindowStateMinimized":
                    // ウィンドウを最小化
                    MainWindow.WindowStateMinimized();
                    break;

                default:
                    if (_btn.Name.IndexOf("BtnList_") >= 0)
                    {
                        // ===============
                        //      LIST
                        // ===============
                        string[] arr = _btn.Name.Split('_');
                        string _ColumnName = arr[1];

                        _list = new StdListBox();

                        //原材料と添加物
                        string _DtName = arr[1] + "Dt";
                        string _MstDtName = "Mst" + arr[2] + "Dt";
                        // 商品区分 ComboBox セット
                        string _SelectDataStr = _vm.Item[_ColumnName].ToString(); //選択している文字列のカンマ区切り

                        if (_ColumnName == "PackageMaterialName")
                        {

                            //原材料・添加物マスタの追加があった場合LISTを再表示するためループ
                            for (int i1 = 0; i1 < 1000; i1++)
                            {
                                //リストボックス作成
                                _list = new StdListBox();
                                _list.SelectDataStr = _SelectDataStr;

                                _list.SetRow(2);

                                string _str1 = "";
                                string _str2 = "";
                                string[] arrstr = _SelectDataStr.Split('／');
                                _str1 = arrstr[0];
                                if (arrstr.Length == 2)
                                {
                                    _str2 = arrstr[1];
                                }
                                //原材料・データセット
                                string _msg1 = _list.ListBoxSet(1, (DataTable)_vm.ItemMst[_MstDtName + "1"], _str1);
                                if (_msg1 != "")
                                {
                                    //データセットエラー
                                    MessageBox.Show(_msg1);
                                    return;
                                }
                                //添加物・データセット
                                string _msg2 = _list.ListBoxSet(2, (DataTable)_vm.ItemMst[_MstDtName + "2"], _str2);
                                if (_msg2 != "")
                                {
                                    //データセットエラー
                                    MessageBox.Show(_msg2);
                                    return;
                                }

                                //---------------------
                                //リストボックス表示
                                //---------------------
                                _list.ShowDialog();
                                if (_list.MasterAddStr == "")
                                {
                                    break;
                                }
                                {
                                    //マスタの追加がある。
                                    _SelectDataStr = SetMstPackageMaterialName(_list.MasterAddStr, _MstDtName);
                                }

                            }

                        }
                        else
                        {
                            //リストボックス作成
                            _list = new StdListBox();
                            _list.SelectDataStr = _SelectDataStr;

                            _list.SetRow(1);

                            string _msg = _list.ListBoxSet(1, (DataTable)_vm.ItemMst[_MstDtName], _SelectDataStr);
                            if (_msg != "")
                            {
                                //データセットエラー
                                MessageBox.Show(_msg);
                                return;
                            }

                            _list.ShowDialog();//リストボックス表示
                        }

                        _vm.Item[_ColumnName] = _list.SelectDataStr;//選択している文字列のカンマ区切り
                        var query2 = _ControlList
                            .FirstOrDefault(x => x.Name == _ColumnName);
                        if (query2 is null)
                        {
                            MessageBox.Show("コンテキストメニューが見つかりません：" + _ColumnName);
                            return;
                        }
                       ((TextBox)(query2.Obj)).Text = _list.SelectDataStr;//選択している文字列のカンマ区切り

                        break;
                    }
                    else if (_btn.Name.IndexOf("BtnHtml_") >= 0)
                    {
                        // ===============
                        //      HTML 
                        // ===============
                        string[] arr = _btn.Name.Split('_');
                        string _ColumnName = arr[1];

                        //StdListBox _list = new StdListBox();
                        StdWebBrowserEdit _WebEdit = new();

                        //インデクサの定義を使用
                        //string _s = _vm.Item[_ColumnName].ToString();
                        //string _sSave = _vm.Item[_ColumnName].ToString();

                        string _s = _vm?.Item?[_ColumnName]?.ToString() ?? "";
                        string _sSave = _vm?.Item?[_ColumnName]?.ToString() ?? "";

                        _WebEdit.SetText(_s);
                        _WebEdit.ShowDialog();
                        _s = _WebEdit.GetText();

                        if (_s == _sSave)
                        {
                            break;
                        }

                        string _Name = "Web" + _ColumnName;
                        var _item = _ControlList
                                        .FirstOrDefault(x => x.Name == _Name)
                                       ;
                        if (_item is null) { MessageBox.Show("ControlListにありません：" + _Name); return; }

                        ////インデクサの定義を使用
                        var query2 = _ControlList
                            .FirstOrDefault(x => x.Name == _ColumnName);
                        if (query2 is null)
                        {
                            MessageBox.Show("コンテキストメニューが見つかりません：" + _ColumnName);
                            return;
                        }
                       ((TextBox)(query2.Obj)).Text = _s;
                        _vm.Item[_ColumnName] = _s;
                        _s = StdWebBrowserEdit.HtmlHheader + _s + StdWebBrowserEdit.HtmlFooter;
                        ((WebView2)_item.Obj).NavigateToString(@_s);
                        //WebView2InitializeAsync();

                        break;
                    }
                    else if (_btn.Name.IndexOf("BtnPop_") >= 0)
                    {
                        string[] arr = _btn.Name.Split('_');
                        string _ColumnName = arr[1];
                        string _s = _vm.Item[_ColumnName].ToString();

                        ////インデクサの定義を使用
                        var query2 = _ControlList
                            .FirstOrDefault(x => x.Name == _ColumnName);
                        if (query2 is null)
                        {
                            MessageBox.Show("コンテキストメニューが見つかりません：" + _ColumnName);
                            return;
                        }

                        TxtMsgBox _txt = new(_s);

                        if (((TextBox)(query2.Obj)).IsReadOnly == true)
                        {
                            _txt.ShowDialog();
                            break;
                        }
                        //更新可
                        _txt.SetEnable();
                        _txt.ShowDialog();
                        string _out = _txt.GetText();
                        if (_s == _out)
                        {
                            break;
                        }

                        //データセット
                       ((TextBox)(query2.Obj)).Text = _out;
                        _vm.Item[_ColumnName] = _out;
                        break;
                    }
                    else if (_btn.Name.IndexOf("BtnCal_") >= 0)
                    {
                        string[] arr = _btn.Name.Split('_');
                        string _ColumnName = arr[1];
                        string _s = _vm.Item[_ColumnName].ToString();

                        StdCalendar.CalData = _s;
                        StdCalendar _cal = new();
                        _cal.ShowDialog();

                        var query2 = _ControlList
                                        .FirstOrDefault(x => x.Name == _ColumnName);
                        if (query2 is null)
                        {
                            MessageBox.Show("コンテキストメニューが見つかりません：" + _ColumnName);
                            return;
                        }
                        if (StdCalendar.CalData != "")
                        {
                            _s = StdCalendar.CalData;

                            ((TextBox)(query2.Obj)).Text = _s;
                            _vm.Item[_ColumnName] = _s;
                        }

                        break;
                    }

                    MessageBox.Show("Buttonエラー：定義がない");
                    break;
            }
        }
        //原材料・添加物マスタの追加
        string SetMstPackageMaterialName(string pMst, string pMstDtName)
        {
            DataTable copiedTable;
            string mstName = "";
            int mstSeq = 0;
            int a = 0;
            int b = 1;

            void addMaster()
            {
                //登録されているマスタでValueが一番大きいものを取得
                for (int i = 0; i < 10000; i++)
                {
                    if (copiedTable.Rows.Count <= b)
                    {
                        return;
                    }
                    if ((int)(copiedTable.Rows[a]["seq"]) > mstSeq) { mstSeq = (int)(copiedTable.Rows[a]["seq"]); }
                    if ((int)(copiedTable.Rows[b]["seq"]) > mstSeq) { mstSeq = (int)(copiedTable.Rows[b]["seq"]); }

                    int ival1 = int.Parse(copiedTable.Rows[a]["value"].ToString());
                    int ival2 = int.Parse(copiedTable.Rows[b]["value"].ToString());
                    if (ival1 <= ival2)
                    {
                        copiedTable.Rows[a].Delete();
                        a = b;
                        b += 1;
                    }
                    else
                    {
                        copiedTable.Rows[b].Delete();
                        b += 1;
                    }
                }
            }

            //マスタ追加
            string[] mst = _list.MasterAddStr.Split(':');
            mstName = mst[1];
            if (mst[0] == "1")
            {
                //原材料マスタ
                copiedTable = ((DataTable)(_vm.ItemMst[pMstDtName + "1"])).Copy();
            }
            else
            {
                //添加物マスタ
                copiedTable = ((DataTable)(_vm.ItemMst[pMstDtName + "2"])).Copy();
            }

            //登録されているマスタでValueが一番大きいものを取得
            addMaster();
            int ival = int.Parse(copiedTable.Rows[a]["value"].ToString());

            DataTable dataTable = new DataTable();

            // 列を追加する
            dataTable.Columns.Add("id", typeof(string));
            dataTable.Columns.Add("division_name", typeof(string));
            dataTable.Columns.Add("value", typeof(string));
            dataTable.Columns.Add("name", typeof(string));
            dataTable.Columns.Add("seq", typeof(int));

            //カンマ区切りをリストに変換
            List<string> _words = _list.ReadingPointToWords(mstName);

            foreach (var _word in _words)
            {
                DataRow row = dataTable.NewRow();

                ival += 1;
                mstSeq += 1;

                row["value"] = ival.ToString("D3");//数値３桁
                row["name"] = _word;
                row["seq"] = mstSeq;
                row["id"] = copiedTable.Rows[a]["id"];
                row["division_name"] = copiedTable.Rows[a]["division_name"];
                DataTable dt = new DataTable();
                dataTable.Rows.Add(row);
            }

            //マスタ追加
            _vm.AddPackageMaterialName(dataTable);
            MessageBox.Show("マスタ登録が完了しました。");

            string _str = _list.SelectDataStr;

            string _str1 = "";
            string _str2 = "";
            string[] arrstr = _str.Split('／');
            _str1 = arrstr[0];
            if (arrstr.Length == 2)
            {
                _str2 = arrstr[1];
            }

            if (mst[0] == "1")
            {
                //原材料マスタ
                if (_str1 != "")
                {
                    _str1 += "、";
                }
                _str1 += mstName;
            }
            else
            {
                //添加物マスタ
                if (_str2 != "")
                {
                    _str2 += "、";
                }
                _str2 += mstName;
            }
            _list.SelectDataStr = _str1 + "／" + _str2;
            return _list.SelectDataStr;
        }



        /// <summary>
        /// メールの送信画面の作成
        /// </summary>
        public void CreateMail()
        {
            // カーソルを待機カーソルに変更
            MainWindow.SetCursorsWait();

            // outlookメールの立ち上げ
            var application = new Outlook.Application();
            Outlook.MailItem mailItem = application.CreateItem(Outlook.OlItemType.olMailItem);
            if (mailItem != null)
            {
                mailItem.Subject = "【商品情報の更新】" + _vm.Item.ProductNameOfficial;
                mailItem.To = @"やずや(正社員)※会長・社長除く <all_syain2@yazuya.jp>; WEBプランニング室(契約社員+パート) <web-keiyaku-part@yazuya.jp>; WEBプランニング室(出向・派遣) <wp-sec2@yazuya.jp>; 校正スタッフ <kousei_staff@yazuya.jp>; キセキ社員のみ(社内用) <kiseki_all@yazuya.jp>; 商品企画室(正社員) <kikaku@yazuya.jp>; 商品企画室(契約社員+パート) <kikaku-keiyaku-part@yazuya.jp>";


                DataTable dtHistory = _vm.GetHistoryDetail(_vm.Item.ProductPlanningID, _vm.Item.version);

                // 本文
                string _s = "";

                string _s1 = "";

                //_s1 += "<style>";
                //_s1 += "p {";
                //_s1 += "  line-height: 1.0;";
                //_s1 += "}";
                //_s1 += "</style>";

                //_s1 += "<font face=\"BIZ UDPゴシック\" size=\"5\"  color=\"blue\">";
                //_s1 += "  <p>全体周知が不要な場合は宛先を削除して送信してください。<br>";
                //_s1 += "     ※全体周知が不要な場合は、宛先を削除してください。 <br>";
                //_s1 += "     -------------------------------------------- </p>";
                //_s1 += "</font>";

                _s1 += "<span style=\"font-family: 'BIZ UDPゴシック'; font-size: 20px; color: blue; font-weight: bold;\">";
                _s1 += "全体周知が不要な場合は宛先を削除して送信してください。<br>";
                _s1 += "</span>";
                _s1 += "<span style=\"font-family: 'BIZ UDPゴシック'; font-size: 20px; color: blue;; font-weight: 100;\">";
                _s1 += "※宛先は過不足がないか必ず確認を<br>";
                _s1 += "</span>";
                _s1 += "<br>";
                _s1 += "<span style=\"font-family: 'BIZ UDPゴシック'; font-size: 20px; color: blue; font-weight: bold;\">";
                _s1 += "●部署内のみに送るもの…他部署周知が不要な項目の更新、他部署には別途周知する内容<br>";
                _s1 += "</span>";
                _s1 += "<span style=\"font-family: 'BIZ UDPゴシック'; font-size: 20px; color: blue;; font-weight: 100;\">";
                _s1 += "※基本表記で出力される箇所は「校正スタッフ」にも送る<br>";
                _s1 += "</span>";
                _s1 += "<span style=\"font-family: 'BIZ UDPゴシック'; font-size: 20px; color: blue; font-weight: bold;\">";
                _s1 += "●上記以外(他部署にも送る場合)…内容がわかりやすいよう説明を追加等する<br>";
                _s1 += "-------------------------------------------- <br>";
                _s1 += "</span>";

                _s1 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"black\">";
                _s1 += "  <p style=\"line-height: 1;\">宛先の皆様へ</p>";
                //_s1 += "  <p>　</p>";
                _s1 += "  <p style=\"line-height: 1; font-weight: 100; \" >お疲れ様です。<br>";
                _s1 += "  以下のとおり商品情報マスタを更新しましたので<br>";
                _s1 += "  お知らせいたします。</p>";
                _s1 += "  <span style=\"background-color: yellow;\">≪更新内容≫</span>";
                _s1 += "  <span>%商品名%</span>";
                _s1 += "</font>";
                _s1 = _s1.Replace("%商品名%", _vm.Item.ProductNameOfficial);

                string _s2 = "";
                foreach (DataRow item in dtHistory.Rows)
                {
                    _s2 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"black\">";
                    _s2 += "  <p>■%項目名%</p>";
                    _s2 += "</font>";

                    _s2 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"black\">";
                    _s2 += "  <p><span style=\"background-color: silver;\">≪更新前≫</span></p>";
                    _s2 += "  <p>　%更新前%</p>";
                    _s2 += "  <p><span style=\"background-color: deepskyblue;\">≪更新後≫</span></p>";
                    _s2 += "</font>";

                    _s2 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"red\">";
                    _s2 += "  <p>　%更新後%</p>";
                    _s2 += "  <p> </p>";
                    _s2 += "</font>";
                    _s2 = _s2.Replace("%項目名%", item["jname"].ToString());

                    string s = _vm.GetMstConv(item["columnname"].ToString(), item["befor"].ToString());
                    if (s.IndexOf("$$$Error$$$") >= 0)
                    {
                        MessageBox.Show(s);
                    }
                    _s2 = _s2.Replace("%更新前%", s);

                    s = _vm.GetMstConv(item["columnname"].ToString(), item["after"].ToString());
                    if (s.IndexOf("$$$Error$$$") >= 0)
                    {
                        MessageBox.Show(s);
                    }
                    _s2 = _s2.Replace("%更新後%", s);
                }

                //Outlook.Recipient to = mailItem.Recipients.Add(@"商品企画室(正社員) kikaku@yazuya.jp; 商品企画室(契約社員+パート) kikaku-keiyaku-part@yazuya.jp\r\n");
                //to.Type = (int)Outlook.OlMailRecipientType.olTo;

                //// Cc
                //Outlook.Recipient cc = mailItem.Recipients.Add("YYY@YYY.co.jp");
                //cc.Type = (int)Outlook.OlMailRecipientType.olCC;

                //// アドレス帳の表示名で表示できる
                //mailItem.Recipients.ResolveAll();

                //// 件名
                //mailItem.Subject = "商品情報変更通知";

                // 本文
                //string _s = "";
                //var query1 = _ControlList
                //    .Where(x => x.Type == "TextBox" && x.UpdateFlg == true)
                //    .Select(x => x)
                //    ;
                //foreach (var item in query1)
                //{
                //    _s += ((TextBox)(item.Obj)).ToolTip + ":" + ((TextBox)(item.Obj)).Text + "\r\n";
                //}
                //mailItem.Body = _s;
                _s = _s1 + _s2;


                mailItem.HTMLBody = "<html><body><h1>" + _s + "</h1></body></html>";

                // 待機が完了したら元のカーソルに戻す
                MainWindow.SetCursorsNormal();

                // 表示(Displayメソッド引数のtrue/falseでモーダル/モードレスウィンドウを指定して表示できる)
                mailItem.Display(true);

            }
        }

        public async Task CreateMail2Async()
        {
            var options = new MailComposerOptions
            {
                MailToPath = @"Z:\商品企画室\OutlookMail\MailTo.txt",
                BodyTemplatePath = @"Z:\商品企画室\OutlookMail\Mail.html",
                SubjectPrefix = "【商品情報の更新】"
            };

            using var mailer = new ProPlaOutlookMailer(options);

            try
            {
                // ① 初期化（件名/宛先/本文テンプレ読込）
                await mailer.InitializeAsync(_vm.Item.ProductNameOfficial);

                // ② 変更履歴を差分ブロックとして積む
                DataTable dtHistory = _vm.GetHistoryDetail(_vm.Item.ProductPlanningID, _vm.Item.version);
                foreach (DataRow row in dtHistory.Rows)
                {
                    string jname = row["jname"]?.ToString() ?? string.Empty;

                    string before = _vm.GetMstConv(row["columnname"]?.ToString() ?? "",
                                                   row["befor"]?.ToString() ?? "");
                    if (before.Contains("$$$Error$$$"))
                    {
                        MessageBox.Show(before);
                        continue; // 当該項目はスキップ
                    }

                    string after = _vm.GetMstConv(row["columnname"]?.ToString() ?? "",
                                                  row["after"]?.ToString() ?? "");
                    if (after.Contains("$$$Error$$$"))
                    {
                        MessageBox.Show(after);
                        continue;
                    }

                    mailer.AppendChangeSection(jname, before, after);
                }

                // ③ Outlook表示（WordEditorの段落余白も整形）
                await mailer.ShowInspectorAsync();
            }
            catch (MailCompositionException ex)
            {
                // リファクタ版で意味のあるメッセージにラップ済み
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                // 想定外
                MessageBox.Show("メール作成で予期せぬエラーが発生しました。\n" + ex.Message);
            }
        }

    void writeDebugLog(string msg, bool start_flg)
    {
        string logPath = @"C:\YzyTs\temp\YzyTs002\debuglog.txt";

        // 日時（ミリ秒まで）
        string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

        // 出力内容
        string logLine = $"{timestamp} {msg}";

        // フォルダが存在しない場合は作成
        Directory.CreateDirectory(Path.GetDirectoryName(logPath));

        // start_flg=true → 上書き / false → 追記
        using (StreamWriter sw = new StreamWriter(logPath, !start_flg))
        {
            sw.WriteLine(logLine);
        }
    }




}
}
