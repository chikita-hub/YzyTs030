using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Propla
{
    /// <summary>
    /// ProPlaHistory.xaml の相互作用ロジック
    /// </summary>
    /// 


    /// <summary>
    /// 
    /// </summary>
    public partial class ProPlaHistory : Window
    {

        private class ViewModelCol : ObservableObject
        {
            public ObservableCollection<column> _Items = new();

            public ViewModelCol()
            {
            }

            public ObservableCollection<column> Items
            {
                get
                {
                    return _Items;
                }
                set
                {
                    SetProperty(ref _Items, value, nameof(Items));
                }
            }


        }
        private class column : ObservableObject
        {
            public string colkey { get; set; } = "";
            public string colname { get; set; } = "";
            public string befor { get; set; } = "";
            public string after { get; set; } = "";
        }



        //public List<string> OutColumnList;
        List<column> columnList = new();

        public List<ColList> colList;

        private ViewModel _vm = new();
        private ViewModelCol _vmCol = new();

        DataTable dtHistory;
        DataTable dtColumnList;
        //private column _column = new();

        const int DISPMODEHISTORY = 1;
        const int DISPMODEOUTCOLM = 2;

        int dispMode = 0;
        public ProPlaHistory(string pProductPlanningId, int pVersion, string pProductNameOfficial, string pUpdateDate, string pUpdateUser)
        {
            InitializeComponent();
            dispMode = DISPMODEHISTORY;

            Title = "更新履歴";

            string _s = "";
            _s += "ID：" + pProductPlanningId + "         ";
            _s += "バージョン：" + pVersion + "         ";
            _s += "商品名：" + pProductNameOfficial + "         ";
            _s += "更新日：" + pUpdateDate + "         ";
            _s += "更新者：" + pUpdateUser + "         ";


            TextInformation.Text = _s;

            dtHistory = _vm.GetHistoryDetail(pProductPlanningId, pVersion);

            foreach (DataRow item in dtHistory.Rows)
            {
                column c = new();
                c.colkey = (string)item["columnname"];
                c.colname = (string)item["jname"];

                string s = _vm.GetMstConv(item["columnname"].ToString(), item["befor"].ToString());
                if (s.IndexOf("$$$Error$$$") >= 0)
                {
                    MessageBox.Show(s);
                }
                c.befor = s;

                s = _vm.GetMstConv(item["columnname"].ToString(), item["after"].ToString());
                if (s.IndexOf("$$$Error$$$") >= 0)
                {
                    MessageBox.Show(s);
                }
                c.after = s;

                _vmCol.Items.Add(c);
            }

            //dtHistory.Columns["columnname"].ColumnName = "項目キー";
            //dtHistory.Columns["jname"].ColumnName = "項目名";
            //dtHistory.Columns["befor"].ColumnName = "更新前の値";
            //dtHistory.Columns["after"].ColumnName = "更新後の値";

            // DataGridにDataTableを設定します。
            DataContext = _vmCol;

            DataGridcolkey.Visibility = Visibility.Collapsed;
            BorderOut.Visibility = Visibility.Collapsed;
            DataGridbefor.Visibility = Visibility.Visible;
            DataGridafter.Visibility = Visibility.Visible;
            DataGridcolname.Width = 300;
            StackPanelHistory.Visibility = Visibility.Visible;

            TextMsg.Text = "";

        }

        public ProPlaHistory()
        {
            InitializeComponent();

            Loaded += WindowLoaded;

            Title = "項目選択";

            dispMode = DISPMODEOUTCOLM;
            TextInformation.Text = "";

            dtColumnList = _vm.GetColumnList();

            {
                //「ご注意(ポップアップリンク)」を一番上に移動
                column c = new();
                c.colkey = "";
                c.colname = "";
                c.befor = "";
                c.after = "";
                _vmCol.Items.Add(c);
            }
            foreach (DataRow item in dtColumnList.Rows)
            {
                string colName = ((string)item["column_name"]).ToLower();

                if (MainWindow.IsOutExcelColum(colName))
                {
                    if (colName == "popuplink")
                    {
                        _vmCol.Items[0].colkey = colName;
                        _vmCol.Items[0].colname = (string)item["comment"];
                        _vmCol.Items[0].befor = "";
                        _vmCol.Items[0].after = "";
                    }
                    else
                    {
                        column c = new();
                        c.colkey = colName;
                        c.colname = (string)item["comment"];
                        c.befor = "";
                        c.after = "";
                        _vmCol.Items.Add(c);
                    }
                }

                //switch (c.colkey.ToLower())
                //{
                //    case "factorynumberlist":
                //    case "disintegrationtestresult":
                //    case "materialoriginlist":
                //    case "materialoriginlistlink":
                //    case "materialoriginlist2":
                //    case "materialoriginlistlink2":
                //    case "samplesetattentionlink":
                //    case "productlot":
                //    case "foodequivalentlink":
                //    case "nutrientscertificatelink":
                //    case "productswitchlist":
                //    case "trademarkcertificate":
                //    case "trademarkacquisitionlist":
                //    case "ngokdisplay":
                //    case "latestinitialdocumentlink":
                //    case "nutrientscomparison":
                //    case "nutrientscertificate":
                //    case "packageimage":
                //    case "packagepaperdata":
                //    case "packagephotodata":
                //    case "packagerevisionhistory":
                //    case "productstudymaterials":
                //    case "productanniversary":
                //        break;
                //    case "disintegrationtestpass":      //崩壊性試験の合否
                //    case "productcost":                 //商品原価(税抜き)
                //    case "changehistory":               //変更履歴
                //    case "productcost2":                //商品原価(税込)
                //    case "grossprofitmargin":           //定期コース粗利率
                //    case "grossprofitmargin2":          //一般価格粗利率
                //        //企画室の権限以上あれば出力可
                //        if (MainWindow.GetAccessPermission(MainWindow.AuthManager,MainWindow.Organization12)
                //            || MainWindow.GetAccessPermission(MainWindow.AuthManager, MainWindow.Organization33)
                //            )
                //        {
                //            _vmCol.Items.Add(c);
                //        }
                //        break;
                //    default:
                //        _vmCol.Items.Add(c);
                //        break;
                //}
            }


            // DataGridにDataTableを設定します。
            DataContext = _vmCol;

            DataGridcolkey.Visibility = Visibility.Collapsed;
            BorderOut.Visibility = Visibility.Visible;
            DataGridbefor.Visibility = Visibility.Collapsed;
            DataGridafter.Visibility = Visibility.Collapsed;
            DataGridcolname.Width = 1000;
            StackPanelHistory.Visibility = Visibility.Collapsed;

            TextMsg.Text = "※選択した順に項目が並んで出力されます。";

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            void setSelect(string pId)
            {
                for (int i = 0; i < DataGridMain.Items.Count; i++)
                {

                    column _item = (column)DataGridMain.Items[i];
                    if (_item.colkey == pId)
                    {
                        DataGridMain.SelectedItems.Add(_item);
                    }
                }
            }

            // Loadedイベントの処理
            //DataTableの全データの「check」項目の値がFALSEに設定
            DataGridMain.SelectedItems.Clear();

            foreach (ColList item in colList)
            {
                setSelect(item.ColId.ToLower());
            }

        }





        //private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    if (dispMode == DISPMODEHISTORY)
        //    {
        //        if (e.PropertyName == "項目キー")
        //        {
        //            e.Column.Header = "項目キー";
        //            e.Column.Visibility = Visibility.Collapsed;
        //        }
        //        else if (e.PropertyName == "項目名")
        //        {
        //            e.Column.Header = "項目名";
        //            e.Column.Width = new DataGridLength(300); // 幅を設定
        //            e.Column.IsReadOnly = true;  // 更新不可
        //        }
        //        else if (e.PropertyName == "更新前の値")
        //        {
        //            e.Column.Header = "更新前の値";
        //            e.Column.Width = new DataGridLength(200); // 幅を設定
        //            e.Column.IsReadOnly = true;  // 更新不可
        //        }
        //        else if (e.PropertyName == "更新後の値")
        //        {
        //            e.Column.Header = "更新後の値";
        //            e.Column.Width = new DataGridLength(200); // 幅を設定
        //            e.Column.IsReadOnly = true;  // 更新不可
        //        }
        //    }
        //    else
        //    {
        //        if (e.PropertyName == "項目キー")
        //        {
        //            e.Column.Header = "項目キー";
        //            e.Column.Visibility = Visibility.Collapsed;
        //        }
        //        else if (e.PropertyName == "項目名")
        //        {
        //            e.Column.Header = "項目名";
        //            e.Column.Width = new DataGridLength(300); // 幅を設定
        //            e.Column.IsReadOnly = true;  // 更新不可
        //        }
        //        else if (e.PropertyName == "選択")
        //        {
        //            e.Column.Header = "選択";
        //            e.Column.Width = new DataGridLength(50); // 幅を設定
        //        }
        //    }

        //}
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
                    case "BtnEnd":

                        if (dispMode == DISPMODEOUTCOLM)
                        {
                            //DataTableの全データの「check」項目の設定
                            colList.Clear();
                            foreach (var selectedItem in DataGridMain.SelectedItems)
                            {
                                ColList c = new();
                                c.ColId = ((column)selectedItem).colkey;
                                c.ColNm = ((column)selectedItem).colname;
                                colList.Add(c);
                            }
                        }
                        DialogResult = false;
                        break;
                    case "BtnAllSel":
                        DataGridMain.SelectAll();
                        break;

                    case "BtnAllLift":
                        DataGridMain.SelectedItems.Clear();
                        break;
                    case "BtnWindowStateMinimized":
                        this.WindowState = WindowState.Minimized;
                        break;

                    default:
                        break;
                }
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

        void selectItem()
        {
            int i = DataGridMain.Items.IndexOf(DataGridMain.CurrentItem);
            column _item = (column)DataGridMain.Items[i];

            string s = "";
            s += _item.colname + "\r\n\r\n"; ;

            s += "■更新前の値\r\n";
            s += _item.befor + "\r\n\r\n"; ;

            s += "■更新後の値\r\n";
            s += _item.after + "\r\n\r\n";

            TxtMsgBox _txt = new(s);
            _txt.ShowDialog();
        }
    }
}
