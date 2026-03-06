using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Propla
{
    /// <summary>
    /// ProPlaSearch.xaml の相互作用ロジック
    /// </summary>
    public partial class ProPlaSearch : Page
    {

        public const string DispModeNormal = "Nor";
        public const string DispModeHistory = "His";
        //public const string DispModeOut = "Out";

        private ViewModel _vm = new();
        //private bool _HistoryMode = false;
        private string _DispMode;
        static private ProPlaHistory _ProPlaHistory;

        string _ProductPlanningID;
        string _ProductReleaseStatus = "";
        string _OutTemplate = "";

        public class OutList
        {
            public int outType { get; set; }
            public string outValue { get; set; }
            public string outJname { get; set; }
        }
        //List<OutList> outColumnList;
        //public class ColList
        //{
        //    public string ColId;
        //    public string ColNm;
        //}
        List<ColList> colList;

        // aクラスのListを作成する
        public void SetDispMode(string pMode)
        {
            _DispMode = pMode;
        }

        public ProPlaSearch(string pId)
        {
            InitializeComponent();

            _ProductPlanningID = pId;
            // Windowの大きさをPageに合わせるなど。
            //MainWindow.WindowsInitSet(this.Height, this.Width, this);

            //　【重要】xamlとのリンクセット
            this.DataContext = _vm;

            CmbProductCategory.ItemsSource = _vm.ItemMst.MstProductCategoryDt.DefaultView;
            CmbProductReleaseStatus.ItemsSource = _vm.ItemMst.MstProductReleaseStatusDt.DefaultView;
            if (MainCmbOutTemplate.ItemsSource == null)
            {
                MainCmbOutTemplate.ItemsSource = _vm.ItemMst.MstOutTemplateDt.DefaultView;
            }
            MainCmbProductReleaseStatus.ItemsSource = _vm.ItemMst.MstProductReleaseStatusDt.DefaultView;

            dispSet();

            Loaded += (o, e) =>
            {
                /// コントロールのロード完了処理
                LoadMain();
            };

        }

        /// <summary>
        /// コントロールのロード完了処理
        /// </summary>
        private void LoadMain()
        {
            switch (_DispMode)
            {
                //case DispModeNormal:
                //    {
                //        dispSet();
                //        break;
                //    }
                case DispModeHistory:
                    {
                        // ProPlaMain からの履歴表示
                        dispSet();

                        int _i = _vm.IdsDataSelectHistory(_ProductPlanningID);
                        break;
                    }
                //case DispModeOut:
                //    {
                //        dispSet();
                //        break;
                //    }
                default:
                    dispSet();
                    break;
            }
            //企画状態　本発売中
            MainCmbProductReleaseStatus.SelectedIndex = 3;
            // カーソルを通常に変更
            MainWindow.SetCursorsNormal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {

                Button _o = (Button)sender;
                switch (_o.Name)
                {
                    //「検索」押下
                    case "BtnSer":
                        {
                            searchItem();
                        }
                        break;
                    //「更新履歴」押下
                    case "BtnHistory":
                        {
                            _DispMode = DispModeHistory;
                            dispSet();

                            int i = DataGridMain.Items.IndexOf(DataGridMain.CurrentItem);
                            ProductPlanning _item = (ProductPlanning)DataGridMain.Items[i];
                            string _s = _item.ProductPlanningID;

                            int _i = _vm.IdsDataSelectHistory(_s);
                            if (_i == 0)
                            {
                                MessageBox.Show("システムエラー「v_productplanninghistory」を作成してください。");
                            }
                        }
                        break;

                    ////「出力画面」押下
                    //case "BtnOutSw":
                    //    {
                    //        List<int> itemList = new();

                    //        //選択項目を退避
                    //        foreach (var item in DataGridMain.SelectedItems)
                    //        {
                    //            int index = DataGridMain.Items.IndexOf(item);
                    //            itemList.Add(index);
                    //        }

                    //        //+   // 新：出力パネルの表示/非表示トグル
                    //        //_DispMode = DispModeOut;
                    //        //dispSet();
                    //        BorderOut.Visibility =
                    //            (BorderOut.Visibility == Visibility.Visible)
                    //                ? Visibility.Collapsed
                    //                : Visibility.Visible;



                    //        //選択項目を戻す
                    //        foreach (int index in itemList)
                    //        {
                    //            if (index >= 0 && index < DataGridMain.Items.Count)
                    //            {
                    //                DataGridMain.SelectedItems.Add(DataGridMain.Items[index]);
                    //            }
                    //        }
                    //    }
                    //    break;
                    //「戻る」押下
                    case "BtnEnd":

                        //// ProPlaMain からの履歴表示
                        //if (_ProductPlanningID != "")
                        //{
                        //    ProPlaSearchWindow.SetDialogResult(false);
                        //    break;
                        //}
                        switch (_DispMode)
                        {
                            //商品情報マスタ選択
                            //case DispModeNormal:
                            //    ProPlaMenu _ProPlaMenu = MainWindow.GetProPlaMenu();
                            //    NavigationService.Navigate(_ProPlaMenu);
                            //    break;
                            //商品情報マスタ更新履歴
                            case DispModeHistory:
                                _DispMode = DispModeNormal;
                                searchItem();
                                break;
                            //データ出力
                            //case DispModeOut:
                            //    List<int> itemList = new();

                            //    //選択項目を退避
                            //    foreach (var item in DataGridMain.SelectedItems)
                            //    {
                            //        int index = DataGridMain.Items.IndexOf(item);
                            //        itemList.Add(index);
                            //    }
                            //    _DispMode = DispModeNormal;

                            //    dispSet();

                            //    //選択項目を戻す
                            //    foreach (int index in itemList)
                            //    {
                            //        if (index >= 0 && index < DataGridMain.Items.Count)
                            //        {
                            //            DataGridMain.SelectedItems.Add(DataGridMain.Items[index]);
                            //        }
                            //    }

                            //    break;
                            default:
                                //NavigationService.Navigate(MainWindow.GetProPlaMenu());
                                MainWindow.GetMainWindow().NavigationProPlaMenu();
                                break;
                        }
                        break;

                    //「選択」押下
                    case "BtnDgSelect":
                        {
                            selectItem();
                        }
                        break;

                    //「新規登録」押下
                    case "BtnNew":

                        // DataSelect
                        //MainWindow.GetProPlaMain().SetProductPlanningID("");
                        //NavigationService.Navigate(MainWindow.GetProPlaMain());
                        MainWindow.GetMainWindow().NavigationProPlaMain("");
                        break;

                    //「全選択」押下
                    case "BtnAllSel":
                        DataGridMain.SelectAll();
                        break;

                    //「全解除」押下
                    case "BtnAllLift":
                        DataGridMain.SelectedItems.Clear();
                        break;

                    //「選択保存」押下
                    case "BtnSelKeep":
                        {
                            List<OutList> outList = new List<OutList>();

                            foreach (var selectedItem in DataGridMain.SelectedItems)
                            {
                                string productPlanningID = ((ProductPlanning)selectedItem).ProductPlanningID;
                                outList.Add(new OutList() { outType = 1, outValue = productPlanningID, outJname = "" });
                            }

                            foreach (var item in colList)
                            {
                                outList.Add(new OutList() { outType = 2, outValue = item.ColId, outJname = item.ColNm });
                            }

                            if (outList.Count != 0)
                            {
                                outListToJson(outList);
                            }
                            break;
                        }

                    //「選択読込」押下
                    case "BtnSelload":
                        {
                            List<OutList> outList = jsonToOutList();
                            if (colList == null)
                            {
                                colList = new();
                            }
                            if (outList != null)
                            {
                                DataGridMain.SelectedItems.Clear();
                                foreach (var item in DataGridMain.Items)
                                {
                                    string productPlanningID = ((ProductPlanning)item).ProductPlanningID;
                                    //outlistにproductPlanningIDが存在するか？
                                    bool hasData = outList.Any(item => item.outValue == productPlanningID && item.outType == 1);
                                    if (hasData)
                                    {
                                        DataGridMain.SelectedItems.Add(item);
                                    }
                                }

                                colList.Clear();
                                foreach (OutList item in outList)
                                {
                                    if (item.outType == 2)
                                    {
                                        ColList c = new();
                                        c.ColId = item.outValue;
                                        c.ColNm = item.outJname;
                                        colList.Add(c);
                                    }
                                }
                            }

                            break;

                        }

                    //「項目選択」押下
                    case "BtnColSelect":
                        {
                            if (colList == null)
                            {
                                colList = new();
                            }

                            ProPlaOutItemSelect _ProPlaOutItemSelect = new();

                            _ProPlaOutItemSelect.colList = colList;
                            _ProPlaOutItemSelect.ShowDialog();

                            colList = _ProPlaOutItemSelect.colList;

                            break;
                        }

                    //「出力」押下
                    case "BtnOut":
                        List<ProductPlanning> productPlannings = new();
                        string outFile = "";

                        foreach (var selectedItem in DataGridMain.SelectedItems)
                        {
                            productPlannings.Add((ProductPlanning)selectedItem);
                        }
                        if (productPlannings.Count == 0)
                        {
                            MessageBox.Show("データが選択されていません。");
                            return;
                        }

                        if (_OutTemplate == "0")
                        {
                            MessageBox.Show("出力種別を選択してください。");
                            return;
                        }
                        else if (_OutTemplate == "1")
                        {
                            if (colList == null || colList.Count == 0)
                            {
                                MessageBox.Show("項目を選択してください。");
                                return;
                            }
                            // カーソルを待機カーソルに変更
                            MainWindow.SetCursorsWait();
                            //項目を選択して出力
                            ProPlaOut proPlaOut = new();
                            outFile = proPlaOut.ExcelOut(productPlannings, colList);
                        }
                        else
                        {
                            // カーソルを待機カーソルに変更
                            MainWindow.SetCursorsWait();
                            //雛形から出力
                            ProPlaOut proPlaOut = new();
                            outFile = proPlaOut.ExcelOut(productPlannings, _OutTemplate);
                        }


                        if (outFile != "")
                        {
                            MessageBox.Show("EXCEL出力が完了しました。");
                            try
                            {
                                // Excelを起動してファイルを開く
                                Process.Start("Excel.exe", outFile);
                            }
                            catch (System.Exception exp)
                            {
                                MessageBox.Show("Excel起動に失敗しました。:" + exp.Message);
                            }
                        }

                        // 待機が完了したら元のカーソルに戻す
                        MainWindow.SetCursorsNormal();

                        break;

                    //「_」押下
                    case "BtnWindowStateMinimized":
                        // ウィンドウを最小化
                        MainWindow.WindowStateMinimized();
                        break;

                    default:
                        break;
                }

            }

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

        void outListToJson(List<OutList> pData)
        {
            // フォルダ選択ダイアログを表示
            // ファイルを保存する場所とファイル名を指定するダイアログを開く
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "JSONファイル (*.json)|*.json";
            dialog.Title = "ファイルを保存する場所とファイル名を指定してください。";
            dialog.FileName = "productPlanningIDs.json";
            dialog.InitialDirectory = @"Z:\商品企画室\SaveDefinition";

            // ダイアログでファイルを保存する場所とファイル名を指定する
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // JSON文字列にシリアル化する
                string json = JsonSerializer.Serialize(pData);

                // ファイルを保存する
                string filePath = dialog.FileName;
                File.WriteAllText(filePath, json);
            }

        }
        List<OutList> jsonToOutList()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 初期ディレクトリを指定する場合は、InitialDirectoryプロパティを設定します。
            openFileDialog.InitialDirectory = @"Z:\商品企画室\SaveDefinition";
            // ファイルの種類を指定する場合は、Filterプロパティを設定します。
            openFileDialog.Filter = "JSONファイル (*.json)|*.json";

            // ダイアログを開き、OKが押された場合は、選択されたファイル名を取得します。
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                // ファイルからJSON文字列を読み込む
                string jsonString = File.ReadAllText(fileName);

                // JSON文字列からオブジェクトを復元する
                List<OutList> outList = JsonSerializer.Deserialize<List<OutList>>(jsonString);

                return outList;
            }

            return null;
        }

        void selectItem()
        {
            int i = DataGridMain.Items.IndexOf(DataGridMain.CurrentItem);
            ProductPlanning _item = (ProductPlanning)DataGridMain.Items[i];

            switch (_DispMode)
            {
                //case DispModeNormal:
                //    {
                //        // カーソルを待機カーソルに変更
                //        MainWindow.SetCursorsWait();

                //        MainWindow.GetProPlaMain().SetProductPlanningID(_item.ProductPlanningID);
                //        NavigationService.Navigate(MainWindow.GetProPlaMain());
                //        break;
                //    }
                case DispModeHistory:
                    {
                        _ProPlaHistory = new(_item.ProductPlanningID, _item.version, _item.ProductNameOfficial, _item.updatedate, _item.updateuser);
                        _ProPlaHistory.ShowDialog();
                        break;
                    }
                //case DispModeOut:
                //    {
                //        break;
                //    }
                default:
                    // カーソルを待機カーソルに変更
                    MainWindow.SetCursorsWait();

                    //MainWindow.GetProPlaMain().SetProductPlanningID(_item.ProductPlanningID);
                    //NavigationService.Navigate(MainWindow.GetProPlaMain());
                    MainWindow.GetMainWindow().NavigationProPlaMain(_item.ProductPlanningID);
                    break;
            }
        }

        //一覧表示データ取得
        void searchItem()
        {

            if (_DispMode == DispModeHistory)
            {
                _DispMode = DispModeNormal;
            }

            dispSet();

            // DataSelect
            string serchText = "";
            if (_ProductReleaseStatus != "0")
            {
                serchText = _ProductReleaseStatus;
            }

            int _i = _vm.IdsDataSelect(TxtSerch.Text, serchText);
            if (_i == 0)
            {
                MessageBox.Show("データがありません。");
            }
        }

        void dispSet()
        {
            switch (_DispMode)
            {
                case DispModeNormal:
                    {
                        StackPanelHistory.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。

                        DataGridupdatedate.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                        DataGridupdateuser.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                        DataGridHistory.Visibility = Visibility.Visible;//表示する。
                        BtnSer.IsEnabled = true;//"検索"
                        //新規登録 企画室・キセキの権限なければ非表示
                        if (MainWindow.GetAccessPermission(MainWindow.AuthRegular, MainWindow.Organization12)
                            || MainWindow.GetAccessPermission(MainWindow.AuthRegular, MainWindow.Organization33)
                            )
                        {
                            BtnNew.IsEnabled = true;
                        }
                        else
                        {
                            BtnNew.IsEnabled = false;
                        }
                        RowPanel2.Visibility = Visibility.Visible;
                        BtnSelect.Visibility = Visibility.Visible;//表示する。
                        BorderKey.Visibility = Visibility.Visible;//表示する。

                        //BorderOut.Visibility = Visibility.Collapsed;
                        BorderOut.Visibility = Visibility.Visible;//表示する。

                        //BtnOutSw.Visibility = Visibility.Visible;//表示する。

                        //CheckBoxSelect.Visibility = Visibility.Collapsed;

                        break;
                    }
                case DispModeHistory:
                    {
                        // ProPlaMain からの履歴表示
                        StackPanelHistory.Visibility = Visibility.Visible;//表示する。

                        DataGridupdatedate.Visibility = Visibility.Visible;//表示する。
                        DataGridupdateuser.Visibility = Visibility.Visible;//表示する。
                        DataGridHistory.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                        BtnSer.IsEnabled = false;//"検索"
                        BtnNew.IsEnabled = false;  //"新規登録"
                        RowPanel2.Visibility = Visibility.Hidden;//表示しない＆スペースを使用する     //"キーワード"
                        BtnSelect.Visibility = Visibility.Visible;//表示する。
                        BorderKey.Visibility = Visibility.Collapsed;

                        BorderOut.Visibility = Visibility.Collapsed;
                        //BtnOutSw.Visibility = Visibility.Hidden;//表示しない＆スペースを使用する     //"キーワード"

                        //CheckBoxSelect.Visibility = Visibility.Collapsed;

                        break;
                    }
                //case DispModeOut:
                //    {
                //        StackPanelHistory.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。

                //        DataGridupdatedate.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                //        DataGridupdateuser.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                //        DataGridHistory.Visibility = Visibility.Collapsed;//表示する。
                //        BtnSer.IsEnabled = true;//"検索"
                //        BtnNew.IsEnabled = false;  //"新規登録"

                //        RowPanel2.Visibility = Visibility.Visible;
                //        BtnSelect.Visibility = Visibility.Collapsed;//表示しない＆スペースを使用しない。上に詰める。
                //        BorderKey.Visibility = Visibility.Visible;//表示する。

                //        BorderOut.Visibility = Visibility.Visible;
                //        BtnOutSw.Visibility = Visibility.Hidden;//表示しない＆スペースを使用する     //"キーワード"

                //        //MainCmbOutTemplate.SelectedIndex = 0;
                //        //BtnColSelect.Visibility = Visibility.Hidden;

                //        //CheckBoxSelect.Visibility = Visibility.Visible;
                //        break;
                //    }
                default:
                    break;
            }


        }


        //private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    if (e.PropertyType == typeof(bool))
        //    {
        //        DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn();
        //        checkBoxColumn.Header = e.Column.Header;
        //        //checkBoxColumn.Tag = e.PropertyName;
        //        checkBoxColumn.ElementStyle = Application.Current.Resources["CheckBoxStyle"] as Style;
        //        e.Column = checkBoxColumn;
        //    }
        //}

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                searchItem();
            }
        }
        private void DataGridMainMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (null != DataGridMain.SelectedItem)
            {
                var ctrl = DataGridMain.ItemContainerGenerator.ContainerFromItem(DataGridMain.SelectedItem) as DataGridRow;
                if (null != ctrl)
                {
                    if (null != ctrl.InputHitTest(e.GetPosition(ctrl)))
                    {
                        selectItem();
                    }
                }
            }
        }
        //private int getCheckBoxIndex()
        //{
        //    // DataGridのColumnsプロパティから、列の一覧を取得する
        //    var columns = DataGridMain.Columns;

        //    // 列を反復処理し、CheckBox列のインデックスを探す
        //    for (int i = 0; i < columns.Count; i++)
        //    {
        //        // DataGridCheckBoxColumnかどうかをチェックする
        //        if (columns[i] is DataGridCheckBoxColumn)
        //        {
        //            // チェックボックスが含まれる列のインデックスを返す
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        //private void setCheckBox(bool pBool)
        //{

        //    //int columnIndex = getCheckBoxIndex();
        //    foreach (var item in DataGridMain.Items)
        //    {
        //        // DataGridの各行をDependencyObjectに変換する
        //        var container = DataGridMain.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

        //        // DependencyObjectがnullでないことを確認する
        //        if (container != null)
        //        {
        //            // DataContextからバインドされたデータを取得する
        //            var dataContext = container.DataContext;

        //            // 選択列にバインドされたプロパティを取得する
        //            var checkBoxColumn = DataGridMain.Columns.FirstOrDefault(c => c.Header.ToString() == "選択") as DataGridTemplateColumn;
        //            var binding = (checkBoxColumn.CellTemplate.LoadContent() as FrameworkElement).DataContext as Binding;
        //            var propertyName = binding?.Path.Path ?? "IsSelected";

        //            // チェックボックスが未チェックであれば、チェックする
        //            var isChecked = (bool)dataContext.GetType().GetProperty(propertyName).GetValue(dataContext);
        //            if (!isChecked)
        //            {
        //                dataContext.GetType().GetProperty(propertyName).SetValue(dataContext, true);
        //            }
        //        }
        //    }
        //}
        private void ComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            // 選択されたアイテムを取得する
            ComboBox _o = (ComboBox)sender;
            switch (_o.Name)
            {
                case "MainCmbProductReleaseStatus":
                    _ProductReleaseStatus = (string)((ComboBox)sender).SelectedValue;
                    break;

                case "MainCmbOutTemplate":
                    _OutTemplate = (string)((ComboBox)sender).SelectedValue;
                    if (((ComboBox)sender).SelectedIndex == 1)
                    {
                        BtnColSelect.Visibility = Visibility.Visible;
                        BtnSelKeep.Visibility = Visibility.Visible;
                        BtnSelload.Visibility = Visibility.Visible;
                        TextBlockOut.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        BtnColSelect.Visibility = Visibility.Hidden;
                        BtnSelKeep.Visibility = Visibility.Hidden;
                        BtnSelload.Visibility = Visibility.Hidden;
                        TextBlockOut.Visibility = Visibility.Hidden;
                    }
                    break;

                default:
                    break;
            }

        }
    }
}
