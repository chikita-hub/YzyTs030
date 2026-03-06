using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Propla
{
    /// <summary>
    /// DivisionsMaster.xaml の相互作用ロジック
    /// </summary>
    /// 


    /// <summary>
    /// 
    /// </summary>
    public partial class DivisionsMaster : Window
    {
        private ViewModel _vm = new();
        private DataTable _DivisionsMasterDt;

        private string value = "";
        private string division_id = "";
        private DataView dataView = null;


        public DivisionsMaster()
        {
            InitializeComponent();

            Loaded += WindowLoaded;
        }


        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DispInit();
            dataView = _vm.GetUpdateDivisions().DefaultView;
            MainCmbDivisionsMaster.ItemsSource = dataView;
        }

        private void DispInit()
        {
            value = "";
            TxtDisplayName.Text = "";
            TxtSeq.Text = "";

            TxtDisplayName.IsEnabled = false;
            TxtSeq.IsEnabled = false;

            BtnMod.IsEnabled = false;
            BtnNew.IsEnabled = false;
            BtnSet.IsEnabled = false;

            //MainCmbDivisionsMaster.ItemsSource = new[]
            //{
            //    new { id= "007"  ,name = "価格システム"  , value = "PriceSystem" },
            //    new { id= "016"  ,name = "他商品との同梱", value = "PackingMethod" },
            //    new { id= "017"  ,name = "サンプルの有無", value = "Sample" },
            //};
        }

        private void SetDataGrid()
        {
            DispInit();

            var selectedValue = MainCmbDivisionsMaster.SelectedValue?.ToString();

            if (selectedValue is null) return;

            _vm = new();
            switch (selectedValue)
            {
                case "007":
                    _DivisionsMasterDt = _vm.ItemMst.MstPriceSystemDt;
                    break;

                case "016":
                    _DivisionsMasterDt = _vm.ItemMst.MstPackingMethodDt;
                    break;

                case "017":
                    _DivisionsMasterDt = _vm.ItemMst.MstSampleDt;
                    break;

                default:
                    return;
            }

            // DataGridにDataTableを設定します。
            DataGridMain.ItemsSource = _DivisionsMasterDt.DefaultView;

            BtnMod.IsEnabled = true;
            BtnNew.IsEnabled = true;
        }

        private void MainCmbDivisionsMaster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                // 選択された値（SelectedValuePath="division_id" の値）
                division_id = cmb.SelectedValue?.ToString();

                SetDataGrid();
            }
        }

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
                        DialogResult = false;
                        break;

                    case "BtnMod": //更新
                        int i = DataGridMain.SelectedIndex;
                        if (i < 0) return;

                        DataRow row = _DivisionsMasterDt.Rows[i];

                        value = (string)row["value"];
                        TxtDisplayName.Text = (string)row["name"];
                        TxtSeq.Text = row["seq"].ToString();

                        TxtDisplayName.IsEnabled = true;
                        TxtSeq.IsEnabled = true;
                        BtnSet.IsEnabled = true;

                        break;

                    case "BtnNew": //新規
                        TxtDisplayName.IsEnabled = true;
                        TxtSeq.IsEnabled = true;
                        BtnSet.IsEnabled = true;

                        break;

                    case "BtnSet": //登録
                        if (TxtDisplayName.Text == "")
                        {
                            MessageBox.Show("区分名を登録してください。");
                            return;
                        }

                        if (TxtSeq.Text == "")
                        {
                            MessageBox.Show("順番を登録してください。");
                            return;
                        }

                        int seq;
                        if (!(int.TryParse(TxtSeq.Text, out seq)))
                        {
                            // 数値でない場合
                            MessageBox.Show("順番を数値で入力してください。");
                            return;
                        }

                        //dataView.RowFilter = "value = '001'";
                        //string division_name = dataView.Count > 0 ? dataView[0]["name"].ToString() : null;

                        string division_name = dataView.Cast<DataRowView>()
                            .Where(row => row["division_id"].ToString() == division_id)
                            .Select(row => row["division_name"].ToString())
                            .FirstOrDefault();
                        bool result = _vm.SaveDivisions(division_id, division_name, value, TxtDisplayName.Text, seq);

                        if (result)
                        {
                            MessageBox.Show("登録しました。");
                        }
                        else
                        {
                            MessageBox.Show("登録に失敗しました。");
                        }

                        DispInit();
                        MainCmbDivisionsMaster.ItemsSource = _vm.GetUpdateDivisions().DefaultView;
                        DataGridMain.ItemsSource = null;
                        division_id = "";

                        break;

                    default:
                        break;
                }
            }
        }

    }
}
