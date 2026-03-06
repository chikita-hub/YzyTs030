using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Propla
{
    /// <summary>
    /// ProPlaOutItemSelect.xaml の相互作用ロジック
    /// </summary>
    /// 
    public partial class ProPlaOutItemSelect : Window
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
            public string colclass { get; set; } = "";
            public string colname { get; set; } = "";
        }

        List<column> columnList = new();

        public List<ColList> colList;

        private ViewModel _vm = new();
        private ViewModelCol _vmCol = new();

        DataTable dtColumnList;

        public ProPlaOutItemSelect()
        {
            InitializeComponent();

            Loaded += WindowLoaded;

            Title = "項目選択";

            dtColumnList = _vm.GetColumnList();

            {
                //「ご注意(ポップアップリンク)」を一番上に移動
                //「ご注意(ポップアップリンク)」用を事前に作成　Items[0]
                column c = new();
                c.colkey = "";
                c.colname = "";
                c.colclass = "";
                _vmCol.Items.Add(c);
            }
            foreach (DataRow item in dtColumnList.Rows)
            {
                string colName = ((string)item["column_name"]).ToLower();

                string s = (string)item["comment"];
                int i = s.IndexOf(" : ");
                string colClass = s.Substring(0, i);
                string colJpName = s.Substring(i + 3);

                if (MainWindow.IsOutExcelColum(colName))
                {
                    if (colName == "popuplink")
                    {
                        _vmCol.Items[0].colkey = colName;
                        _vmCol.Items[0].colclass = "要注意";
                        _vmCol.Items[0].colname = colJpName;
                    }
                    else
                    {
                        column c = new();
                        c.colkey = colName;

                        c.colclass = colClass;
                        c.colname = colJpName;
                        _vmCol.Items.Add(c);
                    }
                }
            }
            // DataGridにDataTableを設定します。
            DataContext = _vmCol;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Loadedイベントの処理
            void setSelect(string id)
            {
                for (int i = 0; i < DataGridMain.Items.Count; i++)
                {

                    column _item = (column)DataGridMain.Items[i];
                    if (_item.colkey == id)
                    {
                        DataGridMain.SelectedItems.Add(_item);
                    }
                }
            }

            //DataTableの全データの「check」項目の値がFALSEに設定
            DataGridMain.SelectedItems.Clear();

            //選択データの復元
            foreach (ColList item in colList)
            {
                setSelect(item.ColId.ToLower());
            }
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
                    case "BtnEnd":

                        colList.Clear();
                        foreach (var selectedItem in DataGridMain.SelectedItems)
                        {
                            ColList c = new();
                            c.ColId = ((column)selectedItem).colkey;
                            c.ColNm = ((column)selectedItem).colname;
                            colList.Add(c);
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

        private void GroupCellMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 1) return;

            // Border -> DataContext は CollectionViewGroup
            if (sender is not Border b) return;

            var group = b.DataContext as CollectionViewGroup;

            // グループキー（= colclass）
            var groupKey = group.Name?.ToString() ?? "";

            for (int i = 0; i < DataGridMain.Items.Count; i++)
            {
                column _item = (column)DataGridMain.Items[i];
                if (_item.colclass == groupKey)
                {
                    if (DataGridMain.SelectedItems.Contains(_item))
                    {
                        // 既に選択済みなら削除
                        DataGridMain.SelectedItems.Remove(_item);
                    }
                    else
                    {
                        // 未選択なら追加
                        DataGridMain.SelectedItems.Add(_item);
                    }
                }
            }
            e.Handled = true;
        }

        // ProPlaOutItemSelect.xaml.cs 内に追加

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                // 現在の選択状態を反転させる
                row.IsSelected = !row.IsSelected;

                // フォーカスを行に移動（必要に応じて）
                row.Focus();

                // イベントをここで完了させ、標準の選択ロジック（他を解除する挙動など）を無効化する
                e.Handled = true;
            }
        }

    }
}
