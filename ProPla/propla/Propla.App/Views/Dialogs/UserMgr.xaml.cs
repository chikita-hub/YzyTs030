using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Propla
{

    public partial class UserMgr : Window
    {

        string _PgName = "";
        string _UserId = "";

        DataTable _AccountListDt;
        DataTable _AccessauthorityDt;
        string passwordSave = "";

        public UserMgr(string pPgName, string pUserId)
        {
            InitializeComponent();

            Title = "ユーザマスタ";

            _PgName = pPgName;
            _UserId = pUserId;

            mainSet();

        }
        void mainSet()
        {


            _AccountListDt = StdLogin.GetAccountList(_PgName);
            _AccessauthorityDt = StdLogin.GetAccessauthority(_PgName);

            CmbAccountauthority.ItemsSource = _AccessauthorityDt.DefaultView;

            CmbAuth.ItemsSource = _AccessauthorityDt.DefaultView;
            CmbAuth.DisplayMemberPath = "accessauthorityname";
            CmbAuth.SelectedValuePath = "accessauthorityid";


            // DataGridにDataTableを設定します。
            DataGridMain.ItemsSource = _AccountListDt.DefaultView;

            TxtUserId.IsEnabled = false;
            TxtUserName.IsEnabled = false;
            TxtMailAddr.IsEnabled = false;
            CmbAuth.IsEnabled = false;
            BtnPass.IsEnabled = false;

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

        }




        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button _o = (Button)sender;
                switch (_o.Name)
                {
                    case "BtnMod": //更新
                        int i = DataGridMain.SelectedIndex;
                        DataRow row = _AccountListDt.Rows[i];

                        TxtUserId.Text = (string)row["userid"];
                        TxtUserName.Text = (string)row["username"];
                        TxtMailAddr.Text = (string)row["mailaddress"];
                        passwordSave = (string)row["password"];

                        CmbAuth.SelectedValue = (string)row["accessauthorityid"];

                        //TxtUserId.IsEnabled = true;
                        TxtUserName.IsEnabled = true;
                        TxtMailAddr.IsEnabled = true;
                        CmbAuth.IsEnabled = true;
                        BtnPass.IsEnabled = true;

                        break;
                    case "BtnNew": //新規
                        _UserId = "";

                        TxtUserId.Text = "";
                        TxtUserName.Text = "";
                        TxtMailAddr.Text = "";
                        CmbAuth.SelectedValue = "";
                        passwordSave = "";

                        TxtUserId.IsEnabled = true;
                        TxtUserName.IsEnabled = true;
                        TxtMailAddr.IsEnabled = true;
                        CmbAuth.IsEnabled = true;
                        BtnPass.IsEnabled = true;

                        break;
                    case "BtnSet": //登録
                        if (_UserId == "")
                        {
                            //新規
                            if (TxtUserId.Text == "")
                            {
                                MessageBox.Show("ユーザIDを登録してください。");
                                return;
                            }
                            if (TxtUserName.Text == "")
                            {
                                MessageBox.Show("ユーザ名を登録してください。");
                                return;
                            }
                            if (CmbAuth.SelectedIndex == -1) // -1 は未選択状態を指します
                            {
                                MessageBox.Show("権限を登録してください。");
                                return;
                            }
                            if (CmbAuth.SelectedValue.ToString() == "" || CmbAuth.SelectedValue == null)
                            {
                                MessageBox.Show("権限を登録してください。");
                                return;
                            }
                            if (passwordSave == "")
                            {
                                MessageBox.Show("パスワードを登録してください。");
                                return;
                            }
                        }

                        DataTable _AccountDt = StdLogin.GetAccount(TxtUserId.Text);
                        _AccountDt.Rows[0]["username"] = TxtUserName.Text;
                        _AccountDt.Rows[0]["mailaddress"] = TxtMailAddr.Text;
                        _AccountDt.Rows[0]["password"] = passwordSave;

                        //権限設定　新規の場合はInsert
                        if (!StdLogin.SetAccountauthority(_PgName, TxtUserId.Text, (string)CmbAuth.SelectedValue))
                        {
                            MessageBox.Show("権限設定に失敗しました。");
                        }

                        if (StdLogin.SetAccount(_AccountDt))
                        {
                            MessageBox.Show("登録しました。");
                        }
                        else
                        {
                            MessageBox.Show("登録に失敗しました。");
                        }

                        mainSet();

                        TxtUserId.Text = "";
                        TxtUserName.Text = "";
                        TxtMailAddr.Text = "";
                        CmbAuth.SelectedValue = "";

                        TxtUserId.IsEnabled = false;
                        TxtUserName.IsEnabled = false;
                        TxtMailAddr.IsEnabled = false;
                        CmbAuth.IsEnabled = false;
                        BtnPass.IsEnabled = false;
                        break;

                    case "BtnPass": //パスワード
                        PasswordMod _PasswordMod = new(TxtUserId.Text);
                        _PasswordMod.ShowDialog();
                        passwordSave = _PasswordMod.Password;
                        break;

                    case "BtnEnd": //戻る
                        DialogResult = false;
                        break;

                    case "BtnWindowStateMinimized": //最小化
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

        void selectItem()
        {
            int i = DataGridMain.Items.IndexOf(DataGridMain.CurrentItem);
            //column _item = (column)DataGridMain.Items[i];

            //string s = "";
            //s += _item.colname + "\r\n\r\n"; ;

            //s += "■更新前の値\r\n";
            //s += _item.befor + "\r\n\r\n"; ;

            //s += "■更新後の値\r\n";
            //s += _item.after + "\r\n\r\n";

            //TxtMsgBox _txt = new(s);
            //_txt.ShowDialog();
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //if (e.PropertyType == typeof(bool))
            //{
            //    DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn();
            //    checkBoxColumn.Header = e.Column.Header;
            //    //checkBoxColumn.Tag = e.PropertyName;
            //    checkBoxColumn.ElementStyle = Application.Current.Resources["CheckBoxStyle"] as Style;
            //    e.Column = checkBoxColumn;
            //}
        }


    }
}
