using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace Propla
{
    /// <summary>
    /// ScreenDisplay.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenDisplay : Page
    {
        DataTable dtScreenDisplay;
        private ViewModel _vm = new();

        public ScreenDisplay()
        {
            InitializeComponent();
            // チェックを外す
            ChkUpdating.IsChecked = false;
        }

        public void SetData(bool pUpdating)
        {
            dtScreenDisplay = _vm.GetScreenDisplay(pUpdating);

            // DataGridにDataTableを設定します。
            DataContext = dtScreenDisplay;
        }
        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button _o = (Button)sender;
                switch (_o.Name)
                {
                    case "BtnEnd":
                        //ProPlaMenu _ProPlaMenu = MainWindow.GetProPlaMenu();
                        //NavigationService.Navigate(_ProPlaMenu);

                        MainWindow.GetMainWindow().NavigationProPlaMenu();
                        break;
                    case "BtnLockRelease":
                        {
                            if (DataGridScreenDisplay.SelectedItems.Count != 1)
                            {
                                MessageBox.Show("選択されていません");
                                return;
                            }

                            // 選択された行の情報を取得します
                            var selectedItems = DataGridScreenDisplay.SelectedItems;

                            foreach (var selectedItem in selectedItems)
                            {
                                DataRowView dataRow = (DataRowView)selectedItem;

                                if (dataRow["表示モード"].ToString() == "★更新中★")
                                {
                                    string _ProductPlanningid = dataRow["商品id"].ToString();
                                    string _Userid = dataRow["ユーザid"].ToString();
                                    string _Starttime = dataRow["開始日時"].ToString();
                                    _vm.SetScreenDisplayLockRelease(_ProductPlanningid, _Userid, _Starttime);

                                    SetData(false);
                                    // チェックを外す
                                    ChkUpdating.IsChecked = false;

                                }
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void ChkUpdatingChecked(object sender, RoutedEventArgs e)
        {
            SetData(true);
        }

        private void ChkUpdatingUnchecked(object sender, RoutedEventArgs e)
        {
            SetData(false);
        }

    }
}
